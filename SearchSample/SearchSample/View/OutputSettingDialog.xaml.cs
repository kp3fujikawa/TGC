using SearchSample.Model;
using SearchSample.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
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

namespace SearchSample.View
{
    /// <summary>
    /// MasterError.xaml の相互作用ロジック
    /// </summary>
    public partial class OutputSettingDialog : MahApps.Metro.Controls.MetroWindow
    {
        private MainWindow main;
        private string dic;
        public ObservableCollection<OutputItem> dataList { get; private set; }

        public DataTable SortDt { get; private set; }

        public DataTable SortDtDir { get; private set; }

        /// <summary>
        /// エラーダイアログ
        /// </summary>
        public OutputSettingDialog(MainWindow main)
        {
            InitializeComponent();

            // WPF版：テーマ設定
            Common.SettingTheme(this);

            // WPF版：DataGrid ColumnHeaderStyle設定
            Common.SettingColumnHeaderStyle(dataGrid);

            this.main = main;

            this.dic = this.main.cmbDataDictinary.SelectedValue is null ? "" : this.main.cmbDataDictinary.SelectedValue.ToString();
        }

        /// <summary>
        /// Loadedイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            OutputSettingDialogViewModel vm = (OutputSettingDialogViewModel)DataContext;

            List<string> colmun_list = new List<string>();
            vm.GetItemList(this.dic, out colmun_list);

            DataTable output_item_dt = new DataTable();
            vm.GetOutputItem(this.dic, out output_item_dt);

            SortDt = new DataTable();
            SortDt.Columns.Add(Common.ComboBoxText);
            SortDt.Columns.Add(Common.ComboBoxValue);

            for (int i = 0; i < colmun_list.Count; i++)
            {
                DataRow newrow = SortDt.NewRow();
                newrow[Common.ComboBoxText] = (i + 1).ToString();
                newrow[Common.ComboBoxValue] = (i + 1).ToString();
                SortDt.Rows.Add(newrow);
            }

            SortDtDir = new DataTable();
            SortDtDir.Columns.Add(Common.ComboBoxText);
            SortDtDir.Columns.Add(Common.ComboBoxValue);

           string[] sort_dir_list = { "昇順", "降順" };

            foreach (string sort_dir in sort_dir_list)
            {
                DataRow newrow = SortDtDir.NewRow();
                newrow[Common.ComboBoxText] = sort_dir;
                newrow[Common.ComboBoxValue] = sort_dir;
                SortDtDir.Rows.Add(newrow);
            }


            // グリッド初期設定
            //dataGrid.IsReadOnly = true;              // 読取専用
            //dataGrid.CanUserDeleteRows = false;      // 行削除禁止
            //dataGrid.CanUserAddRows = false;         // 行挿入禁止
            //dataGrid.HeadersVisibility = DataGridHeadersVisibility.Column;   // 先頭列非表示
            dataGrid.AutoGenerateColumns = false;    // 列の自動追加禁止

            FrameworkElementFactory up = new FrameworkElementFactory(typeof(Button));
            up.SetBinding(TextBlock.TextProperty, new Binding("up"));
            up.SetValue(Button.ContentProperty, "∧");

            DataGridTemplateColumn hlup = new DataGridTemplateColumn()
            {
                Header = "",
                CellTemplate = new DataTemplate()
                {
                    DataType = typeof(Button),
                    VisualTree = up,
                },
                //Width = 50,
            };

            FrameworkElementFactory down = new FrameworkElementFactory(typeof(Button));
            down.SetBinding(TextBlock.TextProperty, new Binding("down"));
            down.SetValue(Button.ContentProperty, "∨");

            DataGridTemplateColumn hldown = new DataGridTemplateColumn()
            {
                Header = "",
                CellTemplate = new DataTemplate()
                {
                    DataType = typeof(Button),
                    VisualTree = down,
                },
                //Width = 50,
            };

            Style display_style = new Style();
            display_style.Setters.Add(new Setter
            {
                Property = CheckBox.HorizontalAlignmentProperty,
                Value = HorizontalAlignment.Center
            }); ;

            FrameworkElementFactory chkDisplay = new FrameworkElementFactory(typeof(CheckBox));
            chkDisplay.SetBinding(ComboBox.TextProperty, new Binding("display"));

            DataGridTemplateColumn chkDisplayColumn = new DataGridTemplateColumn()
            {
                Header = "表示有無",
                CellTemplate = new DataTemplate()
                {
                    DataType = typeof(CheckBox),
                    VisualTree = chkDisplay,
                },
                //Width = 130,
            };

            //DataGridCheckBoxColumn chkDisplayColumn = new DataGridCheckBoxColumn()
            //{
            //    Header = "表示有無",
            //    Binding = new Binding("display"),
            //    CellStyle = display_style,


            //    //Width = 50,
            //};

            FrameworkElementFactory txtItemName = new FrameworkElementFactory(typeof(TextBox));
            txtItemName.SetBinding(TextBox.TextProperty, new Binding("output_item"));
            txtItemName.SetValue(TextBox.IsReadOnlyProperty, true);

            DataGridTemplateColumn txtItemNameColumn = new DataGridTemplateColumn()
            {
                Header = "項目名",
                CellTemplate = new DataTemplate()
                {
                    DataType = typeof(TextBox),
                    VisualTree = txtItemName,
                },
                //Width = 130,
            };

            //DataGridTextColumn txtItemNameColumn = new DataGridTextColumn()
            //{
            //    Header = "項目名",
            //    Binding = new Binding("output_item"),
            //    //Width = 130,
            //};



            FrameworkElementFactory cmbSort = new FrameworkElementFactory(typeof(ComboBox));
            cmbSort.SetValue(ComboBox.DisplayMemberPathProperty, Common.ComboBoxText);
            cmbSort.SetValue(ComboBox.SelectedValuePathProperty, Common.ComboBoxValue);
            cmbSort.SetValue(ComboBox.ItemsSourceProperty, SortDt.DefaultView);
            cmbSort.SetValue(ComboBox.NameProperty, "cmbCondition");
            cmbSort.SetBinding(ComboBox.TextProperty, new Binding("sort"));

            DataGridTemplateColumn cmbSortColumn = new DataGridTemplateColumn()
            {
                Header = "ソート順序",
                CellTemplate = new DataTemplate()
                {
                    DataType = typeof(ComboBox),
                    VisualTree = cmbSort,
                },
                //Width = 130,
            };

            FrameworkElementFactory cmbSortDir = new FrameworkElementFactory(typeof(ComboBox));
            cmbSortDir.SetValue(ComboBox.DisplayMemberPathProperty, Common.ComboBoxText);
            cmbSortDir.SetValue(ComboBox.SelectedValuePathProperty, Common.ComboBoxValue);
            cmbSortDir.SetValue(ComboBox.ItemsSourceProperty, SortDtDir.DefaultView);
            cmbSortDir.SetBinding(ComboBox.TextProperty, new Binding("sort_dir"));

            DataGridTemplateColumn cmbSortDirColumn = new DataGridTemplateColumn()
            {
                Header = "ソート方向",
                CellTemplate = new DataTemplate()
                {
                    DataType = typeof(ComboBox),
                    VisualTree = cmbSortDir,
                },
                //Width = 130,
            };

            //FrameworkElementFactory rdoSortDir1 = new FrameworkElementFactory(typeof(RadioButton));
            //rdoSortDir1.SetValue(RadioButton.NameProperty, "asc");
            //rdoSortDir1.SetValue(RadioButton.ContentProperty, "昇順");
            ////rdoSortDir1.SetBinding(TextBlock.TextProperty, new Binding("sort_dir"));
            //rdoSortDir1.SetValue(RadioButton.IsCheckedProperty, "{Binding Path=ModeArray[0], Mode=TwoWay}");

            //FrameworkElementFactory rdoSortDir2 = new FrameworkElementFactory(typeof(RadioButton));
            //rdoSortDir2.SetValue(RadioButton.NameProperty, "desc");
            //rdoSortDir2.SetValue(RadioButton.ContentProperty, "降順");
            ////rdoSortDir2.SetBinding(TextBlock.TextProperty, new Binding("sort_dir"));

            //FrameworkElementFactory stkSortDir = new FrameworkElementFactory(typeof(StackPanel));
            //stkSortDir.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);
            //stkSortDir.AppendChild(rdoSortDir1);
            //stkSortDir.AppendChild(rdoSortDir2);

            //DataGridTemplateColumn rdoSortDirColumn = new DataGridTemplateColumn()
            //{
            //    Header = "ソート方向",
            //    CellTemplate = new DataTemplate()
            //    {
            //        DataType = typeof(StackPanel),
            //        VisualTree = stkSortDir,
            //    },
            //    //Width = 130,
            //};

            //FrameworkElementFactory txtSortDir = new FrameworkElementFactory(typeof(TextBox));
            //txtSortDir.SetBinding(TextBlock.TextProperty, new Binding("sort_dir"));

            //DataGridTemplateColumn txtSortDirColumn = new DataGridTemplateColumn()
            //{
            //    Header = "",
            //    CellTemplate = new DataTemplate()
            //    {
            //        DataType = typeof(TextBox),
            //        VisualTree = txtSortDir,
            //    },
            //    Width = 0,
            //    //Visibility = Visibility.Hidden,
            //    //Width = 130,
            //};

            DataGridTextColumn txtSortDirColumn = new DataGridTextColumn()
            {
                Header = "",
                Binding = new Binding("sort_dir"),
                Width = 0,
            };

            dataGrid.Columns.Add(hlup);
            dataGrid.Columns.Add(hldown);
            dataGrid.Columns.Add(chkDisplayColumn);
            dataGrid.Columns.Add(txtItemNameColumn);
            dataGrid.Columns.Add(cmbSortColumn);
            dataGrid.Columns.Add(cmbSortDirColumn);
            //dataGrid.Columns.Add(rdoSortDirColumn);
            //dataGrid.Columns.Add(txtSortDirColumn);


            this.dataList = new ObservableCollection<OutputItem>();
            int count = 0;
            foreach (string colmun in colmun_list)
            {

                DataRow[] drs = output_item_dt.Select("項目名 = '" + colmun + "'");

                if (drs.Length>0)
                {
                    bool display = drs[0]["表示有無"].ToString().Equals("有") ? true : false;

                    dataList.Add(new OutputItem
                    {
                        output_item = colmun,
                        display = display,
                        sort = drs[0]["ソート順"].ToString(),
                        sort_dir = drs[0]["ソート方向"].ToString(),
                    });
                }
                else
                {
                    dataList.Add(new OutputItem
                    {
                        output_item = colmun,
                        display = true,
                        sort = "",
                        sort_dir = "",
                    });
                }

                count++;
            }

            dataGrid.ItemsSource = dataList;
        }

        public void UpdateQueueData(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                DataGridRow dgRow = e.Row;
                DataRowView rowView = dgRow.Item as DataRowView;
                DataRow drItem = rowView.Row;
                //Queue.Rows.RemoveAt(e.Row.GetIndex());
                //Queue.ImportRow(drItem);
                //WriteXML();
            }
        }

        private void dataGrid_CurCellChange(object sender, EventArgs e)
        {
            // String variable used to show message.
            string myString = "CurrentCellChanged event raised, cell focus is at ";
            // Get the co-ordinates of the focussed cell.
            string myPoint = dataGrid.CurrentCell.Column + "," +
                           dataGrid.CurrentCell.Item;

            OutputItem item = (OutputItem)dataGrid.CurrentCell.Item;

            // Create the alert message.
            myString = myString + "(" + myPoint + ")";
            // Show Co-ordinates when CurrentCellChanged event is raised.
            //MessageBox.Show(myString, "Current cell co-ordinates");
        }

        private void dataGrid_CheckChange(object sender, EventArgs e)
        {
            // String variable used to show message.
            string myString = "CurrentCellChanged event raised, cell focus is at ";
            // Get the co-ordinates of the focussed cell.
            string myPoint = dataGrid.CurrentCell.Column + "," +
                           dataGrid.CurrentCell.Item;

            OutputItem item = (OutputItem)dataGrid.CurrentCell.Item;

            // Create the alert message.
            myString = myString + "(" + myPoint + ")";
            // Show Co-ordinates when CurrentCellChanged event is raised.
            //MessageBox.Show(myString, "Current cell co-ordinates");
        }

        private void dataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            //DataGrid dataGrid = (DataGrid)sender;

            //// データグリッドの行数を取得します。
            //var rowCount = dataGrid.Items.Count;
            //// データグリッドの行数分繰り返します。
            //for (int i = 0; i < rowCount; ++i)
            //{
            //    // データグリッドの行オブジェクトを取得します。
            //    var row = dataGrid.ItemContainerGenerator.ContainerFromIndex(i) as DataGridRow;

            //    // 行オブジェクトが取得できない場合
            //    if (row == null)
            //    {
            //        // 対象の行が表示されていない場合、行オブジェクトが取得できないため
            //        // 対象の行が表示されるようスクロールします。
            //        dataGrid.UpdateLayout();
            //        dataGrid.ScrollIntoView(dataGrid.Items[i]);
            //        // 再度、行オブジェクトを取得します。
            //        row = dataGrid.ItemContainerGenerator.ContainerFromIndex(i) as DataGridRow;
            //    }

            //    TextBlock txt = (TextBlock)dataGrid.Columns[6].GetCellContent(row);

            //    StackPanel stk = GetChildElement<StackPanel>((ContentPresenter)dataGrid.Columns[5].GetCellContent(row), 0);

            //    if (stk != null)
            //    {
            //        RadioButton rdo1 = GetChildElement<RadioButton>(stk, 0);
            //        RadioButton rdo2 = GetChildElement<RadioButton>(stk, 1);

            //        rdo1.GroupName = "sort_dir_group" + i;
            //        rdo2.GroupName = "sort_dir_group" + i;

            //        if (txt.Text.Equals("asc"))
            //        {
            //            rdo1.IsChecked = true;
            //        }
            //        else if (txt.Text.Equals("desc"))
            //        {
            //            rdo2.IsChecked = true;
            //        }
            //    }

            //}
        }

        private T GetChildElement<T>(DependencyObject reference, int childIdx) where T : FrameworkElement
        {
            try
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
            catch (Exception ex) 
            { 
            }

            return null;
        }

        /// <summary>
        /// 出力項目設定を初期化　ボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnInitialize_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// キャンセル　ボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// OK　ボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            OutputSettingDialogViewModel vm = (OutputSettingDialogViewModel)DataContext;

            List<OutputItem> list = new List<OutputItem>();

            // データグリッドの行数を取得します。
            var rowCount = dataGrid.Items.Count;
            // データグリッドの行数分繰り返します。
            for (int i = 0; i < rowCount; ++i)
            {
                // データグリッドの行オブジェクトを取得します。
                var row = dataGrid.ItemContainerGenerator.ContainerFromIndex(i) as DataGridRow;

                // 行オブジェクトが取得できない場合
                if (row == null)
                {
                    // 対象の行が表示されていない場合、行オブジェクトが取得できないため
                    // 対象の行が表示されるようスクロールします。
                    dataGrid.UpdateLayout();
                    dataGrid.ScrollIntoView(dataGrid.Items[i]);
                    // 再度、行オブジェクトを取得します。
                    row = dataGrid.ItemContainerGenerator.ContainerFromIndex(i) as DataGridRow;
                }

                //CheckBox display = (CheckBox)dataGrid.Columns[2].GetCellContent(row);
                CheckBox display = GetChildElement<CheckBox>((ContentPresenter)dataGrid.Columns[2].GetCellContent(row), 0);
                //TextBlock ouput_item = (TextBlock)dataGrid.Columns[3].GetCellContent(row);
                TextBox ouput_item = GetChildElement<TextBox>((ContentPresenter)dataGrid.Columns[3].GetCellContent(row), 0);
                ComboBox sort = GetChildElement<ComboBox>((ContentPresenter)dataGrid.Columns[4].GetCellContent(row), 0);
                ComboBox sort_dir = GetChildElement<ComboBox>((ContentPresenter)dataGrid.Columns[5].GetCellContent(row), 0);

                list.Add(new OutputItem()
                {
                    display = display == null ? false : (bool)display.IsChecked,
                    output_item = ouput_item.Text,
                    sort = sort.SelectedValue == null ? "" : sort.SelectedValue.ToString(),
                    sort_dir = sort_dir.SelectedValue == null ? "" : sort_dir.SelectedValue.ToString(),
                });


            }

            vm.SetOutputItem(this.dic, list);

            this.Close();
        }
    }
}
