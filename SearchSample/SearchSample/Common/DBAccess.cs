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
        /// データ検索
        /// </summary>
        /// <param name="table_name">対象テーブル名</param>
        /// <param name="itemNameDT">データテーブル</param>
        /// <returns>true/false</returns>
        public bool SearchData(
            SearchData seach,
            string dic,
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
                        SQL += SelectItem1(dic);
                        

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

                        string[] element_cols = GetElementCol(dic);

                        foreach (DataRow row in productdt.Rows)
                        {

                            string pno = row["製造指図番号"].ToString();

                            SQL = string.Empty;

                            SQL += " SELECT ";

                            SQL += SelectItem2(dic);

                            SQL += " FROM [実行処方要素] AS " + T4 + " ";
                            SQL += " LEFT JOIN [実行処方パラメータ] AS " + T5 + " ON " + T4 + ".[製造指図番号] = " + T5 + ".[製造指図番号] AND " + T4 + ".[ステップID] = " + T5 + ".[ステップID] ";

                            SQL += " WHERE " + T4 + ".[製造指図番号] = '" + pno + "'";
                            if (dic.Equals("4"))
                            {
                                SQL += " AND MID(" + T4 + ".[ステップID],1,1) = '3'";
                            }

                            SQL += " ORDER BY " + T4 + ".[製造指図番号], " + T4 + ".[ステップID], " + T5 + ".[パラメータID] ";

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
                                string param_name = row2["パラメータ名称"].ToString().Replace(".","-");
                                string col_name = string.Empty;

                                foreach (string col in element_cols)
                                {
                                    col_name = element_name + Common.ColCennector + col;
                                    if (!paramdt.Columns.Contains(col_name))
                                    {
                                        paramdt.Columns.Add(col_name);
                                    }
                                }

                                if (!string.IsNullOrEmpty(param_name))
                                {
                                    col_name = element_name + Common.ColCennector + param_name;
                                    if (!paramdt.Columns.Contains(col_name))
                                    {
                                        paramdt.Columns.Add(col_name);
                                    }

                                    element_param_list.Add(element_name + Common.ColCennector + param_name);
                                }

                            }

                            DataRow[] drs = paramdt.Select("製造指図番号 = '" + pno + "'");

                            foreach (DataRow row2 in tmpparamdt.Rows)
                            {
                                string element_name = row2["要素名称"].ToString();
                                string param_name = row2["パラメータ名称"].ToString().Replace(".", "-");
                                string col_name = string.Empty;

                                foreach (string col in element_cols)
                                {
                                    col_name = element_name + Common.ColCennector + col;
                                    drs[0][col_name] = row2[col];
                                }

                                if (!string.IsNullOrEmpty(param_name))
                                {
                                    col_name = element_name + Common.ColCennector + param_name;
                                    drs[0][col_name] = row2["実績確定値"];
                                }
                            }

                            /**
                            * 品質データ抽出（検査性状値、検査結果）検索 
                            */

                            if (seach.qulity_check && seach.qulity_list.Count > 0)
                            {

                                testdt = paramdt.Copy();

                                string[] test_cols = {
//                                        "サンプルコード",
//                                        "ホルダコード",
//                                        "試験項目コード",
//                                        "単位コード",
//                                        "ホルダ名",
//                                        "試験項目名",
                                        "単位名",
//                                        "試験結果生データ",
//                                        "試験結果編集後",
                                        "試験結果報告用",
//                                        "規格１上限",
//                                        "規格１下限",
                                        };

                                // 品質データ抽出チェックあり
                                //foreach (string element_param_name in element_param_list)
                                //{

                                //    foreach (string test_name in seach.qulity_list)
                                //    {
                                //        string col_name = string.Empty;

                                //        foreach (string col in test_cols)
                                //        {
                                //            col_name = element_param_name + Common.ColCennector + test_name + Common.ColCennector + col;
                                //            if (!testdt.Columns.Contains(col_name))
                                //            {
                                //                testdt.Columns.Add(col_name);
                                //            }
                                //        }
                                //    }

                                //}

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
//                                SQL += " ," + T9 + ".[サンプルコード] ";
//                                SQL += " ," + T9 + ".[ホルダコード] ";
//                                SQL += " ," + T9 + ".[試験項目コード] ";
//                                SQL += " ," + T9 + ".[単位コード] ";
//                                SQL += " ," + T9 + ".[ホルダ名] ";
                                SQL += " ," + T9 + ".[試験項目名] ";
                                SQL += " ," + T9 + ".[単位名] ";
 //                               SQL += " ," + T9 + ".[試験結果生データ] ";
 //                               SQL += " ," + T9 + ".[試験結果編集後] ";
                                SQL += " ," + T9 + ".[試験結果報告用] ";
 //                               SQL += " ," + T9 + ".[規格１上限] ";
 //                               SQL += " ," + T9 + ".[規格１下限] ";

                                SQL += " FROM ([実行処方要素] AS " + T4 + " ";
                                SQL += " LEFT JOIN [実行処方パラメータ] AS " + T5 + " ON (" + T4 + ".[製造指図番号] = " + T5 + ".[製造指図番号] AND " + T4 + ".[ステップID] = " + T5 + ".[ステップID]) ) ";
                                SQL += " LEFT JOIN [検査性状値] AS " + T9 + " ON (" + T9 + ".[Lab-Aid依頼番号] = " + T5 + ".[Lab-Aid依頼番号]) ";

                                SQL += " WHERE " + T4 + ".[製造指図番号] = '" + pno + "'";
                                if (dic.Equals("4"))
                                {
                                    SQL += " AND MID(" + T4 + ".[ステップID],1,1) = '3'";
                                }
                                SQL += " AND " + T9 + ".[試験項目名] in (" + string.Join(",", param_list) + ")";
                                SQL += " ORDER BY " + T4 + ".[製造指図番号], " + T4 + ".[ステップID], " + T5 + ".[パラメータID] ";

                                DataTable tmptestdt = new DataTable();

                                OleDbDataAdapter adapter3 = new OleDbDataAdapter(SQL, connection);
                                adapter3.Fill(tmptestdt);
                                adapter3.Dispose();

                                foreach (DataRow row2 in tmptestdt.Rows)
                                {
                                    string element_name = row2["要素名称"].ToString();
                                    string param_name = row2["パラメータ名称"].ToString().Replace(".", "-");
                                    string test_name = row2["試験項目名"].ToString();
                                    string col_name = string.Empty;

                                    foreach (string w_test_name in seach.qulity_list)
                                    {
                                        string w_col_name = string.Empty;

                                        foreach (string col in test_cols)
                                        {
                                            w_col_name = element_name + Common.ColCennector + param_name + Common.ColCennector + w_test_name + Common.ColCennector + col;
                                            if (!testdt.Columns.Contains(w_col_name))
                                            {
                                                testdt.Columns.Add(w_col_name);
                                            }
                                        }
                                    }
                                }

                                DataRow[] drs2 = testdt.Select("製造指図番号 = '" + pno + "'");

                                foreach (DataRow row2 in tmptestdt.Rows)
                                {
                                    string element_name = row2["要素名称"].ToString();
                                    string param_name = row2["パラメータ名称"].ToString().Replace(".", "-");
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

        private string SelectItem1(string dic)
        {
            string T1 = Common.table_dic["製造指図情報"];
            string T2 = Common.table_dic["実行処方製品"];
            string T3 = Common.table_dic["実行処方ヘッダ"];
            string T4 = Common.table_dic["実行処方要素"];
            string T5 = Common.table_dic["実行処方パラメータ"];

            string SQL = "";

            SQL += " " + T1 + ".[製造指図番号] ";
            if (string.IsNullOrEmpty(dic) || dic.Equals("0"))
            {
                SQL += " ," + T1 + ".[製造指図ステータス] ";
            }
            SQL += " ," + T1 + ".[品目コード] ";
            SQL += " ," + T1 + ".[品名] ";
            if (string.IsNullOrEmpty(dic) || dic.Equals("0"))
            {
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
            }

            //SQL += " ," + T2 + ".[製造指図番号] ";
            SQL += " ," + T2 + ".[連番] ";
            SQL += " ," + T2 + ".[処方ID] ";
            SQL += " ," + T2 + ".[処方バージョン] ";
            SQL += " ," + T2 + ".[処方製品連番] ";
            if (string.IsNullOrEmpty(dic) || dic.Equals("0"))
            {
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
                SQL += " ," + T3 + ".[備考] AS 処方ヘッダ備考";
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
            }

            return SQL;
        }

        private string SelectItem2(string dic)
        {
            string T1 = Common.table_dic["製造指図情報"];
            string T2 = Common.table_dic["実行処方製品"];
            string T3 = Common.table_dic["実行処方ヘッダ"];
            string T4 = Common.table_dic["実行処方要素"];
            string T5 = Common.table_dic["実行処方パラメータ"];

            string SQL = "";

            SQL += " " + T4 + ".[製造指図番号] ";
            SQL += " ," + T4 + ".[処方ID] ";
            SQL += " ," + T4 + ".[処方バージョン] ";
            SQL += " ," + T4 + ".[ステップID] ";
            SQL += " ," + T4 + ".[要素名称] ";
            if (string.IsNullOrEmpty(dic) || dic.Equals("0"))
            {
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
            }
            SQL += " ," + T4 + ".[開始実績日時] ";
            SQL += " ," + T4 + ".[終了実績日時] ";
            if (string.IsNullOrEmpty(dic) || dic.Equals("0"))
            {
                SQL += " ," + T4 + ".[工程指図メモ] ";
            }

            SQL += " ," + T5 + ".[パラメータID] ";
            SQL += " ," + T5 + ".[パラメータ名称] ";
            SQL += " ," + T5 + ".[設定値] ";
            if (string.IsNullOrEmpty(dic) || dic.Equals("0"))
            {
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
            }
            SQL += " ," + T5 + ".[品目コード] ";
            SQL += " ," + T5 + ".[品名] ";
            if (string.IsNullOrEmpty(dic) || dic.Equals("0"))
            {
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
            }
            SQL += " ," + T5 + ".[実績確定値] ";
            if (string.IsNullOrEmpty(dic) || dic.Equals("0"))
            {
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
            }
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
            if (string.IsNullOrEmpty(dic) || dic.Equals("0"))
            {
                SQL += " ," + T5 + ".[SAP簿外] ";
                SQL += " ," + T5 + ".[SAP転記日] ";
                SQL += " ," + T5 + ".[SAP連携済フラグ] ";
            }

            return SQL;
        }

        private string[] GetElementCol(string dic)
        {

            if (string.IsNullOrEmpty(dic) || dic.Equals("0"))
            {
                return new string[]{
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
                                    "工程指図メモ",
                                    "開始予定日時",
                                    "終了予定日時",
                                    };
            }
            else
            {
                return new string[]{
                                    "開始実績日時",
                                    "終了実績日時",
                                    };
            } 
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
                    string value = " '" + sc.value + "' ";
                    if (sc.condition.Equals("LIKE"))
                    {
                        value = " '%" + sc.value + "%' ";
                    }
                    where += " " + (string.IsNullOrEmpty(sc.combi) ? " AND " : sc.combi)
                        + " " + sc.item + " " + sc.condition + value;
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
        public bool GetTestitemNameList(
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

                        string queryString = "SELECT distinct([試験項目名]) FROM [検査性状値]";
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
        /// プラントデータ検索
        /// </summary>
        /// <param name="table_name">対象テーブル名</param>
        /// <param name="itemNameDT">データテーブル</param>
        /// <returns>true/false</returns>
        public bool SearchPlantData(
            SearchData seach,
            out DataTable resulrdt
            )
        {
            // DB接続文字列作成
            string connectionString = connectionStringBase + LocalDBName;
            bool ret = true;

            resulrdt = new DataTable();
            DataTable plantdt = new DataTable();

            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    try
                    {
                        resulrdt.Columns.Add("time");

                        List<string> param_list = new List<string>();
                        foreach (string param in seach.process_list)
                        {
                            resulrdt.Columns.Add(param);
                            param_list.Add("'" + param + "'");
                        }

                        connection.Open();

                        string SQL = "SELECT *";
                        SQL += " FROM PI ";                        
                        SQL += " WHERE tag in (" + string.Join(",", param_list) + ")";
                        if (seach.product_date_start.HasValue)
                        {
                            SQL += " AND time >= :start_time";
                        }
                        if (seach.product_date_end.HasValue)
                        {
                            SQL += " AND time < :end_time";
                        }
                        SQL += " ORDER BY time";

                        OleDbDataAdapter adapter = new OleDbDataAdapter(SQL, connection);
                        if (seach.product_date_start.HasValue)
                        {
                            adapter.SelectCommand.Parameters.Add(new OleDbParameter("start_time", seach.product_date_start.Value));
                        }
                        if (seach.product_date_end.HasValue)
                        {
                            adapter.SelectCommand.Parameters.Add(new OleDbParameter("end_time", seach.product_date_end.Value));
                        }

                        adapter.Fill(plantdt);
                        adapter.Dispose();

                        foreach (DataRow row in plantdt.Rows)
                        {
                            DataRow[] rows = resulrdt.Select("time = '" + row["time"].ToString() + "'");
                            if (rows.Length == 0)
                            {
                                DataRow newRow = resulrdt.NewRow();
                                newRow["time"] = row["time"];
                                newRow[row["tag"].ToString()] = row["value"].ToString();
                                resulrdt.Rows.Add(newRow);
                            }
                            else
                            {
                                rows[0][row["tag"].ToString()] = row["value"].ToString();
                            }
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

        /// <summary>
        /// データ検索
        /// </summary>
        /// <param name="table_name">対象テーブル名</param>
        /// <param name="itemNameDT">データテーブル</param>
        /// <returns>true/false</returns>
        public bool GetItem1(
            string dic,
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

                        string T1 = Common.table_dic["製造指図情報"];
                        string T2 = Common.table_dic["実行処方製品"];
                        string T3 = Common.table_dic["実行処方ヘッダ"];

                        string SQL = "";

                        SQL += " SELECT ";
                        SQL += SelectItem1(dic);

                        SQL += " FROM ([製造指図情報] AS " + T1 + " ";
                        SQL += " LEFT JOIN [実行処方製品] AS " + T2 + " ON (" + T2 + ".[製造指図番号] = " + T1 + ".[製造指図番号]) ) ";
                        SQL += " LEFT JOIN [実行処方ヘッダ] AS " + T3 + " ON (" + T3 + ".[製造指図番号] = " + T1 + ".[製造指図番号] ) ";

                        SQL += " WHERE 1=0 ";

                        OleDbDataAdapter adapter = new OleDbDataAdapter(SQL, connection);
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
        /// データ検索
        /// </summary>
        /// <param name="table_name">対象テーブル名</param>
        /// <param name="itemNameDT">データテーブル</param>
        /// <returns>true/false</returns>
        public bool GetItem2(
            string dic,
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

                        string T1 = Common.table_dic["製造指図情報"];
                        string T2 = Common.table_dic["実行処方製品"];
                        string T3 = Common.table_dic["実行処方ヘッダ"];
                        string T4 = Common.table_dic["実行処方要素"];
                        string T5 = Common.table_dic["実行処方パラメータ"];

                        string SQL = "";

                        SQL += " SELECT ";
                        SQL += SelectItem2(dic);

                        SQL += " FROM [実行処方要素] AS " + T4 + " ";
                        SQL += " LEFT JOIN [実行処方パラメータ] AS " + T5 + " ON " + T4 + ".[製造指図番号] = " + T5 + ".[製造指図番号] AND " + T4 + ".[ステップID] = " + T5 + ".[ステップID] ";

                        SQL += " WHERE 1=0 ";
                        SQL += " ORDER BY " + T4 + ".[製造指図番号], " + T4 + ".[ステップID], " + T5 + ".[パラメータID] ";

                        OleDbDataAdapter adapter = new OleDbDataAdapter(SQL, connection);
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
        /// データ検索
        /// </summary>
        /// <param name="table_name">対象テーブル名</param>
        /// <param name="itemNameDT">データテーブル</param>
        /// <returns>true/false</returns>
        public bool GetItemParameter(
            string dic,
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

                        string T1 = Common.table_dic["製造指図情報"];
                        string T2 = Common.table_dic["実行処方製品"];
                        string T3 = Common.table_dic["実行処方ヘッダ"];
                        string T4 = Common.table_dic["実行処方要素"];
                        string T5 = Common.table_dic["実行処方パラメータ"];

                        string SQL = string.Empty;

                        SQL += " SELECT ";

                        SQL += " " + T4 + ".[製造指図番号] ";
                        SQL += " ," + T4 + ".[処方ID] ";
                        SQL += " ," + T4 + ".[処方バージョン] ";
                        SQL += " ," + T4 + ".[ステップID] ";
                        SQL += " ," + T4 + ".[要素名称] ";
                        SQL += " ," + T5 + ".[パラメータID] ";
                        SQL += " ," + T5 + ".[パラメータ名称] ";

                        SQL += " FROM [実行処方要素] AS " + T4 + " ";
                        SQL += " LEFT JOIN [実行処方パラメータ] AS " + T5 + " ON " + T4 + ".[製造指図番号] = " + T5 + ".[製造指図番号] AND " + T4 + ".[ステップID] = " + T5 + ".[ステップID] ";

                        SQL += " WHERE 1=1 ";
                        if (dic.Equals("4"))
                        {
                            SQL += " AND MID(" + T4 + ".[ステップID],1,1) = '3'";
                        }

                        SQL += " ORDER BY " + T4 + ".[製造指図番号], " + T4 + ".[ステップID], " + T5 + ".[パラメータID] ";

                        DataTable paramdt = new DataTable();

                        OleDbDataAdapter adapter = new OleDbDataAdapter(SQL, connection);
                        adapter.Fill(paramdt);
                        adapter.Dispose();

                        string[] element_cols = GetElementCol(dic);

                        foreach (DataRow row2 in paramdt.Rows)
                        {
                            string element_name = row2["要素名称"].ToString();
                            string param_name = row2["パラメータ名称"].ToString().Replace(".", "-");
                            string col_name = string.Empty;

                            foreach (string col in element_cols)
                            {
                                col_name = element_name + Common.ColCennector + col;
                                if (!resulrdt.Columns.Contains(col_name))
                                {
                                    resulrdt.Columns.Add(col_name);
                                }
                            }

                            if (!string.IsNullOrEmpty(param_name))
                            {
                                col_name = element_name + Common.ColCennector + param_name;
                                if (!resulrdt.Columns.Contains(col_name))
                                {
                                    resulrdt.Columns.Add(col_name);
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
        /// データ検索
        /// </summary>
        /// <param name="table_name">対象テーブル名</param>
        /// <param name="itemNameDT">データテーブル</param>
        /// <returns>true/false</returns>
        public bool GetOutputItem(
            string dic,
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

                        string SQL = string.Empty;

                        SQL += " SELECT ";

                        SQL += " [項目名] ";
                        SQL += " ,[表示順] ";
                        SQL += " ,[ソート順] ";
                        SQL += " ,[ソート方向] ";
                        SQL += " ,[表示有無] ";

                        SQL += " FROM [出力項目] ";

                        SQL += " WHERE [データディクショナリ] = '"+dic+"' ";
                        
                        SQL += " ORDER BY [表示順] asc";

                        OleDbDataAdapter adapter = new OleDbDataAdapter(SQL, connection);
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
        /// データ検索
        /// </summary>
        /// <param name="table_name">対象テーブル名</param>
        /// <param name="itemNameDT">データテーブル</param>
        /// <returns>true/false</returns>
        public bool GetOutputItemDisplay(
            string dic,
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

                        string SQL = string.Empty;

                        SQL += " SELECT ";

                        SQL += " [項目名] ";
                        SQL += " ,[表示順] ";
                        SQL += " ,[ソート順] ";
                        SQL += " ,[ソート方向] ";
                        SQL += " ,[表示有無] ";

                        SQL += " FROM [出力項目] ";

                        SQL += " WHERE [データディクショナリ] = '" + dic + "' AND [表示有無] = '1'";

                        SQL += " ORDER BY [表示順] asc";

                        OleDbDataAdapter adapter = new OleDbDataAdapter(SQL, connection);
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
        /// データ検索
        /// </summary>
        /// <param name="table_name">対象テーブル名</param>
        /// <param name="itemNameDT">データテーブル</param>
        /// <returns>true/false</returns>
        public bool GetSort(
            string dic,
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

                        string SQL = string.Empty;

                        SQL += " SELECT ";

                        SQL += " [項目名] ";
                        SQL += " ,[表示順] ";
                        SQL += " ,[ソート順] ";
                        SQL += " ,[ソート方向] ";
                        SQL += " ,[表示有無] ";

                        SQL += " FROM [出力項目] ";

                        SQL += " WHERE [データディクショナリ] = '" + dic + "' AND [表示有無] = '1'";
                        SQL += " AND [ソート順] <> '' AND [ソート方向] <> '' ";

                        SQL += " ORDER BY [ソート順] asc";

                        OleDbDataAdapter adapter = new OleDbDataAdapter(SQL, connection);
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
        /// データ検索
        /// </summary>
        /// <param name="table_name">対象テーブル名</param>
        /// <param name="itemNameDT">データテーブル</param>
        /// <returns>true/false</returns>
        public bool SetOutputItem(
            string dic,
            List<OutputItem> list
            )
        {
            // DB接続文字列作成
            string connectionString = connectionStringBase + LocalDBName;
            bool ret = true;

            try
            {
                // データをDelete/Insert
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {

                    OleDbCommand command = new OleDbCommand();
                    OleDbTransaction transaction = null;
                    command.Connection = connection;
                    int cnt = 0;

                    try
                    {
                        connection.Open();

                        // トランザクション開始
                        transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);

                        // Assign transaction object for a pending local transaction.
                        command.Connection = connection;
                        command.Transaction = transaction;

                        // 取込定義削除
                        command.CommandText = "DELETE FROM [出力項目] WHERE [データディクショナリ] = '" + dic + "'";
                        command.ExecuteNonQuery();

                        foreach (OutputItem items in list)
                        {
                            try
                            {
                                string sort_dir = items.sort_dir_asc ? "ASC" : items.sort_dir_desc ? "DESC" : "";

                                List<string> values = new List<string>();
                                values.Add("'" + items.output_item + "'");
                                values.Add("'" + dic + "'");
                                values.Add("'" + items.line_no + "'");
                                values.Add("'" + items.sort + "'");
                                values.Add("'" + sort_dir + "'");
                                values.Add("'" + (items.display ? "1" : "") + "'");

                                command.CommandText =
                                    "INSERT INTO [出力項目] ([項目名],[データディクショナリ],[表示順],[ソート順],[ソート方向],[表示有無]) "+
                                            "VALUES ( " + string.Join(",", values) + " )";
                                cnt += command.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {
                                ret = false;
                                transaction.Rollback();
                            }
                        }

                        // コミット
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        ret = false;
                        try
                        {
                            transaction.Rollback();
                        }
                        catch
                        {
                        }
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
        /// データ検索
        /// </summary>
        /// <param name="table_name">対象テーブル名</param>
        /// <param name="itemNameDT">データテーブル</param>
        /// <returns>true/false</returns>
        public bool DeleteOutputItem(
            string dic
            )
        {
            // DB接続文字列作成
            string connectionString = connectionStringBase + LocalDBName;
            bool ret = true;

            try
            {
                // データをDelete/Insert
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {

                    OleDbCommand command = new OleDbCommand();
                    OleDbTransaction transaction = null;
                    command.Connection = connection;
                    int cnt = 0;

                    try
                    {
                        connection.Open();

                        // トランザクション開始
                        transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);

                        // Assign transaction object for a pending local transaction.
                        command.Connection = connection;
                        command.Transaction = transaction;

                        // 取込定義削除
                        command.CommandText = "DELETE FROM [出力項目] WHERE [データディクショナリ] = '" + dic + "'";
                        command.ExecuteNonQuery();

                        // コミット
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        ret = false;
                        try
                        {
                            transaction.Rollback();
                        }
                        catch
                        {
                        }
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
        /// データ検索
        /// </summary>
        /// <param name="table_name">対象テーブル名</param>
        /// <param name="itemNameDT">データテーブル</param>
        /// <returns>true/false</returns>
        public bool SearchInOutData(
            SearchData seach,
            string dic,
            out DataTable resulrdt
            )
        {
            // DB接続文字列作成
            string connectionString = connectionStringBase + LocalDBName;
            bool ret = true;

            resulrdt = new DataTable();

            bool addparam = false;
            bool addtest = false;

            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    try
                    {

                        connection.Open();

                        string T11 = Common.table_dic["在庫ユニークキー管理"];
                        string T12 = Common.table_dic["ロット情報"];
                        string T13 = Common.table_dic["個体の情報"];
                        string T14 = Common.table_dic["受払"];
                        string T15 = Common.table_dic["移庫指図"];
                        string T16 = Common.table_dic["受払ワーク"];
                        string T17 = Common.table_dic["荷揃指図"];
                        string T18 = Common.table_dic["品転指図"];
                        string T19 = Common.table_dic["在庫（日別）"];
                        string T20 = Common.table_dic["月末在庫"];
                        string T21 = Common.table_dic["タンク"];
                        string T22 = Common.table_dic["タンクレベル履歴"];

                        string SQL = "";
                        SQL += " SELECT ";
                        SQL += SelectInOutItem1(dic);

                        SQL += " FROM "+
                                "("+
                                    "(" +
                                        "[受払] AS " + T14 + " " +
                                        " LEFT JOIN [個体の情報] AS " + T13 + " ON "+
                                        "(" + 
                                            T13 + ".[在庫ユニークキー] = " + T14 + ".[在庫ユニークキー] " +
                                            " AND " + T13 + ".[個体識別] = " + T14 + ".[個体識別] " +
                                            " AND " + T13 + ".[ロケーションID] = " + T14 + ".[ロケーションID]"
                                        +") " +
                                    ") "+
                                    " LEFT JOIN [ロット情報] AS " + T12 + " ON "+
                                    "(" + 
                                        T12 + ".[在庫ユニークキー] = " + T14 + ".[在庫ユニークキー] " +
                                    ") " +
                                " )";
                        SQL += " LEFT JOIN [タンク] AS " + T21 + " ON " +
                                "(" +
                                    T21 + ".[品目コード] = " + T12 + ".[品目コード] " +
                                ") ";

                        SQL += " WHERE 1=1 " + CreateSearchCondiotion(seach, out addparam, out addtest);

                        OleDbDataAdapter adapter = new OleDbDataAdapter(SQL, connection);
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

        private string SelectInOutItem1(string dic)
        {
            string T11 = Common.table_dic["在庫ユニークキー管理"];
            string T12 = Common.table_dic["ロット情報"];
            string T13 = Common.table_dic["個体の情報"];
            string T14 = Common.table_dic["受払"];
            string T15 = Common.table_dic["移庫指図"];
            string T16 = Common.table_dic["受払ワーク"];
            string T17 = Common.table_dic["荷揃指図"];
            string T18 = Common.table_dic["品転指図"];
            string T19 = Common.table_dic["在庫（日別）"];
            string T20 = Common.table_dic["月末在庫"];
            string T21 = Common.table_dic["タンク"];
            string T22 = Common.table_dic["タンクレベル履歴"];

            string SQL = "";

            SQL += " " + T14 + ".[受払SEQ] ";
            SQL += " ," + T14 + ".[受払日時] ";
            SQL += " ," + T14 + ".[受払区分] ";
            SQL += " ," + T14 + ".[在庫ユニークキー] ";
            SQL += " ," + T14 + ".[個体識別] ";
            SQL += " ," + T14 + ".[移動タイプ] ";
            SQL += " ," + T14 + ".[個数] ";
            SQL += " ," + T14 + ".[在庫量（理論値）] ";
            SQL += " ," + T14 + ".[在庫量（実数）] ";
            SQL += " ," + T14 + ".[有姿数量] ";
            SQL += " ," + T14 + ".[純分数量] ";
            SQL += " ," + T14 + ".[ロケーションID] ";
            SQL += " ," + T14 + ".[受払発生元指図] ";
            SQL += " ," + T14 + ".[在庫変更理由] ";
            SQL += " ," + T14 + ".[受払実施者] ";
            SQL += " ," + T14 + ".[伝票日付] ";
            SQL += " ," + T14 + ".[転記日付] ";
            SQL += " ," + T14 + ".[ヘッダテキスト] ";
            SQL += " ," + T14 + ".[移動理由区分] ";
            SQL += " ," + T14 + ".[原価センタコード] ";

            //SQL += " ," + T12 + ".[在庫ユニークキー] ";
            SQL += " ," + T12 + ".[品目コード] ";
            SQL += " ," + T12 + ".[受入ロット] ";
            SQL += " ," + T12 + ".[メーカーロット] ";
            SQL += " ," + T12 + ".[メーカーコード] ";
            SQL += " ," + T12 + ".[仕入先コード] ";
            SQL += " ," + T12 + ".[購入元課] ";
            SQL += " ," + T12 + ".[入荷日] ";
            SQL += " ," + T12 + ".[メーカー製造日] ";
            SQL += " ," + T12 + ".[製造ロット] ";
            SQL += " ," + T12 + ".[出荷ロット] ";
            SQL += " ," + T12 + ".[向け先コード] ";
            SQL += " ," + T12 + ".[製造着手日] ";
            SQL += " ," + T12 + ".[製造完了日] ";
            SQL += " ," + T12 + ".[製造日] ";
            SQL += " ," + T12 + ".[出荷製造日] ";
            SQL += " ," + T12 + ".[有効期限] ";
            SQL += " ," + T12 + ".[製造保証日] ";
            SQL += " ," + T12 + ".[顧客保証期限] ";
            SQL += " ," + T12 + ".[生産課] ";
            SQL += " ," + T12 + ".[入庫日時] ";
            SQL += " ," + T12 + ".[自動引当対象外フラグ] ";
            SQL += " ," + T12 + ".[グレードコード] ";
            SQL += " ," + T12 + ".[荷姿コード] ";
            SQL += " ," + T12 + ".[サンプルフラグ] ";
            SQL += " ," + T12 + ".[SAPロット] ";
            SQL += " ," + T12 + ".[PCN品フラグ] ";
            SQL += " ," + T12 + ".[純分換算値] ";

            //SQL += " ," + T13 + ".[在庫ユニークキー] ";
            //SQL += " ," + T13 + ".[個体識別] ";
            //SQL += " ," + T13 + ".[ロケーションID] ";
            SQL += " ," + T13 + ".[個数] ";
            SQL += " ," + T13 + ".[在庫量（理論値）] ";
            SQL += " ," + T13 + ".[在庫量（実数）] ";
            SQL += " ," + T13 + ".[有姿数量] ";
            SQL += " ," + T13 + ".[純分数量] ";
            SQL += " ," + T13 + ".[引当ロット] ";
            SQL += " ," + T13 + ".[在庫ステータス] ";
            SQL += " ," + T13 + ".[半端品フラグ] ";
            SQL += " ," + T13 + ".[開封・未開封区分] ";
            SQL += " ," + T13 + ".[使い切りフラグ] ";
            SQL += " ," + T13 + ".[サンプル区分] ";
            SQL += " ," + T13 + ".[保管サンプル保管期限] ";
            SQL += " ," + T13 + ".[引当可能数量] ";
            SQL += " ," + T13 + ".[SAP簿外フラグ] ";
            SQL += " ," + T13 + ".[その他情報区分] ";
            SQL += " ," + T13 + ".[備考] ";

            SQL += " ," + T21 + ".[タンクNo] ";
            //SQL += " ," + T21 + ".[品目コード] ";
            SQL += " ," + T21 + ".[タンクロット] ";
            SQL += " ," + T21 + ".[タンクレベル] ";
            SQL += " ," + T21 + ".[タンク内容量] ";
            SQL += " ," + T21 + ".[タンク内重量] ";
            SQL += " ," + T21 + ".[レベル確認日時] ";

            return SQL;
        }

        /// <summary>
        /// データ検索
        /// </summary>
        /// <param name="table_name">対象テーブル名</param>
        /// <param name="itemNameDT">データテーブル</param>
        /// <returns>true/false</returns>
        public bool GetInOutItem1(
            string dic,
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

                        string T11 = Common.table_dic["在庫ユニークキー管理"];
                        string T12 = Common.table_dic["ロット情報"];
                        string T13 = Common.table_dic["個体の情報"];
                        string T14 = Common.table_dic["受払"];
                        string T15 = Common.table_dic["移庫指図"];
                        string T16 = Common.table_dic["受払ワーク"];
                        string T17 = Common.table_dic["荷揃指図"];
                        string T18 = Common.table_dic["品転指図"];
                        string T19 = Common.table_dic["在庫（日別）"];
                        string T20 = Common.table_dic["月末在庫"];
                        string T21 = Common.table_dic["タンク"];
                        string T22 = Common.table_dic["タンクレベル履歴"];

                        string SQL = "";
                        SQL += " SELECT ";
                        SQL += SelectInOutItem1(dic);


                        SQL += " FROM " +
                                "(" +
                                    "(" +
                                        "[受払] AS " + T14 + " " +
                                        " LEFT JOIN [個体の情報] AS " + T13 + " ON " +
                                        "(" +
                                            T13 + ".[在庫ユニークキー] = " + T14 + ".[在庫ユニークキー] " +
                                            " AND " + T13 + ".[個体識別] = " + T14 + ".[個体識別] " +
                                            " AND " + T13 + ".[ロケーションID] = " + T14 + ".[ロケーションID]"
                                        + ") " +
                                    ") " +
                                    " LEFT JOIN [ロット情報] AS " + T12 + " ON " +
                                    "(" +
                                        T12 + ".[在庫ユニークキー] = " + T14 + ".[在庫ユニークキー] " +
                                    ") " +
                                " )";
                        SQL += " LEFT JOIN [タンク] AS " + T21 + " ON " +
                                "(" +
                                    T21 + ".[品目コード] = " + T12 + ".[品目コード] " +
                                ") ";

                        SQL += " WHERE 1=0 ";

                        OleDbDataAdapter adapter = new OleDbDataAdapter(SQL, connection);
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
        /// データ検索
        /// </summary>
        /// <param name="table_name">対象テーブル名</param>
        /// <param name="itemNameDT">データテーブル</param>
        /// <returns>true/false</returns>
        public bool SearchDailyStockData(
            SearchData seach,
            string dic,
            out DataTable resulrdt
            )
        {
            // DB接続文字列作成
            string connectionString = connectionStringBase + LocalDBName;
            bool ret = true;

            resulrdt = new DataTable();

            bool addparam = false;
            bool addtest = false;

            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    try
                    {

                        connection.Open();

                        string T11 = Common.table_dic["在庫ユニークキー管理"];
                        string T12 = Common.table_dic["ロット情報"];
                        string T13 = Common.table_dic["個体の情報"];
                        string T14 = Common.table_dic["受払"];
                        string T15 = Common.table_dic["移庫指図"];
                        string T16 = Common.table_dic["受払ワーク"];
                        string T17 = Common.table_dic["荷揃指図"];
                        string T18 = Common.table_dic["品転指図"];
                        string T19 = Common.table_dic["在庫（日別）"];
                        string T20 = Common.table_dic["月末在庫"];
                        string T21 = Common.table_dic["タンク"];
                        string T22 = Common.table_dic["タンクレベル履歴"];

                        string SQL = "";
                        SQL += " SELECT ";
                        SQL += SelectDailyStockItem1(dic);

                        SQL += " FROM [在庫（日別）] AS " + T19 + " ";

                        SQL += " WHERE 1=1 " + CreateSearchCondiotion(seach, out addparam, out addtest);

                        OleDbDataAdapter adapter = new OleDbDataAdapter(SQL, connection);
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

        private string SelectDailyStockItem1(string dic)
        {
            string T11 = Common.table_dic["在庫ユニークキー管理"];
            string T12 = Common.table_dic["ロット情報"];
            string T13 = Common.table_dic["個体の情報"];
            string T14 = Common.table_dic["受払"];
            string T15 = Common.table_dic["移庫指図"];
            string T16 = Common.table_dic["受払ワーク"];
            string T17 = Common.table_dic["荷揃指図"];
            string T18 = Common.table_dic["品転指図"];
            string T19 = Common.table_dic["在庫（日別）"];
            string T20 = Common.table_dic["月末在庫"];
            string T21 = Common.table_dic["タンク"];
            string T22 = Common.table_dic["タンクレベル履歴"];

            string SQL = "";

            SQL += " " + T19 + ".[在庫日時] ";
            SQL += " ," + T19 + ".[品目コード] ";
            SQL += " ," + T19 + ".[在庫ステータス] ";
            SQL += " ," + T19 + ".[在庫量（理論値）] ";
            SQL += " ," + T19 + ".[在庫量（実数）] ";
            SQL += " ," + T19 + ".[有姿数量] ";
            SQL += " ," + T19 + ".[純分数量] ";


            return SQL;
        }

        /// <summary>
        /// データ検索
        /// </summary>
        /// <param name="table_name">対象テーブル名</param>
        /// <param name="itemNameDT">データテーブル</param>
        /// <returns>true/false</returns>
        public bool GetDailyStockItem1(
            string dic,
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

                        string T11 = Common.table_dic["在庫ユニークキー管理"];
                        string T12 = Common.table_dic["ロット情報"];
                        string T13 = Common.table_dic["個体の情報"];
                        string T14 = Common.table_dic["受払"];
                        string T15 = Common.table_dic["移庫指図"];
                        string T16 = Common.table_dic["受払ワーク"];
                        string T17 = Common.table_dic["荷揃指図"];
                        string T18 = Common.table_dic["品転指図"];
                        string T19 = Common.table_dic["在庫（日別）"];
                        string T20 = Common.table_dic["月末在庫"];
                        string T21 = Common.table_dic["タンク"];
                        string T22 = Common.table_dic["タンクレベル履歴"];

                        string SQL = "";
                        SQL += " SELECT ";
                        SQL += SelectDailyStockItem1(dic);

                        SQL += " FROM [在庫（日別）] AS " + T19 + " ";

                        SQL += " WHERE 1=0 ";

                        OleDbDataAdapter adapter = new OleDbDataAdapter(SQL, connection);
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
        /// データ検索
        /// </summary>
        /// <param name="table_name">対象テーブル名</param>
        /// <param name="itemNameDT">データテーブル</param>
        /// <returns>true/false</returns>
        public bool SearchMonthlyStockData(
            SearchData seach,
            string dic,
            out DataTable resulrdt
            )
        {
            // DB接続文字列作成
            string connectionString = connectionStringBase + LocalDBName;
            bool ret = true;

            resulrdt = new DataTable();

            bool addparam = false;
            bool addtest = false;

            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    try
                    {

                        connection.Open();

                        string T11 = Common.table_dic["在庫ユニークキー管理"];
                        string T12 = Common.table_dic["ロット情報"];
                        string T13 = Common.table_dic["個体の情報"];
                        string T14 = Common.table_dic["受払"];
                        string T15 = Common.table_dic["移庫指図"];
                        string T16 = Common.table_dic["受払ワーク"];
                        string T17 = Common.table_dic["荷揃指図"];
                        string T18 = Common.table_dic["品転指図"];
                        string T19 = Common.table_dic["在庫（日別）"];
                        string T20 = Common.table_dic["月末在庫"];
                        string T21 = Common.table_dic["タンク"];
                        string T22 = Common.table_dic["タンクレベル履歴"];

                        string SQL = "";
                        SQL += " SELECT ";
                        SQL += SelectMonthlyStockItem1(dic);

                        SQL += " FROM [月末在庫] AS " + T20 + " ";

                        SQL += " WHERE 1=1 " + CreateSearchCondiotion(seach, out addparam, out addtest);

                        OleDbDataAdapter adapter = new OleDbDataAdapter(SQL, connection);
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

        private string SelectMonthlyStockItem1(string dic)
        {
            string T11 = Common.table_dic["在庫ユニークキー管理"];
            string T12 = Common.table_dic["ロット情報"];
            string T13 = Common.table_dic["個体の情報"];
            string T14 = Common.table_dic["受払"];
            string T15 = Common.table_dic["移庫指図"];
            string T16 = Common.table_dic["受払ワーク"];
            string T17 = Common.table_dic["荷揃指図"];
            string T18 = Common.table_dic["品転指図"];
            string T19 = Common.table_dic["在庫（日別）"];
            string T20 = Common.table_dic["月末在庫"];
            string T21 = Common.table_dic["タンク"];
            string T22 = Common.table_dic["タンクレベル履歴"];

            string SQL = "";

            SQL += " " + T20 + ".[年月] ";
            SQL += " ," + T20 + ".[在庫ユニークキー] ";
            SQL += " ," + T20 + ".[個体識別] ";
            SQL += " ," + T20 + ".[ロケーションID] ";
            SQL += " ," + T20 + ".[個数] ";
            SQL += " ," + T20 + ".[在庫量（理論値）] ";
            SQL += " ," + T20 + ".[在庫量（実数）] ";

            return SQL;
        }

        /// <summary>
        /// データ検索
        /// </summary>
        /// <param name="table_name">対象テーブル名</param>
        /// <param name="itemNameDT">データテーブル</param>
        /// <returns>true/false</returns>
        public bool GetMonthlyStockItem1(
            string dic,
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

                        string T11 = Common.table_dic["在庫ユニークキー管理"];
                        string T12 = Common.table_dic["ロット情報"];
                        string T13 = Common.table_dic["個体の情報"];
                        string T14 = Common.table_dic["受払"];
                        string T15 = Common.table_dic["移庫指図"];
                        string T16 = Common.table_dic["受払ワーク"];
                        string T17 = Common.table_dic["荷揃指図"];
                        string T18 = Common.table_dic["品転指図"];
                        string T19 = Common.table_dic["在庫（日別）"];
                        string T20 = Common.table_dic["月末在庫"];
                        string T21 = Common.table_dic["タンク"];
                        string T22 = Common.table_dic["タンクレベル履歴"];

                        string SQL = "";
                        SQL += " SELECT ";
                        SQL += SelectMonthlyStockItem1(dic);

                        SQL += " FROM [月末在庫] AS " + T20 + " ";

                        SQL += " WHERE 1=0 ";

                        OleDbDataAdapter adapter = new OleDbDataAdapter(SQL, connection);
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
