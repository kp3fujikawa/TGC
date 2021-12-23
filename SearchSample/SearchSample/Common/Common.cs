using ControlzEx.Theming;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SearchSample
{
    static class Common
    {


        #region "定数"

        /// <summary>
        /// DB接続文字列：Provider
        /// </summary>
        static public string connectionStringBase = "Provider=Microsoft.ACE.OLEDB.15.0; Data Source=";

        /// <summary>
        /// マスタデータ（MDB）
        /// </summary>
        static public string LocalDBName = "Data\\DB.accdb";

        /// <summary>
        /// マスタデータ（MDB）
        /// </summary>
        static public string ComboBoxText = "text";
        static public string ComboBoxValue = "value";

        /// <summary>
        /// 検索時テーブル別名
        /// </summary>
        static public Dictionary<string, string> table_dic =
            new Dictionary<string, string>()
            {
                {"製造指図情報", "T1"},
                {"実行処方製品", "T2"},
                {"実行処方ヘッダ", "T3"},
                {"実行処方要素", "T4"},
                {"実行処方パラメータ", "T5"},
                {"実行処方リソース", "T6"},
                {"実行処方その他情報", "T7"},
                {"原材料引当", "T8"},
                {"検査性状値", "T9"},
                {"検査結果", "T10"},
                {"在庫ユニークキー管理", "T11"},
                {"ロット情報", "T12"},
                {"個体の情報", "T13"},
                {"受払", "T14"},
                {"移庫指図", "T15"},
                {"受払ワーク", "T16"},
                {"荷揃指図", "T17"},
                {"品転指図", "T18"},
                {"在庫（日別）", "T19"},
                {"月末在庫", "T20"},
                {"タンク", "T21"},
                {"タンクレベル履歴", "T22"},
            };

        /// <summary>
        /// カラム接続子
        /// </summary>
        static public string ColCennector = "　";

        /// <summary>
        /// 項目追加
        /// </summary>
        static public string item_param = "要素名.処方パラメータ名";
        static public string item_test = "試験項目名";

        #endregion

        /// <summary>
        /// WPF版：テーマ設定
        /// </summary>
        static public void SettingTheme(FrameworkElement frameworkElement)
        {
            // colorを設定ファイルから呼び出します
            var color = GetCurrentColor();
            SetThemeAndColor(frameworkElement, color);
        }

        // カラー設定を呼び出す
        static public Color GetCurrentColor()
        {
            try
            {
                Color color = (Color)ColorConverter.ConvertFromString(System.Configuration.ConfigurationManager.AppSettings["Metro_Themes"]);
                return color;
            }
            catch
            {
                return Colors.Blue;
            }
        }

        // テーマ・カラーの両方を設定する
        static public void SetThemeAndColor(FrameworkElement frameworkElement, Color color)
        {
            ThemeManager.Current.ThemeSyncMode = ThemeSyncMode.SyncWithHighContrast;
            ThemeManager.Current.SyncTheme();
            ThemeManager.Current.ChangeTheme(Application.Current, ThemeManager.Current.AddTheme(RuntimeThemeGenerator.Current.GenerateRuntimeTheme("Light", color)));

        }

        /// <summary>
        /// WPF版：DataGrid ColumnHeaderStyle設定
        /// </summary>
        static public void SettingColumnHeaderStyle(DataGrid dataGrid)
        {
            // MahApps.Metro テーマを求める
            var Metro_Themes = System.Configuration.ConfigurationManager.AppSettings["Metro_Themes"];
            if (Metro_Themes == null) Metro_Themes = "Blue";

            Brush colHeader = Brushes.LightBlue;
            if (Metro_Themes.Equals("Red"))
            {
                colHeader = Brushes.LightPink;
            }
            else if (Metro_Themes.Equals("Blue"))
            {
                colHeader = Brushes.LightBlue;
            }
            else if (Metro_Themes.Equals("Yellow"))
            {
                colHeader = Brushes.LightYellow;
            }
            else if (Metro_Themes.Equals("Green"))
            {
                colHeader = Brushes.LightGreen;
            }
            else if (Metro_Themes.Equals("Purple"))
            {
                colHeader = Brushes.Lavender;
            }
            else if (Metro_Themes.Equals("DarkGreen"))
            {
                colHeader = Brushes.DarkGreen;
            }

            // 背景色を設定
            Style style = dataGrid.ColumnHeaderStyle;
            style.Setters.Add(new Setter
            {
                Property = System.Windows.Controls.Control.BackgroundProperty,
                Value = colHeader
            });;
            style.Setters.Add(new Setter
            {
                Property = System.Windows.Controls.Control.ForegroundProperty,
                Value = Brushes.White
            });;
            dataGrid.ColumnHeaderStyle = style;
        }

        public static bool GetItemList(DBAccess db, string dic, out List<string> colmun_list)
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

            if (dic.Equals("2"))
            {
                // 入出庫実績・在庫
                DataTable output_item_dt1 = new DataTable();
                db.GetInOutItem1(dic, out output_item_dt1);

                foreach (DataColumn col in output_item_dt1.Columns)
                {
                    if (!except_list.ContainsKey(col.ColumnName))
                    {
                        except_list[col.ColumnName] = 0;
                        colmun_list.Add(col.ColumnName);
                    }
                }
            }
            else if (dic.Equals("14"))
            {
                // 日別在庫
                DataTable output_item_dt1 = new DataTable();
                db.GetDailyStockItem1(dic, out output_item_dt1);

                foreach (DataColumn col in output_item_dt1.Columns)
                {
                    if (!except_list.ContainsKey(col.ColumnName))
                    {
                        except_list[col.ColumnName] = 0;
                        colmun_list.Add(col.ColumnName);
                    }
                }
            }
            else if (dic.Equals("15"))
            {
                // 月末在庫
                DataTable output_item_dt1 = new DataTable();
                db.GetMonthlyStockItem1(dic, out output_item_dt1);

                foreach (DataColumn col in output_item_dt1.Columns)
                {
                    if (!except_list.ContainsKey(col.ColumnName))
                    {
                        except_list[col.ColumnName] = 0;
                        colmun_list.Add(col.ColumnName);
                    }
                }
            }
            else
            {
                DataTable output_item_dt1 = new DataTable();

                //db.GetItem1(dic, out output_item_dt1);

                //foreach (DataColumn col in output_item_dt1.Columns)
                //{
                //    if (!except_list.ContainsKey(col.ColumnName))
                //    {
                //        except_list[col.ColumnName] = 0;
                //        colmun_list.Add(col.ColumnName);
                //    }
                //}

                //DataTable output_item_dt2 = new DataTable();
                //db.GetItem2(dic, out output_item_dt2);

                //foreach (DataColumn col in output_item_dt2.Columns)
                //{
                //    if (!except_list.ContainsKey(col.ColumnName))
                //    {
                //        except_list[col.ColumnName] = 0;
                //        colmun_list.Add(col.ColumnName);
                //    }
                //}

                //DataTable output_item_dt3 = new DataTable();
                //db.GetItemParameter(dic, out output_item_dt3);

                //foreach (DataColumn col in output_item_dt3.Columns)
                //{
                //    //colmun_list.Add("[要素名称"+Common.ColCennector+"パラメータ名称].[" + col.ColumnName + "]");
                //    if (!except_list.ContainsKey(col.ColumnName))
                //    {
                //        except_list[col.ColumnName] = 0;
                //        colmun_list.Add(col.ColumnName);
                //    }
                //}

                db.SearchDataDictionaryItem(dic, out output_item_dt1);
                foreach (DataRow row in output_item_dt1.Rows)
                {
                    colmun_list.Add(row["項目名"].ToString());
                }


            }

            return true;
        }


        public static void SetComboBox(ComboBox cmb, DataTable dt)
        {
            // コンボボックスを作成
            cmb.ItemsSource = dt.DefaultView;
            cmb.DisplayMemberPath = Common.ComboBoxText;
            cmb.SelectedValuePath = Common.ComboBoxValue;
        }

        public static Dictionary<string, string> GetSearchCondition() {
            Dictionary<string, string> cond = new Dictionary<string, string>();

            cond.Add("", "");
            cond.Add("と等しい", "=");
            cond.Add("を含む", "LIKE");
            cond.Add("より大きい", ">");
            cond.Add("より小さい", "<");
            cond.Add("以上", ">=");
            cond.Add("以下", "<=");

            return cond;
        }

        public static string[] GetCombi()
        {
            return new string[] {
            "AND",
            "OR"
            };
        }

        public static Dictionary<string, string> GetDataDictionary()
        {
            Dictionary<string, string> data_dictionary =
                new Dictionary<string, string>()
                {
                    {"0", ""},
                    {"1", "バッチ実績＋出荷実績"},
                    {"2", "入出庫実績"},
                    {"3", "品質情報"},
                    {"4", "PT-395 反応"},
                    {"5", "工程金属マテバラ"},
                    {"6", "フィルターの溶剤通液量"},
                    {"7", "プロセスデータ①(原材料管理)"},
                    {"8", "工程時間予実"},
                    {"14", "日別在庫"},
                    {"15", "月末在庫"},
                };

            return data_dictionary;
        }

        public static void DoError(Exception ex)
        {
            System.Diagnostics.Debug.Print(ex.Message);
        }
    }


}
