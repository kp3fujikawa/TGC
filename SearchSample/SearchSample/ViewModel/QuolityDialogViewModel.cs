﻿using System;
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
    class QuolityDialogViewModel
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
        public QuolityDialogViewModel()
        {

        }

        /// <summary>
        ///検査項目名一覧取得
        /// </summary>
        /// <returns>true/false</returns>
        public bool GetTestitemNameList(out DataTable resulrdt)
        {
            return db.GetTestitemNameList(out resulrdt);
        }
    }
}
