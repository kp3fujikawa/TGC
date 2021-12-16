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
    public partial class LikeRegisterDialog : MahApps.Metro.Controls.MetroWindow
    {
        /// <summary>
        /// エラーダイアログ
        /// </summary>
        public LikeRegisterDialog()
        {
            InitializeComponent();

            // WPF版：テーマ設定
            Common.SettingTheme(this);
        }

        /// <summary>
        /// Loadedイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LikeRegisterDialogViewModel vm = (LikeRegisterDialogViewModel)DataContext;
            
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
