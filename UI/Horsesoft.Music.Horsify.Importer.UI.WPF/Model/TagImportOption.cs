using Horsesoft.Music.Data.Model.Import;
using Prism.Mvvm;

namespace Horsesoft.Music.Horsify.Importer.UI.WPF.Model
{
    public class TagImportOption : BindableBase
    {
        private TagOption _importTagOption;
        /// <summary>
        /// Gets or Sets the ImportTagOption
        /// </summary>
        public TagOption ImportTagOption
        {
            get => _importTagOption;
            set => SetProperty(ref _importTagOption, value);
        }

        private bool _importArt = true;
        /// <summary>
        /// Gets or Sets the ImportArt
        /// </summary>
        public bool ImportArt
        {
            get => _importArt;
            set
            {
                SetProperty(ref _importArt, value);

                if (ImportArt)
                    ImportTagOption |= TagOption.Artwork;
                else
                    ImportTagOption -= TagOption.Artwork;
            }
        }

        private bool _importCountry = true;
        /// <summary>
        /// Gets or Sets the ImportArt
        /// </summary>
        public bool ImportCountry
        {
            get => _importCountry;
            set
            {
                SetProperty(ref _importCountry, value);

                if (ImportCountry)
                    ImportTagOption |= TagOption.Country;
                else
                    ImportTagOption -= TagOption.Country;
            }
        }


        private bool _importDiscog = true;
        /// <summary>
        /// Gets or Sets the ImportDiscog
        /// </summary>
        public bool ImportDiscog
        {
            get => _importDiscog;
            set
            {
                SetProperty(ref _importDiscog, value);

                if (ImportDiscog)
                    ImportTagOption |= TagOption.Discog;
                else
                    ImportTagOption -= TagOption.Discog;
            }
        }


        private bool _importLabel = true;
        /// <summary>
        /// Gets or Sets the ImportLabel
        /// </summary>
        public bool ImportLabel
        {
            get => _importLabel;
            set
            {
                SetProperty(ref _importLabel, value);

                if (ImportLabel)
                    ImportTagOption |= TagOption.Label;
                else
                    ImportTagOption -= TagOption.Label;
            }
        }
    }
}
