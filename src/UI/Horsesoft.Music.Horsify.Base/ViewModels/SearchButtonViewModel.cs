using Horsesoft.Music.Data.Model.Horsify;
using Horsesoft.Music.Data.Model.Menu;
using Horsesoft.Music.Horsify.Base.Helpers;
using Prism.Commands;
using Prism.Events;
using Prism.Logging;
using Prism.Regions;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Horsesoft.Music.Horsify.Base.ViewModels
{
    public interface ISearchButtonViewModel
    {
        ICommand HeldCommand { get; set; }
        ObservableCollection<ISearchButtonViewModel> SearchButtons { get; set; }
    }

    public class SearchListBaseViewModel : HorsifyBindableBase
    {
        private ObservableCollection<ISearchButtonViewModel> _searchButtons;

        public SearchListBaseViewModel(ILoggerFacade loggerFacade) : base(loggerFacade)
        {
        }

        /// <summary>
        /// Gets or Sets the SearchButtons
        /// </summary>
        public ObservableCollection<ISearchButtonViewModel> SearchButtons
        {
            get { return _searchButtons; }
            set { SetProperty(ref _searchButtons, value); }
        }
    }

    public class SearchButtonViewModel : SearchListBaseViewModel, ISearchButtonViewModel
    {
        #region Commands

        public ICommand HeldCommand { get; set; }        

        #endregion

        #region Constructors

        public SearchButtonViewModel(IMenuComponent menuComponent, ILoggerFacade loggerFacade) 
            : base(loggerFacade)
        {
            MenuComponent = menuComponent;
            
            if (MenuComponent is MenuItem)
            {
                //HeldCommand = new RelayCommand<Window>((win) => {
                //    ChangeViewToRandomSongPicker();
                //    Messenger.Default.Send(new RandomSongPickerMessage(this.MenuComponent.Name, this.MenuComponent.SearchType));
                //    _handled = true;
                //});
            }
            else
            {
                //HeldCommand = new RelayCommand(async () => await SelectMenuAsync());
            }


        }

        #endregion

        #region Properties

        public IMenuComponent MenuComponent { get; set; }

        public string ImagePath { get; set; }
        public string SearchTitle { get; set; }

        #endregion

        #region Support Methods


        #endregion
    }
}
