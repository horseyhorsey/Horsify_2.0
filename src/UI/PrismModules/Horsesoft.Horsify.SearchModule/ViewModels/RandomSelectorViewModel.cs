using Horsesoft.Music.Data.Model.Horsify;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using System;

namespace Horsesoft.Horsify.SearchModule.ViewModels
{
    public class RandomSelectorViewModel : BindableBase, IInteractionRequestAware
    {
        public INotification Notification { get; set; }
        public Action FinishInteraction { get; set; }
        public DelegateCommand<object> NavigateCommand { get; set; }

        public RandomSelectorViewModel()
        {
            NavigateCommand = new DelegateCommand<object>(OnRequestNavigate);
        }

        #region Properties
        private RangeFilterOption<byte> _ratingRange = new RangeFilterOption<byte>(0, 0);
        public RangeFilterOption<byte> RatingRange
        {
            get { return _ratingRange; }
            set { SetProperty(ref _ratingRange, value); }
        }

        private int _amount = 5;
        public int Amount
        {
            get { return _amount; }
            set { SetProperty(ref _amount, value); }
        } 
        #endregion

        #region Private Methods

        private void GetRandom()
        {
            var randomOption = new RandomSelectOption(Amount, RatingRange.IsEnabled, RatingRange.Low, RatingRange.Hi);
            Notification.Content = randomOption;
            FinishInteraction?.Invoke();
        }

        /// <summary>
        /// Sends random song options or closes the view
        /// </summary>
        /// <param name="selectRandom"></param>
        private void OnRequestNavigate(object selectRandom)
        {
            var sr = Convert.ToBoolean(selectRandom);
            if (sr)
                GetRandom();
            else
            {
                Notification.Content = null;
                FinishInteraction?.Invoke();
            }
        }
        #endregion
    }
}
