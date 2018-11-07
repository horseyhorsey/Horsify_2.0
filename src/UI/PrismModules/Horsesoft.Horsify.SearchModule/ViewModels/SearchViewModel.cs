using Horsesoft.Music.Horsify.Base;
using Horsesoft.Music.Horsify.Base.ViewModels;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System.Windows.Input;

namespace Horsesoft.Horsify.SearchModule.ViewModels
{
    public interface ISearchViewModel
    {
        ICommand CloseSearchViewCommand { get; set; }
    }

    public class SearchViewModel : BindableBase
    {
        #region Commands
        public ICommand CloseSearchViewCommand { get; set; }
        #endregion

        #region Constructors

        //IHorsifySettingsDataProvider horsifySettings, IQueuedSongsData queuedSongs, IAllJoinedSongsDataProvider allSongsData, INowPlayingInfo nowPlayingInfo, IHistoryDataProvider historyDataProvider

        public SearchViewModel(IEventAggregator eventAggregator, IRegionManager regionManager)
        {
            //_horsifySettings = horsifySettings;

            OnScreenKeyboardViewModel = new OnScreenKeyboardViewModel(eventAggregator);

            CloseSearchViewCommand = new DelegateCommand(() =>
            {
                //eventAggregator.GetEvent<OnNavigateViewEvent<string>>()
                //    .Publish("SearchedSongsView");

                regionManager.RequestNavigate("ContentRegion", "SearchedSongsView");
            });
        }

        #endregion

        private OnScreenKeyboardViewModel _OnScreenKeyboardViewModel;
        /// <summary>
        /// Gets or Sets the OnScreenKeyboardViewModel
        /// </summary>
        public OnScreenKeyboardViewModel OnScreenKeyboardViewModel
        {
            get { return _OnScreenKeyboardViewModel; }
            set { SetProperty(ref _OnScreenKeyboardViewModel, value); }
        }
    }
}
