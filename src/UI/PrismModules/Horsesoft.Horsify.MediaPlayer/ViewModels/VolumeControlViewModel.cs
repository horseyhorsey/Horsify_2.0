using Horsesoft.Music.Horsify.Base;
using Horsesoft.Music.Horsify.Base.Interface;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System.Windows.Input;

namespace Horsesoft.Horsify.MediaPlayer.ViewModels
{
    public class VolumeControlViewModel : BindableBase
    {
        private readonly IHorsifyMediaController _horsifyMediaController;

        #region Constructors
        public VolumeControlViewModel(IEventAggregator eventAggregator, IHorsifyMediaController horsifyMediaController)
        {
            _horsifyMediaController = horsifyMediaController;
            //Listen for changes in the volume elsewhere to reflect change to the control
            eventAggregator.GetEvent<OnMediaChangedVolumeEvent<double>>().Subscribe(OnVolumeChanged);
        }
        #endregion

        #region Properties        
        private int _currentVolume = 100;
        /// <summary>
        /// The Current Volume. TODO: Allow user to turn volume up more 125 maybe
        /// </summary>
        public int CurrentVolume
        {
            get { return _currentVolume; }
            set
            {
                if (SetProperty(ref _currentVolume, value))
                {
                    if (CurrentVolume >= 100) CurrentVolume = 100;
                    else if(CurrentVolume <= 0) CurrentVolume = 0;

                    _horsifyMediaController.SetVolume(CurrentVolume);
                }
            }
        }
        #endregion

        #region Support Methods

        /// <summary>
        /// Called when /[volume changed] from an event
        /// </summary>
        /// <param name="currentVolume">The current volume.</param>
        private void OnVolumeChanged(double currentVolume)
        {
            //CurrentVolume = currentVolume;
        }
        #endregion
    }
}