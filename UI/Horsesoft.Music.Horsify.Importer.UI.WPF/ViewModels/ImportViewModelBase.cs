using Horsesoft.Music.Data.Model.Horsify;
using Prism.Mvvm;

namespace Horsesoft.Music.Horsify.Importer.UI.WPF.ViewModels
{
    public class ImportViewModelBase  : BindableBase
    {
        protected readonly IHorsifySettings _horsifySettings;

        public ImportViewModelBase(IHorsifySettings horsifySettings)
        {
            _horsifySettings = horsifySettings;
        }
    }
}
