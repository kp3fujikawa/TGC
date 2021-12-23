using System;
using SearchSample.DataModel;
using System.Data;
using System.Collections.Generic;
using SearchSample.Model;

namespace SearchSample.ViewModel
{
    /// <summary>
    /// データディクショナリ情報検索画面 ViewModel
    /// </summary>
    class DataDictionaryViewModel
    {
        #region "DBアクセスモデル"

        private DBAccess db = new DBAccess();

        #endregion

        #region "変数"

        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DataDictionaryViewModel()
        {

        }

        /// <summary>
        /// データ検索
        /// </summary>
        /// <returns>true/false</returns>
        public bool SearchData(DataDictionarySearchData seach,out DataTable resulrdt)
        {
            return db.SearchDataDictionary(seach, out resulrdt);
        }

    }
}
