using System.Windows.Input;

namespace EchoGrabber.GUI.WPF.ViewModel
{
    public partial class MainViewModel
    {
        #region Команды
        private DelegateCommand _exitCommand;
        private DelegateCommand _filterActualCommand;
        private DelegateCommand _filterArchiveCommand;
        private DelegateCommand _updateCommand;
        private DelegateCommand<string> _filterPodcastsCommand;
        private DelegateCommand<PodcastInfo> _showPodcastsCommand;
        private DelegateCommand<PodcastInfo> _createPlaylistCommand;
        private DelegateCommand<PodcastInfo> _downloadCommand;

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

        public DelegateCommand<PodcastInfo> DownloadCommand
        {
            get
            {
                if (_downloadCommand == null)
                {
                    _downloadCommand = new DelegateCommand<PodcastInfo>(Download, CanDownload);
                }
                return _downloadCommand;
            }
        }

        public DelegateCommand<PodcastInfo> CreatePlaylistCommand
        {
            get
            {
                if (_createPlaylistCommand == null)
                {
                    _createPlaylistCommand = new DelegateCommand<PodcastInfo>(CreatePlaylist, CanExecute);
                }
                return _createPlaylistCommand;
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

        public DelegateCommand<string> FilterPodcastsCommand
        {
            get
            {
                if (_filterPodcastsCommand == null)
                {
                    _filterPodcastsCommand = new DelegateCommand<string>(FilterPodcasts);
                }
                return _filterPodcastsCommand;
            }
        }

        public DelegateCommand<PodcastInfo> ShowPodcastsCommand
        {
            get
            {
                if (_showPodcastsCommand == null)
                {
                    _showPodcastsCommand = new DelegateCommand<PodcastInfo>(PodcastnOnHtmlPage, CanExecute);
                }
                return _showPodcastsCommand;
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
        #endregion

    }
}
