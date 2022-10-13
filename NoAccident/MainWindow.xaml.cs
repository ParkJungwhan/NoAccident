using System.Windows;

namespace NoAccident
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

#if !DEBUG
            this.WindowState = WindowState.Maximized;
            this.ResizeMode = ResizeMode.NoResize;
            this.AllowsTransparency = true;
            this.WindowStyle = WindowStyle.None;
#endif
        }
    }
}