namespace QueryUELibrary
{
    public partial class QueryForm : Form
    {
        public QueryForm()
        {
            InitializeComponent();
            
            // Add a Click Event Handler
            QueryButton.Click += (sender, args) =>
            {
                // Code to run when the button is clicked
                MessageBox.Show("The 'Query' button has been clicked.");
            };
            
        }
    }
}
