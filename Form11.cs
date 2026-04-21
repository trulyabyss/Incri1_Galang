using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;

namespace Incri1_Galang
{
    public partial class Form11 : Form
    {
        private static readonly HttpClient Http = new HttpClient();
        private readonly List<ResponseUnit> _units;
        private readonly GMapOverlay _markersOverlay = new GMapOverlay("markers");
        private readonly GMapOverlay _routesOverlay = new GMapOverlay("routes");
        private PointLatLng? _pinnedIncidentPoint;
        private string _pinnedIncidentText = string.Empty;

        public Form11(ResponseUnit? selectedUnit = null)
        {
            InitializeComponent();
            _units = Form7.GlobalUnitList.ToList();

            // Use ArcGIS tiles to avoid OSM volunteer tile policy blocking (HTTP 418).
            mapControl.MapProvider = ArcGIS_World_Street_MapProvider.Instance;
            GMaps.Instance.Mode = AccessMode.ServerOnly;
            mapControl.Manager.Mode = AccessMode.ServerOnly;

            // Disable disk cache providers to avoid legacy serialization paths.
            mapControl.Manager.PrimaryCache = null;
            mapControl.Manager.SecondaryCache = null;

            // Disable legacy cache engines in this older GMap package that rely on BinaryFormatter.
            mapControl.Manager.UseRouteCache = false;
            mapControl.Manager.UseDirectionsCache = false;
            mapControl.Manager.UseGeocoderCache = false;
            mapControl.Manager.UsePlacemarkCache = false;
            mapControl.Manager.UseUrlCache = false;
            mapControl.Manager.UseMemoryCache = true;
            mapControl.CanDragMap = true;
            mapControl.DragButton = MouseButtons.Left;
            mapControl.MinZoom = 2;
            mapControl.MaxZoom = 20;
            mapControl.Zoom = 12;
            mapControl.Position = new PointLatLng(10.3157, 123.8854); // Cebu City default
            mapControl.Overlays.Add(_markersOverlay);
            mapControl.Overlays.Add(_routesOverlay);
            mapControl.MouseClick += mapControl_MouseClick;

            comboUnits.DisplayMember = "UnitName";
            comboUnits.ValueMember = "UnitID";
            comboUnits.DataSource = _units;

            if (selectedUnit != null)
            {
                ResponseUnit? matched = _units.FirstOrDefault(u => u.UnitID == selectedUnit.UnitID);
                if (matched != null)
                {
                    comboUnits.SelectedItem = matched;
                    txtIncidentLocation.Text = string.IsNullOrWhiteSpace(matched.IncidentLocation)
                        ? string.Empty
                        : matched.IncidentLocation;
                }
            }
        }

        private async void btnPlotRoute_Click(object sender, EventArgs e)
        {
            if (comboUnits.SelectedItem is not ResponseUnit selectedUnit)
            {
                MessageBox.Show("Please select a unit.", "Map", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string incidentText = txtIncidentLocation.Text.Trim();
            if (string.IsNullOrWhiteSpace(incidentText))
            {
                MessageBox.Show("Please enter an incident location.", "Map", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtIncidentLocation.Focus();
                return;
            }

            try
            {
                btnPlotRoute.Enabled = false;
                btnPlotRoute.Text = "Locating...";

                PointLatLng? unitPoint = await GeocodeAsync(selectedUnit.Location, null);
                PointLatLng? incidentPoint;

                if (_pinnedIncidentPoint != null &&
                    string.Equals(_pinnedIncidentText, incidentText, StringComparison.OrdinalIgnoreCase))
                {
                    incidentPoint = _pinnedIncidentPoint;
                }
                else
                {
                    incidentPoint = await GeocodeAsync(incidentText, unitPoint);
                }

                if (unitPoint == null)
                {
                    MessageBox.Show("Unable to locate the unit station address.", "Map", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (incidentPoint == null)
                {
                    MessageBox.Show("Unable to locate the incident address. Try adding barangay/city (example: Basak San Nicolas, Cebu City).", "Map", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                selectedUnit.IncidentLocation = incidentText;
                UnitDatabase.UpdateUnit(selectedUnit);

                DrawMarkers(selectedUnit, unitPoint.Value, incidentPoint.Value);
                await DrawRouteAsync(unitPoint.Value, incidentPoint.Value);

                mapControl.Position = incidentPoint.Value;
                mapControl.Zoom = Math.Max(mapControl.Zoom, 14);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Map request failed. {ex.Message}", "Map Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnPlotRoute.Enabled = true;
                btnPlotRoute.Text = "Plot Route";
            }
        }

        private void DrawMarkers(ResponseUnit unit, PointLatLng unitPoint, PointLatLng incidentPoint)
        {
            _markersOverlay.Markers.Clear();
            _routesOverlay.Routes.Clear();

            GMarkerGoogle unitMarker = new GMarkerGoogle(unitPoint, GMarkerGoogleType.blue_dot)
            {
                ToolTipText = $"Unit {unit.UnitID}: {unit.UnitName}\n{unit.Location}"
            };

            GMarkerGoogle incidentMarker = new GMarkerGoogle(incidentPoint, GMarkerGoogleType.red_dot)
            {
                ToolTipText = $"Incident\n{txtIncidentLocation.Text.Trim()}"
            };

            _markersOverlay.Markers.Add(unitMarker);
            _markersOverlay.Markers.Add(incidentMarker);
            mapControl.Refresh();
        }

        private async Task DrawRouteAsync(PointLatLng start, PointLatLng end)
        {
            string url = string.Format(
                CultureInfo.InvariantCulture,
                "https://router.project-osrm.org/route/v1/driving/{0},{1};{2},{3}?alternatives=true&overview=full&geometries=geojson&steps=false",
                start.Lng,
                start.Lat,
                end.Lng,
                end.Lat);

            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
            using HttpResponseMessage response = await Http.SendAsync(request);
            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();
            using JsonDocument doc = JsonDocument.Parse(json);
            JsonElement root = doc.RootElement;

            if (!root.TryGetProperty("routes", out JsonElement routes) || routes.GetArrayLength() == 0)
            {
                lblRouteInfo.Text = "Distance: - | ETA: -";
                return;
            }

            JsonElement? bestRoute = null;
            double bestDuration = double.MaxValue;
            double bestDistance = double.MaxValue;
            int bestRouteIndex = 0;

            int idx = 0;
            foreach (JsonElement route in routes.EnumerateArray())
            {
                idx++;
                double duration = route.GetProperty("duration").GetDouble();
                double distance = route.GetProperty("distance").GetDouble();

                if (duration < bestDuration || (Math.Abs(duration - bestDuration) < 0.1 && distance < bestDistance))
                {
                    bestDuration = duration;
                    bestDistance = distance;
                    bestRoute = route;
                    bestRouteIndex = idx;
                }
            }

            if (bestRoute == null)
            {
                lblRouteInfo.Text = "Distance: - | ETA: -";
                return;
            }

            List<PointLatLng> points = new List<PointLatLng>();
            foreach (JsonElement coordinate in bestRoute.Value.GetProperty("geometry").GetProperty("coordinates").EnumerateArray())
            {
                double lng = coordinate[0].GetDouble();
                double lat = coordinate[1].GetDouble();
                points.Add(new PointLatLng(lat, lng));
            }

            if (points.Count == 0)
            {
                return;
            }

            GMapRoute gRoute = new GMapRoute(points, "unitRoute")
            {
                Stroke = new System.Drawing.Pen(System.Drawing.Color.Red, 3)
            };

            _routesOverlay.Routes.Add(gRoute);
            mapControl.Position = end;
            mapControl.Zoom = Math.Max(mapControl.Zoom, 14);

            double distanceKm = bestDistance / 1000.0;
            double durationMin = bestDuration / 60.0;
            DateTime etaTime = DateTime.Now.AddSeconds(bestDuration);
            lblRouteInfo.Text = $"Fastest route #{bestRouteIndex} | Distance: {distanceKm:F2} km | ETA: {durationMin:F0} min ({etaTime:hh:mm tt})";
            mapControl.Refresh();
        }

        private async void mapControl_MouseClick(object? sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            PointLatLng clickedPoint = mapControl.FromLocalToLatLng(e.X, e.Y);
            _pinnedIncidentPoint = clickedPoint;

            string? resolved = await ReverseGeocodeAsync(clickedPoint);
            if (string.IsNullOrWhiteSpace(resolved))
            {
                resolved = string.Format(
                    CultureInfo.InvariantCulture,
                    "Pinned: {0:F6}, {1:F6}",
                    clickedPoint.Lat,
                    clickedPoint.Lng);
            }

            _pinnedIncidentText = resolved;
            txtIncidentLocation.Text = resolved;
            lblRouteInfo.Text = "Pinned incident point from map. Click Plot Route to compute fastest path.";
        }

        private async Task<string?> ReverseGeocodeAsync(PointLatLng point)
        {
            string url = string.Format(
                CultureInfo.InvariantCulture,
                "https://nominatim.openstreetmap.org/reverse?format=jsonv2&lat={0}&lon={1}",
                point.Lat,
                point.Lng);

            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.UserAgent.ParseAdd("Incri1GalangApp/1.0 (student project)");

            using HttpResponseMessage response = await Http.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            string json = await response.Content.ReadAsStringAsync();
            using JsonDocument doc = JsonDocument.Parse(json);

            if (doc.RootElement.TryGetProperty("display_name", out JsonElement displayName))
            {
                string? text = displayName.GetString();
                if (!string.IsNullOrWhiteSpace(text))
                {
                    return text;
                }
            }

            return null;
        }

        private async Task<PointLatLng?> GeocodeAsync(string query, PointLatLng? biasPoint)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return null;
            }

            foreach (string candidate in BuildQueryCandidates(query))
            {
                PointLatLng? point = await GeocodeSingleQueryAsync(candidate, biasPoint);
                if (point != null)
                {
                    return point;
                }
            }

            return null;
        }

        private async Task<PointLatLng?> GeocodeSingleQueryAsync(string query, PointLatLng? biasPoint)
        {
            string url = "https://nominatim.openstreetmap.org/search?format=json&addressdetails=1&limit=5&countrycodes=ph&q=" + Uri.EscapeDataString(query);

            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.UserAgent.ParseAdd("Incri1GalangApp/1.0 (student project)");

            using HttpResponseMessage response = await Http.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            string json = await response.Content.ReadAsStringAsync();
            using JsonDocument doc = JsonDocument.Parse(json);

            if (doc.RootElement.ValueKind != JsonValueKind.Array || doc.RootElement.GetArrayLength() == 0)
            {
                return null;
            }

            PointLatLng? bestPoint = null;
            double bestDistance = double.MaxValue;

            foreach (JsonElement item in doc.RootElement.EnumerateArray())
            {
                string latText = item.GetProperty("lat").GetString() ?? string.Empty;
                string lonText = item.GetProperty("lon").GetString() ?? string.Empty;

                if (!double.TryParse(latText, NumberStyles.Float, CultureInfo.InvariantCulture, out double lat))
                {
                    continue;
                }

                if (!double.TryParse(lonText, NumberStyles.Float, CultureInfo.InvariantCulture, out double lon))
                {
                    continue;
                }

                PointLatLng point = new PointLatLng(lat, lon);

                if (biasPoint == null)
                {
                    return point;
                }

                double distance = ComputeSquaredDistance(biasPoint.Value, point);
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    bestPoint = point;
                }
            }

            return bestPoint;
        }

        private static IEnumerable<string> BuildQueryCandidates(string query)
        {
            string normalized = query.Trim();
            normalized = normalized.Replace(" str ", " street ", StringComparison.OrdinalIgnoreCase);
            normalized = normalized.Replace(" st ", " street ", StringComparison.OrdinalIgnoreCase);

            List<string> candidates = new List<string>
            {
                normalized,
                normalized + ", Cebu City",
                normalized + ", Cebu, Philippines",
                normalized + ", Philippines"
            };

            return candidates.Distinct(StringComparer.OrdinalIgnoreCase);
        }

        private static double ComputeSquaredDistance(PointLatLng a, PointLatLng b)
        {
            double dLat = a.Lat - b.Lat;
            double dLng = a.Lng - b.Lng;
            return (dLat * dLat) + (dLng * dLng);
        }
    }
}
