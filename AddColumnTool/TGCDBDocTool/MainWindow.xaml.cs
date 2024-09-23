using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TGCDBDocTool
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {

        Regex reg = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
        Regex reg2 = new Regex("\t(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// ファイル選択 ボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectFile_Click(object sender, RoutedEventArgs e)
        {
            // ダイアログのインスタンスを生成
            var dialog = new OpenFileDialog();

            // ファイルの種類を設定
            dialog.Filter = "a5erファイル (*.a5er)|*.a5er";

            // ダイアログを表示する
            if (dialog.ShowDialog() == true)
            {
                // 選択されたファイル名 (ファイルパス) をテキストボックスに表示
                txtFileName.Text = dialog.FileName;
            }
        }

        /// <summary>
        /// 終了 ボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// CSVファイル選択 ボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectCSV_Click(object sender, RoutedEventArgs e)
        {
            // ダイアログのインスタンスを生成
            var dialog = new OpenFileDialog();

            // ファイルの種類を設定
            dialog.Filter = "TSVファイル (*.tsv)|*.tsv";

            // ダイアログを表示する
            if (dialog.ShowDialog() == true)
            {
                // 選択されたファイル名 (ファイルパス) をテキストボックスに表示
                txtCSVFile.Text = dialog.FileName;
            }
        }

        /// <summary>
        /// ファイル保存ダイアログを生成する
        /// </summary>
        /// <param name="filter"></param>
        /// <returns>ファイル名</returns>
        private String selectOutputFile(String filter)
        {
            // ファイル保存ダイアログを生成します。
            var dialog = new SaveFileDialog();

            // フィルターを設定します。
            // この設定は任意です。
            dialog.Filter = filter;

            // ファイル保存ダイアログを表示します。
            var result = dialog.ShowDialog() ?? false;

            // 保存ボタン以外が押下された場合
            if (!result)
            {
                // 終了します。
                return "";
            }

            return dialog.FileName;
        }

        private const Int32 SABUN_POS_KUBUN = 0;
        private const Int32 SABUN_POS_ENTITY = 4;
        private const Int32 SABUN_POS_COL = 5;
        private const Int32 SABUN_POS_CHG_COL = 6;

        private void btnProc11_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (File.Exists(txtFileName.Text) && File.Exists(txtCSVFile.Text))
                {
                    Dictionary<string, string[]> changeitems = new Dictionary<string, string[]>();
                    Dictionary<string, string[]> domains = new Dictionary<string, string[]>();
                    Dictionary<string, List<string>> dropitems = new Dictionary<string, List<string>>();

                    // 変更情報
                    if (!string.IsNullOrEmpty(txtCSVFile.Text))
                    {
                        StreamReader srCSV = new StreamReader(txtCSVFile.Text);
                        string strbufCSV = "";      // CSVファイル読み込みバッファ
                        try
                        {
                            // CSVファイルのデータを処理
                            while (srCSV.EndOfStream == false)
                            {
                                strbufCSV = srCSV.ReadLine();

                                // 文字列を分割して配列に格納
                                var arrField = reg2.Split(strbufCSV);

                                if (arrField.Length == 6 || (arrField.Length == 7 && string.IsNullOrEmpty(arrField[SABUN_POS_CHG_COL])))
                                {
                                    if (arrField[SABUN_POS_KUBUN].Trim('\"') == "削除")
                                    {
                                        if (!dropitems.ContainsKey(arrField[SABUN_POS_ENTITY].Trim('\"')))
                                        {
                                            dropitems.Add(arrField[SABUN_POS_ENTITY].Trim('\"'), new List<string>());
                                        }
                                        dropitems[arrField[SABUN_POS_ENTITY].Trim('\"')].Add(arrField[SABUN_POS_COL]);
                                    }
                                    else
                                    {
                                        changeitems.Add(arrField[SABUN_POS_ENTITY].Trim('\"') + "," + arrField[SABUN_POS_COL].Trim('\"'), arrField);
                                    }
                                }
                                else if (arrField.Length == 7)
                                {
                                    changeitems.Add(arrField[SABUN_POS_ENTITY].Trim('\"') + "," + arrField[SABUN_POS_CHG_COL].Trim('\"'), arrField);
                                }
                            }
                        }
                        catch
                        {
                            throw;
                        }
                        finally
                        {
                            // CSVファイルを閉じる
                            srCSV.Close();
                        }
                    }

                    // ファイル保存ダイアログを表示します。
                    String result = selectOutputFile("スクリプトファイル(*.sql)|*.sql");
                    if (String.IsNullOrEmpty(result))
                    {
                        // 終了します。
                        return;
                    }

                    StreamReader sr = new StreamReader(txtFileName.Text);

                    StreamWriter sw = new StreamWriter(result, false, Encoding.UTF8);

                    try
                    {
                        string strbuf = "";         // ファイル読み込みバッファ
                        string TableName = "";
                        string TableName2 = "";
                        bool IsManager = false;     // [Manager]判断フラグ
                        bool IsEntity = false;     // [Entity]判断フラグ                   
                        Dictionary<string, string> items = new Dictionary<string, string>();
                        List<string> dropcol = new List<string>();

                        while (sr.EndOfStream == false)
                        {
                            strbuf = sr.ReadLine();

                            // ブロックを判定
                            if (strbuf != "" && strbuf[0] == '[')
                            {
                                IsManager = false;
                                if (strbuf.IndexOf("[Manager]") != -1)
                                {
                                    IsManager = true;
                                }
                                IsEntity = false;
                                if (strbuf.IndexOf("[Entity]") != -1)
                                {
                                    IsEntity = true;
                                    TableName = "";
                                }
                            }
                            //-----------------------------------------------------
                            // [Manager]の場合
                            //-----------------------------------------------------
                            if (IsManager)
                            {
                                // Domainの場合
                                if (strbuf.IndexOf("DomainInfo=") != -1)
                                {
                                    // 文字列を分割して配列に格納
                                    var arrField = reg.Split(strbuf);
                                    arrField[0] = arrField[0].Replace("DomainInfo=", "");
                                    var domainname = arrField[0].Replace("\"", "");

                                    // ドメイン情報を格納する
                                    if (!domains.ContainsKey(domainname))
                                    {
                                        domains.Add(domainname, arrField);
                                    }
                                }
                            }

                            //-----------------------------------------------------
                            // [Entity]の場合
                            //-----------------------------------------------------
                            if (IsEntity)
                            {
                                // テーブル名を取得
                                if (strbuf.IndexOf("LName") != -1)
                                {
                                    TableName = strbuf.Substring(6);

                                    if (dropitems.ContainsKey(TableName))
                                    {
                                        var data = dropitems[TableName];
                                        if (data.Count > 0)
                                        {
                                            foreach (string colname in data)
                                            {
                                                dropcol.Add("alter table " + TableName2 + " drop column " + colname + ";");
                                            }
                                        }
                                    }
                                }
                                if (strbuf.IndexOf("PName") != -1)
                                {
                                    TableName2 = strbuf.Substring(6);
                                }

                                // Fieldの場合
                                if (strbuf.IndexOf("Field") != -1)
                                {
                                    var checkField = reg.Split(strbuf.Replace("Field=", ""));
                                    var colname = checkField[0].Trim('"');
                                    var colname2 = checkField[1].Trim('"');
                                    var domain = checkField[2].Trim('"').Replace("*", "");
                                    var notnull = String.IsNullOrEmpty(checkField[3].Trim('"').Trim()) ? "" : (" " + checkField[3].Trim('"').Trim());
                                    var comment = checkField[6].Trim('"');

                                    if (changeitems.ContainsKey(TableName + "," + colname))
                                    {
                                        System.Diagnostics.Debug.Print("(1):" + TableName + " " + TableName2 + " " + colname + " " + colname2 + " " + domain);

                                        // 変更情報（論理名）
                                        var data = changeitems[TableName + "," + colname];

                                        if (data[SABUN_POS_KUBUN] == "追加")
                                        {
                                            // 追加
                                            // ドメイン情報
                                            if (domains.ContainsKey(domain))
                                            {
                                                string[] data2 = domains[domain];
                                                var itemtype = data2[1].Trim('"');

                                                // 追加
                                                sw.WriteLine("alter table " + TableName2 + " add " + colname2 + " " + itemtype + notnull + ";");
                                                sw.WriteLine("execute sp_addextendedproperty N'MS_Description', N'" + colname + ":" + comment + "', N'SCHEMA', N'dbo', N'TABLE', N'" + TableName2 + "', N'COLUMN', N'" + colname2 + "';");
                                            }
                                            else
                                            {
                                                // ドメインに存在しない場合は型情報をそのまま出力
                                                sw.WriteLine("alter table " + TableName2 + " add " + colname2 + " " + domain + notnull + ";");
                                                sw.WriteLine("execute sp_addextendedproperty N'MS_Description', N'" + colname + ":" + comment + "', N'SCHEMA', N'dbo', N'TABLE', N'" + TableName2 + "', N'COLUMN', N'" + colname2 + "';");
                                            }
                                        }
                                        else if (data[SABUN_POS_KUBUN] == "変更")
                                        {
                                            // 変更
                                            if (data.Length == 6 || (data.Length == 7 && string.IsNullOrEmpty(data[SABUN_POS_CHG_COL])))
                                            {
                                                // ドメイン情報
                                                if (domains.ContainsKey(domain))
                                                {
                                                    string[] data2 = domains[domain];
                                                    var itemtype = data2[1].Trim('"');

                                                    sw.WriteLine("alter table " + TableName2 + " alter column " + colname2 + " " + itemtype + notnull + ";");
                                                    sw.WriteLine("execute sp_updateextendedproperty N'MS_Description', N'" + colname + ":" + comment + "', N'SCHEMA', N'dbo', N'TABLE', N'" + TableName2 + "', N'COLUMN', N'" + colname2 + "';");
                                                }
                                                else
                                                {
                                                    // ドメインに存在しない場合は型情報をそのまま出力
                                                    sw.WriteLine("alter table " + TableName2 + " alter column " + colname2 + " " + domain + notnull + ";");
                                                    sw.WriteLine("execute sp_updateextendedproperty N'MS_Description', N'" + colname + ":" + comment + "', N'SCHEMA', N'dbo', N'TABLE', N'" + TableName2 + "', N'COLUMN', N'" + colname2 + "';");
                                                }
                                            }
                                            else if (data.Length == 7)
                                            {
                                                // 変更（論理名、コメント）
                                                sw.WriteLine("execute sp_updateextendedproperty N'MS_Description', N'" + colname + ":" + comment + "', N'SCHEMA', N'dbo', N'TABLE', N'" + TableName2 + "', N'COLUMN', N'" + colname2 + "';");
                                            }
                                        }
                                    }
                                    else if (changeitems.ContainsKey(TableName + "," + colname2))
                                    {
                                        System.Diagnostics.Debug.Print("(2):" + TableName + " " + TableName2 + " " + colname + " " + colname2 + " " + domain);

                                        // 変更情報（物理名）
                                        var data = changeitems[TableName + "," + colname2];

                                        if (data[SABUN_POS_KUBUN] == "追加")
                                        {
                                            // ドメイン情報
                                            if (domains.ContainsKey(domain))
                                            {
                                                string[] data2 = domains[domain];
                                                var itemtype = data2[1].Trim('"');

                                                // 追加
                                                sw.WriteLine("alter table " + TableName2 + " add " + colname2 + " " + itemtype + notnull + ";");
                                                sw.WriteLine("execute sp_addextendedproperty N'MS_Description', N'" + colname + ":" + comment + "', N'SCHEMA', N'dbo', N'TABLE', N'" + TableName2 + "', N'COLUMN', N'" + colname2 + "';");
                                            }
                                            else
                                            {
                                                // 追加
                                                sw.WriteLine("alter table " + TableName2 + " add " + colname2 + " " + domain + notnull + ";");
                                                sw.WriteLine("execute sp_addextendedproperty N'MS_Description', N'" + colname + ":" + comment + "', N'SCHEMA', N'dbo', N'TABLE', N'" + TableName2 + "', N'COLUMN', N'" + colname2 + "';");
                                            }
                                        }
                                        else if (data[SABUN_POS_KUBUN] == "変更")
                                        {
                                            if (data.Length == 6 || (data.Length == 7 && string.IsNullOrEmpty(data[SABUN_POS_CHG_COL])))
                                            {
                                                // ドメイン情報
                                                if (domains.ContainsKey(domain))
                                                {
                                                    string[] data2 = domains[domain];
                                                    var itemtype = data2[1].Trim('"');

                                                    // 変更
                                                    sw.WriteLine("alter table " + TableName2 + " alter column " + colname2 + " " + itemtype + notnull + ";");
                                                    sw.WriteLine("execute sp_updateextendedproperty N'MS_Description', N'" + colname + ":" + comment + "', N'SCHEMA', N'dbo', N'TABLE', N'" + TableName2 + "', N'COLUMN', N'" + colname2 + "';");
                                                }
                                                else
                                                {
                                                    // 変更
                                                    sw.WriteLine("alter table " + TableName2 + " alter column " + colname2 + " " + domain + notnull + ";");
                                                    sw.WriteLine("execute sp_updateextendedproperty N'MS_Description', N'" + colname + ":" + comment + "', N'SCHEMA', N'dbo', N'TABLE', N'" + TableName2 + "', N'COLUMN', N'" + colname2 + "';");
                                                }
                                            }
                                            else if (data.Length == 7)
                                            {
                                                // ドメイン情報
                                                if (domains.ContainsKey(domain))
                                                {
                                                    string[] data2 = domains[domain];
                                                    var itemtype = data2[1].Trim('"');

                                                    // 変更（物理名）
                                                    sw.WriteLine("alter table " + TableName2 + " add " + colname2 + " " + itemtype + notnull + ";");
                                                    sw.WriteLine("execute sp_addextendedproperty N'MS_Description', N'" + colname + ":" + comment + "', N'SCHEMA', N'dbo', N'TABLE', N'" + TableName2 + "', N'COLUMN', N'" + colname2 + "';");
                                                    sw.WriteLine("update " + TableName2 + " set " + colname2 + "=" + data[SABUN_POS_COL] + ";");
                                                    sw.WriteLine("alter table " + TableName2 + " drop column " + data[SABUN_POS_COL] + ";");
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        foreach (string buf in dropcol)
                        {
                            sw.WriteLine(buf);
                        }
                    }
                    finally
                    {
                        // ファイルを閉じる
                        sw.Close();
                        sr.Close();
                    }

                    MessageBox.Show("処理が終了しました", Title);
                }
                else
                {
                    MessageBox.Show("ファイルが存在しません", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
