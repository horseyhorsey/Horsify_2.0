using Prism.Mvvm;

namespace Horsesoft.Horsify.Statistics.ViewModels
{
    public class GlobalStatsView : BindableBase
    {
        private string _message;
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        public GlobalStatsView()
        {
            Message = "View A from your Prism Module";
        }
    }
}
