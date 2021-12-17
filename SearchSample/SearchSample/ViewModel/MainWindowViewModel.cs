using System;
using SearchSample.DataModel;
using System.Data;
using System.Collections.Generic;
using SearchSample.Model;
using System.ComponentModel;

namespace SearchSample.ViewModel
{
    /// <summary>
    /// メイン画面 ViewModel
    /// </summary>
    class MainWindowViewModel
    {

        public MainWindowViewModel()
        {

        }

        #region "DBアクセスモデル"

        private DBAccess db = new DBAccess();

        #endregion

        #region "変数"

        private String[] search_condtinon_value = {
            "",
            "=",
            "LIKE",
            ">",
            "<",
            ">=",
            "<="
        };
        private String[] search_condtinon_text = {
            "",
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

        /// <summary>
        /// 検索時テーブル別名
        /// </summary>
        private Dictionary<string, string> data_dictionary =
            new Dictionary<string, string>()
            {
                {"0", ""},
                {"1", "バッチ実績＋出荷実績"},
                {"2", "入出庫実績・在庫"},
                {"3", "品質情報"},
                {"4", "PT-395 反応"},
                {"5", "工程金属マテバラ"},
                {"6", "フィルターの溶剤通液量"},
                {"7", "プロセスデータ①(原材料管理)"},
                {"8", "工程時間予実"},
                {"9", "品目マスタ"},
                {"10", "荷姿マスタ"},
                {"11", "保管場所マスタ"},
                {"12", "配送先マスタ"},
                {"13", "ユーザマスタ"},
                {"14", "日別在庫"},
                {"15", "月末在庫"},
            };

        private DataTable data_dictionary_dt;
        public DataTable DataDictionary
        {
            get
            {
                data_dictionary_dt = new DataTable();
                data_dictionary_dt.Columns.Add(new DataColumn(Common.ComboBoxValue));
                data_dictionary_dt.Columns.Add(new DataColumn(Common.ComboBoxText));
                foreach (KeyValuePair<string,string> dic in data_dictionary)
                {
                    DataRow newrow = data_dictionary_dt.NewRow();
                    newrow[Common.ComboBoxValue] = dic.Key;
                    newrow[Common.ComboBoxText] = dic.Value;
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
        /// テーブルカラム名取得
        /// </summary>
        /// <returns>true/false</returns>
        public bool GetItemData(string dic, out DataTable rtndt)
        {
            rtndt = new DataTable();
            rtndt.Columns.Add(Common.ComboBoxText);
            rtndt.Columns.Add(Common.ComboBoxValue);

            bool ret = true;

            try
            {
                
                DataTable table = new DataTable();
                Dictionary<string, int> except_dic = new Dictionary<string, int>();
                List<string> table_list = new List<string>();

                if (dic.Equals("2"))
                {
                    // 入出庫実績・在庫
                    table_list.Add("受払");
                    table_list.Add("個体の情報");
                    table_list.Add("ロット情報");
                    table_list.Add("タンク");
                }
                else if (dic.Equals("14"))
                {
                    // 日別在庫
                    table_list.Add("在庫（日別）");
                }
                else if (dic.Equals("15"))
                {
                    // 月末在庫
                    table_list.Add("月末在庫");
                }
                else
                {
                    // 上記以外
                    table_list.Add("製造指図情報");
                    table_list.Add("実行処方製品");
                    table_list.Add("実行処方ヘッダ");
                }

                foreach (string table_name in table_list)
                {
                    if (!db.GetItemNameData(table_name, out table))
                    {
                        return false;
                    }

                    foreach (DataColumn col in table.Columns)
                    {
                        if (except_dic.ContainsKey(col.ColumnName))
                        {
                            continue;
                        }
                        DataRow newrow = rtndt.NewRow();
                        newrow[Common.ComboBoxText] = col.ColumnName;
                        newrow[Common.ComboBoxValue] = Common.table_dic[table_name] + ".[" + col.ColumnName + "]";
                        rtndt.Rows.Add(newrow);

                        except_dic[col.ColumnName] = 1;
                    }
                }

            }
            catch (Exception ex)
            {
                ret = false;
            }
            finally
            {
                
            }
            return ret;
        }

        /// <summary>
        /// データ検索
        /// </summary>
        /// <returns>true/false</returns>
        public bool SearchData(SearchData seach, string dic, out DataTable resulrdt)
        {
            if (dic.Equals("2"))
            {
                // 入出庫実績・在庫
                return db.SearchInOutData(seach, dic, out resulrdt);
            }
            else if (dic.Equals("14"))
            {
                // 日別在庫
                return db.SearchDailyStockData(seach, dic, out resulrdt);
            }
            else if (dic.Equals("15"))
            {
                // 月末在庫
                return db.SearchMonthlyStockData(seach, dic, out resulrdt);
            }
            else
            {
                return db.SearchData(seach, dic, out resulrdt);
            }
            
        }

        public bool GetOutputItem(string dic, out DataTable dt)
        {
            dt = new DataTable();

            DataTable tmpdt = new DataTable();
            db.GetOutputItemDisplay(dic, out tmpdt);

            if (tmpdt.Rows.Count>0){
                foreach (DataRow row in tmpdt.Rows)
                {
                    string col = row["項目名"].ToString();
                    if (!dt.Columns.Contains(col))
                    {
                        dt.Columns.Add(col);
                    }
                }
            }
            else
            {
                
                //List<string> colmun_list = new List<string>();
                //this.GetItemList(dic, out colmun_list);

                //foreach (string colmun in colmun_list)
                //{
                //    dt.Columns.Add(colmun);
                //}

            }

            return true;
        }

        public bool GetSort(string dic, out List<string> sort)
        {
            sort = new List<string>();

            DataTable dt = new DataTable();
            db.GetSort(dic, out dt);

            foreach (DataRow row in dt.Rows)
            {
                sort.Add(row["項目名"].ToString() + " " + row["ソート方向"].ToString());
            }

            return true;
        }

        /// <summary>
        /// テーブルカラム名取得
        /// </summary>
        /// <returns>true/false</returns>
        public bool GetItemList(string dic, out List<string> colmun_list)
        {
            colmun_list = new List<string>();

            return Common.GetItemList(db, dic, out colmun_list);

        }


    }
}
