﻿using SearchSample.Model;
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
        public ObservableCollection<OutputItem> dataList { get; set; }

        public DataTable SortDt { get; set; }

        public DataTable SortDtDir { get; set; }

        public List<string> colmun_list { get; set; }

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

            List<string> int_colmun_list = new List<string>();
            vm.GetItemList(this.dic, out int_colmun_list);
            colmun_list = int_colmun_list;

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

            Dictionary<string, string> sort_dir_dic =
            new Dictionary<string, string>()
            {
                {"ASC", "昇順"},
                {"DESC", "降順"},
            };

            foreach (KeyValuePair<string,string> sort_dir in sort_dir_dic)
            {
                DataRow newrow = SortDtDir.NewRow();
                newrow[Common.ComboBoxText] = sort_dir.Value;
                newrow[Common.ComboBoxValue] = sort_dir.Key;
                SortDtDir.Rows.Add(newrow);
            }

            // グリッド初期設定
            //dataGrid.IsReadOnly = true;              // 読取専用
            //dataGrid.CanUserDeleteRows = false;      // 行削除禁止
            //dataGrid.CanUserAddRows = false;         // 行挿入禁止
            //dataGrid.HeadersVisibility = DataGridHeadersVisibility.Column;   // 先頭列非表示
            dataGrid.AutoGenerateColumns = false;    // 列の自動追加禁止

            FrameworkElementFactory up = new FrameworkElementFactory(typeof(Button));
            up.SetValue(Button.ContentProperty, "∧");
            up.SetValue(Button.UidProperty, new Binding("line_no"));
            up.AddHandler(Button.ClickEvent, new RoutedEventHandler(up_event));

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
            down.SetValue(Button.ContentProperty, "∨");
            down.SetValue(Button.UidProperty, new Binding("line_no"));
            down.AddHandler(Button.ClickEvent, new RoutedEventHandler(down_event));

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

            FrameworkElementFactory chkDisplay = new FrameworkElementFactory(typeof(CheckBox));
            chkDisplay.SetValue(CheckBox.HorizontalAlignmentProperty, HorizontalAlignment.Center);

            Binding bind_display = new Binding("display");
            bind_display.NotifyOnSourceUpdated = true;
            bind_display.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            chkDisplay.SetBinding(CheckBox.IsCheckedProperty, bind_display);

            CheckBox headerchk = new CheckBox();
            headerchk.Content = "表示有無";
            headerchk.AddHandler(CheckBox.CheckedEvent, new RoutedEventHandler(chk_event));
            headerchk.AddHandler(CheckBox.UncheckedEvent, new RoutedEventHandler(unchk_event));

            DataGridTemplateColumn chkDisplayColumn = new DataGridTemplateColumn()
            {
                Header = headerchk,
                CellTemplate = new DataTemplate()
                {
                    DataType = typeof(CheckBox),
                    VisualTree = chkDisplay,
                },
                //Width = 130,
            };

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


            DataGridTemplateColumn cmbSortColumn = CreateCombo("sort", "ソート順序", SortDt.DefaultView);

            //DataGridTemplateColumn cmbSortDirColumn = CreateCombo("sort_dir", "ソート方向", SortDtDir.DefaultView);

            FrameworkElementFactory rdoSortDir1 = new FrameworkElementFactory(typeof(RadioButton));
            rdoSortDir1.SetValue(RadioButton.ContentProperty, "昇順");
            Binding bind_asc = new Binding("sort_dir_asc");
            bind_asc.Mode = BindingMode.TwoWay;
            bind_asc.NotifyOnSourceUpdated = true;
            bind_asc.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            rdoSortDir1.SetValue(RadioButton.IsCheckedProperty, bind_asc);

            FrameworkElementFactory rdoSortDir2 = new FrameworkElementFactory(typeof(RadioButton));
            rdoSortDir2.SetValue(RadioButton.ContentProperty, "降順");
            Binding bind_desc = new Binding("sort_dir_desc");
            bind_desc.Mode = BindingMode.TwoWay;
            bind_desc.NotifyOnSourceUpdated = true;
            bind_desc.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            rdoSortDir2.SetValue(RadioButton.IsCheckedProperty, bind_desc);

            FrameworkElementFactory stkSortDir = new FrameworkElementFactory(typeof(StackPanel));
            stkSortDir.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);
            stkSortDir.AppendChild(rdoSortDir1);
            stkSortDir.AppendChild(rdoSortDir2);

            DataGridTemplateColumn rdoSortDirColumn = new DataGridTemplateColumn()
            {
                Header = "ソート方向",
                CellTemplate = new DataTemplate()
                {
                    DataType = typeof(StackPanel),
                    VisualTree = stkSortDir,
                },
                //Width = 130,
            };


            dataGrid.Columns.Add(hlup);
            dataGrid.Columns.Add(hldown);
            dataGrid.Columns.Add(chkDisplayColumn);
            dataGrid.Columns.Add(txtItemNameColumn);
            dataGrid.Columns.Add(cmbSortColumn);
            //dataGrid.Columns.Add(cmbSortDirColumn);
            dataGrid.Columns.Add(rdoSortDirColumn);

            this.dataList = new ObservableCollection<OutputItem>();

            Dictionary<string, int> except_list = new Dictionary<string, int>();

            int max_line_no = 1;

            foreach (DataRow row in output_item_dt.Rows)
            {
                bool display = row["表示有無"].ToString().Equals("1") ? true : false;
                int current_line_no = string.IsNullOrEmpty(row["表示順"].ToString()) ? 0 : int.Parse(row["表示順"].ToString());

                if (current_line_no > max_line_no)
                {
                    max_line_no = current_line_no;
                }

                this.dataList.Add(new OutputItem
                {
                    output_item = row["項目名"].ToString(),
                    display = display,
                    sort = row["ソート順"].ToString(),
                    sort_dir = row["ソート方向"].ToString(),
                    line_no = row["表示順"].ToString(),
                    sort_dir_asc = row["ソート方向"].ToString().Equals("ASC") ? true : false,
                    sort_dir_desc = row["ソート方向"].ToString().Equals("DESC") ? true : false,
                });;

                if (!except_list.ContainsKey(row["項目名"].ToString()))
                {
                    except_list[row["項目名"].ToString()] = 0;
                }
            }

            foreach (string colmun in colmun_list)
            {

                if (except_list.ContainsKey(colmun))
                {
                    continue;
                }

                this.dataList.Add(new OutputItem
                {
                    output_item = colmun,
                    display = false,
                    sort = "",
                    sort_dir = "",
                    line_no = String.Format("{0:D4}", (max_line_no++)).ToString(),
                    sort_dir_asc = false,
                    sort_dir_desc = false,
                });
            }

            dataGrid.ItemsSource = this.dataList;
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
                //Width = 130,
            };
        }

        private void dataGrid_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            //OutputItem row = new OutputItem();

            //var type = e.OriginalSource.GetType();
            //if (type == typeof(ComboBox))
            //{
            //    ComboBox obj = (ComboBox)e.OriginalSource;
            //    row = (OutputItem)obj.DataContext;
            //}
            //else if (type == typeof(CheckBox))
            //{
            //    CheckBox obj = (CheckBox)e.OriginalSource;
            //    row = (OutputItem)obj.DataContext;

            //}

            //else if (type == typeof(TextBox))
            //{
            //    TextBox obj = (TextBox)e.OriginalSource;
            //    row = (OutputItem)obj.DataContext;
            //}

            //OutputItem CheckCd01 = this.dataList.FirstOrDefault(l => l.output_item == row.output_item);

            //DataGrid dataGrid = (DataGrid)sender;

        }

        public void chk_event(object sender, RoutedEventArgs e)
        {
            try
            {
                for (int i = 0; i < this.dataList.Count; i++)
                {
                    OutputItem tmprow = new OutputItem()
                    {
                        output_item = this.dataList[i].output_item,
                        display = true,
                        sort = this.dataList[i].sort,
                        sort_dir = this.dataList[i].sort_dir,
                        line_no = this.dataList[i].line_no,
                    };
                    this.dataList[i] = tmprow;
                }

            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }

        }

        public void unchk_event(object sender, RoutedEventArgs e)
        {
            try
            {
                for (int i = 0; i < this.dataList.Count; i++)
                {
                    OutputItem tmprow = new OutputItem()
                    {
                        output_item = this.dataList[i].output_item,
                        display = false,
                        sort = this.dataList[i].sort,
                        sort_dir = this.dataList[i].sort_dir,
                        line_no = this.dataList[i].line_no,
                    };
                    this.dataList[i] = tmprow;
                }

            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }

        }


        //private RelayCommand<int> _selectNameCommand;
        public void up_event(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = (Button)e.OriginalSource;
                int current_line_no = int.Parse(btn.Uid);

                if (current_line_no > 1)
                {
                    int before_line_no = current_line_no - 1;

                    OutputItem current_data = this.dataList[current_line_no - 1];
                    OutputItem before_data = this.dataList[before_line_no - 1];

                    current_data.line_no = String.Format("{0:D4}", before_line_no);
                    before_data.line_no = String.Format("{0:D4}", current_line_no);

                    this.dataList[current_line_no - 1] = before_data;
                    this.dataList[before_line_no - 1] = current_data;

                }

            }
            catch(Exception ex)
            {
                Common.DoError(ex);
            }

        }

        public void down_event(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = (Button)e.OriginalSource;
                int current_line_no = int.Parse(btn.Uid);

                if (current_line_no < this.dataList.Count + 1)
                {
                    int after_line_no = current_line_no + 1;

                    OutputItem current_data = this.dataList[current_line_no - 1];
                    OutputItem after_data = this.dataList[after_line_no - 1];

                    current_data.line_no = String.Format("{0:D4}", after_line_no);
                    after_data.line_no = String.Format("{0:D4}", current_line_no);

                    this.dataList[current_line_no - 1] = after_data;
                    this.dataList[after_line_no - 1] = current_data;

                }

            }
            catch (Exception ex)
            {
                Common.DoError(ex);
            }
        }


        /// <summary>
        /// 出力項目設定を初期化　ボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnInitialize_Click(object sender, RoutedEventArgs e)
        {
            OutputSettingDialogViewModel vm = (OutputSettingDialogViewModel)DataContext;

            vm.DeleteOutputItem(this.dic);

            this.dataList = new ObservableCollection<OutputItem>();

            int max_line_no = 1;

            foreach (string colmun in colmun_list)
            {
                this.dataList.Add(new OutputItem
                {
                    output_item = colmun,
                    display = false,
                    sort = "",
                    sort_dir = "",
                    line_no = String.Format("{0:D4}", (max_line_no++)).ToString(),
                });
            }

            dataGrid.ItemsSource = this.dataList;

            //this.Close();
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

            vm.SetOutputItem(this.dic, this.dataList.ToList());

            this.Close();
        }
    }
}
