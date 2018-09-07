using GongSolutions.Wpf.DragDrop;
using Horsesoft.Music.Engine.Import;
using Horsesoft.Music.Horsify.Base;
using Horsesoft.Music.Horsify.Importer.UI.WPF.Model;
using Prism.Commands;
using Prism.Events;
using Prism.Logging;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Horsesoft.Music.Horsify.Importer.UI.WPF.ViewModels
{
    public class FileImportViewModel : BindableBase, IDropTarget
    {
        #region Fields
        private IFileImport _fileImporter;
        private ITagImport _tagImport;
        private TagImportOption _tagImportOption;
        private IEventAggregator _eventAggregator;
        private ILoggerFacade _importLogger;
        /// <summary>
        /// Is the long running import running.
        /// </summary>
        public bool isScanRunning = false;

        /// <summary>
        /// The cancellation token for aborting the import.
        /// </summary>
        private CancellationTokenSource ScanMusiccancellationToken; 
        #endregion

        #region Constructors
        public FileImportViewModel(IFileImport fileImporter, ITagImport tagImport, TagImportOption tagImportOption, IEventAggregator eventAggregator, ILoggerFacade loggerFacade)
        {
            _fileImporter = fileImporter;
            _tagImport = tagImport;
            _tagImport.OnTagUpdated += OnFileImported;
            _tagImportOption = tagImportOption;            

            _eventAggregator = eventAggregator;
            _importLogger = loggerFacade;

            _eventAggregator.GetEvent<StartScanSongsEvent>().Subscribe(async () => await OnImportFilesAsync());
            _eventAggregator.GetEvent<StopScanSongsEvent>().Subscribe(OnStopImport);
        }
        #endregion

        #region Properties
        private string _currentFile = "";
        /// <summary>
        /// Current file being processed.
        /// </summary>
        public string CurrentFile
        {
            get => _currentFile;
            set => SetProperty(ref _currentFile, value);
        }

        private ObservableCollection<string> _songDirectories = new ObservableCollection<string>();
        public ObservableCollection<string> SongDirectories
        {
            get { return _songDirectories; }
            set { SetProperty(ref _songDirectories, value); }
        } 
        #endregion

        #region Private Methods

        /// <summary>
        /// Imports the files and saves to database
        /// </summary>
        /// <returns></returns>
        private async Task ImportFilesAsync()
        {
            //Cancel token for the async process
            this.ScanMusiccancellationToken = new CancellationTokenSource();
            CancellationToken token = this.ScanMusiccancellationToken.Token;

            //Get all files and send them to be saved
            CurrentFile = $"Scanning for files....please wait";

            foreach (var songDir in SongDirectories)
            {
                _importLogger.Log($"Scanning for files in {songDir}", Category.Info, Priority.Medium);
                var allFiles = await _fileImporter.ImportFilesAsync(songDir, false);
                _fileImporter.CancelPending = false;

                _importLogger.Log($"Importing scanned files to database", Category.Info, Priority.Medium);
                await _fileImporter.SaveFilesToDatabase(allFiles);
            }
            
            //Untagged files in db scan
            var untagged = _fileImporter.GetUntaggedFilesFromDatabase();
            _importLogger.Log($"Populating tags from untagged files.", Category.Info, Priority.Medium);

            //Show failed files
            var failedFiles = await _tagImport.UpdateAllTags(untagged, _tagImportOption.ImportTagOption);
            _importLogger.Log($"Failed files count: {failedFiles.Count}", Category.Warn, Priority.Medium);            

            if (failedFiles.Count > 0)
            {
                //TODO: Log playlist of failed files.
                _importLogger.Log($"Exporting m3u failed files playlist", Category.Warn, Priority.Medium);                
                //LogFailedFiles(failedFiles);
            }

            this.isScanRunning = false;
            this.OnStopImport();
        }

        private void OnFileImported(string file)
        {            
            CurrentFile = file;
            _importLogger.Log($"{file}", Category.Info, Priority.Medium);
        }

        /// <summary>
        /// Called when [import files asynchronous]. Setups up the import and runs it as a new task. See <see cref="IFileImport"/>
        /// </summary>
        /// <returns></returns>
        private async Task OnImportFilesAsync()
        {
            try
            {
                //Set failed and complete files count to zero
                //CompleteFiles = 0; FailedFiles = 0; //artworkfilesCount = 0;

                //Back out if for some reason dir doesn't exist
                if (SongDirectories.Count <= 0)
                {
                    System.Windows.MessageBox.Show($"No music directories have been added to the import list.");
                    return;
                }

                //Run the import, update buttons
                _importLogger.Log($"Import started", Category.Info, Priority.Medium);
                isScanRunning = true;
                await ImportFilesAsync();
            }
            catch (Exception ex)
            {
                _importLogger.Log($"Import failed: {ex.Message}, {ex.InnerException?.Message}", Category.Exception, Priority.Medium);
            }
            finally
            {
                isScanRunning = false;
                _eventAggregator.GetEvent<ScanSongsEndedEvent>().Publish();
            }
        }

        /// <summary>
        /// Called when [stop import]. Cancels the running import
        /// </summary>
        private void OnStopImport()
        {
            try
            {
                if (isScanRunning)
                {
                    CurrentFile = $"Import Cancelled!";
                    _importLogger.Log($"Stopping import", Category.Warn, Priority.Medium);
                }
                else
                {
                    CurrentFile = $"Import Complete!";
                    _importLogger.Log($"Import complete", Category.Warn, Priority.Medium);
                }

                _fileImporter.CancelImport();
                _fileImporter.CancelPending = true;
                _tagImport.CancelTagging();
            }
            catch { }
            finally
            {
                isScanRunning = false;
                _eventAggregator.GetEvent<ScanSongsEndedEvent>().Publish();
            }
        }

        #endregion

        #region Drag Drop
        public void DragOver(IDropInfo dropInfo)
        {
            var dataObject = dropInfo.Data as System.Windows.IDataObject;
            // look for drag&drop new files
            if (dataObject != null && dataObject.GetDataPresent(DataFormats.FileDrop))
            {
                dropInfo.Effects = DragDropEffects.Copy;
            }
            else
            {
                dropInfo.Effects = DragDropEffects.Move;
            }
        }

        public void Drop(IDropInfo dropInfo)
        {
            var dataObject = dropInfo.Data as DataObject;
            // look for drag&drop new files
            if (dataObject != null && dataObject.ContainsFileDropList())
            {
                this.HandleDropActionAsync(dropInfo, dataObject.GetFileDropList());
            }
            else
            {
                GongSolutions.Wpf.DragDrop.DragDrop.DefaultDropHandler.Drop(dropInfo);
                var data = dropInfo.Data;
                // do something with the data
            }
        }

        private void HandleDropActionAsync(IDropInfo dropInfo, StringCollection stringCollection)
        {
            foreach (var path in stringCollection)
            {
                if (!this.SongDirectories.Any(x => x == path))
                {
                    this.SongDirectories.Add(path);
                }
            }
        }
        #endregion
    }
}
