using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Horsesoft.Horsify.Resource.Windows.CustomControls
{
    public class OnScreenKeyboard : TextboxWithCaretIndex
    {

        public static RoutedCommand SendKeyCommand { get; private set; }

        #region Constructors
        static OnScreenKeyboard()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(OnScreenKeyboard), new FrameworkPropertyMetadata(typeof(OnScreenKeyboard)));
            SendKeyCommand = new RoutedCommand("SendKeyCommand", typeof(OnScreenKeyboard));           
        }

        public OnScreenKeyboard()
        {            
            CommandBindings.Add(new CommandBinding(SendKeyCommand, sendKeyExecuted, sendKeyCanExecute));
        }

        #endregion

        //protected override void OnGotFocus(RoutedEventArgs e)
        //{
        //    //base.OnGotFocus(e);
        //    this.CursorPosition = this.CaretIndex;
        //    e.Handled = true;
        //}

        #region Support Methods
        /// <summary>
        /// The key sent from keyboard. Sets the cursor pos to 0 if clear, and increments cursor pos
        /// </summary>
        /// <param name="key"></param>
        private void OnKeyCommandSent(string key)
        {
            //Return if string is empty and deleting a key
            if (key == "Del" && string.IsNullOrWhiteSpace(this.Text)) return;

            //Clear the text search box and return.
            if (key == "Clr")
            {
                this.Text = string.Empty;
                CursorPosition = 0;
                return;
            }

            //Delete the char or add
            if (key == "Del")
            {
                if (CursorPosition > 0)
                {
                    this.Text = this.Text.Remove(CursorPosition - 1, 1);
                    CursorPosition--;
                }
            }
            else
            {
                if (Text.Length <= CursorPosition)
                    CursorPosition = 0;
                else
                    CursorPosition++;
                this.Text = this.Text.Insert(CursorPosition, key);

            }

        }

        private void sendKeyCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void sendKeyExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e?.Parameter != null)
            {
                OnKeyCommandSent(e.Parameter.ToString());
            }
        }
        #endregion

    }
}
