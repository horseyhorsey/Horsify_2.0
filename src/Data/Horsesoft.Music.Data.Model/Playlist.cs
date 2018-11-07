namespace Horsesoft.Music.Data.Model
{
    public class Playlist
    {
        public long Id { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// Items are defined as SongId, PlayedState
        ///    245,0;5613,0;513,0;6341,1;254,0; 
        /// </summary>
        public string Items { get; set; }
        public int Count { get; set; }
    }
}
