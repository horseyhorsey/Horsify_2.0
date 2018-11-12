using Horsesoft.Music.Data.Model.Horsify;
using Prism.Mvvm;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
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
