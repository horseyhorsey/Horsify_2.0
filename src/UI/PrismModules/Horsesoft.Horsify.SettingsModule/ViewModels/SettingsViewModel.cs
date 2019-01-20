using Horsesoft.Music.Data.Model.Horsify;
using Horsesoft.Music.Horsify.Base.Interface;
using Prism.Mvvm;

namespace Horsesoft.Horsify.SettingsModule.ViewModels
{
    public class SettingsViewModel : BindableBase
    {
        private IVoiceControl _voiceControl;
        private IDiscordRpcService _discordRpcService;

        public SettingsViewModel(IVoiceControl voiceControl, IDiscordRpcService discordRpcService)
        {
            _voiceControl = voiceControl;
            _discordRpcService = discordRpcService;
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

        private bool _discordEnabled = true;
        /// <summary>
        /// Gets or Sets the DiscordEnabled to allow pushing RichPresence to discord status
        /// </summary>
        public bool DiscordEnabled
        {
            get { return _discordEnabled; }
            set
            {
                if (SetProperty(ref _discordEnabled, value))
                {
                    if (_discordEnabled)
                        _discordRpcService.Enable(true);
                    else
                    {
                        _discordRpcService.Enable(false);
                    }
                }

            }
        }
    }
}
