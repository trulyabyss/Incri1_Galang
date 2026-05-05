using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Incri1_Galang
{
    internal enum ChatRole
    {
        Resident,
        Admin,
        Barangay
    }

    internal sealed class ChatMessage
    {
        public ChatMessage(DateTime timestamp, ChatRole role, string text)
        {
            Timestamp = timestamp;
            Role = role;
            Text = text;
        }

        public DateTime Timestamp { get; }
        public ChatRole Role { get; }
        public string Text { get; }
        public string DisplayText => $"{Timestamp:HH:mm} {Role}: {Text}";
    }

    internal static class ChatStore
    {
        public static BindingList<ChatMessage> Messages { get; } = new BindingList<ChatMessage>();

        public static void AddMessage(ChatMessage message)
        {
            Messages.Add(message);
        }

        public static void RemoveMessage(ChatMessage message)
        {
            Messages.Remove(message);
        }

        public static void ClearMessages()
        {
            Messages.Clear();
        }
    }

    internal sealed class ChatForm : Form
    {
        private readonly ChatRole _role;
        private readonly ListBox _messageList;
        private readonly TextBox _inputBox;
        private readonly Button _sendButton;
        private readonly Button? _deleteButton;

        public ChatForm(ChatRole role)
        {
            _role = role;

            Text = $"Chat - {role}";
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(420, 360);

            _messageList = new ListBox
            {
                Dock = DockStyle.Fill,
                IntegralHeight = false
            };

            Panel inputPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 52,
                Padding = new Padding(8)
            };

            _inputBox = new TextBox
            {
                Dock = DockStyle.Fill
            };

            _sendButton = new Button
            {
                Text = "Send",
                Dock = DockStyle.Right,
                Width = 80
            };

            inputPanel.Controls.Add(_inputBox);

            if (_role == ChatRole.Admin)
            {
                _deleteButton = new Button
                {
                    Text = "Delete",
                    Dock = DockStyle.Right,
                    Width = 80
                };

                _deleteButton.Click += (_, _) => HandleDeleteClicked();
                inputPanel.Controls.Add(_deleteButton);
            }

            inputPanel.Controls.Add(_sendButton);

            Controls.Add(_messageList);
            Controls.Add(inputPanel);

            _messageList.DataSource = ChatStore.Messages;
            _messageList.DisplayMember = nameof(ChatMessage.DisplayText);

            _sendButton.Click += (_, _) => SendMessage();
            _inputBox.KeyDown += HandleInputKeyDown;

            ChatStore.Messages.ListChanged += HandleMessagesChanged;
            FormClosed += HandleFormClosed;

            AcceptButton = _sendButton;
        }

        private void HandleDeleteClicked()
        {
            if (_role != ChatRole.Admin)
            {
                return;
            }

            if (ChatStore.Messages.Count == 0)
            {
                MessageBox.Show("No messages to delete.", "Delete Chat",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult choice = MessageBox.Show(
                "Delete selected message?\nYes = delete selected message\nNo = delete entire conversation\nCancel = do nothing",
                "Delete Chat",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question);

            if (choice == DialogResult.Yes)
            {
                DeleteSelectedMessage();
            }
            else if (choice == DialogResult.No)
            {
                DeleteConversation();
            }
        }

        private void DeleteSelectedMessage()
        {
            if (_messageList.SelectedItem is ChatMessage selected)
            {
                ChatStore.RemoveMessage(selected);
                return;
            }

            MessageBox.Show("Select a message to delete first.", "Delete Chat",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void DeleteConversation()
        {
            ChatStore.ClearMessages();
        }

        private void HandleFormClosed(object? sender, FormClosedEventArgs e)
        {
            ChatStore.Messages.ListChanged -= HandleMessagesChanged;
            FormClosed -= HandleFormClosed;
        }

        private void HandleInputKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }

            e.SuppressKeyPress = true;
            SendMessage();
        }

        private void HandleMessagesChanged(object? sender, ListChangedEventArgs e)
        {
            if (ChatStore.Messages.Count == 0 || !IsHandleCreated || IsDisposed)
            {
                return;
            }

            BeginInvoke(new Action(() =>
            {
                if (!IsDisposed)
                {
                    _messageList.SelectedIndex = ChatStore.Messages.Count - 1;
                }
            }));
        }

        private void SendMessage()
        {
            string text = _inputBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            ChatStore.AddMessage(new ChatMessage(DateTime.Now, _role, text));
            _inputBox.Clear();
            _inputBox.Focus();
        }
    }
}
