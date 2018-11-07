using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Horsesoft.Horsify.Resource.Windows.CustomControls
{
    public class TextboxWithCaretIndex : TextBox
    {
        public TextboxWithCaretIndex()
        {            
        }

        public int CursorPosition
        {
            get { return (int)GetValue(CursorPositionProperty); }
            set { SetValue(CursorPositionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CursorPosition.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CursorPositionProperty =
            DependencyProperty.Register("CursorPosition", typeof(int), typeof(TextboxWithCaretIndex), new PropertyMetadata(0));

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            this.CursorPosition = this.CaretIndex;
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);
            e.Handled = true;

            //var txtChanged = e.Changes.ElementAt(0) as TextChange;
            //if (txtChanged.RemovedLength > 0)
            //    this.CaretIndex -= txtChanged.RemovedLength;
            //else if (txtChanged.AddedLength > 0)
            //    this.CaretIndex += txtChanged.AddedLength;
            //else
            //    return;           

            //this.CursorPosition = this.CaretIndex;
            //this.Focus();
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            var index = this.CaretIndex;
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            var index = this.CaretIndex;
        }

        //protected override void OnLostFocus(RoutedEventArgs e)
        //{
        //    base.OnLostFocus(e);
        //    this.CursorPosition = this.CaretIndex;
        //    e.Handled = true;
        //}

        //protected override void OnGotFocus(RoutedEventArgs e)
        //{
        //    base.OnGotFocus(e);
        //    this.CursorPosition = this.CaretIndex;
        //    e.Handled = true;
        //}
    }
}
