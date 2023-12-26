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
            QueryButton.Click += (sender, args) =>
            {
                MessageBox.Show("The 'Query' button has been clicked.");
            };
            
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
                JQQuery.Text = "Remember, ONLY 1 Object-Where query is allowed at a time, the below is just examples of such queries, submitting this \"as is\" will result in an error\r\n" +
                               "$_.AssetPath -match \"spruce\" -and $_.AssetType -match \"staticmesh\"\r\n\t # - look for all assets, containing the text \"spruce\", where the asset is a static mesh.\r\n\r\n" +
                               "$_.SizeOnDisk -gt 104857600\r\n\t #- Look for all assets, where the size on disk, is greater than 100MB.\r\n\r\n" +
                               "$_.AssetPath -match \"combat.*sword\" -and $_.AssetType -match \"animation\"\r\n\t #- Look for all assets, where the name regex matches combat and sword, where the asset type is an animation.";
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
    }
}
