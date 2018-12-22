using Prism.Mvvm;

namespace Horsesoft.Horsify.SearchModule.ViewModels
{
    public class SongTemplateSwitchViewModel : BindableBase
    {
        public SongTemplateSwitchViewModel()
        {

        }

        #region Properties
        private bool _isHorizontal = true;
        /// <summary>
        /// Is Listview Horizontal
        /// </summary>
        public bool IsHorizontal
        {
            get { return _isHorizontal; }
            set { SetProperty(ref _isHorizontal, value); }
        } 
        #endregion
    }
}
