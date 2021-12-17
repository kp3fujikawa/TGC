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
            return Common.GetItemList( db, dic, out colmun_list);
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
