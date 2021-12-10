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
    public partial class QuolityDialog : MahApps.Metro.Controls.MetroWindow
    {
        private MainWindow main;

        /// <summary>
        /// エラーダイアログ
        /// </summary>
        public QuolityDialog(MainWindow main)
        {
            InitializeComponent();

            // WPF版：テーマ設定
            Common.SettingTheme(this);

            this.main = main;
        }

        /// <summary>
        /// Loadedイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            QuolityDialogViewModel vm = (QuolityDialogViewModel)DataContext;
            DataTable dt = new DataTable();
            vm.GetTestitemList(out dt);

            List<string> list = new List<string>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(row["試験項目名"].ToString());
            }

            list1.ItemsSource = list;
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

        /// <summary>
        /// 設定　ボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSetting_Click(object sender, RoutedEventArgs e)
        {
            
            List<string> rtn = new List<string>();
            foreach (string a in list1.SelectedItems)
            {
                if (string.IsNullOrEmpty(a)) continue;
                rtn.Add(a);
            }

            main.QualityList = rtn;

            this.Close();
        }
    }
}
