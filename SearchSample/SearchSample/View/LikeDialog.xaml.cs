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
    public partial class LikeDialog : MahApps.Metro.Controls.MetroWindow
    {
        private MainWindow main;

        /// <summary>
        /// エラーダイアログ
        /// </summary>
        public LikeDialog(MainWindow main)
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
            LikeDialogViewModel vm = (LikeDialogViewModel)DataContext;
            DataTable dt = new DataTable();
            dt.Columns.Add("表示有無");
            dt.Columns.Add("お気に入り名称");

            for(int i=0; i<20; i++)
            {
                DataRow newrow = dt.NewRow();
                newrow["表示有無"] = "1";
                newrow["お気に入り名称"] = "お気に入り名称"+(i+1);
                dt.Rows.Add(newrow);
            }

            //vm.GetTestitemList(out dt);

            //List<string> list = new List<string>();
            //foreach (DataRow row in dt.Rows)
            //{
            //    list.Add(row["試験項目名"].ToString());
            //}

            // グリッド初期設定
            dataGrid.IsReadOnly = true;              // 読取専用
            //dataGrid.CanUserDeleteRows = false;      // 行削除禁止
            //dataGrid.CanUserAddRows = false;         // 行挿入禁止
            dataGrid.HeadersVisibility = DataGridHeadersVisibility.Column;   // 先頭列非表示
            dataGrid.AutoGenerateColumns = false;    // 列の自動追加禁止

            FrameworkElementFactory rdobtn = new FrameworkElementFactory(typeof(RadioButton));
            rdobtn.SetBinding(TextBlock.TextProperty, new Binding("表示有無"));

            DataGridTemplateColumn chkColumn = new DataGridTemplateColumn()
            {
                Header = "表示有無",
                CellTemplate = new DataTemplate()
                {
                    DataType = typeof(RadioButton),
                    VisualTree = rdobtn,
                },
            };

            DataGridTextColumn txtColumn = new DataGridTextColumn()
            {
                Header = "お気に入り名称",
                Binding = new Binding("お気に入り名称"),
            };

            dataGrid.Columns.Add(chkColumn);
            dataGrid.Columns.Add(txtColumn);

            dataGrid.ItemsSource = dt.DefaultView;
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
