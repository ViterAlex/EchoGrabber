using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace EchoGrabber.GUI.WPF.View
{
    /// <summary>
    /// Interaction logic for SplashWindow.xaml
    /// </summary>
    public partial class StatusWindow : Window
    {
        public StatusWindow()
        {
            InitializeComponent();
            Loaded += SplashWindow_Loaded;
        }

        public StatusWindow(string url)
        {
            InitializeComponent();
            DataContext = this;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            DragMove();
            ShowInTaskbar = true;
            Loaded += async (s, e) =>
            {
                var name = await Task.Factory.StartNew(() => Grabber.GetProgramName(url).Trim());
                descrLabel.Content = $"«{name.Trim()}»";
                await Task.Factory.StartNew(() => CreateHtml(name, url));
                Close();
            };
        }

        public Visibility ShowCancelButton
        {
            get { return (Visibility)GetValue(ShowCancelButtonProperty); }
            set { SetValue(ShowCancelButtonProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowCancelButton.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowCancelButtonProperty =
            DependencyProperty.Register("ShowCancelButton", typeof(Visibility), typeof(StatusWindow), new PropertyMetadata(Visibility.Visible));

        internal void LoadPodcasts()
        {
            EchoPrograms.Actual = Grabber.GetPodcastLinks().ToList();
            EchoPrograms.Archived = Grabber.GetPodcastLinks("/programs/archived").ToList();
        }

        private void SplashWindow_Loaded(object sender, RoutedEventArgs e)
        {
            IAsyncResult result = null;

            // This is an anonymous delegate that will be called when the initialization has COMPLETED
            var initCompleted = (AsyncCallback)delegate (IAsyncResult ar)
            {
                App.Current.ApplicationInitialize.EndInvoke(result);

                // Ensure we call close on the UI Thread.
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)delegate { Close(); });
            };

            // This starts the initialization process on the Application
            result = App.Current.ApplicationInitialize.BeginInvoke(this, initCompleted, null);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        internal void CreateHtml(string name, string url)
        {
            var filename = $"{url.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries)[1]}.html";
            Grabber.LinkProcessed += (s, e) =>
            {
                Dispatcher.Invoke(() => descrLabel.Content = $"«{name}». Выпусков: {e}");
            };
            Grabber.CreateHtml(filename, url);
            Process.Start(filename);
        }
    }
}
