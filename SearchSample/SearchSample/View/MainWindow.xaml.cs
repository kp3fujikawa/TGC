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

        private DataTable Table;

        private DataTable itemdt = new DataTable();

        private DataTable cmbitem_dt = new DataTable();

        private List<string> _quality_list = new List<string>();

        //private ObservableCollection<SearchGrid> dataList = new ObservableCollection<SearchGrid>();

        //public ObservableCollection<SearchCondition> dataList { get; private set; }

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
                CreateDataDictinary(vm);

                Table = new DataTable();
                foreach (DataRow row in itemdt.Rows)
                {
                    Table.Columns.Add(new DataColumn(row[0].ToString()));
                }

                dataGrid2.ItemsSource = Table.DefaultView;

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

        private void ResetSearch()
        {
            // データグリッドにデータを設定します。
            // データグリッドのItemSourceに設定するデータはObservableCollectionにする必要があります。
            ObservableCollection<SearchCondition> dataList = new ObservableCollection<SearchCondition>();
            for (int i = 0; i < 3; ++i)
            {
                dataList.Add(new SearchCondition
                {
                    item = "",
                    value = "",
                    condition = "",
                    combi = "",
                });
            }
            searchGrid.ItemsSource = dataList;
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

            this.cmbitem_dt = itemdt.Copy();

            DataRow newrow1 = this.cmbitem_dt.NewRow();
            newrow1[Common.ComboBoxText] = Common.item_param;
            newrow1[Common.ComboBoxValue] = Common.item_param;
            this.cmbitem_dt.Rows.Add(newrow1);

            DataRow newrow2 = this.cmbitem_dt.NewRow();
            newrow2[Common.ComboBoxText] = Common.item_test;
            newrow2[Common.ComboBoxValue] = Common.item_test;
            this.cmbitem_dt.Rows.Add(newrow2);

            FrameworkElementFactory cmbItem = new FrameworkElementFactory(typeof(ComboBox));
            cmbItem.SetValue(ComboBox.DisplayMemberPathProperty, Common.ComboBoxText);
            cmbItem.SetValue(ComboBox.SelectedValuePathProperty, Common.ComboBoxValue);
            cmbItem.SetValue(ComboBox.ItemsSourceProperty, this.cmbitem_dt.DefaultView);
            cmbItem.SetValue(ComboBox.NameProperty, "cmbItem");
            cmbItem.SetBinding(ComboBox.TextProperty, new Binding("item"));

            DataGridTemplateColumn cmbItemColumn = new DataGridTemplateColumn()
            {
                Header = "項目",
                CellTemplate = new DataTemplate()
                {
                    DataType = typeof(ComboBox),
                    VisualTree = cmbItem,
                },
                Width = 130,
            };


            FrameworkElementFactory txtValue = new FrameworkElementFactory(typeof(TextBox));
            txtValue.SetBinding(TextBox.TextProperty, new Binding("value"));
            txtValue.SetValue(TextBox.NameProperty, "txtValue");

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

            FrameworkElementFactory cmbCondition = new FrameworkElementFactory(typeof(ComboBox));
            cmbCondition.SetValue(ComboBox.DisplayMemberPathProperty, Common.ComboBoxText);
            cmbCondition.SetValue(ComboBox.SelectedValuePathProperty, Common.ComboBoxValue);
            cmbCondition.SetValue(ComboBox.ItemsSourceProperty, vm.SearchCondition.DefaultView);
            cmbCondition.SetValue(ComboBox.NameProperty, "cmbCondition");
            cmbCondition.SetBinding(ComboBox.TextProperty, new Binding("condition"));

            DataGridTemplateColumn cmbConditionColumn = new DataGridTemplateColumn()
            {
                Header = "条件",
                CellTemplate = new DataTemplate()
                {
                    DataType = typeof(ComboBox),
                    VisualTree = cmbCondition,
                },
                Width = 130,
            };

            FrameworkElementFactory cmbCombi = new FrameworkElementFactory(typeof(ComboBox));
            cmbCombi.SetValue(ComboBox.DisplayMemberPathProperty, Common.ComboBoxText);
            cmbCombi.SetValue(ComboBox.SelectedValuePathProperty, Common.ComboBoxValue);
            cmbCombi.SetValue(ComboBox.ItemsSourceProperty, vm.SearchCombi.DefaultView);
            cmbCombi.SetValue(ComboBox.NameProperty, "cmbCombi");
            cmbCombi.SetBinding(ComboBox.TextProperty, new Binding("Combi"));

            DataGridTemplateColumn cmbCombiColumn = new DataGridTemplateColumn()
            {
                Header = "結合子",
                CellTemplate = new DataTemplate()
                {
                    DataType = typeof(ComboBox),
                    VisualTree = cmbCombi,
                },
                Width = 130,
            };

            searchGrid.Columns.Add(cmbItemColumn);
            searchGrid.Columns.Add(txtValueColumn);
            searchGrid.Columns.Add(cmbConditionColumn);
            searchGrid.Columns.Add(cmbCombiColumn);

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
            allClear();

            string value = cmbDataDictinary.SelectedValue != null ? cmbDataDictinary.SelectedValue.ToString() : "";
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
            ObservableCollection<SearchCondition> dataList = searchGrid.ItemsSource as ObservableCollection<SearchCondition>;
            dataList.Add(new SearchCondition
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

            // 検索条件生成
            SearchData s = new SearchData();

            // データグリッドの行数を取得します。
            var rowCount = searchGrid.Items.Count;
            // データグリッドの行数分繰り返します。
            for (int i = 0; i < rowCount; ++i)
            {
                // データグリッドの行オブジェクトを取得します。
                var row = searchGrid.ItemContainerGenerator.ContainerFromIndex(i) as DataGridRow;
                // 行オブジェクトが取得できない場合
                if (row == null)
                {
                    // 対象の行が表示されていない場合、行オブジェクトが取得できないため
                    // 対象の行が表示されるようスクロールします。
                    searchGrid.UpdateLayout();
                    searchGrid.ScrollIntoView(searchGrid.Items[i]);
                    // 再度、行オブジェクトを取得します。
                    row = searchGrid.ItemContainerGenerator.ContainerFromIndex(i) as DataGridRow;
                }

                ComboBox cell1 = GetChildElement<ComboBox>((ContentPresenter)searchGrid.Columns[0].GetCellContent(row), 0);
                TextBox cell2  = GetChildElement<TextBox>((ContentPresenter)searchGrid.Columns[1].GetCellContent(row), 0);
                ComboBox cell3 = GetChildElement<ComboBox>((ContentPresenter)searchGrid.Columns[2].GetCellContent(row), 0);
                ComboBox cell4 = GetChildElement<ComboBox>((ContentPresenter)searchGrid.Columns[3].GetCellContent(row), 0);

                s.search_list.Add(new SearchCondition
                {
                    item = cell1.SelectedValue != null ? cell1.SelectedValue.ToString() : "",
                    value = cell2.Text,
                    condition = cell3.SelectedValue != null ? cell3.SelectedValue.ToString() : "",
                    combi = cell4.SelectedValue != null ? cell4.SelectedValue.ToString() : "",

                });

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

            MainWindowViewModel vm = (MainWindowViewModel)DataContext;
            vm.SearchData(s, cmbDataDictinary.SelectedValue is null ? "" : cmbDataDictinary.SelectedValue.ToString(), out Table);

            dataGrid2.Columns.Clear();
            dataGrid2.ItemsSource = Table.DefaultView;

            ResultRecords.Text = Table.DefaultView.Count.ToString();
            ResultColumns.Text = Table.Columns.Count.ToString();

        }

        private void btnPlant_Click(object sender, RoutedEventArgs e)
        {
            PlantSearch frm1 = new PlantSearch();

            frm1.Show();
        }

        private T GetChildElement<T>(DependencyObject reference, int childIdx) where T : FrameworkElement
        {
            //==== 子要素取得 ====//
            var child = VisualTreeHelper.GetChild(reference, childIdx);
            if (child == null)
            {
                return null;
            }
            if (child is T)
            {
                return child as T;
            }


            //==== 子要素内を探す ====//
            DependencyObject elem = reference;
            int count = VisualTreeHelper.GetChildrenCount(child);
            for (int idx = 0; idx < count; idx++)
            {
                elem = GetChildElement<T>(child, idx);
                if (elem != null)
                {
                    break;
                }
            }


            return elem as T;
        }
    }

}
