using System.Diagnostics;

namespace QueryUELibrary
{
    /// <summary>
    /// The query form.
    /// </summary>
    public partial class QueryForm : Form
    {
        private static readonly string Version = "Version: 1.0, alwyn.j.dippenaar@gmail.com";

        /// <summary>
        /// Default constructor.
        /// </summary>
        public QueryForm()
        {
            InitializeComponent();
            StatusLabel.Text = Version;

            // Do the Query.
            QueryButton.Click += (sender, args) => { Query(); };

            // Goto URL.
            ScriptPathLabel.Click += (sender, args) =>
            {
                Process.Start(new ProcessStartInfo("cmd", $"/c start https://learn.microsoft.com/en-us/powershell/")
                    { CreateNoWindow = true });
            };
            QueryInputLabel.Click += (sender, args) =>
            {
                Process.Start(new ProcessStartInfo("cmd",
                        $"/c start https://learn.microsoft.com/en-us/powershell/module/microsoft.powershell.core/where-object")
                    { CreateNoWindow = true });
            };

            // Update the query text with some examples.
            ExamplesButton.Click += (sender, args) =>
            {
                JQQuery.Text =
                    "Remember, ONLY 1 Object-Where query is allowed at a time, the below is just examples of such queries, submitting this \"as is\" will result in an error\r\n" +
                    "# Look for all assets, containing the text \"spruce\", where the asset is a static mesh.\r\n" +
                    "$_.AssetPath -match \"spruce\" -and $_.AssetType -match \"staticmesh\"\r\n\r\n" +
                    
                    "# Look for all assets, where the size on disk, is greater than 100MB.\r\n" +
                    "$_.SizeOnDisk -gt 104857600\r\n\r\n" +
                    
                    "# Look for all assets, where the name regex matches combat and sword, where the asset type is an animation.\r\n" +
                    "$_.AssetPath -match \"slash.*sword\" -and $_.AssetType -match \"anim\"\r\n\r\n" +

                    "# More complex queries are also possible.\r\n" +
                    "($_.AssetPath -match 'armor' -and $_.AssetType -match 'static|skeletal') -or ($_.AssetPath -match 'combat' -and $_.AssetType -match 'anim')";
            };

            // html help.
            HTMLHelpButton.Click += (sender, args) =>
            {
                // help form, with centered label.
                Form myForm = new Form
                {
                    Text = "Json Structure",
                    Size = new Size(Screen.PrimaryScreen.Bounds.Width / 3, Screen.PrimaryScreen.Bounds.Height / 3),
                    StartPosition = FormStartPosition.CenterScreen
                };

                // Create a new WebBrowser control
                WebBrowser webBrowser = new WebBrowser
                {
                    DocumentText = File.ReadAllText("help.html"), // if it does not exists, then bomb
                    AllowNavigation = false,
                    Dock = DockStyle.Fill, //Dock the control
                    IsWebBrowserContextMenuEnabled = false,
                };

                myForm.Controls.Add(webBrowser);
                myForm.ShowDialog();
            };
        }

        /// <summary>
        /// Performs the query.
        /// </summary>
        private void Query()
        {
            Logging.Debug($"{GetType().Name}.Query:-- START");
            if (string.IsNullOrEmpty(JQQuery.Text))
            {
                JQQuery.Text = "Put some text here, use \"Examples\" if not sure.";
                return;
            }
            
            // Creating an instance of QueryJQ
            var queryJq = new QueryUELibrary.QueryJQ();

            // Display the dialog with "Busy..." text.
            var busyDialog = new System.Windows.Forms.Form()
            {
                ControlBox = true,
                Size = this.Size, // Same size as current form
                StartPosition = FormStartPosition.CenterScreen,
                Text = "Processing...",
                FormBorderStyle = FormBorderStyle.Sizable, // Make the form resizable
            };

            StatusStrip statusBar = new StatusStrip();
            ToolStripStatusLabel statusLabel = new ToolStripStatusLabel();
            statusBar.Items.Add(statusLabel);            
            busyDialog.Controls.Add(statusBar);
            
            // Set up split container
            var splitContainer = new SplitContainer { Dock = DockStyle.Fill, Orientation = Orientation.Horizontal, BorderStyle = BorderStyle.FixedSingle };
            splitContainer.SplitterDistance = (int)(busyDialog.Height * .75);

            // Set up result label and textbox
            var resultLabel = new Label { Text = "Result:", Dock = DockStyle.Top };
            var resultTextBox = new TextBox { Multiline = true, ReadOnly = true, Dock = DockStyle.Fill, ScrollBars = ScrollBars.Vertical };
            splitContainer.Panel1.Controls.AddRange(new Control[] { resultTextBox, resultLabel });

            // Set up error label and textbox
            var errorLabel = new Label { Text = "Error:", Dock = DockStyle.Top };
            var errorTextBox = new TextBox { Multiline = true, ReadOnly = true, Dock = DockStyle.Fill, ScrollBars = ScrollBars.Vertical };
            splitContainer.Panel2.Controls.AddRange(new Control[] { errorTextBox, errorLabel });

            busyDialog.Controls.Add(splitContainer);

            // Add a terminate button
            var terminate = new Button()
            {
                Text = "TERMINATE",
                Dock = DockStyle.Bottom,
            };
            terminate.Click += (s, e) => { Application.Exit(); };
            busyDialog.Controls.Add(terminate);
            
            // Add a Close button
            var closeButton = new Button()
            {
                Text = "Close",
                Dock = DockStyle.Bottom,
            };
            closeButton.Click += (s, e) => { busyDialog.Close(); };
            busyDialog.Controls.Add(closeButton);

            busyDialog.Shown += async (o, e) =>
            {
                var result = await queryJq.RunCommand(ScriptPath.Text, JQQuery.Text,
                    result => BeginInvoke((Action)(() =>
                    {
                        resultTextBox.Text += result + Environment.NewLine;
                        statusLabel.Text = $"Output: {ToReadableSize(resultTextBox.Text.Length)}, Error: {ToReadableSize(errorTextBox.Text.Length)}, Processing..";
                    })),
                    error => BeginInvoke((Action)(() =>
                    {
                        errorTextBox.Text += error + Environment.NewLine;
                        errorTextBox.SelectionStart = errorTextBox.Text.Length;
                        statusLabel.Text = $"Output: {ToReadableSize(resultTextBox.Text.Length)}, Error: {ToReadableSize(errorTextBox.Text.Length)}, Processing..";
                        errorTextBox.ScrollToCaret();
                    })),
                    () => BeginInvoke((Action)(() =>
                    {
                        statusLabel.Text = $"Output: {ToReadableSize(resultTextBox.Text.Length)}, Error: {ToReadableSize(errorTextBox.Text.Length)}, Completed.";
                    }))
                );

                resultTextBox.Text = result.Result;
                errorTextBox.Text = result.Error;

                busyDialog.Text = "Completed!";
            };

            busyDialog.ShowDialog();
        }
        
        /// <summary>
        /// Format number.
        /// </summary>
        public static string ToReadableSize(int length)
        {
            var sizes = new[] { "B", "KB", "MB", "GB" };
            double len = length;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len /= 1024;
            }
            return $"{len:0.##} {sizes[order]}".PadLeft(10);
        }        
    }
}