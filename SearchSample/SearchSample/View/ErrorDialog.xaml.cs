using System;
using System.Collections.Generic;
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
    public partial class ErrorDialog : MahApps.Metro.Controls.MetroWindow
    {
        public List<string> Message { get; set; }

        /// <summary>
        /// エラーダイアログ
        /// </summary>
        public ErrorDialog()
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
            // メッセージをRichTextBoxに設定
            FlowDocument myFlowDoc = new FlowDocument();
            Paragraph myParagraph = new Paragraph();
            myParagraph.Inlines.Add(string.Join("\n", Message));
            myFlowDoc.Blocks.Add(myParagraph);
            richTextBox1.Document = myFlowDoc;

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
