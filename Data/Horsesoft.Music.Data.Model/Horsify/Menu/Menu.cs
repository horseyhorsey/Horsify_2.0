using System.Collections.Generic;

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

        public int Tets { get; set; }

        IEnumerable<IMenuComponent> iterator = null;

        public override IEnumerator<IMenuComponent> GetIterator()
        {
            return MenuComponents.GetEnumerator();
        }

        public List<IMenuComponent> MenuComponents { get; set; }

        public override string Name { get; set; }

        public override string Image { get; set; }

        public override void Add(IMenuComponent menuComponent)
        {
            MenuComponents.Add(menuComponent);                    
        }

        public override IMenuComponent GetChild(int index)
        {
            if (index <= MenuComponents.Count)
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
