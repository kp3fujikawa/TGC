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
    /// PlantSearch.xaml の相互作用ロジック
    /// </summary>
    public partial class PlantSearch : MahApps.Metro.Controls.MetroWindow
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
        public PlantSearch()
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

                PlantSearchViewModel vm = (PlantSearchViewModel)DataContext;

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
            PlantSearchViewModel vm = (PlantSearchViewModel)DataContext;
            CreateDataDictinary(vm);
            CreateSearch(vm);

        }

        private void CreateDataDictinary(PlantSearchViewModel vm)
        {

        }
        private void CreateSearch(PlantSearchViewModel vm)
        {
            DataTable cmbitem_dt = itemdt.Copy();
            cmbitem_dt.Columns.Add(Common.ComboBoxText);
            cmbitem_dt.Columns.Add(Common.ComboBoxValue);

            DataRow newrow1 = cmbitem_dt.NewRow();
            newrow1[Common.ComboBoxText] = "秒";
            newrow1[Common.ComboBoxValue] = "S";
            cmbitem_dt.Rows.Add(newrow1);

            DataRow newrow2 = cmbitem_dt.NewRow();
            newrow2[Common.ComboBoxText] = "分";
            newrow2[Common.ComboBoxValue] = "M";
            cmbitem_dt.Rows.Add(newrow2);

            DataRow newrow3 = cmbitem_dt.NewRow();
            newrow3[Common.ComboBoxText] = "時";
            newrow3[Common.ComboBoxValue] = "H";
            cmbitem_dt.Rows.Add(newrow3);

            SetComboBox(cmbItem1, cmbitem_dt);
            cmbItem1.SelectedIndex = 0;

        }
        private void SetComboBox(ComboBox cmb, DataTable dt)
        {
            // コンボボックスを作成
            //cmb.Items.Clear();
            cmb.ItemsSource = dt.DefaultView;
            cmb.DisplayMemberPath = Common.ComboBoxText;
            cmb.SelectedValuePath = Common.ComboBoxValue;
        }

        /// <summary>
        /// 閉じる　ボタンクリック
        /// </summary>
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// プラント情報検索　ボタンクリック
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
            PlantSearchViewModel vm = (PlantSearchViewModel)DataContext;
            CreateSearch(vm);

            ProductDate1.SelectedDate = null;
            ProductDate2.SelectedDate = null;
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
            SearchData s = new SearchData();
            s.product_date_start = ProductDate1.SelectedDate;
            s.product_date_end = ProductDate2.SelectedDate;
            s.process_list.Clear();
            s.process_list.AddRange(txtValue1.Text.Split(new char[] { ',' }));

            PlantSearchViewModel vm = (PlantSearchViewModel)DataContext;
            vm.SearchData(s, out Table);

            dataGrid2.Columns.Clear();
            dataGrid2.ItemsSource = Table.DefaultView;

            foreach (DataGridColumn col in dataGrid2.Columns)
            {
                col.Width = 130;
            }

        }

    }

}

