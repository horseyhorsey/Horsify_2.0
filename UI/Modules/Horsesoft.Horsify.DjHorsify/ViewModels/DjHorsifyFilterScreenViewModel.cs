using Horsesoft.Horsify.DjHorsify.Model;
using Horsesoft.Music.Horsify.Base.Interface;
using Prism.Mvvm;

namespace Horsesoft.Horsify.DjHorsify.ViewModels
{
    public class DjHorsifyFilterScreenViewModel : BindableBase
    {
        private IDjHorsifyService _djHorsifyService;

        public DjHorsifyFilterScreenViewModel(IDjHorsifyService djHorsifyService)
        {
            _djHorsifyService = djHorsifyService;
            _djHorsifyService = djHorsifyService;
            DjHorsifyOption = _djHorsifyService.DjHorsifyOption as DjHorsifyOption;
        }

        private DjHorsifyOption _djHorsifyOption;
        public DjHorsifyOption DjHorsifyOption
        {
            get { return _djHorsifyOption; }
            set { SetProperty(ref _djHorsifyOption, value); }
        }
    }
}
