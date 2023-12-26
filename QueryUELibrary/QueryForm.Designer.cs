using System.Diagnostics;

namespace QueryUELibrary
{
    partial class QueryForm
    {


        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        
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
        /// The status bar windows nonsnese.
        /// </summary>
        public readonly StatusStrip StatusBar = new StatusStrip();
        
        /// <summary>
        /// The status bar windows nonsnese.
        /// </summary>
        public readonly ToolStripStatusLabel StatusLabel = new ToolStripStatusLabel();
        
        /// <summary>
        /// The help button.
        /// </summary>
        public readonly Button HTMLHelpButton = new Button();
        
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

            StatusBar.Items.Add(StatusLabel);            
            this.Controls.Add(StatusBar);            

            // Creating new "hyperlinked" label and text box for "Path to Powershell script"
            ScriptPathLabel.Text = "Path to Powershell Script:";
            ScriptPathLabel.Font = new Font(ScriptPathLabel.Font, FontStyle.Bold | FontStyle.Underline);
            ScriptPathLabel.ForeColor = Color.Blue;
            ScriptPathLabel.SetBounds(10, 35, 800, 20);
            ScriptPathLabel.Cursor = Cursors.Hand;
            
            ScriptPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            ScriptPath.SetBounds(10, 55, ClientSize.Width - 20, 20);
            ScriptPath.Text = "query.ps1";
            this.Controls.Add(ScriptPathLabel);
            this.Controls.Add(ScriptPath);            

            // Creating new label and text area for the input area
            QueryInputLabel.Text = "PS Where-Object Query: (replaces {QUERY_JQ} in the script)";
            QueryInputLabel.Font = new Font(ScriptPathLabel.Font, FontStyle.Bold | FontStyle.Underline);
            QueryInputLabel.ForeColor = Color.Blue;
            QueryInputLabel.Cursor = Cursors.Hand;
            QueryInputLabel.SetBounds(10, 85, 150, 20);
            
            JQQuery.Anchor = AnchorStyles.Top | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            JQQuery.Multiline = true;
            JQQuery.ScrollBars = ScrollBars.Vertical;
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
            QueryButton.SetBounds(10, btnPos, 150, 55); 
            this.Controls.Add(QueryButton);                        

            // Create the Button
            HTMLHelpButton.Text = "Json";
            HTMLHelpButton.Font = new Font(QueryButton.Font, FontStyle.Bold);
            HTMLHelpButton.SetBounds(QueryButton.Right + 10, QueryButton.Top, 100, 45);
            this.Controls.Add(HTMLHelpButton);                        

            ExamplesButton.Text = "Examples";
            ExamplesButton.Font = new Font(QueryButton.Font, FontStyle.Bold);
            ExamplesButton.SetBounds(HTMLHelpButton.Right + 10, HTMLHelpButton.Top, 100, 45);
            this.Controls.Add(ExamplesButton);                        
            
            
            ResumeLayout(false);
        }
    }
}
