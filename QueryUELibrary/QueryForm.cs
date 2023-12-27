using System.Diagnostics;
using System.Text.Json;

namespace QueryUELibrary
{
    /// <summary>
    /// The query form.
    /// </summary>
    public partial class QueryForm : Form
    {
        private static readonly string Version = "Version: 1.0, alwyn.j.dippenaar@gmail.com";
        
        
        /// <summary>
        /// Query Button
        /// </summary>
        public readonly Button QueryButton = new Button();

        /// <summary>
        /// The path to the script.
        /// </summary>
        public readonly TextBox ScriptPath = new TextBox();

        /// <summary>
        /// The JQ query.
        /// </summary>
        public readonly TextBox JQQuery = new TextBox();

        /// <summary>
        /// The examples button.
        /// </summary>
        public readonly Button ExamplesButton = new Button();

        /// <summary>
        /// The label for Script path.
        /// </summary>
        public readonly Label ScriptPathLabel = new Label();

        /// <summary>
        /// The query input label. 
        /// </summary>
        public readonly Label QueryInputLabel = new Label();

        /// <summary>
        /// The status bar windows nonsense.
        /// </summary>
        public readonly StatusStrip StatusBar = new StatusStrip();
        
        /// <summary>
        /// The status bar windows nonsense.
        /// </summary>
        public readonly ToolStripStatusLabel StatusLabel = new ToolStripStatusLabel();
        
        /// <summary>
        /// The help button.
        /// </summary>
        public readonly Button HTMLHelpButton = new Button();
        

        private List<UEJson> UEObjects = new List<UEJson>();

        private Action QueryCompleted;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public QueryForm()
        {
            InitializeComponent();
            AddControls();
            
            StatusLabel.Text = Version;
            QueryCompleted += OnQueryCompleted;

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
        /// Adds the controls.
        /// </summary>
        private void AddControls()
        {
            // 
            // QueryForm
            // 
            // Color defined for Dark theme
            Color backColor = Color.FromArgb(37, 37, 38);
            Color foreColor = Color.FromArgb(204, 204, 204);
            
            Color buttonColor = Color.FromArgb(55, 55, 56);
            Color buttonForeColor = Color.FromArgb(204, 204, 204);            
            
            this.BackColor = backColor;
            this.ForeColor = foreColor;

            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Size primaryDisplaySize = Screen.PrimaryScreen.Bounds.Size;
            this.ClientSize = new Size(primaryDisplaySize.Width / 2, primaryDisplaySize.Height / 2);
            this.StartPosition = FormStartPosition.CenterScreen;            
            Name = "QueryForm";
            Text = "Query UELibrary";

            StatusBar.BackColor = buttonColor;
            StatusBar.ForeColor = buttonForeColor;            
            StatusBar.Items.Add(StatusLabel);            
            this.Controls.Add(StatusBar);            

            // Creating new "hyperlinked" label and text box for "Path to Powershell script"
            ScriptPathLabel.Text = "Path to Powershell Script:";
            ScriptPathLabel.Font = new Font(ScriptPathLabel.Font, FontStyle.Bold | FontStyle.Underline);
            ScriptPathLabel.ForeColor = foreColor;
            ScriptPathLabel.SetBounds(10, 35, 800, 20);
            ScriptPathLabel.Cursor = Cursors.Hand;
            ScriptPathLabel.BackColor = backColor;
            
            ScriptPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            ScriptPath.SetBounds(10, 55, ClientSize.Width - 20, 20);
            ScriptPath.Text = "query.ps1";
            ScriptPath.BackColor = backColor;
            ScriptPath.ForeColor = foreColor;            
            this.Controls.Add(ScriptPathLabel);
            this.Controls.Add(ScriptPath);            

            // Creating new label and text area for the input area
            QueryInputLabel.Text = "PS Where-Object Query: (replaces {QUERY_JQ} in the script)";
            QueryInputLabel.Font = new Font(ScriptPathLabel.Font, FontStyle.Bold | FontStyle.Underline);
            QueryInputLabel.ForeColor = foreColor;
            QueryInputLabel.BackColor = backColor;
            QueryInputLabel.Cursor = Cursors.Hand;
            QueryInputLabel.SetBounds(10, 85, 150, 20);
            
            JQQuery.Anchor = AnchorStyles.Top | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            JQQuery.Multiline = true;
            JQQuery.ScrollBars = ScrollBars.Vertical;
            JQQuery.BackColor = backColor;
            JQQuery.ForeColor = foreColor;            
            JQQuery.SetBounds(10, 105, ClientSize.Width - 20, 100);

            // Add new controls to the form
            this.Controls.Add(QueryInputLabel);
            this.Controls.Add(JQQuery);            
            
            // New y-coordinate for the button
            int btnPos = JQQuery.Bottom + 15; 
    
            // Create the 'Query' Button
            // Set the y-coordinate to the variable
            // Add the Query button to the form
            QueryButton.Text = "Query";
            QueryButton.Font = new Font(QueryButton.Font, FontStyle.Bold);
            QueryButton.BackColor = buttonColor;
            QueryButton.ForeColor = buttonForeColor;            
            QueryButton.SetBounds(10, btnPos, 150, 55); 
            this.Controls.Add(QueryButton);                        

            // Create the Button
            HTMLHelpButton.Text = "Json";
            HTMLHelpButton.Font = new Font(QueryButton.Font, FontStyle.Bold);
            HTMLHelpButton.BackColor = buttonColor;
            HTMLHelpButton.ForeColor = buttonForeColor;            
            HTMLHelpButton.SetBounds(QueryButton.Right + 10, QueryButton.Top, 100, 45);
            this.Controls.Add(HTMLHelpButton);                        

            ExamplesButton.Text = "Examples";
            ExamplesButton.Font = new Font(QueryButton.Font, FontStyle.Bold);
            ExamplesButton.BackColor = buttonColor;
            ExamplesButton.ForeColor = buttonForeColor;            
            ExamplesButton.SetBounds(HTMLHelpButton.Right + 10, HTMLHelpButton.Top, 100, 45);
            this.Controls.Add(ExamplesButton);                        
            
        }

        /// <summary>
        /// Indicates the query has completed.
        /// </summary>
        private void OnQueryCompleted()
        {
            Logging.Debug($"{GetType().Name}.OnQueryCompleted:-- START");
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

            // color scheme, externalize if you want to
            Color backColor = Color.FromArgb(37, 37, 38);
            Color foreColor = Color.FromArgb(204, 204, 204);

            Color buttonColor = Color.FromArgb(55, 55, 56);
            Color buttonForeColor = Color.FromArgb(204, 204, 204);            
            
            // Display the dialog with "Busy..." text.
            var busyDialog = new System.Windows.Forms.Form()
            {
                ControlBox = true,
                Size = this.Size, // Same size as current form
                StartPosition = FormStartPosition.CenterScreen,
                Text = "Processing...",
                FormBorderStyle = FormBorderStyle.Sizable, // Make the form resizable
                BackColor = backColor,
                ForeColor = foreColor,
            };

            // controls.
            StatusStrip statusBar = new StatusStrip();
            ToolStripStatusLabel statusLabel = new ToolStripStatusLabel();
            statusBar.BackColor = buttonColor;
            statusBar.ForeColor = buttonForeColor;            
            
            statusBar.Items.Add(statusLabel);            
            busyDialog.Controls.Add(statusBar);
            
            // Set up split container
            var splitContainer = new SplitContainer { Dock = DockStyle.Fill, Orientation = Orientation.Horizontal, BorderStyle = BorderStyle.FixedSingle };
            splitContainer.SplitterDistance = (int)(busyDialog.Height * .75);

            // Set up result label and textbox
            var resultLabel = new Label { Text = "Result:", Dock = DockStyle.Top };
            resultLabel.BackColor = backColor;
            resultLabel.ForeColor = foreColor;            
            
            var resultTextBox = new TextBox { Multiline = true, ReadOnly = true, Dock = DockStyle.Fill, ScrollBars = ScrollBars.Vertical };
            resultTextBox.BackColor = backColor;
            resultTextBox.ForeColor = foreColor;            
            splitContainer.Panel1.Controls.AddRange(new Control[] { resultTextBox, resultLabel });

            // Set up error label and textbox
            var errorLabel = new Label { Text = "Error:", Dock = DockStyle.Top };
            errorLabel.BackColor = backColor;
            errorLabel.ForeColor = foreColor;            
            
            var errorTextBox = new TextBox { Multiline = true, ReadOnly = true, Dock = DockStyle.Fill, ScrollBars = ScrollBars.Vertical };
            errorTextBox.BackColor = backColor;
            errorTextBox.ForeColor = foreColor;            
            
            splitContainer.Panel2.Controls.AddRange(new Control[] { errorTextBox, errorLabel });
            busyDialog.Controls.Add(splitContainer);

            // Add a terminate button
            var terminate = new Button()
            {
                Text = "TERMINATE",
                Dock = DockStyle.Bottom,
            };
            terminate.BackColor = buttonColor;
            terminate.ForeColor = buttonForeColor;            
            terminate.Click += (s, e) => { Application.Exit(); };
            busyDialog.Controls.Add(terminate);
            
            // Add a Close button
            var closeButton = new Button()
            {
                Text = "Close",
                Dock = DockStyle.Bottom,
            };
            closeButton.BackColor = buttonColor;
            closeButton.ForeColor = buttonForeColor;            
            closeButton.Click += (s, e) => { busyDialog.Close(); };
            busyDialog.Controls.Add(closeButton);

            // hook into UI thread on Shown
            busyDialog.Shown += async (o, e) =>
            {
                // run the query, update the text as we get it back
                var result = await queryJq.RunCommand(ScriptPath.Text, JQQuery.Text,
                    result => BeginInvoke((Action)(() =>
                    {
                        resultTextBox.Text += result + Environment.NewLine;
                        statusLabel.Text = $"Output: {FormatNumber(resultTextBox.Text.Length)}, Error: {FormatNumber(errorTextBox.Text.Length)}, Processing..";
                    })),
                    error => BeginInvoke((Action)(() =>
                    {
                        statusLabel.Text = $"Output: {FormatNumber(resultTextBox.Text.Length)}, Error: {FormatNumber(errorTextBox.Text.Length)}, Processing..";
                        AppendAndScrollToEnd(errorTextBox, error);
                    })), null
                );

                // done, update finals
                resultTextBox.Text = result.Result;
                errorTextBox.Text = result.Error + Environment.NewLine;
                
                Logging.Debug($"{GetType().Name}.Query Completed.... checking result JSON");
                busyDialog.Text = "Completed!";

                // attempt to parse result
                try
                {
                    if (!string.IsNullOrEmpty(resultTextBox.Text))
                    {
                        // good
                        Logging.Debug($"{GetType().Name}.Query Attempting to parse JSON: {resultTextBox.Text.Length}");
                        UEObjects.Clear();
                        UEObjects = JsonSerializer.Deserialize<List<UEJson>>(resultTextBox.Text);
                        
                        string msg = $"Parsed JSON OK: Size: {FormatNumber(resultTextBox.Text.Length)}, JSON Objects: {UEObjects.Count}";
                        Logging.Debug($"{GetType().Name}.Query result: {msg}");
                        AppendAndScrollToEnd(errorTextBox, msg);
                        statusLabel.Text = $"Output: {FormatNumber(resultTextBox.Text.Length)}, Error: {FormatNumber(errorTextBox.Text.Length)}, OK, {UEObjects.Count} Matches.";
                        QueryCompleted?.Invoke();
                    }
                    else
                    {
                        // bad
                        string msg = $"No result text to PARSE. JSON Parse failed.";
                        Logging.Debug($"{GetType().Name}.Query result fail(result null): {msg}");
                        AppendAndScrollToEnd(errorTextBox, msg);
                        statusLabel.Text = $"Output: {FormatNumber(resultTextBox.Text.Length)}, Error: {FormatNumber(errorTextBox.Text.Length)}, Failed: Result empty.";
                    }
                }
                catch (Exception ex)
                {
                    // ugly
                    string msg = $"Parsed JSON Error: Size: {FormatNumber(resultTextBox.Text.Length)}, {ex}";
                    Logging.Error(ex, $"{GetType().Name}.Query: {msg}");
                    AppendAndScrollToEnd(errorTextBox, msg);
                    statusLabel.Text = $"Output: {FormatNumber(resultTextBox.Text.Length)}, Error: {FormatNumber(errorTextBox.Text.Length)}, Failed: Error.";
                }
            };

            // show the thing.
            busyDialog.ShowDialog();
        }

        /// <summary>
        /// Scrolls to end.
        /// </summary>
        private void AppendAndScrollToEnd(TextBox textBox, string text)
        {
            textBox.Text += text + Environment.NewLine;
            textBox.SelectionStart = textBox.Text.Length;
            textBox.ScrollToCaret();
        }

        /// <summary>
        /// Format number.
        /// </summary>
        private static string FormatNumber(int length)
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