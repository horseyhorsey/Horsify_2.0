using System;

namespace Horsesoft.Music.Data.Model.Import
{
    [Flags]
    public enum Rating
    {
        None    = 0,
        Rank1   = 1,
        Rank2   = 64,
        Rank3   = 128,
        Rank4   = 196,
        Rank5   = 255,
    }

    public enum WmpRating
    {
        None = 0,
        Rank1 = 1,
        Rank2 = 64,
        Rank3 = 128,
        Rank4 = 196,
        Rank5 = 255
    }

    public enum TraktorRating
    {
        None = 0,
        Rank1 = 51,
        Rank2 = 102,
        Rank3 = 153,
        Rank4 = 204,
        Rank5 = 255
    }

    [Flags]
    public enum SongFileExtension
    {
        MP3     = 1 << 0,
        FLAC    = 1 << 1,
        OGG     = 1 << 2,
        WAV     = 1 << 3,
        WMA     = 1 << 4,   
    }

    [Flags]
    public enum SongImportType
    {
        All = 0,
        MP3 = SongFileExtension.MP3,
        FLAC = SongFileExtension.FLAC,
        WMA = SongFileExtension.WMA,
        WAV = SongFileExtension.WAV,
    }

    [Flags]
    public enum TagOption
    {
        All        = 0,
        Artwork     = 1 << 0,
        Bpm         = 1 << 1,
        Discog      = 1 << 2,
        MusicKey    = 1 << 3,
        Country = 1 << 4,
        Comment = 1 << 5,
        Label = 1 << 6,
    }

    [Flags]
    /// <summary>
    /// Keys for the Camelot wheel
    /// </summary>    
    public enum CamelotMinorKey
    {
        None    = 0,
        Abm     = 1 << 0,    //1A
        Ebm     = 1 << 1,    //2A
        Bbm     = 1 << 2,
        Fm      = 1 << 3,
        Cm      = 1 << 4,
        Gm      = 1 << 5,
        Dm      = 1 << 6,
        Am      = 1 << 7,
        Em      = 1 << 8,
        Bm      = 1 << 9,
        Gbm     = 1 << 10,
        Dbm     = 1 << 11
    }

    [Flags]
    public enum CamelotMajorKey
    {        
        B       = 1 << 12,    //1B
        Gb      = 1 << 13,    //2B
        Db      = 1 << 14,
        Ab      = 1 << 15,
        Eb      = 1 << 16,
        Bb      = 1 << 17,
        F       = 1 << 18,
        C       = 1 << 19,
        G       = 1 << 20,  //9b
        D       = 1 << 21,
        A       = 1 << 22,
        E       = 1 << 23
    }

    [Flags]
    public enum OpenKeyNotation
    {
        None = 0,
        Am      = CamelotMinorKey.Am,     //1m
        Em      = CamelotMinorKey.Em,     //2m
        Bm      = CamelotMinorKey.Bm,     //3m
        Gbm     = CamelotMinorKey.Gbm,    //4m
        Dbm     = CamelotMinorKey.Dbm,       //5m
        Abm     = CamelotMinorKey.Abm,       //6m
        Ebm     = CamelotMinorKey.Ebm,       //7m
        Bbm     = CamelotMinorKey.Bbm,      //8m
        Fm      = CamelotMinorKey.Fm,       //9m
        Cm      = CamelotMinorKey.Cm,       //10m
        Gm      = CamelotMinorKey.Gm,       //11m
        Dm      = CamelotMinorKey.Dm,       //12m
        //Inner row - Major
        C       = CamelotMajorKey.C,     //1d
        G       = CamelotMajorKey.G,     //2d
        D       = CamelotMajorKey.D,     //3d
        A       = CamelotMajorKey.A,
        E       = CamelotMajorKey.E,
        B       = CamelotMajorKey.B,
        Gb      = CamelotMajorKey.Gb,
        Db      = CamelotMajorKey.Db,
        Ab      = CamelotMajorKey.Ab,
        Eb      = CamelotMajorKey.Eb,
        Bb      = CamelotMajorKey.Bb,
        F       = CamelotMajorKey.F
    }
}
