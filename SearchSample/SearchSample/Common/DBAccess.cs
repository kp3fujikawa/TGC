using SearchSample.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;

namespace SearchSample
{
    /// <summary>
    /// DBアクセスクラス
    /// </summary>
    class DBAccess
    {
        #region "定数"
        /// <summary>
        /// DB接続文字列：Provider
        /// </summary>
        public string connectionStringBase = Common.connectionStringBase;

        /// <summary>
        /// 内部保持データ（MDB）
        /// </summary>
        private string LocalDBName = Common.LocalDBName;

        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DBAccess()
        {

        }

        /// <summary>
        /// 対象テーブルの読み込み
        /// </summary>
        /// <param name="table_name">対象テーブル名</param>
        /// <param name="dic_target_table">データ配列</param>
        /// <returns>true/false</returns>
        public bool GetTargetTable(
            String table_name,
            ref Dictionary<string, DataColumn> dic_target_table
            )
        {
            // DB接続文字列作成
            string connectionString = connectionStringBase + LocalDBName;
            bool ret = true;

            DataTable dt = new DataTable();
            dic_target_table = new Dictionary<string, DataColumn>();

            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    connection.Open();

                    string queryString = "SELECT * FROM " + table_name + " WHERE 0=1";
                    OleDbDataAdapter adapter = new OleDbDataAdapter(queryString, connection);
                    adapter.Fill(dt);
                    adapter.Dispose();

                    connection.Close();

                    foreach (DataColumn col in dt.Columns)
                    {
                        if (!dic_target_table.ContainsKey(col.ColumnName))
                        {
                            dic_target_table[col.ColumnName] = col;
                        }
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
        /// 項目名の読み込み
        /// </summary>
        /// <param name="table_name">対象テーブル名</param>
        /// <param name="itemNameDT">データテーブル</param>
        /// <returns>true/false</returns>
        public bool GetItemNameData(
            String table_name,
            out DataTable itemNameDT
            )
        {
            // DB接続文字列作成
            string connectionString = connectionStringBase + LocalDBName;
            bool ret = true;

            itemNameDT = new DataTable();

            if (string.IsNullOrEmpty(table_name)) return true;

            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    try
                    {

                        connection.Open();

                        //DataTable dt = new DataTable();
                        string queryString = "SELECT * FROM [" + table_name + "] WHERE 0=1";
                        OleDbDataAdapter adapter = new OleDbDataAdapter(queryString, connection);
                        adapter.Fill(itemNameDT);
                        adapter.Dispose();

                    }
                    catch (Exception ex)
                    {
                        ret = false;
                    }
                    finally
                    {
                        connection.Close();
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
        /// 項目名の読み込み
        /// </summary>
        /// <param name="table_name">対象テーブル名</param>
        /// <param name="itemNameDT">データテーブル</param>
        /// <returns>true/false</returns>
        public bool SearchData(
            SearchData seach,
            out DataTable resulrdt
            )
        {
            // DB接続文字列作成
            string connectionString = connectionStringBase + LocalDBName;
            bool ret = true;

            resulrdt = new DataTable();
            DataTable productdt = new DataTable();
            DataTable paramdt = new DataTable();
            DataTable testdt = new DataTable();

            bool addparam = false;
            bool addtest = false;

            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    try
                    {

                        connection.Open();

                        string T1 = Common.table_dic["製造指図情報"];
                        string T2 = Common.table_dic["実行処方製品"];
                        string T3 = Common.table_dic["実行処方ヘッダ"];
                        string T4 = Common.table_dic["実行処方要素"];
                        string T5 = Common.table_dic["実行処方パラメータ"];
                        string T6 = Common.table_dic["実行処方リソース"];
                        string T7 = Common.table_dic["実行処方その他情報"];
                        string T8 = Common.table_dic["原材料引当"];
                        string T9 = Common.table_dic["検査性状値"];
                        string T10 = Common.table_dic["検査結果"];


                        /**
                         * 実行処方ヘッダ　検索 
                         */

                        string SQL = "";
                        SQL += " SELECT ";

                        SQL += " " + T1 + ".[製造指図番号] ";
                        SQL += " ," + T1 + ".[製造指図ステータス] ";
                        SQL += " ," + T1 + ".[品目コード] ";
                        SQL += " ," + T1 + ".[品名] ";
                        SQL += " ," + T1 + ".[SAP製造指図番号] ";
                        SQL += " ," + T1 + ".[ASTキー] ";
                        SQL += " ," + T1 + ".[指図作成区分] ";
                        SQL += " ," + T1 + ".[ASTPLANNER計画取込状態] ";
                        SQL += " ," + T1 + ".[ASTPLANNER計画取込状態変更者] ";
                        SQL += " ," + T1 + ".[ASTPLANNER計画取込状態変更日時] ";
                        SQL += " ," + T1 + ".[Exapilot連携状況] ";
                        SQL += " ," + T1 + ".[製造指図確認者] ";
                        SQL += " ," + T1 + ".[製造指図確認日] ";
                        SQL += " ," + T1 + ".[充填包装確認者] ";
                        SQL += " ," + T1 + ".[充填包装確認日] ";
                        SQL += " ," + T1 + ".[締め実行フラグ] ";

                        //SQL += " ," + T2 + ".[製造指図番号] ";
                        SQL += " ," + T2 + ".[連番] ";
                        SQL += " ," + T2 + ".[処方ID] ";
                        SQL += " ," + T2 + ".[処方バージョン] ";
                        SQL += " ," + T2 + ".[処方製品連番] ";
                        SQL += " ," + T2 + ".[表示順] ";
                        SQL += " ," + T2 + ".[製品種別] ";
                        SQL += " ," + T2 + ".[荷姿コード] ";
                        SQL += " ," + T2 + ".[荷姿名] ";
                        SQL += " ," + T2 + ".[入り目] ";
                        SQL += " ," + T2 + ".[小数点以下有効桁数] ";
                        SQL += " ," + T2 + ".[端数処理コード] ";
                        SQL += " ," + T2 + ".[製品グループ] ";
                        SQL += " ," + T2 + ".[製品コード] ";
                        SQL += " ," + T2 + ".[製品グレード] ";
                        SQL += " ," + T2 + ".[製品名] ";
                        SQL += " ," + T2 + ".[製造予定量] ";
                        SQL += " ," + T2 + ".[備考] ";
                        SQL += " ," + T2 + ".[向け先コード] ";
                        SQL += " ," + T2 + ".[向け先名称] ";
                        SQL += " ," + T2 + ".[出荷ロット] ";
                        SQL += " ," + T2 + ".[SAPロット] ";
                        SQL += " ," + T2 + ".[在庫ユニークキー] ";
                        SQL += " ," + T2 + ".[製造予定量確定値] ";
                        SQL += " ," + T2 + ".[製造実績量収集値] ";
                        SQL += " ," + T2 + ".[製造実績量確定値] ";
                        SQL += " ," + T2 + ".[SAP製造日] ";
                        SQL += " ," + T2 + ".[SAP保証日] ";

                        //SQL += " ," + T3 + ".[製造指図番号] ";
                        //SQL += " ," + T3 + ".[処方ID] ";
                        //SQL += " ," + T3 + ".[処方バージョン] ";
                        SQL += " ," + T3 + ".[有効期限（自）] ";
                        SQL += " ," + T3 + ".[有効期限（至）] ";
                        SQL += " ," + T3 + ".[系列コード] ";
                        SQL += " ," + T3 + ".[組織コード] ";
                        SQL += " ," + T3 + ".[組織名] ";
                        SQL += " ," + T3 + ".[製品コード] ";
                        SQL += " ," + T3 + ".[製品名] ";
                        SQL += " ," + T3 + ".[製品グレード] ";
                        SQL += " ," + T3 + ".[処方スケール] ";
                        SQL += " ," + T3 + ".[単位] ";
                        SQL += " ," + T3 + ".[最大値] ";
                        SQL += " ," + T3 + ".[最小値] ";
                        SQL += " ," + T3 + ".[理論収量設定値] ";
                        SQL += " ," + T3 + ".[処方作成日時] ";
                        SQL += " ," + T3 + ".[処方承認日時] ";
                        SQL += " ," + T3 + ".[処方作成者] ";
                        SQL += " ," + T3 + ".[処方承認者] ";
                        SQL += " ," + T3 + ".[元処方ID] ";
                        SQL += " ," + T3 + ".[元処方バージョン] ";
                        SQL += " ," + T3 + ".[有効期間ID] ";
                        SQL += " ," + T3 + ".[SAP製造バージョン] ";
                        SQL += " ," + T3 + ".[備考] ";
                        SQL += " ," + T3 + ".[バッチ実行回数] ";
                        SQL += " ," + T3 + ".[製造予定数量] ";
                        SQL += " ," + T3 + ".[製造実績数量] ";
                        SQL += " ," + T3 + ".[開始予定日時] ";
                        SQL += " ," + T3 + ".[終了予定日時] ";
                        SQL += " ," + T3 + ".[開始実績日時] ";
                        SQL += " ," + T3 + ".[終了実績日時] ";
                        SQL += " ," + T3 + ".[製造ロット] ";
                        SQL += " ," + T3 + ".[着手実績日時] ";
                        SQL += " ," + T3 + ".[中止実績日時] ";
                        SQL += " ," + T3 + ".[Exapilot終了実績日時] ";
                        SQL += " ," + T3 + ".[Exapilot中止実績日時] ";
                        SQL += " ," + T3 + ".[収量用ブレンド量] ";
                        SQL += " ," + T3 + ".[収量] ";
                        SQL += " ," + T3 + ".[理論収量] ";
                        SQL += " ," + T3 + ".[収率] ";
                        SQL += " ," + T3 + ".[製造指図メモ] ";

                        SQL += " FROM ([製造指図情報] AS " + T1 + " ";
                        SQL += " LEFT JOIN [実行処方製品] AS " + T2 + " ON (" + T2 + ".[製造指図番号] = " + T1 + ".[製造指図番号]) ) ";
                        SQL += " LEFT JOIN [実行処方ヘッダ] AS " + T3 + " ON (" + T3 + ".[製造指図番号] = " + T1 + ".[製造指図番号] ) ";

                        SQL += " WHERE 1=1 " + CreateSearchCondiotion(seach, out addparam, out addtest);

                        OleDbDataAdapter adapter = new OleDbDataAdapter(SQL, connection);
                        adapter.Fill(productdt);
                        adapter.Dispose();

                        //List<string> param_list = new List<string>();
                        //foreach (string param in seach.qulity_list)
                        //{
                        //    param_list.Add("'" + param + "'");
                        //}

                        /**
                         * 実行処方要素、パラメータ　検索 
                         */

                        paramdt = productdt.Copy();

                        string[] element_cols = {
                                    "工程予定時間",
                                    "工程標準廃液量",
                                    "機器グループID",
                                    "前段取り時間",
                                    "製造時間",
                                    "後段取り時間",
                                    "標準廃液量",
                                    "備考",
                                    "後続ステップID",
                                    "開始予定日時",
                                    "終了予定日時",
                                    "工程ステータス",
                                    "工程終了取消フラグ",
                                    "使用機器ID",
                                    "実績前段取り時間",
                                    "実績製造時間",
                                    "実績後段取り時間",
                                    "開始実績日時",
                                    "終了実績日時",
                                    "工程指図メモ"
                                    };

                        foreach (DataRow row in productdt.Rows)
                        {

                            string pno = row["製造指図番号"].ToString();

                            SQL = string.Empty;

                            SQL += " SELECT ";

                            SQL += " " + T4 + ".[製造指図番号] ";
                            SQL += " ," + T4 + ".[処方ID] ";
                            SQL += " ," + T4 + ".[処方バージョン] ";
                            SQL += " ," + T4 + ".[ステップID] ";
                            SQL += " ," + T4 + ".[要素名称] ";
                            SQL += " ," + T4 + ".[工程予定時間] ";
                            SQL += " ," + T4 + ".[工程標準廃液量] ";
                            SQL += " ," + T4 + ".[機器グループID] ";
                            SQL += " ," + T4 + ".[前段取り時間] ";
                            SQL += " ," + T4 + ".[製造時間] ";
                            SQL += " ," + T4 + ".[後段取り時間] ";
                            SQL += " ," + T4 + ".[標準廃液量] ";
                            SQL += " ," + T4 + ".[備考] ";
                            SQL += " ," + T4 + ".[後続ステップID] ";
                            SQL += " ," + T4 + ".[開始予定日時] ";
                            SQL += " ," + T4 + ".[終了予定日時] ";
                            SQL += " ," + T4 + ".[工程ステータス] ";
                            SQL += " ," + T4 + ".[工程終了取消フラグ] ";
                            SQL += " ," + T4 + ".[使用機器ID] ";
                            SQL += " ," + T4 + ".[実績前段取り時間] ";
                            SQL += " ," + T4 + ".[実績製造時間] ";
                            SQL += " ," + T4 + ".[実績後段取り時間] ";
                            SQL += " ," + T4 + ".[開始実績日時] ";
                            SQL += " ," + T4 + ".[終了実績日時] ";
                            SQL += " ," + T4 + ".[工程指図メモ] ";

                            SQL += " ," + T5 + ".[パラメータID] ";
                            SQL += " ," + T5 + ".[パラメータ名称] ";
                            SQL += " ," + T5 + ".[設定値] ";
                            SQL += " ," + T5 + ".[工業単位] ";
                            SQL += " ," + T5 + ".[指図確定時確認] ";
                            SQL += " ," + T5 + ".[表示順] ";
                            SQL += " ," + T5 + ".[実績収集] ";
                            SQL += " ," + T5 + ".[小数点以下有効桁数] ";
                            SQL += " ," + T5 + ".[端数処理コード] ";
                            SQL += " ," + T5 + ".[パラメータ属性] ";
                            SQL += " ," + T5 + ".[ExapilotDL対象] ";
                            SQL += " ," + T5 + ".[パラメータ種別] ";
                            SQL += " ," + T5 + ".[SAP構成品明細番号] ";
                            SQL += " ," + T5 + ".[品目コード] ";
                            SQL += " ," + T5 + ".[品名] ";
                            SQL += " ," + T5 + ".[原材料グレード] ";
                            SQL += " ," + T5 + ".[計量パターン] ";
                            SQL += " ," + T5 + ".[計量上限] ";
                            SQL += " ," + T5 + ".[計量下限] ";
                            SQL += " ," + T5 + ".[仕込パターン] ";
                            SQL += " ," + T5 + ".[端切単位] ";
                            SQL += " ," + T5 + ".[原材料投入口] ";
                            SQL += " ," + T5 + ".[廃液排出予定時間] ";
                            SQL += " ," + T5 + ".[廃液排出タンク] ";
                            SQL += " ," + T5 + ".[充填パターン] ";
                            SQL += " ," + T5 + ".[充填包装設備] ";
                            SQL += " ," + T5 + ".[製品種別] ";
                            SQL += " ," + T5 + ".[荷姿コード] ";
                            SQL += " ," + T5 + ".[荷姿名称] ";
                            SQL += " ," + T5 + ".[荷姿数量] ";
                            SQL += " ," + T5 + ".[荷姿個数] ";
                            SQL += " ," + T5 + ".[数量] ";
                            SQL += " ," + T5 + ".[サンプル採取予定時刻] ";
                            SQL += " ," + T5 + ".[サンプルコード] ";
                            SQL += " ," + T5 + ".[プロトコルコード] ";
                            SQL += " ," + T5 + ".[SAP消費生産] ";
                            SQL += " ," + T5 + ".[在庫受払対象] ";
                            SQL += " ," + T5 + ".[SAP連携対象] ";
                            SQL += " ," + T5 + ".[備考] AS 備考T5 ";
                            SQL += " ," + T5 + ".[確定値] ";
                            SQL += " ," + T5 + ".[実績収集値] ";
                            SQL += " ," + T5 + ".[実績確定値] ";
                            SQL += " ," + T5 + ".[設定値変更者] ";
                            SQL += " ," + T5 + ".[設定値変更日時] ";
                            SQL += " ," + T5 + ".[実績値変更者] ";
                            SQL += " ," + T5 + ".[実績値変更日時] ";
                            SQL += " ," + T5 + ".[設定値確認者] ";
                            SQL += " ," + T5 + ".[設定値確認日時] ";
                            SQL += " ," + T5 + ".[実績値確認者] ";
                            SQL += " ," + T5 + ".[実績値確認日時] ";
                            SQL += " ," + T5 + ".[実績品目コード] ";
                            SQL += " ," + T5 + ".[実績品名] ";
                            SQL += " ," + T5 + ".[実績ロット] ";
                            SQL += " ," + T5 + ".[実績廃液排出時間] ";
                            SQL += " ," + T5 + ".[実績サンプル採取時間] ";
                            SQL += " ," + T5 + ".[MES検査予定番号] ";
                            SQL += " ," + T5 + ".[実績サンプルコード] ";
                            SQL += " ," + T5 + ".[実績プロトコルコード] ";
                            SQL += " ," + T5 + ".[Lab-Aid依頼番号] ";
                            SQL += " ," + T5 + ".[Lab-Aid合否判定] ";
                            SQL += " ," + T5 + ".[計量支援指図番号] ";
                            SQL += " ," + T5 + ".[実績荷姿コード] ";
                            SQL += " ," + T5 + ".[実績荷姿名] ";
                            SQL += " ," + T5 + ".[実績荷姿数量] ";
                            SQL += " ," + T5 + ".[実績荷姿個数] ";
                            SQL += " ," + T5 + ".[実績数量] ";
                            SQL += " ," + T5 + ".[SAP簿外] ";
                            SQL += " ," + T5 + ".[SAP転記日] ";
                            SQL += " ," + T5 + ".[SAP連携済フラグ] ";

                            SQL += " FROM [実行処方要素] AS " + T4 + " ";
                            SQL += " LEFT JOIN [実行処方パラメータ] AS " + T5 + " ON " + T4 + ".[製造指図番号] = " + T5 + ".[製造指図番号] AND " + T4 + ".[ステップID] = " + T5 + ".[ステップID] ";

                            SQL += " WHERE " + T4 + ".[製造指図番号] = '" + pno + "'";
                            //if (param_list.Count>0)
                            //{
                            //    SQL += " AND " + T5 + ".[パラメータ名称] in (" + string.Join(",", param_list) + ")";
                            //}
                            
                            DataTable tmpparamdt = new DataTable();

                            OleDbDataAdapter adapter2 = new OleDbDataAdapter(SQL, connection);
                            adapter2.Fill(tmpparamdt);
                            adapter2.Dispose();

                            List<string> element_param_list = new List<string>();

                            foreach (DataRow row2 in tmpparamdt.Rows)
                            {
                                string element_name = row2["要素名称"].ToString();
                                string param_name = row2["パラメータ名称"].ToString();
                                string col_name = string.Empty;

                                foreach (string col in element_cols)
                                {
                                    col_name = element_name + Common.ColCennector + col;
                                    if (!paramdt.Columns.Contains(col_name))
                                    {
                                        paramdt.Columns.Add(col_name);
                                    }
                                }

                                col_name = element_name + Common.ColCennector + param_name;
                                if (!paramdt.Columns.Contains(col_name))
                                {
                                    paramdt.Columns.Add(col_name);
                                }

                                element_param_list.Add(element_name + Common.ColCennector + param_name);

                            }

                            DataRow[] drs = paramdt.Select("製造指図番号 = '" + pno + "'");

                            foreach (DataRow row2 in tmpparamdt.Rows)
                            {
                                string element_name = row2["要素名称"].ToString();
                                string param_name = row2["パラメータ名称"].ToString();
                                string col_name = string.Empty;

                                foreach (string col in element_cols)
                                {
                                    col_name = element_name + Common.ColCennector + col;
                                    drs[0][col_name] = row2[col];
                                }

                                col_name = element_name + Common.ColCennector + param_name;
                                drs[0][col_name] = row2["実績確定値"];
                            }

                            /**
                            * 品質データ抽出（検査性状値、検査結果）検索 
                            */

                            if (seach.qulity_check && seach.qulity_list.Count > 0)
                            {

                                testdt = paramdt.Copy();

                                string[] test_cols = {
                                        "サンプルコード",
                                        "ホルダコード",
                                        "試験項目コード",
                                        "単位コード",
                                        "ホルダ名",
                                        "試験項目名",
                                        "単位名",
                                        "試験結果（生データ）",
                                        "試験結果（編集後）",
                                        "試験結果（報告用）",
                                        "規格１上限",
                                        "規格１下限",
                                        };

                                // 品質データ抽出チェックあり
                                foreach (string element_param_name in element_param_list)
                                {

                                    foreach (string test_name in seach.qulity_list)
                                    {
                                        string col_name = string.Empty;

                                        foreach (string col in test_cols)
                                        {
                                            col_name = element_param_name + Common.ColCennector + test_name + Common.ColCennector + col;
                                            if (!testdt.Columns.Contains(col_name))
                                            {
                                                testdt.Columns.Add(col_name);
                                            }
                                        }
                                    }

                                }

                                List<string> param_list = new List<string>();
                                foreach (string param in seach.qulity_list)
                                {
                                    param_list.Add("'" + param + "'");
                                }

                                SQL = string.Empty;

                                SQL += " SELECT ";

                                SQL += " " + T4 + ".[要素名称] ";
                                SQL += " ," + T5 + ".[パラメータ名称] ";

                                SQL += " ," + T9 + ".[Lab-Aid依頼番号] ";
                                SQL += " ," + T9 + ".[サンプルコード] ";
                                SQL += " ," + T9 + ".[ホルダコード] ";
                                SQL += " ," + T9 + ".[試験項目コード] ";
                                SQL += " ," + T9 + ".[単位コード] ";
                                SQL += " ," + T9 + ".[ホルダ名] ";
                                SQL += " ," + T9 + ".[試験項目名] ";
                                SQL += " ," + T9 + ".[単位名] ";
                                SQL += " ," + T9 + ".[試験結果（生データ）] ";
                                SQL += " ," + T9 + ".[試験結果（編集後）] ";
                                SQL += " ," + T9 + ".[試験結果（報告用）] ";
                                SQL += " ," + T9 + ".[規格１上限] ";
                                SQL += " ," + T9 + ".[規格１下限] ";

                                SQL += " FROM ([実行処方要素] AS " + T4 + " ";
                                SQL += " LEFT JOIN [実行処方パラメータ] AS " + T5 + " ON (" + T4 + ".[製造指図番号] = " + T5 + ".[製造指図番号] AND " + T4 + ".[ステップID] = " + T5 + ".[ステップID]) ) ";
                                SQL += " LEFT JOIN [検査性状値] AS " + T9 + " ON (" + T9 + ".[Lab-Aid依頼番号] = " + T5 + ".[Lab-Aid依頼番号]) ";

                                SQL += " WHERE " + T4 + ".[製造指図番号] = '" + pno + "'";
                                SQL += " AND " + T9 + ".[試験項目名] in (" + string.Join(",", param_list) + ")";

                                DataTable tmptestdt = new DataTable();

                                OleDbDataAdapter adapter3 = new OleDbDataAdapter(SQL, connection);
                                adapter3.Fill(tmptestdt);
                                adapter3.Dispose();

                                DataRow[] drs2 = testdt.Select("製造指図番号 = '" + pno + "'");

                                foreach (DataRow row2 in tmptestdt.Rows)
                                {
                                    string element_name = row2["要素名称"].ToString();
                                    string param_name = row2["パラメータ名称"].ToString();
                                    string test_name = row2["試験項目名"].ToString();
                                    string col_name = string.Empty;

                                    foreach (string col in test_cols)
                                    {
                                        col_name = element_name + Common.ColCennector + param_name + Common.ColCennector + test_name + Common.ColCennector + col;
                                        drs2[0][col_name] = row2[col];
                                    }
                                }

                                paramdt = testdt.Copy();

                            }

                        }

                        if (seach.qulity_check && seach.qulity_list.Count > 0)
                        {
                            resulrdt = testdt.Copy();
                        }
                        else
                        {
                            resulrdt = paramdt.Copy();
                        }

                    }
                    catch (Exception ex)
                    {
                        ret = false;
                    }
                    finally
                    {
                        connection.Close();
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

        public string CreateSearchCondiotion(SearchData s, out bool addparam, out bool addtest)
        {
            string where = "";
            string T1 = Common.table_dic["製造指図情報"];
            string T2 = Common.table_dic["実行処方製品"];
            string T3 = Common.table_dic["実行処方ヘッダ"];

            addparam = false;
            addtest = false;

            foreach (SearchCondition sc in s.search_list)
            {
                if (sc.item.Equals(Common.item_param))
                {
                    addparam = true;
                    continue;
                }
                if (sc.item.Equals(Common.item_test))
                {
                    addtest = true;
                    continue;
                }

                if (!string.IsNullOrEmpty(sc.item) && !string.IsNullOrEmpty(sc.condition))
                {
                    where += " " + (string.IsNullOrEmpty(sc.combi) ? " AND " : sc.combi)
                        + " " + sc.item + " " + sc.condition + " '" + sc.value + "' ";
                }
            }

            if (s.product_date_start != null)
            {
                DateTime dt = (DateTime)s.product_date_start;
                where += " AND " + T3 + ".[開始実績日時] >= '" + dt.ToString("yyyyMMdd") + "000000'" ;
            }

            if (s.product_date_end != null)
            {
                DateTime dt = (DateTime)s.product_date_end;
                where += " AND " + T3 + ".[開始実績日時] <= '" + dt.ToString("yyyyMMdd") + "235959'";
            }

            //if (s.qulity_list.Count>0) addparam = true;
            //if (s.process_list.Count > 0) addtest = true;

            return where;

        }

        /// <summary>
        /// 項目名の読み込み
        /// </summary>
        /// <param name="table_name">対象テーブル名</param>
        /// <param name="itemNameDT">データテーブル</param>
        /// <returns>true/false</returns>
        public bool GetElementParameterList(
            out DataTable resulrdt
            )
        {
            // DB接続文字列作成
            string connectionString = connectionStringBase + LocalDBName;
            bool ret = true;

            resulrdt = new DataTable();

            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    try
                    {

                        connection.Open();

                        string queryString = "SELECT * FROM [実行処方パラメータ]";
                        OleDbDataAdapter adapter = new OleDbDataAdapter(queryString, connection);
                        adapter.Fill(resulrdt);
                        adapter.Dispose();

                    }
                    catch (Exception ex)
                    {
                        ret = false;
                    }
                    finally
                    {
                        connection.Close();
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
        /// 項目名の読み込み
        /// </summary>
        /// <param name="table_name">対象テーブル名</param>
        /// <param name="itemNameDT">データテーブル</param>
        /// <returns>true/false</returns>
        public bool GetTestitemList(
            out DataTable resulrdt
            )
        {
            // DB接続文字列作成
            string connectionString = connectionStringBase + LocalDBName;
            bool ret = true;

            resulrdt = new DataTable();

            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    try
                    {

                        connection.Open();

                        string queryString = "SELECT * FROM [検査性状値]";
                        OleDbDataAdapter adapter = new OleDbDataAdapter(queryString, connection);
                        adapter.Fill(resulrdt);
                        adapter.Dispose();

                    }
                    catch (Exception ex)
                    {
                        ret = false;
                    }
                    finally
                    {
                        connection.Close();
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


    }
}
