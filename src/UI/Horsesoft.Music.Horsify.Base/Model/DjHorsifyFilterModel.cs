﻿using Horsesoft.Music.Data.Model.Horsify;
using Prism.Mvvm;
using System.Collections.Generic;

namespace Horsesoft.Music.Horsify.Base.Model
{
    public class DjHorsifyFilterModel : BindableBase, IFilter
    {
        public DjHorsifyFilterModel()
        {

        }

        public DjHorsifyFilterModel(IFilter filter)
        {
            this.Id = filter.Id;
            this.SearchAndOrOption = SearchAndOrOption.None;
            this.SearchType = filter.SearchType;
            this.FileName = filter.FileName;
            this.Filters = filter.Filters;
        }

        private int _id;
        public int Id
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }

        private string _fileName;
        public string FileName
        {
            get { return _fileName; }
            set { SetProperty(ref _fileName, value); }
        }

        private List<string> _filters;
        public List<string> Filters
        {
            get { return _filters; }
            set { SetProperty(ref _filters, value); }
        }

        private SearchType _searchType;
        public SearchType SearchType
        {
            get { return _searchType; }
            set { SetProperty(ref _searchType, value); }
        }

        private SearchAndOrOption searchAndOrOption;
        public SearchAndOrOption SearchAndOrOption
        {
            get { return searchAndOrOption; }
            set { SetProperty(ref searchAndOrOption, value); }
        }

    }
}
