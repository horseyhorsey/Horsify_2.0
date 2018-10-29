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
        #region Commands        
        /// <summary>
        /// Gets or sets the change volume command.
        /// </summary>
        /// <remarks>
        /// This is fired when the volume control has been touched then loses focus
        /// </remarks>
        public ICommand ChangeVolumeCommand { get; set; }
        #endregion

        #region Constructors
        public VolumeControlViewModel(IEventAggregator eventAggregator, IHorsifyMediaController horsifyMediaController)
        {            
            //Publish a SetVolumeEvent
            ChangeVolumeCommand = new DelegateCommand(() =>
            {
                horsifyMediaController.SetVolume(CurrentVolume);
                //eventAggregator.GetEvent<SetVolumeEvent>().Publish(CurrentVolume);
            });

            //Listen for changes in the volume elsewhere to reflect change to the control
            eventAggregator.GetEvent<OnMediaChangedVolumeEvent<double>>().Subscribe(OnVolumeChanged);
        }
        #endregion

        #region Properties        
        private int _currentVolume = 100;
        /// <summary>
        /// The Current Volume.
        /// </summary>
        public int CurrentVolume
        {
            get { return _currentVolume; }
            set
            {
                SetProperty(ref _currentVolume, value);
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