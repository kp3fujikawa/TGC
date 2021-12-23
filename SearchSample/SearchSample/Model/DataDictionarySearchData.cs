using System;
using System.Collections.Generic;

namespace SearchSample.Model
{
    public class DataDictionarySearchData
    {

        public string DataType { get; set; }

        public string DataDictionary { get; set; }

        public string DataDictionary_status { get; set; }

        public DataDictionarySearchData()
        {
            DataType = string.Empty;
            DataDictionary = string.Empty;
            DataDictionary_status = string.Empty;
        }
    }
}
