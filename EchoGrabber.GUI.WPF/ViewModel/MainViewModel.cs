using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using Application = System.Windows.Application;
using System.Collections.Generic;
using System.Windows.Threading;
using EchoGrabber.GUI.WPF.Helpers;

namespace EchoGrabber.GUI.WPF.ViewModel
{
    public partial class MainViewModel : ViewModelBase
    {

        private System.Threading.Timer _timer;
        private Dispatcher _dispatcher = Dispatcher.CurrentDispatcher;
        //private string _msgTitle = Application.Current.Resources["Title"].ToString();
        private Predicate<object> _podcastsFilter;
        private ProcessViewModel _dvm;

        public ICollectionView Browsers
        {
            get { return (ICollectionView)GetValue(BrowsersProperty); }
            set { SetValue(BrowsersProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Browsers.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BrowsersProperty =
            DependencyProperty.Register("Browsers", typeof(ICollectionView), typeof(MainViewModel), new PropertyMetadata(null));

        public ICollectionView Podcasts
        {
            get { return (ICollectionView)GetValue(PodcastsProperty); }
            set { SetValue(PodcastsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Podcasts.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PodcastsProperty =
            DependencyProperty.Register("Podcasts", typeof(ICollectionView), typeof(MainViewModel), new PropertyMetadata(null));

        public bool Available
        {
            get { return (bool)GetValue(AvailableProperty); }
            set { SetValue(AvailableProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Available.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AvailableProperty =
            DependencyProperty.Register("Available", typeof(bool), typeof(MainViewModel), new PropertyMetadata(false));

        private bool CanExecute(PodcastInfo p)
        {
            return EchoPrograms.Actual.Count != 0 && EchoPrograms.Archived.Count != 0 && Podcasts.CurrentItem != null;
        }


        private bool CanDownload(PodcastInfo podcast)
        {
            return false;
        }

        private void Download(PodcastInfo podcast)
        {
            _dvm = new ProcessViewModel(podcast, Browsers.CurrentItem as BrowserInfo);
            _dvm.Download();
        }

        private void Update()
        {
            StopTimer();
            Dispatcher.Invoke(() =>
            {
                Podcasts = CollectionViewSource.GetDefaultView(EchoPrograms.Actual);
                Browsers = CollectionViewSource.GetDefaultView(OsTools.GetBrowsers());
            });
            StartTimer();
        }

        private void CreatePlaylist(PodcastInfo podcast)
        {
            _dvm = new ProcessViewModel(podcast, Browsers.CurrentItem as BrowserInfo);
            _dvm.CreatePlaylist();
            //using (var dialog = new SaveFileDialog())
            //{
            //    dialog.Filter = "Плейлисты|*.m3u";
            //    dialog.FileName = $"{podcast.Title}";
            //    if (dialog.ShowDialog() != DialogResult.OK) return;

            //    var result = Grabber.CreatePlaylist(dialog.FileName, podcast.Url);
            //    if (!result)
            //    {
            //        CreatePlaylistFailed(podcast);
            //        return;
            //    }
            //    MessageBox.Show("Плейлист создан!", _msgTitle, MessageBoxButton.OK, MessageBoxImage.Information);
            //}
        }


        private void FilterArchive()
        {
            Podcasts = CollectionViewSource.GetDefaultView(EchoPrograms.Archived ?? new List<PodcastInfo>());
            Podcasts.Filter = _podcastsFilter;
            Podcasts.Refresh();
        }

        private void FilterActual()
        {
            Podcasts = CollectionViewSource.GetDefaultView(EchoPrograms.Actual);
            Podcasts.Filter = _podcastsFilter;
            Podcasts.Refresh();
        }
        /// <summary>
        /// Показать содержимое подкаста в виде html-страницы
        /// </summary>
        /// <param name="podcast">Ссылка на экземпляр класса Podcast</param>
        private void PodcastnOnHtmlPage(PodcastInfo podcast)
        {
            _dvm = new ProcessViewModel(podcast, Browsers.CurrentItem as BrowserInfo);
            _dvm.CreateHtml(Browsers.CurrentItem as BrowserInfo);
            //var sw = new StatusWindow(podcast.Url, Browsers.CurrentItem as BrowserInfo)
            //{
            //    ShowCancelButton = Visibility.Hidden
            //};
            ////IsBusy = Visibility.Collapsed;
            //sw.ShowDialog();
            ////IsBusy = Visibility.Visible;
        }

        private void FilterPodcasts(string filterIndex)
        {
            var index = int.Parse(filterIndex);
            _podcastsFilter = EchoPrograms.Filters[index];
            Podcasts.Filter = _podcastsFilter;
        }


        #region Constructor

        public MainViewModel()
        {
            Available = Helper.EchoIsOnline;
            Update();
        }

        #endregion

        private void Exit()
        {
            Application.Current.Shutdown();
        }

        #region Таймер, проверяющий наличие интернета
        private void timer_callback(object state)
        {
            var result = Helper.EchoIsOnline;
            _dispatcher.Invoke(() => Available = result);
            if ((EchoPrograms.Actual.Count == 0 || EchoPrograms.Archived.Count == 0) && result)
            {
                Update();
            }
        }

        private void StartTimer()
        {
            _timer = new System.Threading.Timer(timer_callback, null, 0, 1000);
        }

        private void StopTimer()
        {
            _timer?.Dispose();
        }
        #endregion
    }
}
