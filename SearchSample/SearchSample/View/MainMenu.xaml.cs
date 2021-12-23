using NLog;
using System;
using System.Windows;
using System.Windows.Controls;
using SearchSample.ViewModel;
using System.Data;
using SearchSample.Model;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace SearchSample.View
{
    /// <summary>
    /// MainMenu.xaml の相互作用ロジック
    /// </summary>
    public partial class MainMenu : MahApps.Metro.Controls.MetroWindow
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        #region "定数"

        #endregion

        #region "変数"

        #endregion

        /// <summary>
        /// メインメニュー画面
        /// </summary>
        public MainMenu()
        {
            InitializeComponent();

            DataContext = new MainMenuViewModel();

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
            try
            {
                logger.Info("初期表示");

                MainMenuViewModel vm = (MainMenuViewModel)DataContext;

                DataTable dt = new DataTable();
                dt.Columns.Add("タイトル");
                dt.Columns.Add("登録日時");
                dt.Columns.Add("登録者");

                this.infoGrid.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 閉じる　ボタンクリック
        /// </summary>
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MenuItem1_Click(object sender, RoutedEventArgs e)
        {
            MainWindow frm1 = new MainWindow();

            frm1.Show();
        }

        private void MenuItem2_Click(object sender, RoutedEventArgs e)
        {
            PlantSearch frm1 = new PlantSearch();

            frm1.Show();
        }

        private void btnInfoDetail_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItem3_Click(object sender, RoutedEventArgs e)
        {

            DataDictionary frm1 = new DataDictionary();

            frm1.Show();
        }

        private void MenuItem4_Click(object sender, RoutedEventArgs e)
        {

        }
    }

}
