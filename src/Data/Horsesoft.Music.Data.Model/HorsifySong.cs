using Horsesoft.Music.Data.Model.Import;

namespace Horsesoft.Music.Data.Model
{
    public class HorsifySong : AllJoinedTable
    {
        public bool ChangesMade { get; set; }
        public byte[] Picture { get; set; }
        public string SongImage { get; set; } 
        public OpenKeyNotation OpenKeyNotationKey { get; set; }

        public override string ToString()
        {
            return $"{Artist} - {Title}";
        }
    }
}
