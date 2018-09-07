using Horsesoft.Music.Horsify.Base.Interface;
using System;
using System.Configuration;
using System.IO;

namespace Horsesoft.Music.Horsify.Importer.UI.WPF
{
    public class ImportSettings : IHorsifySettings
    {
        public ImportSettings()
        {
            ConnectionString = ConfigurationManager.ConnectionStrings["HorsifyDatabase"].ConnectionString;
        }

        #region Properties

        public string ConnectionString { get; private set; }

        public string HorsifyArtworkPath
        {
            get { return Properties.Settings.Default.HorsifyArtwork; }
            set
            {
                Properties.Settings.Default.HorsifyArtwork = value;
            }
        }

        public string HorsifyPath
        {
            get { return Properties.Settings.Default.HorsifyPath; }
            set
            {
                Properties.Settings.Default.HorsifyPath = value;
            }
        }        

        public string HorsifyProgramDataDirectory { get; set; }

        public string LogPath { get; set; }
        public string PlaylistPath { get; set; }        
        #endregion

        #region Public Methods

        public void Load()
        {                                    
            SetupHorsifyPaths();            
        }
        
        public void Save()
        {
            Properties.Settings.Default.Save();
        }

        #endregion

        /// <summary>
        /// Setups the horsify paths needed for an installation.
        /// </summary>
        /// <param name="horsifyAppDirectory">The horsify application directory.</param>
        private void SetupHorsifyPaths()
        {
            //Setup the ProgramData Directory for all users using CommonApplicationData.
            var appPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            this.HorsifyProgramDataDirectory = Path.Combine(appPath, "Horsify");

            //Set directories
            this.HorsifyPath = this.HorsifyProgramDataDirectory;
            LogPath = Path.Combine(this.HorsifyProgramDataDirectory, "Logs");
            PlaylistPath = Path.Combine(this.HorsifyProgramDataDirectory, "Playlists");
            this.HorsifyArtworkPath = Path.Combine(this.HorsifyProgramDataDirectory, "CoverArt");
            
            //Check and create directories
            CheckAndCreateDirectories(this.HorsifyProgramDataDirectory);
            CheckAndCreateDirectories(this.LogPath);
            CheckAndCreateDirectories(this.HorsifyArtworkPath);
            CheckAndCreateDirectories(this.PlaylistPath);
        }

        private void CheckAndCreateDirectories(string directory)
        {
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
        }
    }
}
