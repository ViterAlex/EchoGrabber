﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using MessageBox = System.Windows.MessageBox;
using Application = System.Windows.Application;
using System.Collections.Generic;
using System.Windows.Threading;
using System.Linq;

namespace EchoGrabber.GUI.WPF.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private DelegateCommand _exitCommand;
        private DelegateCommand _filterActualCommand;
        private DelegateCommand _filterArchiveCommand;
        private DelegateCommand _updateCommand;
        private DelegateCommand<string> _filterCommand;
        private DelegateCommand<PodcastInfo> _showPodcastsCommand;
        private DelegateCommand<PodcastInfo> _createPlaylistCommand;

        private System.Threading.Timer timer;
        private Dispatcher _dispatcher = Dispatcher.CurrentDispatcher;
        private string _msgTitle = Application.Current.Resources["Title"].ToString();
        private Predicate<object> _podcastsFilter;

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

        public Visibility IsUpdating
        {
            get { return (Visibility)GetValue(IsUpdatingProperty); }
            set { SetValue(IsUpdatingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsUpdating.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsUpdatingProperty =
            DependencyProperty.Register("IsUpdating", typeof(Visibility), typeof(MainViewModel), new PropertyMetadata(Visibility.Hidden));

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

        public DelegateCommand UpdateCommand
        {
            get
            {
                if (_updateCommand == null)
                {
                    _updateCommand = new DelegateCommand(Update);
                }
                return _updateCommand;
            }
        }

        //TODO:Автообновление при возобновлении связи
        private void Update()
        {
            Dispatcher.Invoke(() => IsUpdating = Visibility.Visible);
            EchoPrograms.Actual = Grabber.GetPodcastLinks().ToList();
            EchoPrograms.Archived = Grabber.GetPodcastLinks("/programs/archived").ToList();
            Dispatcher.Invoke(() =>
            {
                IsUpdating = Visibility.Hidden;
                Podcasts = CollectionViewSource.GetDefaultView(EchoPrograms.Actual);
            });

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
                var result = Grabber.CreatePlaylist(dialog.FileName, podcast.Url);
                if (!result)
                {
                    CreatePlaylistFailed(podcast);
                    return;
                }
                MessageBox.Show("Плейлист создан!", _msgTitle, MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void CreatePlaylistFailed(PodcastInfo podcast)
        {
            var result = MessageBox.Show($"Не удалось создать плейлист для программы \"{podcast.Title}\".\r\n" +
                $" Открыть страницу программы в браузере?", _msgTitle, MessageBoxButton.YesNo, MessageBoxImage.Error);
            if (result == MessageBoxResult.Yes)
            {
                Process.Start($"https://echo.msk.ru{podcast.Url}");
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
            _podcastsFilter = EchoPrograms.Filters[index];
            Podcasts.Filter = _podcastsFilter;
        }


        #region Constructor

        public MainViewModel()
        {
            Available = Helper.EchoIsOnline;
            Update();
            StartTimer();
        }

        #endregion
        private void timer_callback(object state)
        {
            var result = Helper.EchoIsOnline;
            _dispatcher.Invoke(() =>  Available = result);
            if ((EchoPrograms.Actual.Count == 0 || EchoPrograms.Archived.Count == 0) && result)
            {
                StopTimer();
                Update();
                StartTimer();
            }
        }

        private void StartTimer()
        {
            timer = new System.Threading.Timer(timer_callback, null, 0, 1000);
        }

        private void StopTimer()
        {
            timer.Dispose();
        }

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
