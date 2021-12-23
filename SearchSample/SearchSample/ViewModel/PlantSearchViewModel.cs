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

        private readonly DBAccess db = new DBAccess();

        #endregion

        #region "変数"

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
