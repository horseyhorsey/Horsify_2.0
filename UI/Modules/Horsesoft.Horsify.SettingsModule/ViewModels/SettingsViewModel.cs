using Prism.Commands;
using Prism.Mvvm;
using System.Windows.Input;

namespace Horsesoft.Horsify.SettingsModule.ViewModels
{
    public class SettingsViewModel : BindableBase
    {
        public ICommand SwitchColorThemeCommand { get; set; }

        public SettingsViewModel()
        {
            //Change skin theme
            SwitchColorThemeCommand = new DelegateCommand<string>((skinDictionary) =>
            {
                
            });
        }
    }
}
