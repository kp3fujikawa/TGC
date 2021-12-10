using SearchSample.ViewModel;
using System;
using System.Collections.Generic;
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
        }

        /// <summary>
        /// Loadedイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            OutputSettingDialogViewModel vm = (OutputSettingDialogViewModel)DataContext;
            DataTable dt = new DataTable();
            dt.Columns.Add("上へ");
            dt.Columns.Add("下へ");
            dt.Columns.Add("表示有無");
            dt.Columns.Add("項目名");
            dt.Columns.Add("ソート順序");
            dt.Columns.Add("ソート方向");

            List<string> colmun_list = new List<string>();
            vm.GetItemList(out colmun_list);

            foreach (string colmun in colmun_list)
            {
                DataRow newrow = dt.NewRow();
                newrow["上へ"] = "∧";
                newrow["下へ"] = "∨";
                newrow["表示有無"] = "1";
                newrow["項目名"] = colmun;
                newrow["ソート順序"] = "";
                newrow["ソート方向"] = "昇順";
                dt.Rows.Add(newrow);
            }

            // グリッド初期設定
            dataGrid.IsReadOnly = true;              // 読取専用
            //dataGrid.CanUserDeleteRows = false;      // 行削除禁止
            //dataGrid.CanUserAddRows = false;         // 行挿入禁止
            dataGrid.HeadersVisibility = DataGridHeadersVisibility.Column;   // 先頭列非表示
            dataGrid.AutoGenerateColumns = false;    // 列の自動追加禁止

            

            DataGridTextColumn txtUpColumn = new DataGridTextColumn()
            {
                Header = "",
                Binding = new Binding("上へ"),
                //Width = 50,
            };
            DataGridTextColumn txtDwonColumn = new DataGridTextColumn()
            {
                Header = "",
                Binding = new Binding("下へ"),
                //Width = 50,
            };

            FrameworkElementFactory chkDisplay = new FrameworkElementFactory(typeof(CheckBox));
            chkDisplay.SetBinding(TextBlock.TextProperty, new Binding("表示有無"));

            DataGridTemplateColumn chkDisplayColumn = new DataGridTemplateColumn()
            {
                Header = "表示有無",
                CellTemplate = new DataTemplate()
                {
                    DataType = typeof(CheckBox),
                    VisualTree = chkDisplay,
                },
                //Width = 50,
            };

            DataGridTextColumn txtItemNameColumn = new DataGridTextColumn()
            {
                Header = "項目名",
                Binding = new Binding("項目名"),
                //Width = 150,
            };

            FrameworkElementFactory cmbSort = new FrameworkElementFactory(typeof(ComboBox));
            cmbSort.SetBinding(TextBlock.TextProperty, new Binding("ソート順序"));

            DataGridTemplateColumn cmbSortColumn = new DataGridTemplateColumn()
            {
                Header = "ソート順序",
                CellTemplate = new DataTemplate()
                {
                    DataType = typeof(ComboBox),
                    VisualTree = cmbSort,
                },
                //Width = 100,
            };

            FrameworkElementFactory rdoSort = new FrameworkElementFactory(typeof(RadioButton));
            rdoSort.SetBinding(TextBlock.TextProperty, new Binding("ソート方向"));

            DataGridTemplateColumn rdoSortColumn = new DataGridTemplateColumn()
            {
                Header = "ソート方向",
                CellTemplate = new DataTemplate()
                {
                    DataType = typeof(RadioButton),
                    VisualTree = rdoSort,
                },
                //Width = 100,
            };

            dataGrid.Columns.Add(txtUpColumn);
            dataGrid.Columns.Add(txtDwonColumn);
            dataGrid.Columns.Add(chkDisplayColumn);
            dataGrid.Columns.Add(txtItemNameColumn);
            dataGrid.Columns.Add(cmbSortColumn);
            dataGrid.Columns.Add(rdoSortColumn);


            dataGrid.ItemsSource = dt.DefaultView;
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
            
            //List<string> rtn = new List<string>();
            //foreach (string a in list1.SelectedItems)
            //{
            //    if (string.IsNullOrEmpty(a)) continue;
            //    rtn.Add(a);
            //}

            //main.QualityList = rtn;

            this.Close();
        }
    }
}
