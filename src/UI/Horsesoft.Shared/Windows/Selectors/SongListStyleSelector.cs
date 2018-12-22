using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Horsesoft.Horsify.Resource.Windows.Selectors
{
    public class SongListStyleSelector : StyleSelector
    {
        public override Style SelectStyle(object item, DependencyObject container)
        {
            var element = container as FrameworkElement;
            return element.FindResource("ListViewTouchDefaultVerticalStyle") as Style;
        }
    }
}
