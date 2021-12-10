using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public bool GetItemList(out List<string> colmun_list)
        {
            colmun_list = new List<string>();

            foreach (KeyValuePair<string, string> kvp in Common.table_dic)
            {
                string table_name = kvp.Key;

                DataTable table = new DataTable();
                if (!db.GetItemNameData(table_name, out table))
                {
                    return false;
                }

                foreach (DataColumn col in table.Columns)
                {
                    colmun_list.Add("["+table_name+"].["+col.ColumnName+"]");
                }

            }

            return true;
        }
    }
}
