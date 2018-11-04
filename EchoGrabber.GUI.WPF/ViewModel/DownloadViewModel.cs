using EchoGrabber.GUI.WPF.Helpers;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace EchoGrabber.GUI.WPF.ViewModel
{
    public class DownloadViewModel : ViewModelBase
    {
        public PodcastInfo CurrentPodcast
        {
            get { return (PodcastInfo)GetValue(CurrentPodcastProperty); }
            set { SetValue(CurrentPodcastProperty, value); }
        }

        private readonly BrowserInfo _browserInfo;

        // Using a DependencyProperty as the backing store for CurrentPodcast.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentPodcastProperty =
            DependencyProperty.Register("CurrentPodcast", typeof(PodcastInfo), typeof(DownloadViewModel), new PropertyMetadata(null));

        public bool IsBusy
        {
            get { return (bool)GetValue(IsBusyProperty); }
            set { SetValue(IsBusyProperty, value); }
        }

        public string Status
        {
            get { return (string)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Status.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register("Status", typeof(string), typeof(DownloadViewModel), new PropertyMetadata(string.Empty));



        // Using a DependencyProperty as the backing store for IsUpdating.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsBusyProperty =
            DependencyProperty.Register("IsBusy", typeof(bool), typeof(MainViewModel), new PropertyMetadata(false));
        private readonly string _msgTitle = Application.Current.Resources["Title"].ToString();

        public DownloadViewModel(PodcastInfo podcast, BrowserInfo browserInfo)
        {
            CurrentPodcast = podcast;
            _browserInfo = browserInfo;
        }

        internal async void CreateHtml(BrowserInfo browserInfo)
        {
            IsBusy = true;
            var url = CurrentPodcast.Url;
            var name = Grabber.GetProgramName(url);
            var filename = $"{url.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries)[1]}.html";
            filename = Path.Combine(Environment.GetEnvironmentVariable("temp"), filename);
            Grabber.LinkProcessed += (s, e) =>
            {
                Dispatcher.Invoke(() => Status = $"«{name}». Выпусков: {e}");
            };
            await Grabber.CreateHtmlAsync(filename, url);
            Process.Start(browserInfo.StartupPath, filename);
        }

        internal async void CreatePlaylist()
        {
            IsBusy = true;
            var url = CurrentPodcast.Url;
            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = "Плейлисты|*.m3u";
                dialog.FileName = $"{CurrentPodcast.Title}";
                if (dialog.ShowDialog() != DialogResult.OK) return;

                var result = await Grabber.CreatePlaylistAsync(dialog.FileName, url);
                if (!result)
                {
                    CreatePlaylistFailed();
                    return;
                }
                var dlgResult = MessageBox.Show("Плейлист создан!\r\nОткрыть его в проводнике?", _msgTitle, MessageBoxButton.YesNo, MessageBoxImage.Information);
                if (dlgResult==MessageBoxResult.Yes)
                {
                    Process.Start( $"explorer.exe", $"/select, \"{dialog.FileName}\"");
                }
            }

            //throw new NotImplementedException();
        }

        private void CreatePlaylistFailed()
        {
            var result = MessageBox.Show($"Не удалось создать плейлист для программы \"{CurrentPodcast.Title}\".\r\n" +
                $" Открыть страницу программы в браузере?", _msgTitle, MessageBoxButton.YesNo, MessageBoxImage.Error);
            if (result == MessageBoxResult.Yes)
            {
                Process.Start(_browserInfo.StartupPath, $"https://echo.msk.ru{CurrentPodcast.Url}");
            }
        }


        internal void Download()
        {
            IsBusy = true;
            throw new NotImplementedException();
        }
    }
}
