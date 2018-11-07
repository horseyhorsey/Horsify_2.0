using Horsesoft.Music.Data.Model;
using Horsesoft.Music.Horsify.Base;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System.Windows.Input;

namespace Horsesoft.Horsify.QueueModule.ViewModels
{
    public class QueueItemViewModel : BindableBase
    {                
        public QueueItemViewModel()
        {            
        }

        private AllJoinedTable _queuedSong;        
        public AllJoinedTable QueuedSong
        {
            get { return _queuedSong; }
            set { SetProperty(ref _queuedSong, value); }
        }        
    }
}
