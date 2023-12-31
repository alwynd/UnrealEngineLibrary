using System.Collections.Concurrent;
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
        private static readonly int ItemsPerPage = 256;
        
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
        /// The results label.
        /// </summary>
        public readonly Label resultsLabel = new Label();

        /// <summary>
        /// The results panel.
        /// </summary>
        public readonly Panel resultsPanel = new Panel();
        
        /// <summary>
        /// The results scrolling panel.
        /// </summary>
        public readonly FlowLayoutPanel resultsScrollContainer = new FlowLayoutPanel();
        
        /// <summary>
        /// The help button.
        /// </summary>
        public readonly Button HTMLHelpButton = new Button();
        
        /// <summary>
        /// The previous page button.
        /// </summary>
        public readonly Button PrevButton = new Button();

        /// <summary>
        /// The next page button
        /// </summary>
        public readonly Button NextButton = new Button();

        /// <summary>
        /// The page index label.
        /// </summary>
        public readonly Label PageLabel = new Label();
        

        private Action QueryCompleted;
        
        private List<UEJson> UEObjects = new List<UEJson>();
        private List<UEJson> paginatedUEObjects = new List<UEJson>();
        
        private int currentPage = 1;                            // Simple pagination modification in OnQueryCompleted method
        private ConcurrentDictionary<int, List<CustomPanel>> cachedPanels = new ConcurrentDictionary<int, List<CustomPanel>>();

        // Color defined for Dark theme
        Color backColor = Color.FromArgb(37, 37, 38);
        Color foreColor = Color.FromArgb(204, 204, 204);
            
        Color buttonColor = Color.FromArgb(55, 55, 56);
        Color buttonForeColor = Color.FromArgb(204, 204, 204);            
        
        Color resultPanelbackColor = Color.FromArgb(37, 37, 38);
        Color resultPanelforeColor = Color.FromArgb(204, 204, 204);
        Color resultPanelhoverBorderColor = Color.White;

        
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
                Process.Start(new ProcessStartInfo("cmd", $"/c start https://learn.microsoft.com/en-us/powershell/") { CreateNoWindow = true });
            };
            QueryInputLabel.Click += (sender, args) =>
            {
                Process.Start(new ProcessStartInfo("cmd", $"/c start https://learn.microsoft.com/en-us/powershell/module/microsoft.powershell.core/where-object") { CreateNoWindow = true });
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
            
            
            NextButton.Click += (sender, args) =>
            {
                if (currentPage * ItemsPerPage < UEObjects.Count)
                {
                    currentPage++;
                    OnQueryCompleted();
                }
            };

            PrevButton.Click += (sender, args) =>
            {
                if (currentPage > 1)
                {
                    currentPage--;
                    OnQueryCompleted();
                }
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
            HTMLHelpButton.SetBounds(QueryButton.Right + 10, QueryButton.Top, 150, 55);
            this.Controls.Add(HTMLHelpButton);                        

            ExamplesButton.Text = "Examples";
            ExamplesButton.Font = new Font(QueryButton.Font, FontStyle.Bold);
            ExamplesButton.BackColor = buttonColor;
            ExamplesButton.ForeColor = buttonForeColor;            
            ExamplesButton.SetBounds(HTMLHelpButton.Right + 10, HTMLHelpButton.Top, 150, 55);
            this.Controls.Add(ExamplesButton);                        
            
            // Create a new label.
            // Set its background color to your preferred color. Using black here.
            // Set its height to 1 to make it thin like a divider.
            // Make it extend across the form.
            // Position it at the desired place. Set it in the middle of the form's height in this example.
            // Add it to the form.
            Label divider = new Label();
            divider.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            divider.BackColor = Color.White;
            divider.Height = 2;
            divider.Width = ClientSize.Width - 20;
            divider.Left = 10;
            divider.Top = ExamplesButton.Bottom + 15;
            this.Controls.Add(divider);            
            
            
            // Creating new label for "Results"
            resultsLabel.Text = "Results:";
            resultsLabel.Font = new Font(resultsLabel.Font, FontStyle.Bold);
            resultsLabel.ForeColor = foreColor;
            resultsLabel.BackColor = backColor;
            resultsLabel.AutoSize = true;
            resultsLabel.Left = 10;
            resultsLabel.Top = divider.Bottom + 30;
            //resultsLabel.SetBounds(10, ExamplesButton.Bottom + 15, ClientSize.Width - 20, 20); 
            this.Controls.Add(resultsLabel);

            // Add required button controls
            PrevButton.Text = "Prev";
            PrevButton.Font = new Font(QueryButton.Font, FontStyle.Bold);
            PrevButton.BackColor = buttonColor;
            PrevButton.ForeColor = buttonForeColor;            
            PrevButton.SetBounds(resultsLabel.Right + 10, resultsLabel.Top, 80, 25);
            this.Controls.Add(PrevButton);

            // Add required labels
            PageLabel.Text = "Page 1 of 1";
            PageLabel.Font = new Font(resultsLabel.Font, FontStyle.Bold);
            PageLabel.ForeColor = foreColor;
            PageLabel.BackColor = backColor;
            PageLabel.AutoSize = true;
            PageLabel.Left = PrevButton.Right + 10;
            PageLabel.Top = PrevButton.Top;
            //PageLabel.SetBounds(PrevButton.Right + 10, PrevButton.Top, ClientSize.Width - 20, 20); 
            this.Controls.Add(PageLabel);            

            
            NextButton.Text = "Next";
            NextButton.Font = new Font(QueryButton.Font, FontStyle.Bold);
            NextButton.BackColor = buttonColor;
            NextButton.ForeColor = buttonForeColor;            
            NextButton.SetBounds(PageLabel.Right + 10, PageLabel.Top, 80, 25);
            this.Controls.Add(NextButton);
            
            
            // Creating new scrollable panel for results
            // Add this line to give the panel a border
            // Replace this with a slightly darker color
            resultsPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            resultsPanel.SetBounds(10, PageLabel.Bottom + 30, ClientSize.Width - 20, ClientSize.Height - PageLabel.Bottom - 75);
            resultsPanel.BorderStyle = BorderStyle.FixedSingle;
            resultsPanel.BackColor = Color.FromArgb(27, 27, 28); // Darker shade of your existing backColor (37, 37, 38)
            resultsPanel.ForeColor = foreColor; 
            
            // Create a FlowLayoutPanel for vertical scrolling
            // Add the flowLayoutPanel to the resultsPanel
            resultsScrollContainer.Dock = DockStyle.Fill;
            resultsScrollContainer.AutoScroll = true;
            resultsScrollContainer.WrapContents = true;
            resultsScrollContainer.FlowDirection = FlowDirection.LeftToRight;
            resultsScrollContainer.AutoSize = false;
            resultsScrollContainer.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            resultsPanel.Controls.Add(resultsScrollContainer);            
            this.Controls.Add(resultsPanel);

            this.Shown += async (o, e) =>
            {
                await Task.Run(LoadImages);
            };
        }

        /// <summary>
        /// Loads the image paths.
        /// </summary>
        private async Task LoadImages()
        {
            try
            {
                UEProjectCSV.Initialize();
                await UEImageLibrary.Instance.Initialize(
                    progress => BeginInvoke((Action)(() =>
                    {
                        // update image counts, as they get loaded/discovered
                        StatusLabel.Text = $"{Version}, Discovering Thumbnails... Found {progress} Images...";
                    })),
                    () => BeginInvoke((Action)(() =>
                    {
                        // complete
                        StatusLabel.Text = $"{Version}, Found {UEImageLibrary.Instance.UELibraryImages.Count} Thumbnails.";
                    }))
                );
            }
            catch (Exception ex)
            {
                Logging.Error(ex, $"{GetType().Name}.LoadImages Error.");
                StatusLabel.Text = $"{Version}, Exception occurred while loading thumbnails, check log for details.";
                ErrorDialog errorDialog = new ErrorDialog($"Exception occurred while loading thumbnails: {ex}"); errorDialog.ShowDialog();
            }
        }

        /// <summary>
        /// The query completed.
        /// </summary>
        private async void OnQueryCompleted()
        {
            Logging.Debug($"{GetType().Name}.OnQueryCompleted:-- START");

            paginatedUEObjects = UEObjects.Skip((currentPage - 1) * ItemsPerPage).Take(ItemsPerPage).ToList();
            PageLabel.Text = $"Page {currentPage} of {Math.Ceiling((double)UEObjects.Count / ItemsPerPage)}";
            
            StatusLabel.Text = $"{Version}, Loading page: {currentPage} - {paginatedUEObjects.Count} Thumbnails";
            paginatedUEObjects.Sort((x, y) => string.Compare(x.AssetPath.ToLower().Split(".").Last(), y.AssetPath.ToLower().Split(".").Last()));

            // Clear/Remove any child components in the resultsScrollContainer
            resultsScrollContainer.Controls.Clear();
            await Task.Run(() =>
            {
                try
                {
                    var panels = PaginatePanels();
                    this.Invoke((Action)(() =>
                    {
                        foreach (var panel in panels)
                        {
                            resultsScrollContainer.Controls.Add(panel);
                        }
                        StatusLabel.Text = $"{Version}: Current Page: {panels.Count} items.";
                    }));
                }
                catch (Exception ex)
                {
                    Logging.Error(ex, $"{GetType().Name}.OnQueryCompleted Error.");
                    StatusLabel.Text = $"{Version}, Exception occurred while paginating thumbnails, check log for details.";
                    ErrorDialog errorDialog = new ErrorDialog($"Exception occurred while paginating thumbnails: {ex}"); errorDialog.ShowDialog();
                }
            });

            Logging.Debug($"{GetType().Name}.OnQueryCompleted:-- END");
        }

        /// <summary>
        /// Paginate the results.
        /// </summary>
        private List<CustomPanel> PaginatePanels()
        {
                List<CustomPanel> panels = null;
                if (cachedPanels.ContainsKey(currentPage)) return cachedPanels[currentPage]; 
                
                panels = PanelQuery();
                cachedPanels[currentPage] = panels; 
                return panels;
        }

        /// <summary>
        /// Panel query.
        /// </summary>
        private List<CustomPanel>? PanelQuery()
        {
            var panels = paginatedUEObjects.AsParallel().AsOrdered()
            .Select(ueObject =>
            {
                var matchingImagePath = UEImageLibrary.Instance.ImagePathsWithoutPng.FirstOrDefault(key =>
                {
                    return key.EndsWith(ueObject.AssetPath.ToLower().Replace("\\", "/"));
                }) + ".png";
                Logging.Debug($"{GetType().Name}.OnQueryCompleted FOUND: ueObject.AssetPath: {ueObject.AssetPath}, AssetType: {ueObject.AssetType}, SizeOnDisk: {FormatNumber(ueObject.SizeOnDisk)}, matchingImagePath: {matchingImagePath}");

                var panel = new CustomPanel
                {
                    Width = 256,
                    Height = 300,
                    BackColor = resultPanelbackColor,
                    ForeColor = resultPanelforeColor,
                };

                ToolTip toolTip = new ToolTip();  // Create a new ToolTip instance
                toolTip.SetToolTip(panel, ueObject.AssetPath);  // Set the tooltip text for the panel
                
                // create a new PictureBox
                var pictureBox = new PictureBox
                {
                    Image = Image.FromFile(matchingImagePath),
                    Width = panel.Width -
                            panel.BorderThickness * 2, // Lessen the width to allow for the custom border
                    Height = 256 - panel.BorderThickness * 2, // Lessen the height to allow for the custom border
                    BackColor = resultPanelbackColor,
                    BorderStyle = BorderStyle.None, // Set border to none as we're using a custom border
                    Location = new Point(panel.BorderThickness, panel.BorderThickness) // Shift the picture box inside to expose the custom border
                };

                // scope
                {
                    // find the URL for this image, based on the asset path content folder.
                    Logging.Debug($"{GetType().Name}.OnQueryCompleted.FINDURL: ueObject.AssetPath: {ueObject.AssetPath}");
                    string contentFolder = ueObject.AssetPath.Replace("\\", "/").Split("/")[2];
                    
                    Logging.Debug($"{GetType().Name}.OnQueryCompleted.FINDURL: ueObject.AssetPath: {ueObject.AssetPath}, contentFolder: {contentFolder}");
                    
                    //find the URL for this, if it has any
                    // log the URL
                    UELibrary project = UEProjectCSV.UELibraryProjects.FirstOrDefault(x => x.ContentFolder.ToLower().Trim() == contentFolder.ToLower().Trim());
                    Logging.Debug($"{GetType().Name}.OnQueryCompleted.FINDURL: ueObject.AssetPath: {ueObject.AssetPath}, contentFolder: {contentFolder}, URL: {project?.URL}");
                    toolTip.SetToolTip(pictureBox, $"{ueObject.AssetPath}{Environment.NewLine}URL: {project?.URL}");  // Set the tooltip text for the panel
                }

                panel.Controls.Add(pictureBox);

                var label = new Label
                {
                    Text = $"{ueObject.AssetPath.Replace("\\", "/").Split("/")[2]}\r\n{ueObject.AssetPath.Split(".").Last()}\r\n" +
                           $"{ueObject.AssetType.Replace("\\", "/").Split("/").Last().Split(".").Last()}, {FormatNumber(ueObject.SizeOnDisk)}",
                    Width = pictureBox.Width, Location = new Point(panel.BorderThickness, pictureBox.Height + panel.BorderThickness), // Position the label just below the PictureBox
                    AutoSize = true,
                    BackColor = resultPanelbackColor,
                    ForeColor = resultPanelforeColor,
                };
                

                // Attach hover event handlers to highlight border on hover
                Action<object, EventArgs> mouseEnter = (s, e) =>
                {
                    panel.BorderColor = resultPanelhoverBorderColor;
                    panel.Invalidate();
                    toolTip.Active = true;   // Activate the tooltip when the mouse enters
                };
                Action<object, EventArgs> mouseLeave = (s, e) =>
                {
                    panel.BorderColor = Color.Black;
                    panel.Invalidate();
                    toolTip.Active = false;   // Activate the tooltip when the mouse enters
                };

                panel.MouseEnter += new EventHandler(mouseEnter);
                pictureBox.MouseEnter += new EventHandler(mouseEnter);
                pictureBox.Click += (sender, e) =>
                {
                    // find the URL for this image, based on the asset path content folder.
                    Logging.Debug($"{GetType().Name}.OnQueryCompleted.Click: ueObject.AssetPath: {ueObject.AssetPath}");
                    string contentFolder = ueObject.AssetPath.Replace("\\", "/").Split("/")[2];
                    
                    Logging.Debug($"{GetType().Name}.OnQueryCompleted.Click: ueObject.AssetPath: {ueObject.AssetPath}, contentFolder: {contentFolder}");
                    
                    //find the URL for this, if it has any
                    // log the URL
                    UELibrary project = UEProjectCSV.UELibraryProjects.FirstOrDefault(x => x.ContentFolder.ToLower().Trim() == contentFolder.ToLower().Trim());
                    Logging.Debug($"{GetType().Name}.OnQueryCompleted.Click: ueObject.AssetPath: {ueObject.AssetPath}, contentFolder: {contentFolder}, URL: {project?.URL}");
                    if (project != default)
                    {
                        // open the URL
                        Process.Start(new ProcessStartInfo("cmd", $"/c start {project.URL}") { CreateNoWindow = true });
                    }
                };                    
                label.MouseEnter += new EventHandler(mouseEnter);

                panel.MouseLeave += new EventHandler(mouseLeave);
                pictureBox.MouseLeave += new EventHandler(mouseLeave);
                label.MouseLeave += new EventHandler(mouseLeave);

                panel.Controls.Add(label);
                panel.Height = pictureBox.Height + label.Height + panel.BorderThickness * 2; //Modify the panel height to fit the pictureBox and label
                return panel;
            }).ToList();

            return panels;
        }

        /// <summary>
        /// Performs the query.
        /// </summary>
        private void Query()
        {
            Logging.Debug($"{GetType().Name}.Query:-- START");
            if (!UEImageLibrary.Instance.Initialized)
            {
                JQQuery.Text = "Thumbnail paths are not completely loaded yet.";
                return;
            }
            if (UEImageLibrary.Instance.UELibraryImages.Count < 1)
            {
                JQQuery.Text = "Thumbnails repository is not on disk, check the UELibrary path/ini settings.";
                return;
            }
            
            if (string.IsNullOrEmpty(JQQuery.Text))
            {
                JQQuery.Text = "Put some text here, use \"Examples\" if not sure.";
                return;
            }
            
            // Creating an instance of QueryJQ
            var queryJq = new QueryUELibrary.QueryJQ();

            currentPage = 1;
            cachedPanels.Clear();
            
            // color scheme, externalize if you want to
            Color backColor = Color.FromArgb(37, 37, 38);
            Color foreColor = Color.FromArgb(204, 204, 204);

            Color buttonColor = Color.FromArgb(55, 55, 56);
            Color buttonForeColor = Color.FromArgb(204, 204, 204);            
            
            // Display the dialog with "Busy..." text.
            var busyDialog = new System.Windows.Forms.Form()
            {
                ControlBox = true,
                Size = this.Size/2, // Same size as current form
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
            var errorLabel = new Label { Text = "Messages:", Dock = DockStyle.Top };
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
                    null,
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
                        RemoveItemsWithNoImages();
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
        /// Removes items with no images.
        /// </summary>
        private void RemoveItemsWithNoImages()
        {
            Logging.Debug($"{GetType().Name}.RemoveItemsWithNoImages:-- START, UEObjects: {UEObjects.Count}");

            // Use PLINQ to perform operation in parallel
            UEObjects = UEObjects.AsParallel()
                .Where(ueObject =>
                {
                    string transformedAssetPath = ueObject.AssetPath.ToLower().Replace("\\", "/");
                    return UEImageLibrary.Instance.ImagePathsWithoutPng.Any(path => path.EndsWith(transformedAssetPath));
                })
                .ToList();
            Logging.Debug($"{GetType().Name}.RemoveItemsWithNoImages DONE: UEObjects: {UEObjects.Count}");            
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
        private static string FormatNumber(long length)
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
    
    /// <summary>
    /// meh
    /// </summary>
    public class ErrorDialog : Form
    {
        /// <summary>
        /// more meh
        /// </summary>
        public ErrorDialog(string errorMessage)
        {
            // Dialog title and size
            Text = "Error";
            ClientSize = new Size(800, 600);
            StartPosition = FormStartPosition.CenterScreen;

            // RichTextBox to display the error message
            RichTextBox messageBox = new RichTextBox
            {
                ReadOnly = true,
                Dock = DockStyle.Fill,
                Text = errorMessage,
                BackColor = SystemColors.Control,
                BorderStyle = BorderStyle.None,
                ScrollBars = RichTextBoxScrollBars.Vertical,
                ContextMenuStrip = new ContextMenuStrip()
            };            

            // "Close" Button to close the dialog
            Button closeButton = new Button
            {
                Text = "Close",
                Dock = DockStyle.Bottom,
                TextAlign = ContentAlignment.MiddleCenter,
                Margin = new Padding(5)
            };

            closeButton.Click += (sender, e) => Close();
        
            // Add controls to form
            Controls.Add(messageBox);
            Controls.Add(closeButton);

            // Set the icon to the error icon
            Icon = SystemIcons.Error;

            // Allow copying the message
            messageBox.ContextMenuStrip.Items.Add("Copy", null, (_, _) => Clipboard.SetText(messageBox.Text));
        }
    }
    
    /// <summary>
    /// Custom panel.
    /// </summary>
    public class CustomPanel : Panel
    {
        /// <summary>
        /// A custom border color.
        /// </summary>
        public Color BorderColor { get; set; } = Color.Black;
        
        /// <summary>
        /// The border thickness.
        /// </summary>
        public int BorderThickness { get; set; } = 2;

        /// <inheritdoc/>
        protected override void OnPaint(PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, e.ClipRectangle, 
                this.BorderColor, this.BorderThickness, ButtonBorderStyle.Solid,
                this.BorderColor, this.BorderThickness, ButtonBorderStyle.Solid,
                this.BorderColor, this.BorderThickness, ButtonBorderStyle.Solid,
                this.BorderColor, this.BorderThickness, ButtonBorderStyle.Solid);
        }
    }    
}