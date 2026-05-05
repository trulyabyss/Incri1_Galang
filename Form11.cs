using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;

namespace Incri1_Galang
{
    public partial class Form11 : Form
    {
        private const string OsmUserAgent = "Incri1GalangApp/1.0 (mailto:mvincenttttt@gmail.com)";
        private const string MapHtmlTemplate = "<!DOCTYPE html>\n" +
            "<html>\n" +
            "<head>\n" +
            "  <meta charset='utf-8' />\n" +
            "  <meta name='viewport' content='initial-scale=1, maximum-scale=1, user-scalable=no' />\n" +
            "  <link rel='stylesheet' href='https://unpkg.com/leaflet@1.9.4/dist/leaflet.css' " +
            "integrity='sha256-p4NxAoJBhIIN+hmNHrzRCf9tD/miZyoHS5obTRR9BMY=' crossorigin='' />\n" +
            "  <style>html, body, #map { height: 100%; margin: 0; }</style>\n" +
            "</head>\n" +
            "<body>\n" +
            "  <div id='map'></div>\n" +
            "  <script src='https://unpkg.com/leaflet@1.9.4/dist/leaflet.js' " +
            "integrity='sha256-20nQCchB9co0qIjJZRGuk2/Z9VM+kNiyxNV1lvTlZBo=' crossorigin=''></script>\n" +
            "  <script>\n" +
            "    const map = L.map('map').setView([10.3157, 123.8854], 12);\n" +
            "    L.tileLayer('https://api.maptiler.com/maps/streets/{z}/{x}/{y}.png?key=__MAPTILER_KEY__', { maxZoom: 20, attribution: '&copy; OpenStreetMap contributors' }).addTo(map);\n" +
            "    let marker = null;\n" +
            "    window.setMarker = function(lat, lng, label) {\n" +
            "      if (marker) { marker.setLatLng([lat, lng]); } else { marker = L.marker([lat, lng]).addTo(map); }\n" +
            "      if (label) { marker.bindPopup(label).openPopup(); }\n" +
            "    };\n" +
            "    window.centerMap = function(lat, lng, zoom) {\n" +
            "      const z = zoom || map.getZoom();\n" +
            "      map.setView([lat, lng], z);\n" +
            "    };\n" +
            "    map.on('click', function(e) {\n" +
            "      if (window.chrome && window.chrome.webview) {\n" +
            "        window.chrome.webview.postMessage({ type: 'mapClick', lat: e.latlng.lat, lng: e.latlng.lng });\n" +
            "      }\n" +
            "    });\n" +
            "  </script>\n" +
            "</body>\n" +
            "</html>";

        private static readonly HttpClient Http = new HttpClient();
        private readonly List<ResponseUnit> _units;
        private GeoPoint? _pinnedIncidentPoint;
        private string _pinnedIncidentText = string.Empty;
        private bool _mapReady;

        private readonly record struct GeoPoint(double Lat, double Lng);

        public Form11(ResponseUnit? selectedUnit = null)
        {
            InitializeComponent();
            _units = Form7.GlobalUnitList.ToList();

            lblRouteInfo.Text = "Click the map or press Enter to pin a location.";
            txtIncidentLocation.KeyDown += HandleIncidentLocationKeyDown;

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

            Shown += HandleFormShown;
        }

        private async void HandleFormShown(object? sender, EventArgs e)
        {
            await InitializeMapAsync();
        }

        private async Task InitializeMapAsync()
        {
            try
            {
                await mapView.EnsureCoreWebView2Async();
                mapView.CoreWebView2.WebMessageReceived += HandleWebMessageReceived;

                string mapTilerApiKey = LoadMapTilerApiKey();
                if (string.IsNullOrWhiteSpace(mapTilerApiKey))
                {
                    MessageBox.Show("Map tiles require a MapTiler API key. Set MAPTILER_API_KEY or appsettings.json -> MapTiler:ApiKey.",
                        "Map Setup", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                mapView.NavigateToString(BuildMapHtml(mapTilerApiKey));
                _mapReady = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to initialize the map view. " + ex.Message,
                    "Map Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void HandleIncidentLocationKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            e.Handled = true;
            e.SuppressKeyPress = true;

            _ = PinLocationFromTextAsync();
        }

        private ResponseUnit? GetSelectedUnitOrWarn()
        {
            if (comboUnits.SelectedItem is not ResponseUnit selectedUnit)
            {
                MessageBox.Show("Please select a unit to save the incident.", "Map", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            return selectedUnit;
        }

        private async Task PinLocationFromTextAsync()
        {
            ResponseUnit? selectedUnit = GetSelectedUnitOrWarn();
            if (selectedUnit == null)
            {
                return;
            }

            string incidentText = txtIncidentLocation.Text.Trim();
            if (string.IsNullOrWhiteSpace(incidentText))
            {
                MessageBox.Show("Please enter a landmark to pin.", "Map", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtIncidentLocation.Focus();
                return;
            }

            try
            {
                lblRouteInfo.Text = "Pinning location...";

                GeoPoint? biasPoint = null;
                if (!string.IsNullOrWhiteSpace(selectedUnit.Location))
                {
                    biasPoint = await GeocodeAsync(selectedUnit.Location, null);
                }

                GeoPoint? incidentPoint;

                if (_pinnedIncidentPoint != null &&
                    string.Equals(_pinnedIncidentText, incidentText, StringComparison.OrdinalIgnoreCase))
                {
                    incidentPoint = _pinnedIncidentPoint;
                }
                else
                {
                    incidentPoint = await GeocodeAsync(incidentText, biasPoint);
                }

                if (incidentPoint == null)
                {
                    MessageBox.Show("Unable to locate the incident address. Try adding barangay/city (example: Basak San Nicolas, Cebu City).", "Map", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _pinnedIncidentPoint = incidentPoint;
                _pinnedIncidentText = incidentText;

                selectedUnit.IncidentLocation = incidentText;
                UnitDatabase.UpdateUnit(selectedUnit);

                await SetMarkerAsync(incidentPoint.Value, incidentText);

                lblRouteInfo.Text = "Pinned location on map.";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Map request failed. {ex.Message}", "Map Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void HandleWebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                using JsonDocument doc = JsonDocument.Parse(e.WebMessageAsJson);
                if (!doc.RootElement.TryGetProperty("type", out JsonElement typeElement))
                {
                    return;
                }

                string? type = typeElement.GetString();
                if (!string.Equals(type, "mapClick", StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                double lat = doc.RootElement.GetProperty("lat").GetDouble();
                double lng = doc.RootElement.GetProperty("lng").GetDouble();
                await HandleMapClickAsync(new GeoPoint(lat, lng));
            }
            catch
            {
                // Ignore malformed messages.
            }
        }

        private async Task HandleMapClickAsync(GeoPoint point)
        {
            _pinnedIncidentPoint = point;

            string? resolved = await ReverseGeocodeAsync(point);
            if (string.IsNullOrWhiteSpace(resolved))
            {
                resolved = string.Format(CultureInfo.InvariantCulture, "Pinned: {0:F6}, {1:F6}", point.Lat, point.Lng);
            }

            _pinnedIncidentText = resolved;
            txtIncidentLocation.Text = resolved;

            ResponseUnit? selectedUnit = GetSelectedUnitOrWarn();
            if (selectedUnit != null)
            {
                selectedUnit.IncidentLocation = resolved;
                UnitDatabase.UpdateUnit(selectedUnit);
            }

            await SetMarkerAsync(point, resolved);
            lblRouteInfo.Text = "Pinned location on map.";
        }

        private async Task SetMarkerAsync(GeoPoint point, string label)
        {
            if (!_mapReady || mapView.CoreWebView2 == null)
            {
                return;
            }

            string lat = point.Lat.ToString(CultureInfo.InvariantCulture);
            string lng = point.Lng.ToString(CultureInfo.InvariantCulture);
            string safeLabel = JsonSerializer.Serialize(label ?? string.Empty);
            string script = $"window.setMarker({lat}, {lng}, {safeLabel}); window.centerMap({lat}, {lng}, 14);";
            await mapView.CoreWebView2.ExecuteScriptAsync(script);
        }

        private async Task<string?> ReverseGeocodeAsync(GeoPoint point)
        {
            string url = string.Format(
                CultureInfo.InvariantCulture,
                "https://nominatim.openstreetmap.org/reverse?format=jsonv2&lat={0}&lon={1}",
                point.Lat,
                point.Lng);

            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.UserAgent.ParseAdd(OsmUserAgent);

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

        private async Task<GeoPoint?> GeocodeAsync(string query, GeoPoint? biasPoint)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return null;
            }

            foreach (string candidate in BuildQueryCandidates(query))
            {
                GeoPoint? point = await GeocodeSingleQueryAsync(candidate, biasPoint);
                if (point != null)
                {
                    return point;
                }
            }

            return null;
        }

        private async Task<GeoPoint?> GeocodeSingleQueryAsync(string query, GeoPoint? biasPoint)
        {
            string url = "https://nominatim.openstreetmap.org/search?format=json&addressdetails=1&limit=5&countrycodes=ph&q=" + Uri.EscapeDataString(query);

            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.UserAgent.ParseAdd(OsmUserAgent);

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

            GeoPoint? bestPoint = null;
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

                GeoPoint point = new GeoPoint(lat, lon);

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

        private static double ComputeSquaredDistance(GeoPoint a, GeoPoint b)
        {
            double dLat = a.Lat - b.Lat;
            double dLng = a.Lng - b.Lng;
            return (dLat * dLat) + (dLng * dLng);
        }

        private static string BuildMapHtml(string apiKey)
        {
            string safeKey = Uri.EscapeDataString(apiKey ?? string.Empty);
            return MapHtmlTemplate.Replace("__MAPTILER_KEY__", safeKey);
        }

        private static string LoadMapTilerApiKey()
        {
            string? envKey = Environment.GetEnvironmentVariable("MAPTILER_API_KEY");
            if (!string.IsNullOrWhiteSpace(envKey))
            {
                return envKey.Trim();
            }

            string settingsPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
            if (!File.Exists(settingsPath))
            {
                return string.Empty;
            }

            try
            {
                using FileStream stream = File.OpenRead(settingsPath);
                using JsonDocument doc = JsonDocument.Parse(stream);
                if (doc.RootElement.TryGetProperty("MapTiler", out JsonElement mapTiler) &&
                    mapTiler.TryGetProperty("ApiKey", out JsonElement apiKey))
                {
                    return apiKey.GetString()?.Trim() ?? string.Empty;
                }
            }
            catch
            {
                return string.Empty;
            }

            return string.Empty;
        }
    }

}
