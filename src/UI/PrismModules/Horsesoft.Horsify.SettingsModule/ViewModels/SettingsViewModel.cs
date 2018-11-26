using Horsesoft.Music.Data.Model.Horsify;
using Prism.Commands;
using Prism.Mvvm;
using System.Windows.Input;

namespace Horsesoft.Horsify.SettingsModule.ViewModels
{
    public class SettingsViewModel : BindableBase
    {
        private IVoiceControl _voiceControl;

        public SettingsViewModel(IVoiceControl voiceControl)
        {
            _voiceControl = voiceControl;
        }

        private bool _voiceEnabled;
        /// <summary>
        /// Gets or Sets the VoiceEnabled
        /// </summary>
        public bool VoiceEnabled
        {
            get { return _voiceEnabled; }
            set
            {
                if (SetProperty(ref _voiceEnabled, value))
                {
                    if (_voiceEnabled)
                        _voiceControl.Start();
                    else
                    {
                        _voiceControl.Stop();
                    }
                }
                    
            }
        }
    }
}
