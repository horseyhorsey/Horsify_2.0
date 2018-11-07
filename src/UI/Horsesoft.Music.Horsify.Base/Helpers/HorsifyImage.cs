using Horsesoft.Music.Data.Model;

namespace Horsesoft.Music.Horsify.Base.Helpers
{
    public static class HorsifyImage
    {
        public static string GetImageLocation(AllJoinedTable song)
        {
            if (!string.IsNullOrWhiteSpace(song.ImageLocation))
                return song.ImageLocation;
            else
                return string.Empty;
        }
    }
}
