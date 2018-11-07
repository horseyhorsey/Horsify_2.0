using Horsesoft.Music.Engine.Import;
using Horsesoft.Music.Horsify.Repositories;
using System.Diagnostics;
using System.Linq;
using Xunit;

namespace Horsesoft.Music.EngineTests
{
    public class FileImportTests
    {
        IFileImport _fileImporter;

        public FileImportTests()
        {            
            _fileImporter = new FileImport();
        }

        [Fact(Skip = "Integration Test")]
        public async void ConvertFileTests()
        {
            var file = await _fileImporter.ImportFileAsync(Constants.SONGFILE, false);

            Assert.NotNull(file);
        }

        [Fact(Skip = "Integration Test")]
        public async void ConvertFileBatchTests()
        {            
            var files = await _fileImporter.ImportFilesAsync(Constants.WAV_FOLDER, false);

            Assert.True(files.Count() > 0);
        }

        [Fact(Skip = "Integration Test")]
        public async void ImportFiles_AndSaveToDatabase()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var files = await _fileImporter.ImportFilesAsync(Constants.ALL_SONGS, false);

            await _fileImporter.SaveFilesToDatabase(files);

            if (stopWatch.IsRunning)
                stopWatch.Stop();

            var time = stopWatch.Elapsed;
        }
    }
}
