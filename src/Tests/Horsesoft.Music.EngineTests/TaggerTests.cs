using Horsesoft.Music.Engine.Tagging;
using Xunit;

namespace Horsesoft.Music.EngineTests
{
    public class TaggerTests
    {
        ISongTagger _songTagger;

        [Fact(Skip = "Integration")]
        public void GetMp3TagWithId3()
        {
            _songTagger = new SongTaggerId3();
            //_songTagger.PopulateSongTag(new Data.Mod Constants.SONGFILE);
            var songtagged = _songTagger.PopulateSongTag(Constants.BADIMPORTFILE);
        }

        [Fact(Skip = "Integration")]
        public void GetMp3TagWithTagLib()
        {
            _songTagger = new SongTaggerTagLib();
            
            //var song = _songTagger.PopulateSongTag(Constants.SONGFILE);
        }
    }
}
