using EchoGrabber.GUI.WPF.View;
using System;
using System.Windows;
using System.Windows.Threading;

namespace EchoGrabber.GUI.WPF
{
    //Реализация окна-заставки взята из https://stackoverflow.com/a/22026639/3085027

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            ApplicationInitialize = (w) =>
            {
                w.LoadPodcasts();
                // Create the main window, but on the UI thread.
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)delegate
                {
                    App.Current.MainWindow = new MainWindow
                    {
                        DataContext = new ViewModel.MainViewModel()
                    }; ;
                    MainWindow.Show();
                });
            };
        }

        internal Action<StatusWindow> ApplicationInitialize;

        public static new App Current
        {
            get { return Application.Current as App; }
        }
    }
}
