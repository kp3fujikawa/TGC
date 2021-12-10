using System;
using System.Collections.Generic;

namespace SearchSample.Model
{
    public class SearchData
    {

        public List<SearchCondition> search_list { get; set; }

        public DateTime? product_date_start { get; set; }

        public DateTime? product_date_end { get; set; }

        public List<string> qulity_list { get; set; }

        public List<string> process_list { get; set; }

        public bool qulity_check { get; set; }

        public bool process_check { get; set; }

        public SearchData()
        {
            search_list = new List<SearchCondition>();
            qulity_list = new List<string>();
            process_list = new List<string>();
        }
    }
}
