using Horsesoft.Music.Data.Model;
using Horsesoft.Music.Horsify.Base.ViewModels;
using Prism.Commands;
using Prism.Logging;
using Prism.Mvvm;
using System.Linq;
using System.Windows.Input;

namespace Horsesoft.Horsify.PlaylistsModule.ViewModels
{
    public class PlaylistItemViewModel : HorsifyBindableBase
    {
        public ICommand MoveSongCommand { get; set; }
        public AllJoinedTable Song { get; set; }

        private int _playedState = 1;
        /// <summary>
        /// Gets or sets the played state of a song in the playlist
        /// </summary>
        public int PlayedState
        {
            get { return _playedState; }
            set { SetProperty(ref _playedState, value); }
        }

        public PlaylistItemViewModel(ILoggerFacade loggerFacade) : base(loggerFacade)
        {
            MoveSongCommand = new DelegateCommand<object>(OnMoveSong);
        }        

        private void OnMoveSong(object obj)
        {
            var vm = obj as PlaylistTabViewModel;
            Log("Playlist item moving", Category.Debug);

            if (vm != null)
            {
                if (!vm.PlayListItemViewModels.Any(x => x.Song == this.Song))
                {
                    Log($"Adding song to playlist Tab: {vm.TabHeader}", Category.Debug);
                    vm.AddPlaylistItem(this);
                    return;
                }

                Log($"Playlist Item already exists: {vm.TabHeader}", Category.Debug);
            }
        }


    }
}
