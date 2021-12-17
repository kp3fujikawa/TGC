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
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : MahApps.Metro.Controls.MetroWindow
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        #region "定数"

        #endregion

        #region "変数"

        private List<string> _quality_list = new List<string>();

        public ObservableCollection<SearchCondition> searchList { get; set; }
        public ObservableCollection<ComboItem> itemList { get; set; }

        private DataTable dataTable;
        public DataView DataTableView => new DataView(dataTable);

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

            DataContext = new MainWindowViewModel();

            // WPF版：テーマ設定
            Common.SettingTheme(this);

            // WPF版：DataGrid ColumnHeaderStyle設定
            Common.SettingColumnHeaderStyle(searchGrid);
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

                string dic = cmbDataDictinary.SelectedValue is null ? "" : cmbDataDictinary.SelectedValue.ToString();

                // グリッド初期設定
                //dataGrid2.IsReadOnly = true;              // 読取専用
                //dataGrid2.CanUserDeleteRows = false;      // 行削除禁止
                //dataGrid2.CanUserAddRows = false;         // 行挿入禁止
                //dataGrid2.HeadersVisibility = DataGridHeadersVisibility.Column;   // 先頭列非表示
                //dataGrid2.AutoGenerateColumns = false;    // 列の自動追加禁止
                //dataGrid2.EnableHeadersVisualStyles = false;

                // コンボボックスリスト生成
                CreateDataDictinary(vm);

                DataTable output_item_dt = new DataTable();
                vm.GetOutputItem(dic, out output_item_dt);

                dataTable = output_item_dt.Clone();

                dataGrid2.ItemsSource = DataTableView;

                // 検索条件生成
                CreateSearch();

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
        /// 検索条件生成
        /// </summary>
        /// <returns>true/false</returns>
        private void CreateSearch()
        {
            CreateGridColmun();
            ResetSearch();
        }

        private void CreateGridColmun()
        {
            MainWindowViewModel vm = (MainWindowViewModel)DataContext;

            // グリッド初期設定
            //searchGrid.IsReadOnly = true;              // 読取専用
            //dataGrid.CanUserDeleteRows = false;      // 行削除禁止
            //dataGrid.CanUserAddRows = false;         // 行挿入禁止
            //searchGrid.HeadersVisibility = DataGridHeadersVisibility.Column;   // 先頭列非表示
            searchGrid.AutoGenerateColumns = false;    // 列の自動追加禁止

            CreateSearchItemComboList();

            searchGrid.Columns.Add(CreateCombo("item", "項目", this.itemList));

            FrameworkElementFactory txtValue = new FrameworkElementFactory(typeof(TextBox));
            Binding bind_value = new Binding("value");
            bind_value.NotifyOnSourceUpdated = true;
            bind_value.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            txtValue.SetBinding(TextBox.TextProperty, bind_value);

            DataGridTemplateColumn txtValueColumn = new DataGridTemplateColumn()
            {
                Header = "値",
                CellTemplate = new DataTemplate()
                {
                    DataType = typeof(TextBox),
                    VisualTree = txtValue,
                },
                Width = 130,
            };

            searchGrid.Columns.Add(txtValueColumn);

            searchGrid.Columns.Add(CreateCombo("condition", "条件", vm.SearchCondition.DefaultView));
            searchGrid.Columns.Add(CreateCombo("combi", "結合子", vm.SearchCombi.DefaultView));

        }

        private void ResetSearch()
        {
            // データグリッドにデータを設定します。
            // データグリッドのItemSourceに設定するデータはObservableCollectionにする必要があります。
            this.searchList = new ObservableCollection<SearchCondition>();
            for (int i = 0; i < 3; ++i)
            {
                this.searchList.Add(new SearchCondition
                {
                    item = "",
                    value = "",
                    condition = "",
                    combi = "",
                });
            }

            searchGrid.ItemsSource = this.searchList;
        }

        private void CreateSearchItemComboList()
        {
            MainWindowViewModel vm = (MainWindowViewModel)DataContext;

            string dic = cmbDataDictinary.SelectedValue is null ? "" : cmbDataDictinary.SelectedValue.ToString();

            // 項目取得
            DataTable cmbitem_dt = new DataTable();
            vm.GetItemData(dic, out cmbitem_dt);

            this.itemList = new ObservableCollection<ComboItem>();

            foreach (DataRow row in cmbitem_dt.Rows)
            {
                string value = row[Common.ComboBoxValue].ToString();
                string text = row[Common.ComboBoxText].ToString();
                this.itemList.Add(new ComboItem()
                {
                    text = text,
                    value = value
                });
            }

            if (
                dic.Equals("0") ||	// 
                dic.Equals("1") ||	// バッチ実績＋出荷実績
                dic.Equals("3") ||	// 品質情報
                dic.Equals("4") ||	// PT-395 反応
                dic.Equals("5") ||	// 工程金属マテバラ
                dic.Equals("6") ||	// フィルターの溶剤通液量
                dic.Equals("7") ||	// プロセスデータ①(原材料管理)
                dic.Equals("8") 	// 工程時間予実

                )
            {
                // 要素・パラメータ名称追加
                this.itemList.Add(new ComboItem()
                {
                    text = Common.item_param,
                    value = Common.item_param
                });
            }

            if (
                dic.Equals("0") ||	// 
                dic.Equals("1") ||	// バッチ実績＋出荷実績
                dic.Equals("3")     // 品質情報
                )
            {
                // 試験項目追加
                this.itemList.Add(new ComboItem()
                {
                    text = Common.item_test,
                    value = Common.item_test
                });
            }

        }

        private DataGridTemplateColumn CreateCombo(string bind_name, string header_name, object src)
        {
            FrameworkElementFactory cmb = new FrameworkElementFactory(typeof(ComboBox));
            cmb.SetValue(ComboBox.DisplayMemberPathProperty, Common.ComboBoxText);
            cmb.SetValue(ComboBox.SelectedValuePathProperty, Common.ComboBoxValue);

            Binding bind_selected_value = new Binding(bind_name);
            bind_selected_value.NotifyOnSourceUpdated = true;
            bind_selected_value.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            cmb.SetValue(ComboBox.SelectedValueProperty, bind_selected_value);

            Binding bind_source = new Binding();
            bind_source.Source = src;
            bind_source.Mode = BindingMode.OneWay;
            bind_source.NotifyOnSourceUpdated = true;
            bind_source.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            cmb.SetValue(ComboBox.ItemsSourceProperty, bind_source);

            return new DataGridTemplateColumn()
            {
                Header = header_name,
                CellTemplate = new DataTemplate()
                {
                    DataType = typeof(ComboBox),
                    VisualTree = cmb,
                },
                Width = 130,
            };
        }

        private void ReCreateSearchItem()
        {
            CreateSearchItemComboList();
            searchGrid.Columns[0] = CreateCombo("item", "項目", this.itemList);
        }

        private void CreateDataDictinary(MainWindowViewModel vm)
        {
            SetComboBox(cmbDataDictinary, vm.DataDictionary);
        }

        private void SetComboBox(ComboBox cmb, DataTable dt)
        {
            // コンボボックスを作成
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
            ReCreateSearchItem();
            allClear();

            string dic = cmbDataDictinary.SelectedValue != null ? cmbDataDictinary.SelectedValue.ToString() : "";
            if (dic.Equals("1") || dic.Equals("4"))
            {
                // バッチ実績＋出荷実績
                QualityEnabled();
                ProcessDisabled();

            }
            else if (dic.Equals("2"))
            {
                // 入出庫実績・在庫
                QualityDisabled();
                ProcessDisabled();
            }
            else if (dic.Equals("3"))
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

            ResetGridData();
        }

        private void allClear()
        {
            ResetSearch();

            ProductDate1.SelectedDate = null;
            ProductDate2.SelectedDate = null;

            chkQualityData.IsChecked = true;
            txtQualityData.Text = string.Empty;

            chkProcessData.IsChecked = true;
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
            // データグリッドに行を追加します。
            this.searchList.Add(new SearchCondition
            {
                item = "",
                value = "",
                condition = "",
                combi = "",
            });
        }

        /// お気に入り　リンククリック
        /// </summary>
        private void lnkLike_Click(object sender, EventArgs e)
        {
            LikeDialog frm1 = new LikeDialog();

            frm1.ShowDialog();
        }

        /// お気に入り登録　リンククリック
        /// </summary>
        private void lnkLikeRegister_Click(object sender, EventArgs e)
        {
            LikeRegisterDialog frm1 = new LikeRegisterDialog();

            frm1.ShowDialog();
        }

        /// 条件クリア　リンククリック
        /// </summary>
        private void lnkConditionClear_Click(object sender, EventArgs e)
        {
            allClear();
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

            MainWindowViewModel vm = (MainWindowViewModel)DataContext;

            string dic = cmbDataDictinary.SelectedValue != null ? cmbDataDictinary.SelectedValue.ToString() : "";

            // 検索条件生成
            SearchData s = new SearchData();

            foreach (SearchCondition sc in this.searchList)
            {
                s.search_list.Add(sc);
            }

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

            DataTable tmpdt = new DataTable();
            vm.SearchData(s, dic, out tmpdt);

            ResetGridData();

            foreach (DataRow row in tmpdt.Rows)
            {
                DataRow newrow = dataTable.NewRow();
                foreach (DataColumn col in dataTable.Columns)
                {
                    try
                    {
                        newrow[col.ColumnName] = row[col.ColumnName];
                    }
                    catch (Exception ex)
                    {
                        Console.Write(ex);
                    }
                }
                dataTable.Rows.Add(newrow);
            }

            // ソート設定
            List<string> sort = new List<string>(); ;
            vm.GetSort(dic, out sort);

            DataTableView.Sort = string.Join(",",sort);

            dataGrid2.ItemsSource = DataTableView;

            ResultRecords.Text = dataTable.DefaultView.Count.ToString();
            ResultColumns.Text = dataTable.Columns.Count.ToString();

        }

        private void ResetGridData()
        {
            MainWindowViewModel vm = (MainWindowViewModel)DataContext;

            string dic = cmbDataDictinary.SelectedValue != null ? cmbDataDictinary.SelectedValue.ToString() : "";

            DataTable output_item_dt = new DataTable();
            vm.GetOutputItem(dic, out output_item_dt);

            dataTable = output_item_dt.Clone();

            dataGrid2.ItemsSource = DataTableView;

            ResultRecords.Text = dataTable.DefaultView.Count.ToString();
            ResultColumns.Text = dataTable.Columns.Count.ToString();

        }

        private void btnPlant_Click(object sender, RoutedEventArgs e)
        {
            PlantSearch frm1 = new PlantSearch();

            frm1.Show();
        }

    }

}
