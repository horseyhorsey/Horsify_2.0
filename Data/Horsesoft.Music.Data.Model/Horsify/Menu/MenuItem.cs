using Horsesoft.Music.Data.Model.Horsify;

namespace Horsesoft.Music.Data.Model.Menu
{
    /// <summary>
    /// Menu item just overrides the Name property
    /// </summary>
    /// <seealso cref="Horsesoft.Music.Viewer.Model.MenuComponent" />
    public class MenuItem : MenuComponent
    {
        public override string Name { get; set; }

        public override string Image { get; set; }

        public override string SearchString { get; set; }
        
        public override SearchType SearchType { get; set; }

        public override ExtraSearchType ExtraSearchType { get; set; }        


    }
}
