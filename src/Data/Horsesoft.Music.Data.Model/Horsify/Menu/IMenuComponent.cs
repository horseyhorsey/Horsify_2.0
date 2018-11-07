using Horsesoft.Music.Data.Model.Horsify;
using System.Collections.Generic;

namespace Horsesoft.Music.Data.Model.Menu
{
    /// <summary>
    /// Interface to share all functions and properties
    /// </summary>
    public interface IMenuComponent
    {
        string Name { get; set; }
        string Image { get; set; }
        string SearchString { get; set; }

        SearchType SearchType { get; set; }
        ExtraSearchType ExtraSearchType { get; set; }

        void Add(IMenuComponent menuComponent);
        void Remove(IMenuComponent menuComponent);

        IMenuComponent Parent { get; set; }
        IMenuComponent GetChild(int index);

        IEnumerator<IMenuComponent> GetIterator();
    }
}
