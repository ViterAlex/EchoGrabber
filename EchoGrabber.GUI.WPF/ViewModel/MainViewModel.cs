using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using MessageBox = System.Windows.MessageBox;
using Application = System.Windows.Application;
using System.Collections.Generic;

namespace EchoGrabber.GUI.WPF.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private DelegateCommand _exitCommand;
        private DelegateCommand _filterActualCommand;
        private DelegateCommand _filterArchiveCommand;
        private DelegateCommand<string> _filterCommand;
        private DelegateCommand<PodcastInfo> _showPodcastsCommand;
        private DelegateCommand<PodcastInfo> _createPlaylistCommand;

        private Predicate<object> _podcastsFilter;

        public ICollectionView Podcasts
        {
            get { return (ICollectionView)GetValue(PodcastsProperty); }
            set { SetValue(PodcastsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Podcasts.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PodcastsProperty =
            DependencyProperty.Register("Podcasts", typeof(ICollectionView), typeof(MainViewModel), new PropertyMetadata(null));

        public DelegateCommand<string> FilterCommand
        {
            get
            {
                if (_filterCommand == null)
                {
                    _filterCommand = new DelegateCommand<string>(FilterPodcasts);
                }
                return _filterCommand;
            }
        }
        
        public DelegateCommand<PodcastInfo> ShowPodcastsCommand
        {
            get
            {
                if (_showPodcastsCommand == null)
                {
                    _showPodcastsCommand = new DelegateCommand<PodcastInfo>(ShowPodcasts);
                }
                return _showPodcastsCommand;
            }
        }

        public DelegateCommand FilterActualCommand
        {
            get
            {
                if (_filterActualCommand == null)
                {
                    _filterActualCommand = new DelegateCommand(FilterActual);
                }
                return _filterActualCommand;
            }
        }

        public DelegateCommand FilterArchiveCommand
        {
            get
            {
                if (_filterArchiveCommand == null)
                {
                    _filterArchiveCommand = new DelegateCommand(FilterArchive);
                }
                return _filterArchiveCommand;
            }
        }
        
        public DelegateCommand<PodcastInfo> CreatePlaylistCommand
        {
            get
            {
                if (_createPlaylistCommand == null)
                {
                    _createPlaylistCommand = new DelegateCommand<PodcastInfo>(CreatePlaylist);
                }
                return _createPlaylistCommand;
            }
        }

        private void CreatePlaylist(PodcastInfo podcast)
        {
            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = "Плейлисты|*.m3u";
                dialog.FileName = $"{podcast.Title}";
                if (dialog.ShowDialog() != DialogResult.OK) return;
                Grabber.CreatePlaylist(dialog.FileName, podcast.Url);
                MessageBox.Show("Плейлист создан");
            }
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
        /// Показать содержимое подкаста
        /// </summary>
        /// <param name="podcast">Ссылка на экземпляр класса Podcast</param>
        private void ShowPodcasts(PodcastInfo podcast)
        {
            var path = "eg.exe";
#if DEBUG
            path = System.IO.Path.GetFullPath("../../../EchoGrabber/bin/Debug/eg.exe");
#endif
            Process.Start($"{path}", $"{podcast.Url}");
        }

        private void FilterPodcasts(string filterIndex)
        {
            var index = int.Parse(filterIndex);
            _podcastsFilter = (Predicate<object>)EchoPrograms.Filters[index];
            Podcasts.Filter = _podcastsFilter;
        }


        #region Constructor

        public MainViewModel()
        {
            Podcasts = CollectionViewSource.GetDefaultView(EchoPrograms.Actual);
        }

        #endregion

        public ICommand ExitCommand
        {
            get
            {
                if (_exitCommand == null)
                {
                    _exitCommand = new DelegateCommand(Exit);
                }
                return _exitCommand;
            }
        }

        private void Exit()
        {
            Application.Current.Shutdown();
        }
    }
}
