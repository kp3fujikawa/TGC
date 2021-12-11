using System;
using SearchSample.DataModel;
using System.Data;
using System.Collections.Generic;
using SearchSample.Model;

namespace SearchSample.ViewModel
{
    /// <summary>
    /// プラント情報検索画面 ViewModel
    /// </summary>
    class PlantSearchViewModel
    {
        #region "DBアクセスモデル"

        private DBAccess db = new DBAccess();

        #endregion

        #region "変数"

        private String[] search_condtinon_value = {
            "=",
            "LIKE",
            ">",
            "<",
            ">=",
            "<="
        };
        private String[] search_condtinon_text = {
            "と等しい",
            "を含む",
            "より大きい",
            "より小さい",
            "以上",
            "以下"
        };

        private DataTable search_condtinon_dt;
        public DataTable SearchCondition
        {
            get
            {
                search_condtinon_dt = new DataTable();
                search_condtinon_dt.Columns.Add(new DataColumn(Common.ComboBoxValue));
                search_condtinon_dt.Columns.Add(new DataColumn(Common.ComboBoxText));
                for (int i = 0; i < search_condtinon_value.Length; i++)
                {
                    DataRow newrow = search_condtinon_dt.NewRow();
                    newrow[Common.ComboBoxValue] = search_condtinon_value[i];
                    newrow[Common.ComboBoxText] = search_condtinon_text[i];
                    search_condtinon_dt.Rows.Add(newrow);
                }

                return search_condtinon_dt;
            }
            set
            {
                search_condtinon_dt = value;
            }
        }

        private String[] search_combi = { 
            "AND", 
            "OR"
        };
        private DataTable search_combi_dt;
        public DataTable SearchCombi
        {
            get
            {
                search_combi_dt = new DataTable();
                search_combi_dt.Columns.Add(new DataColumn(Common.ComboBoxValue));
                search_combi_dt.Columns.Add(new DataColumn(Common.ComboBoxText));
                for (int i = 0; i < search_combi.Length; i++)
                {
                    DataRow newrow = search_combi_dt.NewRow();
                    newrow[Common.ComboBoxValue] = search_combi[i];
                    newrow[Common.ComboBoxText] = search_combi[i];
                    search_combi_dt.Rows.Add(newrow);
                }

                return search_combi_dt;
            }
            set
            {
                search_combi_dt = value;
            }
        }

        private String[] page_size = { 
            "20",
            "50",
            "100"
        };
        private DataTable page_size_dt;
        public DataTable PageSize
        {
            get
            {
                page_size_dt = new DataTable();
                page_size_dt.Columns.Add(new DataColumn(Common.ComboBoxValue));
                page_size_dt.Columns.Add(new DataColumn(Common.ComboBoxText));
                for (int i = 0; i < page_size.Length; i++)
                {
                    DataRow newrow = page_size_dt.NewRow();
                    newrow[Common.ComboBoxValue] = page_size[i];
                    newrow[Common.ComboBoxText] = page_size[i];
                    page_size_dt.Rows.Add(newrow);
                }

                return page_size_dt;
            }
            set
            {
                page_size_dt = value;
            }
        }

        private String[] data_dictionary_value = {
            "0",
            "1",
            "2",
            "3"
        };
        private String[] data_dictionary_text = {
            "",
            "バッチ実績＋出荷実績",
            "入出庫実績・在庫",
            "品質情報"
        };

        private DataTable data_dictionary_dt;
        public DataTable DataDictionary
        {
            get
            {
                data_dictionary_dt = new DataTable();
                data_dictionary_dt.Columns.Add(new DataColumn(Common.ComboBoxValue));
                data_dictionary_dt.Columns.Add(new DataColumn(Common.ComboBoxText));
                for (int i = 0; i < data_dictionary_value.Length; i++)
                {
                    DataRow newrow = data_dictionary_dt.NewRow();
                    newrow[Common.ComboBoxValue] = data_dictionary_value[i];
                    newrow[Common.ComboBoxText] = data_dictionary_text[i];
                    data_dictionary_dt.Rows.Add(newrow);
                }

                return data_dictionary_dt;
            }
            set
            {
                data_dictionary_dt = value;
            }
        }

        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PlantSearchViewModel()
        {

        }

        /// <summary>
        /// データ検索
        /// </summary>
        /// <returns>true/false</returns>
        public bool SearchData(SearchData seach,out DataTable resulrdt)
        {
            return db.SearchPlantData(seach, out resulrdt);
        }

    }
}
