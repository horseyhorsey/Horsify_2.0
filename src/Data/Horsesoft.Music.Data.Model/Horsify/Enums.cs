using System.Runtime.Serialization;

namespace Horsesoft.Music.Data.Model.Horsify
{
    [DataContract]
    [System.Flags]
    public enum SearchType
    {
        [EnumMember]
        All             = 0,
        [EnumMember]
        Album           = 1 << 0,
        [EnumMember]
        Artist          = 1 << 1,
        [EnumMember]
        Bpm             = 1 << 2,
        [EnumMember]
        FileLocation    = 1 << 3,
        [EnumMember]
        Genre           = 1 << 4,
        [EnumMember]
        Label           = 1 << 5,
        [EnumMember]
        Title           = 1 << 6,
        [EnumMember]
        Year            = 1 << 7,
    }

    public enum ExtraSearchType
    {
        None,
        MostPlayed,
        RecentlyAdded,
        RecentlyPlayed,
    }

    public enum SearchAndOrOption
    {
        None,
        Or,
        Not,
        And               
    }
}
