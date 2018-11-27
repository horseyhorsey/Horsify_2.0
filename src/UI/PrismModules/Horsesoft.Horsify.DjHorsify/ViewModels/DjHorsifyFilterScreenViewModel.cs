using Horsesoft.Horsify.DjHorsify.Model;
using Horsesoft.Music.Horsify.Base.Interface;
using Horsesoft.Music.Horsify.Base.ViewModels;
using Prism.Logging;
using Prism.Regions;

namespace Horsesoft.Horsify.DjHorsify.ViewModels
{
    public class DjHorsifyFilterScreenViewModel : HorsifyViewModelBase
    {
        private IDjHorsifyService _djHorsifyService;        

        public DjHorsifyFilterScreenViewModel(IDjHorsifyService djHorsifyService, IRegionManager regionManager, ILoggerFacade loggerFacade) : base(regionManager, loggerFacade)
        {
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
