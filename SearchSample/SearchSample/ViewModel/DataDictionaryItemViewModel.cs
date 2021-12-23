using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchSample.ViewModel
{
    /// <summary>
    /// エラーダイアログ ViewModel
    /// </summary>
    class DataDictionaryItemViewModel
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
        public DataDictionaryItemViewModel()
        {

        }

        /// <summary>
        /// データ検索
        /// </summary>
        /// <returns>true/false</returns>
        public bool SearchData(string dataDictionaryId, out DataTable resulrdt)
        {
            return db.SearchDataDictionaryItem(dataDictionaryId, out resulrdt);
        }
    }
}
