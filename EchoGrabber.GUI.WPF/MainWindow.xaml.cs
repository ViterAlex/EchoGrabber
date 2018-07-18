using System.Linq;
using System.Windows;
using EchoGrabber.GUI.WPF.ViewModel;

namespace EchoGrabber.GUI.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            EchoPrograms.Actual = Grabber.GetPodcastLinks().ToList();
            EchoPrograms.Archived = Grabber.GetPodcastLinks("/programs/archived").ToList();
            var vm = new MainViewModel();
            DataContext = vm;
        }
    }
}
