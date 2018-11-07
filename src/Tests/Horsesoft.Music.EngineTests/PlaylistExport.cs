using Horsesoft.Music.Engine.Import;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Horsesoft.Music.EngineTests
{
    public class PlaylistExport
    {
        IFileImport _fileImporter;

        public PlaylistExport()
        {
            _fileImporter = new FileImport();
        }

        [Fact(Skip = "Integration")]
        public void ExportFilesWithoutYear_ToPlaylist()
        {
            var noYearFiles = _fileImporter.GetFilesWithoutYear();
            Assert.True(noYearFiles.Count() > 0);

            ExportFilesToPlaylist(noYearFiles);
        }

        private void ExportFilesToPlaylist(IEnumerable<Horsesoft.Music.Data.Model.File> files)
        {
            using (var txt = System.IO.File.CreateText("Songs-No-Year.m3u"))
            {
                txt.WriteLine("# ".PadRight(10, '*'));
                txt.WriteLine("# THESE SONG HAVE NO YEAR ASSIGNED");

                foreach (var file in files)
                {
                    txt.WriteLine($"{file.ToString()}");
                }
            }
        }
    }
}
