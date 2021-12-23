using NLog;
using System;
using System.Windows;
using System.Windows.Controls;
using SearchSample.ViewModel;
using System.Data;
using SearchSample.Model;
using System.Collections.Generic;

namespace SearchSample.View
{
    /// <summary>
    /// DataDictionary.xaml の相互作用ロジック
    /// </summary>
    public partial class DataDictionary : MahApps.Metro.Controls.MetroWindow
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        #region "定数"

        #endregion

        #region "変数"

        private DataTable Table;

        private DataTable itemdt = new DataTable();

        #endregion

        /// <summary>
        /// メイン画面
        /// </summary>
        public DataDictionary()
        {
            InitializeComponent();

            // WPF版：テーマ設定
            Common.SettingTheme(this);

            // WPF版：DataGrid ColumnHeaderStyle設定
            Common.SettingColumnHeaderStyle(dataGrid2);

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

                DataDictionaryViewModel vm = (DataDictionaryViewModel)DataContext;

                // グリッド初期設定
                dataGrid2.IsReadOnly = true;              // 読取専用
                //dataGrid2.CanUserDeleteRows = false;      // 行削除禁止
                //dataGrid2.CanUserAddRows = false;         // 行挿入禁止
                dataGrid2.HeadersVisibility = DataGridHeadersVisibility.Column;   // 先頭列非表示
                //dataGrid2.AutoGenerateColumns = false;    // 列の自動追加禁止
                //dataGrid2.EnableHeadersVisualStyles = false;

                // コンボボックスリスト生成
                CreateComboBox();

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 項目名コンボボックスリスト生成
        /// </summary>
        /// <returns>true/false</returns>
        private void CreateComboBox()
        {
            DataDictionaryViewModel vm = (DataDictionaryViewModel)DataContext;
            CreateDataDictinary(vm);
            CreateSearch(vm);

        }

        private void CreateDataDictinary(DataDictionaryViewModel vm)
        {

        }
        private void CreateSearch(DataDictionaryViewModel vm)
        {
           
        }

        /// <summary>
        /// 閉じる　ボタンクリック
        /// </summary>
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// データディクショナリ情報検索　ボタンクリック
        /// </summary>
        private void btnPlant_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// データディクショナリ　変更
        /// </summary>
        private void cmbDataDictinary_SelectionChanged(object sender, EventArgs e)
        {
            allClear();
        }

        private void allClear()
        {
            DataDictionaryViewModel vm = (DataDictionaryViewModel)DataContext;
            CreateSearch(vm);

        }

        /// <summary>
        /// 検索　ボタンクリック
        /// </summary>
        private void btnSearch_Click(object sender, EventArgs e)
        {
            Search();
        }


        /// <summary>
        /// Refreshes the list of products. Called by navigation commands.
        /// </summary>
        private void Search()
        {

            // 検索条件生成
            DataDictionarySearchData s = new DataDictionarySearchData();

            DataDictionaryViewModel vm = (DataDictionaryViewModel)DataContext;
            vm.SearchData(s, out Table);

            //dataGrid2.Columns.Clear();
            dataGrid2.ItemsSource = Table.DefaultView;
            

        }


        /// 新規作成　リンククリック
        /// </summary>
        private void lnkAdd_Click(object sender, EventArgs e)
        {
            DataDictionaryItem frm1 = new DataDictionaryItem();

            frm1.ShowDialog();
        }

        /// 編集　リンククリック
        /// </summary>
        private void lnkEdit_Click(object sender, EventArgs e)
        {
            DataDictionaryItem frm1 = new DataDictionaryItem();
            frm1.DataDictionaryId = "4";
            frm1.ShowDialog();
        }

        /// 条件クリア　リンククリック
        /// </summary>
        private void lnkConditionClear_Click(object sender, EventArgs e)
        {
            allClear();
        }
    }

}

