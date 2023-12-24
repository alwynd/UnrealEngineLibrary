using System.Diagnostics;

namespace QueryUELibrary
{
    partial class QueryForm
    {

        public readonly static string HelpSnippet = "<!DOCTYPE html>\n<html>\n<head>\n<style>\nbody {\nbackground-color: black;\ncolor: #D3D3D3;\nfont-family: Arial, sans-serif;\n}\nh1 {\nfont-weight: bold;\n}\n.code-snippet {\nbackground-color: #2A2A2A;\nborder: solid thin white;\npadding: 20px;\nwhite-space: pre;\ncolor: #D3D3D3;\n}\n</style>\n</head>\n<body>\n<h1>Unreal Engine Library JSON Structure</h1>\n<div class=\"code-snippet\">\n\"/Game/Characters/Mannequins/Meshes/SK_Mannequin.SK_Mannequin\":<br/>\n{\n&nbsp;&nbsp;&nbsp;&nbsp;\"AssetPath\": \"/Game/Characters/Mannequins/Meshes/SK_Mannequin.SK_Mannequin\",\n&nbsp;&nbsp;&nbsp;&nbsp;\"SizeOnDisk\": 160872,\n&nbsp;&nbsp;&nbsp;&nbsp;\"AssetType\": \"/Script/Engine.Skeleton\"\n}\n</div>\n<p style='color:white;'>Each JSON file in the UELibrary consists of arrays using this structure.<br/>To query the JSON, use JQ (examples are included in the main form).<br/>Asset types are derived from Unreal Engine, so you can use JQ to find unique asset types.<br/><br/>The JQ query will run against all files in the UELibrary.<br/>Use smart, case insensitive searches when looking for specific asset types, such as 'assettype contains xyz'.<br/>You can also include logical operators like 'and' and 'or' in your search queries.<br/><br/>To refine searches further, combine these techniques with asset names, or even sizes (where the size is the number of bytes on disk).</p>\n<p style='color:white;'>The following assets (and their thumbnails) were extracted and classified from Unreal Engine Projects:<ul style='color:white;'><li>Textures</li>\n<li>Materials (includes instances)</li>\n<li>Static/Skeletal Meshes</li>\n<li>Animations</li>\n<li>Blueprints</li>\n<li>Any other Unreal Objects included may not have thumbnails</li>\n</ul></p>\n</body>\n</html>";
        
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        
        /// <summary>
        /// Query Button
        /// </summary>
        public readonly Button QueryButton = new Button();

        /// <summary>
        /// The path to CygWin.
        /// </summary>
        public readonly TextBox CygwinPath = new TextBox();

        /// <summary>
        /// The JQ query.
        /// </summary>
        public readonly TextBox JQQuery = new TextBox();


        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // QueryForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Size primaryDisplaySize = Screen.PrimaryScreen.Bounds.Size;
            this.ClientSize = new Size(primaryDisplaySize.Width / 2, primaryDisplaySize.Height / 2);
            this.StartPosition = FormStartPosition.CenterScreen;            
            Name = "QueryForm";
            Text = "Query UELibrary";

            StatusStrip statusBar = new StatusStrip();
            ToolStripStatusLabel statusLabel = new ToolStripStatusLabel();
            statusLabel.Text = "Version: 1.0, alwyn.j.dippenaar@gmail.com";
            statusBar.Items.Add(statusLabel);            
            this.Controls.Add(statusBar);            

            // Creating new "hyperlinked" label and text box for "Path to Cygwin"
            Label lblCygwinPath = new Label();
            lblCygwinPath.Text = "Path to Cygwin ( ensure jq is installed as well ):";
            lblCygwinPath.Font = new Font(lblCygwinPath.Font, FontStyle.Bold | FontStyle.Underline);
            lblCygwinPath.ForeColor = Color.Blue;
            lblCygwinPath.SetBounds(10, 35, 800, 20);
            lblCygwinPath.Cursor = Cursors.Hand;

            // Add event handler for click event
            lblCygwinPath.Click += (sender, args) => 
            {
                Process.Start(new ProcessStartInfo("cmd", $"/c start https://www.cygwin.com/") { CreateNoWindow = true });
            };
            
            CygwinPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            CygwinPath.SetBounds(10, 55, ClientSize.Width - 20, 20);
            this.Controls.Add(lblCygwinPath);
            this.Controls.Add(CygwinPath);            

            // Creating new label and text area for the input area
            Label lblTextArea = new Label();
            lblTextArea.Text = "JQ Query:";
            lblTextArea.Font = new Font(lblCygwinPath.Font, FontStyle.Bold | FontStyle.Underline);
            lblTextArea.ForeColor = Color.Blue;
            lblTextArea.Cursor = Cursors.Hand;
            lblTextArea.SetBounds(10, 85, 150, 20);

            // Add event handler for click event
            lblTextArea.Click += (sender, args) => 
            {
                Process.Start(new ProcessStartInfo("cmd", $"/c start https://www.baeldung.com/linux/jq-command-json") { CreateNoWindow = true });
            };
            
            JQQuery.Anchor = AnchorStyles.Top | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            JQQuery.Multiline = true;
            JQQuery.ScrollBars = ScrollBars.Vertical;
            JQQuery.SetBounds(10, 105, ClientSize.Width - 20, 100);

            // Add new controls to the form
            this.Controls.Add(lblTextArea);
            this.Controls.Add(JQQuery);            
            
            // New y-coordinate for the button
            int btnPos = JQQuery.Bottom + 15; 
    
            // Create the 'Query' Button
            // Set the y-coordinate to the variable
            // Add the Query button to the form
            QueryButton.Text = "Query";
            QueryButton.Font = new Font(QueryButton.Font, FontStyle.Bold);
            QueryButton.SetBounds(10, btnPos, 150, 55); 
            this.Controls.Add(QueryButton);                        

            // Create the Button
            Button helpButton = new Button();
            helpButton.Text = "Json";
            helpButton.Font = new Font(QueryButton.Font, FontStyle.Bold);
            helpButton.SetBounds(QueryButton.Right + 10, QueryButton.Top, 100, 45);
            helpButton.Click += (sender, args) =>
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
                    DocumentText = HelpSnippet,
                    Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                    AllowNavigation = false
                };
                webBrowser.IsWebBrowserContextMenuEnabled = false;
                webBrowser.SetBounds(10, 10, myForm.Width - 15, myForm.Height - 15);
                myForm.Controls.Add(webBrowser);
                myForm.ShowDialog();                
            };
            
            this.Controls.Add(helpButton);                        
            
            ResumeLayout(false);
        }
    }
}
