using Horsesoft.Music.Data.Model.Import;
using System.Collections.Generic;

namespace Horsesoft.Music.Data.Model.Tags
{
    public class OpenKeyColors
    {
        public static IDictionary<OpenKeyNotation, string> OpenKeyColorsDict = new Dictionary<OpenKeyNotation, string>
        {
            
        };

        static OpenKeyColors()
        {
            OpenKeyColorsDict.Add(OpenKeyNotation.None, "White");
            OpenKeyColorsDict.Add(OpenKeyNotation.Am, "#ecb6ed");
            OpenKeyColorsDict.Add(OpenKeyNotation.Em, "#baafc6");
            OpenKeyColorsDict.Add(OpenKeyNotation.Bm, "#a8bfd8");
            OpenKeyColorsDict.Add(OpenKeyNotation.Gbm, "#9fd9fb");
            OpenKeyColorsDict.Add(OpenKeyNotation.Dbm, "#97f2f6");
            OpenKeyColorsDict.Add(OpenKeyNotation.Abm, "#9de5bf");
            OpenKeyColorsDict.Add(OpenKeyNotation.Ebm, "#a4f89c");
            OpenKeyColorsDict.Add(OpenKeyNotation.Bbm, "#d4f2a6");
            OpenKeyColorsDict.Add(OpenKeyNotation.Fm, "#fff0a1");
            OpenKeyColorsDict.Add(OpenKeyNotation.Cm, "#fad198");
            OpenKeyColorsDict.Add(OpenKeyNotation.Gm, "#f29897");
            OpenKeyColorsDict.Add(OpenKeyNotation.Dm, "#ff95cf");

            OpenKeyColorsDict.Add(OpenKeyNotation.C, "#e285e6");
            OpenKeyColorsDict.Add(OpenKeyNotation.G, "#85769f");
            OpenKeyColorsDict.Add(OpenKeyNotation.D, "#6a90c5");
            OpenKeyColorsDict.Add(OpenKeyNotation.A, "#5abcfb");
            OpenKeyColorsDict.Add(OpenKeyNotation.E, "#51e5e9");
            OpenKeyColorsDict.Add(OpenKeyNotation.B, "#4fd195");
            OpenKeyColorsDict.Add(OpenKeyNotation.Gb, "#77f676");
            OpenKeyColorsDict.Add(OpenKeyNotation.Db, "#ade257");
            OpenKeyColorsDict.Add(OpenKeyNotation.Ab, "#ffe65b");
            OpenKeyColorsDict.Add(OpenKeyNotation.Eb, "#ffb456");
            OpenKeyColorsDict.Add(OpenKeyNotation.Bb, "#e74d4c");
            OpenKeyColorsDict.Add(OpenKeyNotation.F, "#fd4da7");
        }
    }
}
