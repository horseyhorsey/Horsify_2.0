using Horsesoft.Music.Data.Model.Horsify;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System.Windows.Input;

namespace Horsesoft.Music.Horsify.Base.ViewModels
{
    public interface IOnScreenKeyboardViewModel
    {
        ICommand SendKeyCommand { get; set; }
        ICommand SelectionChanged { get; set; }        
    }

    public class OnScreenKeyboardViewModel : BindableBase
    {
        #region Commands
        public ICommand SendKeyCommand { get; set; }
        public ICommand SelectionChanged { get; set; }
        public ICommand RunSearchCommand { get; set; }
        #endregion

        #region Constructors
        public OnScreenKeyboardViewModel()
        {
            SendKeyCommand = new DelegateCommand<string>((key) => OnKeyCommandSent(key));
        }
        #endregion

        #region Properties
        private string _searchText = string.Empty;
        /// <summary>
        /// Gets or Sets the SearchText
        /// </summary>
        public string SearchText
        {
            get { return _searchText; }
            set { SetProperty(ref _searchText, value); }
        }

        private int _caretIndex;
        public int CursorPosition
        {
            get { return _caretIndex; }
            set { SetProperty(ref _caretIndex, value); }
        }
        #endregion

        #region Support Methods
        /// <summary>
        /// The key sent from keyboard. Sets the cursor pos to 0 if clear, and increments cursor pos
        /// </summary>
        /// <param name="key"></param>
        private void OnKeyCommandSent(string key)
        {
            //Return if string is empty and deleting a key
            if (key == "Del" && string.IsNullOrWhiteSpace(SearchText)) return;

            //Clear the text search box and return.
            if (key == "Clr")
            {                
                SearchText = string.Empty;
                CursorPosition = 0;
                return;
            }

            //Delete the char or add
            if (key == "Del")
            {
                if (CursorPosition > 0)
                {
                    SearchText = SearchText.Remove(CursorPosition - 1, 1);
                    CursorPosition--;
                }
            }                
            else
            {
                SearchText = SearchText.Insert(CursorPosition, key);
                CursorPosition++;
            }
                
        } 
        #endregion
    }
}
