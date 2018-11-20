using Horsesoft.Music.Data.Model.Horsify;
using System;
using System.Collections.Generic;
using System.Text;

namespace Horsesoft.Music.Data.Model
{
    public class FiltersSearch
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int? MaxAmount { get; set; }

        public int? RandomAmount { get; set; }

        public string SearchFilterContent { get; set; }

        public SearchFilter ConvertToSearchFilter()
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<SearchFilter>(this.SearchFilterContent);
        }
    }
}
