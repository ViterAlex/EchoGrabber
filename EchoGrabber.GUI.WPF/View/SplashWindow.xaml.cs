using System;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace EchoGrabber.GUI.WPF.View
{
    /// <summary>
    /// Interaction logic for SplashWindow.xaml
    /// </summary>
    public partial class SplashWindow : Window
    {
        public SplashWindow()
        {
            InitializeComponent();
            Loaded += SplashWindow_Loaded;
        }
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
    }
}
