using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace Horsesoft.Horsify.SearchModule.ViewModels
{
    public class SortSelectorViewModel : BindableBase, IInteractionRequestAware
    {
        public INotification Notification { get; set; }
        public Action FinishInteraction { get; set; }

        public ICommand SortCommand { get; set; }

        public SortSelectorViewModel()
        {
            SortCommand = new DelegateCommand<string>(OnSortRequested);
        }

        private SortDescription _sortDescription;
        public SortDescription SortDescription
        {
            get { return _sortDescription; }
            set { SetProperty(ref _sortDescription, value); }
        }

        private ListSortDirection _listSortDirection = ListSortDirection.Descending;
        public ListSortDirection ListSortDirection
        {
            get { return _listSortDirection; }
            set { SetProperty(ref _listSortDirection, value); }
        }

        private void OnSortRequested(string propName)
        {
            SortDescription = new SortDescription(propName, ListSortDirection);
            Notification.Content = SortDescription;
            FinishInteraction?.Invoke();
        }
    }
}
