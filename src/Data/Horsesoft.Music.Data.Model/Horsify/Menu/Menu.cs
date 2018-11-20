using System.Collections.Generic;
using System.Runtime.Serialization;
using Horsesoft.Music.Data.Model.Horsify;

namespace Horsesoft.Music.Data.Model.Menu
{
    /// <summary>
    /// Has a list of IMenuComponent and methods
    /// </summary>
    /// <seealso cref="Horsesoft.Music.Viewer.Model.MenuComponent" /> 
    public class Menu : MenuComponent
    {
        public Menu()
        {
            MenuComponents = new List<IMenuComponent>();
        }

        public override IEnumerator<IMenuComponent> GetIterator()
        {
            return MenuComponents.GetEnumerator();
        }

        public List<IMenuComponent> MenuComponents { get; set; }

        public override string Name { get; set; }
        public override string Image { get; set; }

        public override ExtraSearchType ExtraSearchType { get; set; }
        public override SearchType SearchType { get; set; }
        public override string SearchString { get; set; }

        public override void Add(IMenuComponent menuComponent)
        {
            MenuComponents.Add(menuComponent);                    
        }

        public override IMenuComponent GetChild(int index)
        {
            if (index <= MenuComponents.Count && MenuComponents.Count > 0)
            {
                return this.MenuComponents[index];
            }

            return null;
        }

        public override void Remove(IMenuComponent menuComponent)
        {
            MenuComponents.Remove(menuComponent);
        }
    }
}
