namespace UnrealProjectHub
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeContext();
        }

        private void InitializeContext()
        {
            DataContext = new MainViewModel();
        }
    }
}
