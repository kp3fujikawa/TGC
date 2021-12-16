using SearchSample.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SearchSample.ViewModel
{
    /// <summary>
    /// お気に入りダイアログ ViewModel
    /// </summary>
    class OutputSettingDialogViewModel
    {
        #region "DBアクセスモデル"

        private DBAccess db = new DBAccess();

        #endregion

        #region "定数"

        #endregion

        #region "変数"

        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public OutputSettingDialogViewModel()
        {

        }

        /// <summary>
        /// テーブルカラム名取得
        /// </summary>
        /// <returns>true/false</returns>
        public bool GetItemList(string dic, out List<string> colmun_list)
        {
            colmun_list = new List<string>();
            Dictionary<string, int> except_list = new Dictionary<string, int>();

            //List<string> tables = new List<string>();
            //tables.Add("製造指図情報");
            //tables.Add("実行処方製品");
            //tables.Add("実行処方ヘッダ");
            //tables.Add("実行処方要素");
            //tables.Add("実行処方パラメータ");
            //if (!dic.Equals("2"))
            //{
            //    tables.Add("検査性状値");
            //}

            //foreach (string table_name in tables)
            //{
            //   DataTable table = new DataTable();
            //    if (!db.GetItemNameData(table_name, out table))
            //    {
            //        return false;
            //    }

            //    foreach (DataColumn col in table.Columns)
            //    {
            //        colmun_list.Add("["+table_name+"].["+col.ColumnName+"]");
            //    }

            //}

            DataTable output_item_dt1 = new DataTable();
            db.GetItem1(dic, out output_item_dt1);

            foreach (DataColumn col in output_item_dt1.Columns)
            {
                if (!except_list.ContainsKey(col.ColumnName))
                {
                    except_list[col.ColumnName] = 0;
                    colmun_list.Add(col.ColumnName);
                }
            }

            DataTable output_item_dt2 = new DataTable();
            db.GetItem2(dic, out output_item_dt2);

            foreach (DataColumn col in output_item_dt2.Columns)
            {
                if (!except_list.ContainsKey(col.ColumnName))
                {
                    except_list[col.ColumnName] = 0;
                    colmun_list.Add(col.ColumnName);
                }
            }

            DataTable output_item_dt3 = new DataTable();
            db.GetItemParameter(dic, out output_item_dt3);

            foreach (DataColumn col in output_item_dt3.Columns)
            {
                //colmun_list.Add("[要素名称"+Common.ColCennector+"パラメータ名称].[" + col.ColumnName + "]");
                if (!except_list.ContainsKey(col.ColumnName))
                {
                    except_list[col.ColumnName] = 0;
                    colmun_list.Add(col.ColumnName);
                }
            }

            return true;
        }

        public bool GetOutputItem(string dic, out DataTable dt)
        {
            db.GetOutputItem(dic, out dt);
            return true;
        }

        public bool SetOutputItem(string dic, List<OutputItem> list)
        {
            db.SetOutputItem(dic, list);
            return true;
        }

        public bool DeleteOutputItem(string dic)
        {
            db.DeleteOutputItem(dic);
            return true;
        }

    }
}
