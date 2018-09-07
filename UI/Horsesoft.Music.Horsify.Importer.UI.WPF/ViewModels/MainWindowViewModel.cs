using Horsesoft.Music.Engine.Import;
using Horsesoft.Music.Horsify.Base.Interface;
using Prism.Mvvm;
using System.Windows.Shell;

namespace Horsesoft.Music.Horsify.Importer.UI.WPF.ViewModels
{
    public class MainWindowViewModel : ImportViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainWindowViewModel(IHorsifySettings horsifySettings, IHorsifyDbConnection horsifyDbConnection) : base(horsifySettings)
        {
            _horsifySettings.Load();

            // Create the database if not exisitng.
            bool createdNewDatabase = horsifyDbConnection.InitDb(_horsifySettings.HorsifyPath).Result;
            if (createdNewDatabase)
                System.Windows.MessageBox.Show($"Database Created.");
        }

        #region Properties                
        private TaskbarItemProgressState _progressState = TaskbarItemProgressState.Normal;
        /// <summary>
        /// Shows the taskbars busy indicator, progress.
        /// </summary>
        public TaskbarItemProgressState ProgressState
        {
            get { return _progressState; }
            set { SetProperty(ref _progressState, value); }
        }
        #endregion
    }
}
