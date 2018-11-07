using Prism.Events;
using Prism.Mvvm;

namespace Horsesoft.Horsify.QueueModule.ViewModels
{
    public class QuickQueueViewModel : BindableBase
    {
        private IEventAggregator _eventAggregator;       

        public QuickQueueViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }
    }
}
