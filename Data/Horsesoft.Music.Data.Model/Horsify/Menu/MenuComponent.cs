using Horsesoft.Music.Data.Model.Horsify;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Horsesoft.Music.Data.Model.Menu
{
    /// <summary>
    /// Abstract class that implements the interface and default implementation.
    /// </summary>
    /// <seealso cref="Horsesoft.Music.Viewer.Model.IMenuComponent" />    
    public abstract class MenuComponent : IMenuComponent
    {
        public virtual string Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public virtual SearchType SearchType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public virtual ExtraSearchType ExtraSearchType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        
        public virtual string Image { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        
        public virtual string SearchString { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        
        public IMenuComponent Parent { get; set; }

        public virtual void Add(IMenuComponent menuComponent)
        {
            throw new NotImplementedException();
        }

        public virtual IMenuComponent GetChild(int index)
        {
            throw new NotImplementedException();
        }

        public virtual IEnumerator<IMenuComponent> GetIterator()
        {
            throw new NotImplementedException();
        }

        public virtual void Remove(IMenuComponent menuComponent)
        {
            throw new NotImplementedException();
        }
    }
}
