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
            };

        /// <summary>
        /// カラム接続子
        /// </summary>
        static public string ColCennector = "_";

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
    }


}
