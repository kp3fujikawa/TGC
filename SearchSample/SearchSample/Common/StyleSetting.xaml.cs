using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SearchSample
{
    /// <summary>
    /// StyleSetting.xaml の相互作用ロジック
    /// </summary>
    public partial class StyleSetting : MahApps.Metro.Controls.MetroWindow
    {
        /// <summary>
        /// スタイル設定
        /// </summary>
        public StyleSetting()
        {
            InitializeComponent();

            // WPF版：テーマ設定
            Common.SettingTheme(this);
        }

        /// <summary>
        /// Loadedイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // MahApps.Metro スタイル
            var Metro_BaseColor = ConfigurationManager.AppSettings["Metro_BaseColor"];

            if (Metro_BaseColor != null && Metro_BaseColor.Equals("Black"))
            {
                rbtnBGBlack.IsChecked = true;
            }
            else
            {
                rbtnBGWhite.IsChecked = true;
            }
            // MahApps.Metro テーマ
            var Metro_Themes = System.Configuration.ConfigurationManager.AppSettings["Metro_Themes"];
            if (Metro_Themes == null) Metro_Themes = "Blue";

            if (Metro_Themes.Equals("Red"))
            {
                rbtnThemeRed.IsChecked = true;
            }
            else if (Metro_Themes.Equals("Blue"))
            {
                rbtnThemeBlue.IsChecked = true;
            }
            else if (Metro_Themes.Equals("Yellow"))
            {
                rbtnThemeYellow.IsChecked = true;
            }
            else if (Metro_Themes.Equals("Green"))
            {
                rbtnThemeGreen.IsChecked = true;
            }
            else if (Metro_Themes.Equals("Purple"))
            {
                rbtnThemePurple.IsChecked = true;
            }
            else
            {
                rbtnThemeBlue.IsChecked = true;
            }
        }

        /// <summary>
        /// 設定　ボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            // MahApps.Metro スタイル
            var Metro_BaseColor = "White";

            if (rbtnBGBlack.IsChecked == true)
            {
                Metro_BaseColor = "Black";
            }
            // MahApps.Metro テーマ
            var Metro_Themes = "Blue";

            if (rbtnThemeRed.IsChecked == true)
            {
                Metro_Themes = "Red";
            }
            else if (rbtnThemeBlue.IsChecked == true)
            {
                Metro_Themes = "Blue";
            }
            else if (rbtnThemeYellow.IsChecked == true)
            {
                Metro_Themes = "Yellow";
            }
            else if (rbtnThemeGreen.IsChecked == true)
            {
                Metro_Themes = "Green";
            }
            else if (rbtnThemePurple.IsChecked == true)
            {
                Metro_Themes = "Purple";
            }

            // 設定値を保存
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["Metro_BaseColor"].Value = Metro_BaseColor;
            config.AppSettings.Settings["Metro_Themes"].Value = Metro_Themes;
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(config.AppSettings.SectionInformation.Name);

            this.DialogResult = true;
            this.Close();
        }

        /// <summary>
        /// 閉じる　ボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
