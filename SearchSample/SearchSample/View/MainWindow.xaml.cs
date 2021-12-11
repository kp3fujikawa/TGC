using NLog;
using System;
using System.Windows;
using System.Windows.Controls;
using SearchSample.ViewModel;
using System.Data;
using SearchSample.Model;
using System.Collections.Generic;
using System.Windows.Input;

namespace SearchSample.View
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : MahApps.Metro.Controls.MetroWindow
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        #region "定数"

        #endregion

        #region "変数"

        private DataTable Table;

        private DataTable itemdt = new DataTable();

        private List<string> _quality_list = new List<string>();

        #endregion

        public List<string> QualityList { 
            get 
            { 
                return _quality_list; 
            } 
            set 
            { 
                _quality_list = value;
                string quality_list = string.Join(",", _quality_list);
                txtQualityData.Text = quality_list;
            } 
        }

        /// <summary>
        /// メイン画面
        /// </summary>
        public MainWindow()
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

                MainWindowViewModel vm = (MainWindowViewModel)DataContext;

                // グリッド初期設定
                dataGrid2.IsReadOnly = true;              // 読取専用
                //dataGrid2.CanUserDeleteRows = false;      // 行削除禁止
                //dataGrid2.CanUserAddRows = false;         // 行挿入禁止
                dataGrid2.HeadersVisibility = DataGridHeadersVisibility.Column;   // 先頭列非表示
                //dataGrid2.AutoGenerateColumns = false;    // 列の自動追加禁止
                //dataGrid2.EnableHeadersVisualStyles = false;

                // 項目取得
                vm.GetItemData(out itemdt);

                // コンボボックスリスト生成
                CreateComboBox();

                Table = new DataTable();
                foreach (DataRow row in itemdt.Rows)
                {
                    Table.Columns.Add(new DataColumn(row[0].ToString()));
                }

                dataGrid2.ItemsSource = Table.DefaultView;

                //foreach (DataGridColumn col in dataGrid2.Columns)
                //{
                //    col.Width = 130;
                //}

                chkQualityData.IsChecked = true;
                chkProcessData.IsChecked = true;

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
            MainWindowViewModel vm = (MainWindowViewModel)DataContext;
            CreateDataDictinary(vm);
            CreateSearch(vm);

        }

        private void CreateDataDictinary(MainWindowViewModel vm)
        {
            SetComboBox(cmbDataDictinary, vm.DataDictionary);

        }
        private void CreateSearch(MainWindowViewModel vm)
        {
            SetComboBox(cmbCondition1, vm.SearchCondition);
            SetComboBox(cmbCondition2, vm.SearchCondition);
            SetComboBox(cmbCondition3, vm.SearchCondition);

            SetComboBox(cmbCombi1, vm.SearchCombi);
            SetComboBox(cmbCombi2, vm.SearchCombi);
            SetComboBox(cmbCombi3, vm.SearchCombi);

            DataTable cmbitem_dt = itemdt.Copy();

            DataRow newrow1 = cmbitem_dt.NewRow();
            newrow1[Common.ComboBoxText] = Common.item_param;
            newrow1[Common.ComboBoxValue] = Common.item_param;
            cmbitem_dt.Rows.Add(newrow1);

            DataRow newrow2 = cmbitem_dt.NewRow();
            newrow2[Common.ComboBoxText] = Common.item_test;
            newrow2[Common.ComboBoxValue] = Common.item_test;
            cmbitem_dt.Rows.Add(newrow2);

            SetComboBox(cmbItem1, cmbitem_dt);
            SetComboBox(cmbItem2, cmbitem_dt);
            SetComboBox(cmbItem3, cmbitem_dt);

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
        /// データディクショナリ　変更
        /// </summary>
        private void cmbDataDictinary_SelectionChanged(object sender, EventArgs e)
        {
            allClear();

            string value = cmbDataDictinary.SelectedValue.ToString();
            if (value.Equals("1") || value.Equals("4"))
            {
                // バッチ実績＋出荷実績
                QualityEnabled();
                ProcessDisabled();

            }
            else if (value.Equals("2"))
            {
                // 入出庫実績・在庫
                QualityDisabled();
                ProcessDisabled();
            }
            else if (value.Equals("3"))
            {
                // 品質情報
                QualityEnabled();
                ProcessDisabled();
            }
            else
            {
                QualityEnabled();
                ProcessEnabled();
            }
        }

        private void allClear()
        {
            MainWindowViewModel vm = (MainWindowViewModel)DataContext;
            CreateSearch(vm);

            ProductDate1.SelectedDate = null;
            ProductDate2.SelectedDate = null;

            chkQualityData.IsChecked = false;
            txtQualityData.Text = string.Empty;

            chkProcessData.IsChecked = false;
            txtProcessData.Text = string.Empty;
        }

        private void QualityDisabled()
        {
            chkQualityData.IsHitTestVisible = false;
            chkQualityData.IsTabStop = false;
            txtQualityData.IsEnabled = false;
            btnQualityData.IsEnabled = false;
        }

        private void QualityEnabled()
        {
            chkQualityData.IsHitTestVisible = true;
            chkQualityData.IsTabStop = true;
            txtQualityData.IsEnabled = true;
            btnQualityData.IsEnabled = true;
        }

        private void ProcessDisabled()
        {
            chkProcessData.IsHitTestVisible = false;
            chkProcessData.IsTabStop = false;
            txtProcessData.IsEnabled = false;
            btnProcessData.IsEnabled = false;
        }

        private void ProcessEnabled()
        {
            chkProcessData.IsHitTestVisible = true;
            chkProcessData.IsTabStop = true;
            txtProcessData.IsEnabled = true;
            btnProcessData.IsEnabled = true;
        }

        /// <summary>
        /// 検索　ボタンクリック
        /// </summary>
        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                Search();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        /// <summary>
        /// 品質データ抽出 選択　ボタンクリック
        /// </summary>
        private void btnQualityData_Click(object sender, EventArgs e)
        {
            QualityList = new List<string>(txtQualityData.Text.Split(','));

            QuolityDialog frm1 =new QuolityDialog(this);

            frm1.ShowDialog();
        }

        /// <summary>
        /// プロセスデータ抽出 選択　ボタンクリック
        /// </summary>
        private void btnProcessData_Click(object sender, EventArgs e)
        {
            //Search();
        }

        /// <summary>
        /// 出力項目選択　ボタンクリック
        /// </summary>
        private void btnOutputSelect_Click(object sender, EventArgs e)
        {
            OutputSettingDialog frm1 = new OutputSettingDialog(this);

            frm1.ShowDialog();
        }

        /// 追加　リンククリック
        /// </summary>
        private void lnkAdd_Click(object sender, EventArgs e)
        {
            //Search();
        }

        /// お気に入り　リンククリック
        /// </summary>
        private void lnkLike_Click(object sender, EventArgs e)
        {
            LikeDialog frm1 = new LikeDialog(this);

            frm1.ShowDialog();
        }

        /// お気に入り登録　リンククリック
        /// </summary>
        private void lnkLikeRegister_Click(object sender, EventArgs e)
        {
            LikeRegisterDialog frm1 = new LikeRegisterDialog(this);

            frm1.ShowDialog();
        }

        /// 条件クリア　リンククリック
        /// </summary>
        private void lnkConditionClear_Click(object sender, EventArgs e)
        {
            //Search();
        }

        /// ファイルに保存　リンククリック
        /// </summary>
        private void lnkCSVOutput_Click(object sender, EventArgs e)
        {
            //Search();
        }

        /// DPIファイルに保存　リンククリック
        /// </summary>
        private void lnkDPIOutput_Click(object sender, EventArgs e)
        {
            //Search();
        }

        /// <summary>
        /// Refreshes the list of products. Called by navigation commands.
        /// </summary>
        private void Search()
        {

            // 検索条件生成
            SearchData s = new SearchData();
            s.search_list.Add(new SearchCondition
            {
                item = cmbItem1.SelectedValue != null ? cmbItem1.SelectedValue.ToString() : "",
                value = txtValue1.Text,
                condition = cmbCondition1.SelectedValue != null ? cmbCondition1.SelectedValue.ToString() : "",
                combi = cmbCombi1.SelectedValue != null ? cmbCombi1.SelectedValue.ToString() : "",

            });
            s.search_list.Add(new SearchCondition
            {
                item = cmbItem2.SelectedValue != null ? cmbItem2.SelectedValue.ToString() : "",
                value = txtValue2.Text,
                condition = cmbCondition2.SelectedValue != null ? cmbCondition2.SelectedValue.ToString() : "",
                combi = cmbCombi2.SelectedValue != null ? cmbCombi2.SelectedValue.ToString() : "",

            });
            s.search_list.Add(new SearchCondition
            {
                item = cmbItem3.SelectedValue != null ? cmbItem3.SelectedValue.ToString() : "",
                value = txtValue3.Text,
                condition = cmbCondition3.SelectedValue != null ? cmbCondition3.SelectedValue.ToString() : "",
                combi = cmbCombi3.SelectedValue != null ? cmbCombi3.SelectedValue.ToString() : "",

            });

            s.product_date_start = ProductDate1.SelectedDate;
            s.product_date_end = ProductDate2.SelectedDate;

            s.qulity_check = (bool)chkQualityData.IsChecked;

            if (
                chkQualityData.IsChecked == true && 
                !string.IsNullOrEmpty(txtQualityData.Text)
                )
            {
                s.qulity_list = new List<string>(txtQualityData.Text.Split(','));
            }

            s.process_check = (bool)chkProcessData.IsChecked;

            if (
                chkProcessData.IsChecked == true &&
                !string.IsNullOrEmpty(txtProcessData.Text)
                )
            {
                s.process_list = new List<string>(txtProcessData.Text.Split(','));
            }

            MainWindowViewModel vm = (MainWindowViewModel)DataContext;
            vm.SearchData(s, cmbDataDictinary.SelectedValue is null ? "" : cmbDataDictinary.SelectedValue.ToString(), out Table);

            dataGrid2.Columns.Clear();
            dataGrid2.ItemsSource = Table.DefaultView;

            ResultRecords.Text = Table.DefaultView.Count.ToString();
            ResultColumns.Text = Table.Columns.Count.ToString();

            //foreach (DataGridColumn col in dataGrid2.Columns)
            //{
            //    col.Width = 130;
            //}

        }

        private void btnPlant_Click(object sender, RoutedEventArgs e)
        {
            PlantSearch frm1 = new PlantSearch();

            frm1.Show();
        }
    }

}
