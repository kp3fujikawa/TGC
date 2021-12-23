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
    public partial class DataDictionaryItem : MahApps.Metro.Controls.MetroWindow
    {
        public List<string> Message { get; set; }
        public string DataDictionaryId { get; set; }
        
        private DataTable Table;

        /// <summary>
        /// エラーダイアログ
        /// </summary>
        public DataDictionaryItem()
        {
            InitializeComponent();

            // WPF版：テーマ設定
            Common.SettingTheme(this);

            Message = new List<string>();
        }

        /// <summary>
        /// Loadedイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // 検索条件生成
            DataDictionaryItemViewModel vm = (DataDictionaryItemViewModel)DataContext;
            vm.SearchData(DataDictionaryId, out Table);

            //dataGrid2.Columns.Clear();
            dataGrid2.ItemsSource = Table.DefaultView;

        }

        /// <summary>
        /// 閉じる　ボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
