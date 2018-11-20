using System.Collections;
using System.ComponentModel;
using System.Windows.Data;

namespace Horsesoft.Horsify.DjHorsify.Model
{
    public class MyCollectionView : ListCollectionView
    {
        public MyCollectionView(IList sourceCollection) : base(sourceCollection)
        {
            foreach (var item in sourceCollection)
            {
                if (item is INotifyPropertyChanged)
                {
                    ((INotifyPropertyChanged)item).PropertyChanged +=
                                                      (s, e) => Refresh();
                }
            }
        }
    }       
}
