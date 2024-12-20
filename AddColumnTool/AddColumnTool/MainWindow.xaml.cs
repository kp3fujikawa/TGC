﻿using System;
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
using System.Xml.Linq;
using Microsoft.Win32;      // ファイル選択ダイアログ

namespace AddColumnTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        Regex reg = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
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
        /// 【１】テーブルにサロゲートキーを追加する ボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExec_Click(object sender, RoutedEventArgs e)
        {
            try
            { 
                if (File.Exists(txtFileName.Text))
                {
                    // 出力ファイル名
                    int loc = txtFileName.Text.IndexOf("a5er");
                    string OutFileName = txtFileName.Text.Insert(loc - 1, "_追加");

                    StreamReader sr = new StreamReader(txtFileName.Text);

                    StreamWriter sw = new StreamWriter(OutFileName, false, Encoding.UTF8);

                    string strbuf = "";         // ファイル読み込みバッファ
                    string TableName = "";      // テーブル名
                    bool IsEntity = false;      // [Entity]判断フラグ
                    while (sr.EndOfStream == false)
                    {
                        strbuf = sr.ReadLine();

                        // ブロックを判定
                        if (strbuf != "" && strbuf[0] == '[')
                        {
                            IsEntity = false;
                            if (strbuf.IndexOf("[Entity]") != -1)
                            {
                                IsEntity = true;
                                TableName = "";
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
                            }

                            // Fieldの場合
                            if (strbuf.IndexOf("Field") != -1)
                            {
                                string addKeyName = TableName + "Key";
                                // 先頭に"xxxx+Key"という名称のカラムがあるかチェック
                                bool IsExistKey = false;        // キー存在フラグ
                                var checkField = reg.Split(strbuf);
                                var colname = checkField[0].Trim('"');
                                if (colname.Length > 3 &&
                                    (colname.Substring(colname.Length - 3).Equals("Key") ||
                                    colname.Substring(colname.Length - 2).Equals("キー"))) IsExistKey = true;

                                // 先頭にKeyがない場合
                                if (!IsExistKey)
                                { 
                                    // Keyを追加
                                    string addline = String.Format("Field=\"{0}\",\"{0}\",\"\",,0,\"\",\"\",$FFFFFFFF,\"\"",
                                        addKeyName);
                                    sw.WriteLine(addline);
                                }

                                // Fieldのデータ行を処理
                                while (sr.EndOfStream == false)
                                {
                                    // 先頭にKeyがない場合
                                    if (!IsExistKey)
                                    { 
                                        // 文字列を分割して配列に格納
                                        var arrField = reg.Split(strbuf);

                                        // キー項目の場合
                                        if (arrField[4] != "")
                                        {
                                            // 「主キー」をクリアする
                                            arrField[4] = "";

                                            // 配列を結合して文字列にする
                                            string strCsvData = string.Join(",", arrField);

                                            sw.WriteLine(strCsvData);
                                        }
                                        // キー以外の場合
                                        else
                                        {
                                            // 読み込んだ内容をそのまま書き出す
                                            sw.WriteLine(strbuf);
                                        }
                                    }
                                    // 先頭にKeyがある場合
                                    else
                                    {
                                        // 読み込んだ内容をそのまま書き出す
                                        sw.WriteLine(strbuf);
                                    }

                                    strbuf = sr.ReadLine();

                                    // Field以外なら抜ける
                                    if (strbuf.IndexOf("Field") == -1)
                                    {
                                        // 読み込んだ内容をそのまま書き出す
                                        sw.WriteLine(strbuf);
                                        break;
                                    }
                                }
                            }
                            // Field以外の場合
                            else
                            {
                                // 読み込んだ内容をそのまま書き出す
                                sw.WriteLine(strbuf);
                            }
                        }
                        //-----------------------------------------------------
                        // [Entity]以外の場合
                        //-----------------------------------------------------
                        else
                        {
                            // 読み込んだ内容をそのまま書き出す
                            sw.WriteLine(strbuf);
                        }
                    }

                    // ファイルを閉じる
                    sw.Close();
                    sr.Close();

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
            dialog.Filter = "CSVファイル (*.csv)|*.csv";

            // ダイアログを表示する
            if (dialog.ShowDialog() == true)
            {
                // 選択されたファイル名 (ファイルパス) をテキストボックスに表示
                txtCSVFile.Text = dialog.FileName;
            }
        }

        /// <summary>
        /// 【２】Fieldの1番目と3番目の値をCSV出力する ボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnProc2_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (File.Exists(txtFileName.Text))
                {
                    SortedDictionary<String, string> items = new SortedDictionary<string, string>();

                    StreamReader sr = new StreamReader(txtFileName.Text);

                    string strbuf = "";         // ファイル読み込みバッファ
                    bool IsEntity = false;      // [Entity]判断フラグ
                    while (sr.EndOfStream == false)
                    {
                        strbuf = sr.ReadLine();

                        // ブロックを判定
                        if (strbuf != "" && strbuf[0] == '[')
                        {
                            IsEntity = false;
                            if (strbuf.IndexOf("[Entity]") != -1)
                            {
                                IsEntity = true;
                            }
                        }
                        //-----------------------------------------------------
                        // [Entity]の場合
                        //-----------------------------------------------------
                        if (IsEntity)
                        {
                            // Fieldの場合
                            if (strbuf.IndexOf("Field") != -1)
                            {
                                // Fieldの1番目と3番目の値が同じかチェック
                                var checkField = reg.Split(strbuf);
                                int pos = checkField[0].IndexOf("=");
                                var colname = checkField[0].Substring(pos+1);

                                String samekey = String.Format("{0},{1}", colname, checkField[2]);

                                // Fieldの1番目と3番目の値が同じ場合
                                if (!items.ContainsKey(samekey))
                                {
                                    items.Add(samekey, samekey);
                                }
                            }
                        }
                    }

                    // ファイルを閉じる
                    sr.Close();

                    // ファイル保存ダイアログを表示します。
                    String result = selectOutputFile("CSVファイル(*.csv)|*.csv");
                    if (String.IsNullOrEmpty(result))
                    {
                        // 終了します。
                        return;
                    }

                    // 出力ファイル名
                    StreamWriter sw = new StreamWriter(result, false, Encoding.UTF8);
                    foreach (var key in items.Keys)
                    {
                        sw.WriteLine(items[key]);
                    }
                    sw.Close();

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

        /// <summary>
        /// 【３】Fieldの3番目の箇所にFieldの1番目の値の先頭に"*"を付ける ボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnProc3_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (File.Exists(txtFileName.Text))
                {
                    // ファイル保存ダイアログを表示します。
                    String result = selectOutputFile("A5M2ファイル(*.a5er)|*.a5er");
                    if (String.IsNullOrEmpty(result))
                    {
                        // 終了します。
                        return;
                    }

                    StreamReader sr = new StreamReader(txtFileName.Text);

                    StreamWriter sw = new StreamWriter(result, false, Encoding.UTF8);

                    string strbuf = "";         // ファイル読み込みバッファ
                    bool IsEntity = false;      // [Entity]判断フラグ
                    while (sr.EndOfStream == false)
                    {
                        strbuf = sr.ReadLine();

                        // ブロックを判定
                        if (strbuf != "" && strbuf[0] == '[')
                        {
                            IsEntity = false;
                            if (strbuf.IndexOf("[Entity]") != -1)
                            {
                                IsEntity = true;
                            }
                        }
                        //-----------------------------------------------------
                        // [Entity]の場合
                        //-----------------------------------------------------
                        if (IsEntity)
                        {
                            // Fieldの場合
                            if (strbuf.IndexOf("Field") != -1)
                            {
                                // Fieldのデータ行を処理
                                while (sr.EndOfStream == false)
                                {
                                    // 文字列を分割して配列に格納
                                    var arrField = reg.Split(strbuf);

                                    // Fieldの1番目の値を取得
                                    int pos = arrField[0].IndexOf("=");
                                    var colname = arrField[0].Substring(pos + 1).Trim('\"');

                                    // Fieldの3番目の値を設定
                                    arrField[2] = "\"*" + colname + "\"";

                                    // 配列を結合して文字列にする
                                    string strCsvData = string.Join(",", arrField);

                                    sw.WriteLine(strCsvData);

                                    strbuf = sr.ReadLine();

                                    // Field以外なら抜ける
                                    if (strbuf.IndexOf("Field") == -1)
                                    {
                                        // 読み込んだ内容をそのまま書き出す
                                        sw.WriteLine(strbuf);
                                        break;
                                    }
                                }
                            }
                            // Field以外の場合
                            else
                            {
                                // 読み込んだ内容をそのまま書き出す
                                sw.WriteLine(strbuf);
                            }
                        }
                        //-----------------------------------------------------
                        // [Entity]以外の場合
                        //-----------------------------------------------------
                        else
                        {
                            // 読み込んだ内容をそのまま書き出す
                            sw.WriteLine(strbuf);
                        }
                    }

                    // ファイルを閉じる
                    sw.Close();
                    sr.Close();

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

        /// <summary>
        /// 【４】CSVファイルを読込み、[Manager]の枠に情報を追加する ボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnProc4_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (File.Exists(txtFileName.Text) && File.Exists(txtCSVFile.Text))
                {
                    Dictionary<string, string> domains = new Dictionary<string, string>();

                    // ドメインを追加する
                    StreamReader srCSV = new StreamReader(txtCSVFile.Text);
                    string strbufCSV = "";      // CSVファイル読み込みバッファ
                    try
                    {
                        // CSVファイルのデータを処理
                        while (srCSV.EndOfStream == false)
                        {
                            strbufCSV = srCSV.ReadLine();

                            // 文字列を分割して配列に格納
                            var arrField = reg.Split(strbufCSV);

                            domains.Add(arrField[0].Trim('\"'), strbufCSV);
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

                    // ファイル保存ダイアログを表示します。
                    String result = selectOutputFile("A5M2ファイル(*.a5er)|*.a5er");
                    if (String.IsNullOrEmpty(result))
                    {
                        // 終了します。
                        return;
                    }

                    StreamReader sr = new StreamReader(txtFileName.Text);

                    StreamWriter sw = new StreamWriter(result, false, Encoding.UTF8);

                    string strbuf = "";         // ファイル読み込みバッファ
                    bool IsManager = false;     // [Manager]判断フラグ
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
                        }
                        //-----------------------------------------------------
                        // [Manager]の場合
                        //-----------------------------------------------------
                        if (IsManager)
                        {
                            // [Manager]のデータ行を処理
                            while (sr.EndOfStream == false)
                            {
                                // DecodeDomainの場合
                                if (strbuf.IndexOf("DecodeDomain=") != -1)
                                {
                                    // 読み込んだ内容をそのまま書き出す
                                    sw.WriteLine(strbuf);
                                }
                                // Domainの場合
                                else if (strbuf.IndexOf("Domain=") != -1 || strbuf.IndexOf("DomainInfo=") != -1)
                                {
                                    // 既に存在するDomain、DomainInfoは削除する
                                }
                                // Domain以外の場合
                                else
                                {
                                    // 読み込んだ内容をそのまま書き出す
                                    sw.WriteLine(strbuf);
                                }

                                strbuf = sr.ReadLine();

                                // 次のブロックが来たら抜ける
                                if (strbuf != "" && strbuf[0] == '[')
                                {
                                    IsManager = false;
                                    break;
                                }
                            }
                            // ドメインを追加する
                            foreach (var domain_name in domains.Keys)
                            {
                                // 文字列を分割して配列に格納
                                var arrField = reg.Split(domains[domain_name]);

                                // Domainを追加
                                string addline = String.Format("Domain={0}={1}",
                                    arrField[0].Trim('\"'), arrField[1].Trim('\"'));
                                sw.WriteLine(addline);

                                // DomainInfoを追加
                                addline = String.Format("DomainInfo=\"{0}\",\"{1}\",\"{2}\",\"\"",
                                    arrField[0].Trim('\"'), arrField[1].Trim('\"'), arrField[2].Trim('\"'));
                                sw.WriteLine(addline);
                            }

                            // CSVファイルを閉じる
                            srCSV.Close();

                            // 読み込んだ内容をそのまま書き出す
                            sw.WriteLine(strbuf);
                        }
                        else if (strbuf.IndexOf("Field=") == 0)
                        {
                            var fields = reg.Split(strbuf);
                            String domain_name = fields[0].Replace("Field=", "").Trim('\"');

                            if (domains.ContainsKey(domain_name))
                            {
                                var domain = reg.Split(domains[domain_name]);

                                // 物理名変更
                                fields[1] = "\"" + domain[2].Trim('\"') + "\"";

                                sw.WriteLine(string.Join(",", fields));
                            }
                            else
                            {
                                // 読み込んだ内容をそのまま書き出す
                                sw.WriteLine(strbuf);
                            }
                        }
                        //-----------------------------------------------------
                        // [Manager]以外の場合
                        //-----------------------------------------------------
                        else
                        {
                            // 読み込んだ内容をそのまま書き出す
                            sw.WriteLine(strbuf);
                        }
                    }

                    // ファイルを閉じる
                    sw.Close();
                    sr.Close();

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

        /// <summary>
        /// A5M2の定義をObjectBrowser用定義に変換する ボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnProc5_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (File.Exists(txtA5FileName.Text) && File.Exists(txtOBFileName.Text))
                {
                    List<TableDAO> datas = new List<TableDAO>();

                    StreamReader sr = new StreamReader(txtA5FileName.Text);

                    string strbuf = "";         // ファイル読み込みバッファ
                    bool IsEntity = false;      // [Entity]判断フラグ

                    TableDAO tableDao = null;

                    while (sr.EndOfStream == false)
                    {
                        strbuf = sr.ReadLine();

                        // ブロックを判定
                        if (strbuf != "" && strbuf[0] == '[')
                        {
                            IsEntity = false;
                            if (strbuf.IndexOf("[Entity]") != -1)
                            {
                                IsEntity = true;
                                tableDao = new TableDAO();
                                datas.Add(tableDao);
                            }
                        }
                        //-----------------------------------------------------
                        // [Entity]の場合
                        //-----------------------------------------------------
                        if (IsEntity)
                        {
                            if (strbuf.IndexOf("PName=") == 0)
                            {
                                // テーブル名を取得
                                tableDao.Table.Name = strbuf.Replace("PName=", "");
                            }
                            else if (strbuf.IndexOf("LName=") == 0)
                            {
                                // テーブル名を取得
                                tableDao.Table.TableName = strbuf.Replace("LName=", "");
                            }
                            else if (strbuf.IndexOf("Comment=") == 0)
                            {
                                // コメントを取得
                                tableDao.Table.Comment = strbuf.Replace("Comment=","");
                            }
                            else if (strbuf.IndexOf("Field=") == 0)
                            {
                                var fields = reg.Split(strbuf.Replace("Field=", ""));

                                RecordEntity recordEntity = new RecordEntity()
                                {
                                    Name = fields[0].Trim('"'),
                                    ColName = fields[1].Trim('"'),
                                    Domain = fields[2].Trim('"'),
                                    DataType = "",
                                    PrimaryKey = fields[4].Trim('"'),
                                    Comment = fields[6].Trim('"'),
                                };
                                
                                tableDao.Records.Add(recordEntity);
                            }
                        }
                    }

                    // ファイルを閉じる
                    sr.Close();

                    //xmlファイルを指定する
                    XElement xml = XElement.Load(@txtOBFileName.Text);

                    var ents = xml.Elements("ENTITY").ToList();
                    foreach (var target in ents)
                    {
                        target.Remove();
                    }

                    var modelview = xml.Element("MODELVIEW");
                    foreach (var target in modelview.Elements("ENTITY").ToList())
                    {
                        target.Remove();
                    }

                    //メンバー情報分ループして、コンソールに表示
                    int count = 0;
                    int top = 100;
                    int left = 150;
                    foreach (TableDAO info in datas)
                    {
                        ++count;

                        // MODELVIEW
                        XElement modelroot =
                           new XElement("ENTITY");
                        modelroot.SetAttributeValue("ID", count.ToString());
                        modelroot.SetAttributeValue("LEFT", (left+(count%10*150)).ToString());
                        modelroot.SetAttributeValue("TOP",(top+(count/10*200)).ToString());
                        modelroot.SetAttributeValue("RIGHT","276");
                        modelroot.SetAttributeValue("BOTTOM","200");
                        modelroot.SetAttributeValue("BRUSHCOLOR"," - 16777211");
                        modelroot.SetAttributeValue("PENCOLOR"," - 16777208");
                        modelroot.SetAttributeValue("NAMEFONTNAME","ＭＳ Ｐゴシック");
                        modelroot.SetAttributeValue("NAMEFONTSIZE","9");
                        modelroot.SetAttributeValue("NAMEFONTCOLOR"," - 16777208");
                        modelroot.SetAttributeValue("NAMEFONTBL","0");
                        modelroot.SetAttributeValue("NAMEFONTIT","0");
                        modelroot.SetAttributeValue("NAMEFONTUL","0");
                        modelroot.SetAttributeValue("NAMEFONTSO","0");
                        modelroot.SetAttributeValue("ATTRFONTNAME","ＭＳ Ｐゴシック");
                        modelroot.SetAttributeValue("ATTRFONTSIZE","9");
                        modelroot.SetAttributeValue("ATTRFONTCOLOR"," - 16777208");
                        modelroot.SetAttributeValue("ATTRFONTBL","0");
                        modelroot.SetAttributeValue("ATTRFONTIT","0");
                        modelroot.SetAttributeValue("ATTRFONTUL","0");
                        modelroot.SetAttributeValue("ATTRFONTSO","0");
                        modelroot.SetAttributeValue("FKFONTNAME","ＭＳ Ｐゴシック");
                        modelroot.SetAttributeValue("FKFONTSIZE","9");
                        modelroot.SetAttributeValue("FKFONTCOLOR"," - 16777208");
                        modelroot.SetAttributeValue("FKFONTBL","1");
                        modelroot.SetAttributeValue("FKFONTIT","0");
                        modelroot.SetAttributeValue("FKFONTUL","0");
                        modelroot.SetAttributeValue("FKFONTSO","0");

                        modelview.Add(modelroot);

                        // ENTITY
                        XElement root =
                            new XElement("ENTITY");
                        root.SetAttributeValue("ID", count.ToString());
                        root.SetAttributeValue("L-NAME", info.Table.TableName);
                        root.SetAttributeValue("DEPENDENT", "");
                        root.SetAttributeValue("P-NAME", info.Table.Name);
                        root.SetAttributeValue("SCHEMA", "");
                        root.SetAttributeValue("DBMAPPINGID", "0");
                        root.SetAttributeValue("COMMENT", info.Table.Comment);
                        root.SetAttributeValue("SHOWTYPE", "0");
                        root.SetAttributeValue("PRE-SQL", "");
                        root.SetAttributeValue("POST-SQL", "");
                        root.SetAttributeValue("ENGINE", "");
                        root.SetAttributeValue("EstInit", "0");
                        root.SetAttributeValue("EstInc", "0");
                        root.SetAttributeValue("MIGSCHEMA", "");
                        root.SetAttributeValue("MIGTABLENAME", "");

                        int colcount = 0;
                        foreach (RecordEntity rec in info.Records)
                        {
                            XElement recent =
                                new XElement("ATTR");
                            recent.SetAttributeValue("ID", (++colcount).ToString());
                            recent.SetAttributeValue("L-NAME", rec.ColName);
                            recent.SetAttributeValue("P-NAME", rec.Name);
                            recent.SetAttributeValue("DDOMAINID", rec.Domain);
                            recent.SetAttributeValue("DATATYPE", "");
                            recent.SetAttributeValue("LENGTH", "");
                            recent.SetAttributeValue("SCALE", "0");
                            recent.SetAttributeValue("NULL", "");
                            recent.SetAttributeValue("DEFID", "0");
                            recent.SetAttributeValue("DEF", "");
                            recent.SetAttributeValue("RULEID", "0");
                            recent.SetAttributeValue("RULE", "");
                            recent.SetAttributeValue("CODEDEFINEID", "0");
                            recent.SetAttributeValue("COMMENT", rec.Comment);
                            recent.SetAttributeValue("COLLATE", "");
                            recent.SetAttributeValue("PK", rec.PrimaryKey);
                            recent.SetAttributeValue("FKCRT", "0");
                            recent.SetAttributeValue("COLUMNTYPE", "0");
                            recent.SetAttributeValue("MIGSETTYPE", "0");
                            recent.SetAttributeValue("MIGSETPARAM", "");

                            root.Add(recent);
                        }

                        xml.Add(root);
                    }


                    // ファイル保存ダイアログを表示します。
                    String result = selectOutputFile("ObjectBrowserファイル (*.edm)|*.edm");
                    if (String.IsNullOrEmpty(result))
                    {
                        // 終了します。
                        return;
                    }

                    FileStream fs = new FileStream(result, FileMode.Create);
                    xml.Save(fs);
                    fs.Close();
                    fs.Dispose();

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

        private void btnSelectA5File_Click(object sender, RoutedEventArgs e)
        {
            // ダイアログのインスタンスを生成
            var dialog = new OpenFileDialog();

            // ファイルの種類を設定
            dialog.Filter = "a5erファイル (*.a5er)|*.a5er";

            // ダイアログを表示する
            if (dialog.ShowDialog() == true)
            {
                // 選択されたファイル名 (ファイルパス) をテキストボックスに表示
                txtA5FileName.Text = dialog.FileName;
            }
        }

        private void btnSelectOBFile_Click(object sender, RoutedEventArgs e)
        {
            // ダイアログのインスタンスを生成
            var dialog = new OpenFileDialog();

            // ファイルの種類を設定
            dialog.Filter = "ObjectBrowserファイル (*.edm)|*.edm";

            // ダイアログを表示する
            if (dialog.ShowDialog() == true)
            {
                // 選択されたファイル名 (ファイルパス) をテキストボックスに表示
                txtOBFileName.Text = dialog.FileName;
            }
        }

        private void btnProc6_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (File.Exists(txtDomainCSVFileName.Text) && File.Exists(txtDomainOBFileName.Text))
                {
                    StreamReader sr = new StreamReader(txtDomainCSVFileName.Text);

                    string strbuf = "";         // ファイル読み込みバッファ

                    List<String> domains = new List<string>();
                    
                    while (sr.EndOfStream == false)
                    {
                        strbuf = sr.ReadLine();

                        domains.Add(strbuf);
                    }

                    // ファイルを閉じる
                    sr.Close();

                    //xmlファイルを指定する
                    XElement xml = XElement.Load(@txtDomainOBFileName.Text);

                    var ents = xml.Elements("DOMAIN").ToList();
                    foreach (var target in ents)
                    {
                        target.Remove();
                    }

                    //メンバー情報分ループして、コンソールに表示
                    int count = 0;
                    foreach (String info in domains)
                    {
                        String[] domain = info.Split(",");
                        String[] length = domain[3].Split(".");

                        ++count;

                        // DOMAIN
                        XElement root =
                           new XElement("DOMAIN");

                        root.SetAttributeValue("ID", count.ToString());
                        root.SetAttributeValue("L-NAME", domain[0]);
                        root.SetAttributeValue("P-NAME", domain[1]);
                        root.SetAttributeValue("DATATYPE", domain[2]);
                        root.SetAttributeValue("LENGTH", length[0]);
                        if (length.Length > 1)
                        {
                            root.SetAttributeValue("SCALE", length[1]);
                        }
                        else
                        {
                            root.SetAttributeValue("SCALE", "0");
                        }
                        root.SetAttributeValue("NULL","0" );
                        root.SetAttributeValue("DEFID","0" );
                        root.SetAttributeValue("DEF","" );
                        root.SetAttributeValue("RULEID","0");
                        root.SetAttributeValue("CODEDEFINEID","0"); 
                        root.SetAttributeValue("RULE","");
                        root.SetAttributeValue("COMMENT",""); 
                        root.SetAttributeValue("GROUP","0");

                        xml.Add(root);
                    }


                    // ファイル保存ダイアログを表示します。
                    String result = selectOutputFile("ObjectBrowserファイル (*.edm)|*.edm");
                    if (String.IsNullOrEmpty(result))
                    {
                        // 終了します。
                        return;
                    }

                    FileStream fs = new FileStream(result, FileMode.Create);
                    xml.Save(fs);
                    fs.Close();
                    fs.Dispose();

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

        private void btnSelectDomanCSVFile_Click(object sender, RoutedEventArgs e)
        {
            // ダイアログのインスタンスを生成
            var dialog = new OpenFileDialog();

            // ファイルの種類を設定
            dialog.Filter = "CSVファイル (*.csv)|*.csv";

            // ダイアログを表示する
            if (dialog.ShowDialog() == true)
            {
                // 選択されたファイル名 (ファイルパス) をテキストボックスに表示
                txtDomainCSVFileName.Text = dialog.FileName;
            }
        }

        private void btnSelectDomainOBFile_Click(object sender, RoutedEventArgs e)
        {
            // ダイアログのインスタンスを生成
            var dialog = new OpenFileDialog();

            // ファイルの種類を設定
            dialog.Filter = "ObjectBrowserファイル (*.edm)|*.edm";

            // ダイアログを表示する
            if (dialog.ShowDialog() == true)
            {
                // 選択されたファイル名 (ファイルパス) をテキストボックスに表示
                txtDomainOBFileName.Text = dialog.FileName;
            }
        }

        private void btnProc7_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (File.Exists(txtFileName.Text) && File.Exists(txtCSVFile.Text))
                {
                    // ファイル保存ダイアログを表示します。
                    String result = selectOutputFile("A5M2ファイル(*.a5er)|*.a5er");
                    if (String.IsNullOrEmpty(result))
                    {
                        // 終了します。
                        return;
                    }

                    // テーブル情報を取得
                    StreamReader srCSV = new StreamReader(txtCSVFile.Text);
                    string strbufCSV = "";      // CSVファイル読み込みバッファ

                    Dictionary<string, string> tables = new Dictionary<string, string>();

                    try
                    {
                        // CSVファイルのデータを処理
                        while (srCSV.EndOfStream == false)
                        {
                            strbufCSV = srCSV.ReadLine().Replace("\"","");

                            // 文字列を分割して配列に格納
                            var arrField = strbufCSV.Split(',');

                            tables.Add(arrField[0], arrField[1]);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        // CSVファイルを閉じる
                        srCSV.Close();
                    }

                    StreamReader sr = new StreamReader(txtFileName.Text);

                    StreamWriter sw = new StreamWriter(result, false, Encoding.UTF8);

                    string strbuf = "";         // ファイル読み込みバッファ

                    while (sr.EndOfStream == false)
                    {
                        strbuf = sr.ReadLine();

                        if (strbuf.IndexOf("PName=") != -1)
                        {
                            String pname = strbuf.Substring(6);
                            if (tables.ContainsKey(pname))
                            {
                                sw.WriteLine("PName=" + tables[pname]);
                            }
                            else
                            {
                                sw.WriteLine(strbuf);
                            }
                        }
                        else if (strbuf.IndexOf("Entity1=") != -1)
                        {
                            String pname = strbuf.Substring(8);
                            if (tables.ContainsKey(pname))
                            {
                                sw.WriteLine("Entity1=" + tables[pname]);
                            }
                            else
                            {
                                sw.WriteLine(strbuf);
                            }
                        }
                        else if (strbuf.IndexOf("Entity2=") != -1)
                        {
                            String pname = strbuf.Substring(8);
                            if (tables.ContainsKey(pname))
                            {
                                sw.WriteLine("Entity2=" + tables[pname]);
                            }
                            else
                            {
                                sw.WriteLine(strbuf);
                            }
                        }
                        //-----------------------------------------------------
                        // [Entity]以外の場合
                        //-----------------------------------------------------
                        else
                        {
                            // 読み込んだ内容をそのまま書き出す
                            sw.WriteLine(strbuf);
                        }
                    }

                    // ファイルを閉じる
                    sw.Close();
                    sr.Close();

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

        private void btnProc4_2_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (File.Exists(txtFileName.Text) && File.Exists(txtCSVFile.Text))
                {
                    Dictionary<string, string> domains = new Dictionary<string, string>();

                    // ドメインを追加する
                    StreamReader srCSV = new StreamReader(txtCSVFile.Text);
                    string strbufCSV = "";      // CSVファイル読み込みバッファ
                    try
                    {
                        // CSVファイルのデータを処理
                        while (srCSV.EndOfStream == false)
                        {
                            strbufCSV = srCSV.ReadLine();

                            // 文字列を分割して配列に格納
                            var arrField = reg.Split(strbufCSV);

                            domains.Add(arrField[0].Trim('\"'), strbufCSV);
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

                    // ファイル保存ダイアログを表示します。
                    String result = selectOutputFile("A5M2ファイル(*.a5er)|*.a5er");
                    if (String.IsNullOrEmpty(result))
                    {
                        // 終了します。
                        return;
                    }

                    StreamReader sr = new StreamReader(txtFileName.Text);

                    StreamWriter sw = new StreamWriter(result, false, Encoding.UTF8);

                    string strbuf = "";         // ファイル読み込みバッファ
                    while (sr.EndOfStream == false)
                    {
                        strbuf = sr.ReadLine();

                        if (strbuf.IndexOf("Field=") == 0)
                        {
                            var fields = reg.Split(strbuf);
                            String domain_name = fields[0].Replace("Field=", "").Trim('\"');

                            if (domains.ContainsKey(domain_name))
                            {
                                var domain = reg.Split(domains[domain_name]);

                                // 物理名変更
                                fields[1] = "\"" + domain[2].Trim('\"') + "\"";

                                sw.WriteLine(string.Join(",", fields));
                            }
                            else
                            {
                                // 読み込んだ内容をそのまま書き出す
                                sw.WriteLine(strbuf);
                            }
                        }
                        else
                        {
                            // 読み込んだ内容をそのまま書き出す
                            sw.WriteLine(strbuf);
                        }
                    }

                    // ファイルを閉じる
                    sw.Close();
                    sr.Close();

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

        private void btnProc8_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var changedata = new Dictionary<String, String>();
                changedata.Add("SAP", "Sap");
                changedata.Add("MES", "Mes");
                changedata.Add("OPC", "Opc");
                changedata.Add("DCS", "Dcs");
                changedata.Add("PCN", "Pcn");
                changedata.Add("TAG", "Tag");
                changedata.Add("SEQ", "Seq");
                changedata.Add("ASTPLANNNER", "Astplanner");


                if (File.Exists(txtFileName.Text))
                {
                    // ファイル保存ダイアログを表示します。
                    String result = selectOutputFile("A5M2ファイル(*.a5er)|*.a5er");
                    if (String.IsNullOrEmpty(result))
                    {
                        // 終了します。
                        return;
                    }

                    StreamReader sr = new StreamReader(txtFileName.Text);

                    StreamWriter sw = new StreamWriter(result, false, Encoding.UTF8);

                    string strbuf = "";         // ファイル読み込みバッファ
                    bool IsEntity = false;      // [Entity]判断フラグ
                    bool bSkip = false;
                    while (sr.EndOfStream == false)
                    {
                        strbuf = sr.ReadLine();

                        // ブロックを判定
                        if (strbuf != "" && strbuf[0] == '[')
                        {
                            IsEntity = false;
                            if (strbuf.IndexOf("[Entity]") != -1)
                            {
                                IsEntity = true;
                            }
                        }
                        else if (strbuf.IndexOf("DomainInfo=") != -1)
                        {
                            // 文字列を分割して配列に格納
                            var arrField = reg.Split(strbuf);
                            if (arrField.Length < 3)
                            {
                                sw.WriteLine(strbuf);
                                continue;
                            }

                            var fieldname = arrField[2].Trim('\"');
                            if (string.IsNullOrEmpty(fieldname))
                            {
                                sw.WriteLine(strbuf);
                                continue;
                            }

                            // 値変換
                            foreach (var key in changedata.Keys)
                            {
                                fieldname = fieldname.Replace(key, changedata[key]);
                            }
                            fieldname = fieldname.Substring(0, 1).ToLower() + fieldname.Substring(1);

                            // Fieldの2番目の値を設定
                            arrField[2] = "\"" + fieldname + "\"";

                            // 配列を結合して文字列にする
                            string strCsvData = string.Join(",", arrField);

                            sw.WriteLine(strCsvData);

                            continue;
                        }

                        //-----------------------------------------------------
                        // [Entity]の場合
                        //-----------------------------------------------------
                        if (IsEntity)
                        {
                            if (strbuf.IndexOf("Page=") != -1)
                            {
                                bSkip = (strbuf.IndexOf("Page=マスタ（基盤）") != -1
                                    || strbuf.IndexOf("Page=システム（基盤）") != -1
                                    || strbuf.IndexOf("Page=履歴") != -1);
                            }
                            // Fieldの場合
                            if (strbuf.IndexOf("Field") != -1 && !bSkip)
                            {
                                // Fieldのデータ行を処理
                                while (sr.EndOfStream == false)
                                {
                                    // 文字列を分割して配列に格納
                                    var arrField = reg.Split(strbuf);

                                    // Fieldの1番目の値を取得
                                    int pos = arrField[0].IndexOf("=");
                                    var colname = arrField[0].Substring(pos + 1).Trim('\"');
                                    var fieldname = arrField[1].Trim('\"');

                                    // 値変換
                                    foreach (var key in changedata.Keys){
                                        fieldname = fieldname.Replace(key, changedata[key]);
                                    }
                                    fieldname = fieldname.Substring(0, 1).ToLower() + fieldname.Substring(1);

                                    // Fieldの2番目の値を設定
                                    arrField[1] = "\"" + fieldname + "\"";

                                    // 配列を結合して文字列にする
                                    string strCsvData = string.Join(",", arrField);

                                    sw.WriteLine(strCsvData);

                                    strbuf = sr.ReadLine();

                                    // Field以外なら抜ける
                                    if (strbuf.IndexOf("Field") == -1)
                                    {
                                        // 読み込んだ内容をそのまま書き出す
                                        sw.WriteLine(strbuf);
                                        break;
                                    }
                                }
                            }
                            // Field以外の場合
                            else
                            {
                                // 読み込んだ内容をそのまま書き出す
                                sw.WriteLine(strbuf);
                            }
                        }
                        //-----------------------------------------------------
                        // [Entity]以外の場合
                        //-----------------------------------------------------
                        else
                        {
                            // 読み込んだ内容をそのまま書き出す
                            sw.WriteLine(strbuf);
                        }
                    }

                    // ファイルを閉じる
                    sw.Close();
                    sr.Close();

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

        private void btnProc9_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (File.Exists(txtFileName.Text))
                {
                    // ファイル保存ダイアログを表示します。
                    String result = selectOutputFile("A5M2ファイル(*.a5er)|*.a5er");
                    if (String.IsNullOrEmpty(result))
                    {
                        // 終了します。
                        return;
                    }

                    Dictionary<string, string> tables = new Dictionary<string, string>();

                    StreamReader sr_st = new StreamReader(txtFileName.Text);

                    string strbuf = "";         // ファイル読み込みバッファ
                    string TableName = "";      // テーブル名
                    bool IsEntity = false;      // [Entity]判断フラグ
                    while (sr_st.EndOfStream == false)
                    {
                        strbuf = sr_st.ReadLine();

                        // ブロックを判定
                        if (strbuf != "" && strbuf[0] == '[')
                        {
                            IsEntity = false;
                            if (strbuf.IndexOf("[Entity]") != -1)
                            {
                                IsEntity = true;
                                TableName = "";
                            }
                        }
                        if (IsEntity)
                        {
                            // テーブル名を取得
                            if (strbuf.IndexOf("LName") != -1)
                            {
                                TableName = strbuf.Substring(6);
                            }
                            // ブロックを判定
                            if (strbuf.IndexOf("Tag=") > -1)
                            {
                                var tagname = strbuf.Substring(4);
                                tables.Add(TableName, tagname);
                            }
                        }
                    }
                    sr_st.Close();

                    StreamReader sr = new StreamReader(txtFileName.Text);

                    StreamWriter sw = new StreamWriter(result, false, Encoding.UTF8);

                    IsEntity = false;      // [Entity]判断フラグ
                    while (sr.EndOfStream == false)
                    {
                        strbuf = sr.ReadLine();

                        // ブロックを判定
                        if (strbuf != "" && strbuf[0] == '[')
                        {
                            IsEntity = false;
                            if (strbuf.IndexOf("[Entity]") != -1)
                            {
                                IsEntity = true;
                                TableName = "";
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
                            }
                            // Fieldの場合
                            if (strbuf.IndexOf("Field") != -1)
                            {
                                bool IsExistKey = false;        // 存在フラグ

                                // Fieldのデータ行を処理
                                while (sr.EndOfStream == false)
                                {
                                    if (!tables.ContainsKey(TableName) ||
                                        (tables.ContainsKey(TableName) &&
                                        tables[TableName] != "マスタ" &&
                                        tables[TableName] != "マスタ（基盤）" &&
                                        tables[TableName] != "システム（基盤）"))
                                    {
                                        // 文字列を分割して配列に格納
                                        var arrField = reg.Split(strbuf);

                                        // キー項目の場合
                                        if (arrField[0].Equals("削除フラグ"))
                                        {
                                            IsExistKey = true;
                                        }
                                    }
                                    else
                                    {
                                        IsExistKey = true;
                                    }

                                    // 読み込んだ内容をそのまま書き出す
                                    sw.WriteLine(strbuf);

                                    strbuf = sr.ReadLine();

                                    // Field以外なら抜ける
                                    if (strbuf.IndexOf("Field") == -1)
                                    {
                                        // 削除フラグ追加
                                        if (!IsExistKey)
                                        {
                                            string addline = String.Format("Field=\"削除フラグ\",\"*削除フラグ\",\"NUMERIC(1)\",,,\"\",\"\",$FFFFFFFF,\"\"");
                                            sw.WriteLine(addline);
                                        }

                                        // 読み込んだ内容をそのまま書き出す
                                        sw.WriteLine(strbuf);

                                        break;
                                    }
                                }
                            }
                            // Field以外の場合
                            else
                            {
                                // 読み込んだ内容をそのまま書き出す
                                sw.WriteLine(strbuf);
                            }
                        }
                        //-----------------------------------------------------
                        // [Entity]以外の場合
                        //-----------------------------------------------------
                        else
                        {
                            // 読み込んだ内容をそのまま書き出す
                            sw.WriteLine(strbuf);
                        }
                    }

                    // ファイルを閉じる
                    sw.Close();
                    sr.Close();

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

        private void btnProc10_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (File.Exists(txtFileName.Text) && File.Exists(txtCSVFile.Text))
                {
                    Dictionary<string, string> indexes = new Dictionary<string, string>();

                    // 制約を追加する
                    StreamReader srCSV = new StreamReader(txtCSVFile.Text);
                    string strbufCSV = "";      // CSVファイル読み込みバッファ
                    try
                    {
                        // CSVファイルのデータを処理
                        while (srCSV.EndOfStream == false)
                        {
                            strbufCSV = srCSV.ReadLine();

                            // 文字列を分割して配列に格納
                            var arrField = reg.Split(strbufCSV);

                            indexes.Add(arrField[0].Trim('\"'), strbufCSV);
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

                    // ファイル保存ダイアログを表示します。
                    String result = selectOutputFile("A5M2ファイル(*.a5er)|*.a5er");
                    if (String.IsNullOrEmpty(result))
                    {
                        // 終了します。
                        return;
                    }

                    StreamReader sr = new StreamReader(txtFileName.Text);

                    StreamWriter sw = new StreamWriter(result, false, Encoding.UTF8);

                    string strbuf = "";         // ファイル読み込みバッファ
                    string TableName = "";
                    bool IsEntity = false;     // [Entity]判断フラグ                   
                    Dictionary<string, string> items = new Dictionary<string, string>();
                    while (sr.EndOfStream == false)
                    {
                        strbuf = sr.ReadLine();

                        // ブロックを判定
                        if (strbuf != "" && strbuf[0] == '[')
                        {
                            IsEntity = false;
                            if (strbuf.IndexOf("[Entity]") != -1)
                            {
                                IsEntity = true;
                                TableName = "";
                            }
                        }
                        //-----------------------------------------------------
                        // [Entity]の場合
                        //-----------------------------------------------------
                        if (IsEntity)
                        {
                            // テーブル名を取得
                            if (strbuf.IndexOf("PName") != -1)
                            {
                                TableName = strbuf.Substring(6);

                                items.Clear();
                                if (indexes.ContainsKey(TableName))
                                {
                                    String[] datas = reg.Split(indexes[TableName]);
                                    for (int i=2; i<datas.Length; i++)
                                    {
                                        items.Add(datas[i], datas[i]);
                                    }
                                }
                            }

                            // Fieldの場合
                            if (strbuf.IndexOf("Field") != -1)
                            {
                                var checkField = reg.Split(strbuf.Replace("Field=",""));
                                var colname = checkField[0].Trim('"');
                                var colname2 = checkField[1].Trim('"');

                                // 制約対象かチェック
                                if (colname.IndexOf("nk") != 0 && items.ContainsKey(colname2))
                                {
                                    // 論理名にnkを追加
                                    sw.WriteLine(strbuf.Replace("\""+colname+"\"","\"nk"+colname+"\""));
                                }
                                else
                                {
                                    // 読み込んだ内容をそのまま書き出す
                                    sw.WriteLine(strbuf);
                                }
                            }
                            // EffectModeの場合
                            else if (strbuf.IndexOf("EffectMode") != -1)
                            {
                                // 制約を出力
                                if (items.Count > 0)
                                {
                                    var outdata = string.Format("Index={0}={1},{2}", TableName + "NK", "1", string.Join(",", items.Values));
                                    sw.WriteLine(outdata);
                                }
                                sw.WriteLine(strbuf);
                            }
                            // Field以外の場合
                            else
                            {
                                // 読み込んだ内容をそのまま書き出す
                                sw.WriteLine(strbuf);
                            }
                        }
                        //-----------------------------------------------------
                        // [Entity]以外の場合
                        //-----------------------------------------------------
                        else
                        {
                            // 読み込んだ内容をそのまま書き出す
                            sw.WriteLine(strbuf);
                        }
                    }

                    // ファイルを閉じる
                    sw.Close();
                    sr.Close();

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
                                var arrField = reg.Split(strbufCSV);

                                if (arrField.Length == 3 || (arrField.Length == 4 && string.IsNullOrEmpty(arrField[3])))
                                {
                                    if (arrField[0].Trim('\"') == "D")
                                    {
                                        if (!dropitems.ContainsKey(arrField[1].Trim('\"')))
                                        {
                                            dropitems.Add(arrField[1].Trim('\"'), new List<string>());
                                        }
                                        dropitems[arrField[1].Trim('\"')].Add(arrField[2]);
                                    }
                                    else
                                    {
                                        changeitems.Add(arrField[1].Trim('\"') + "," + arrField[2].Trim('\"'), arrField);
                                    }
                                }
                                else if (arrField.Length == 4)
                                {
                                    changeitems.Add(arrField[1].Trim('\"') + "," + arrField[3].Trim('\"'), arrField);
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
                                    var domainname = arrField[0].Replace("\"","");

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
                                    var domain = checkField[2].Trim('"').Replace("*","");
                                    var notnull = String.IsNullOrEmpty(checkField[3].Trim('"').Trim()) ? "" : (" " + checkField[3].Trim('"').Trim());
                                    var comment = checkField[6].Trim('"');

                                    if (changeitems.ContainsKey(TableName + "," + colname))
                                    {
                                        System.Diagnostics.Debug.Print("(1):" + TableName + " " + TableName2 + " " + colname + " " + colname2 + " " + domain);

                                        // 変更情報（論理名）
                                        var data = changeitems[TableName + "," + colname];

                                        if (data[0] == "A")
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
                                        }
                                        else if (data[0] == "U")
                                        {
                                            // 変更
                                            if (data.Length == 3 || (data.Length == 4 && string.IsNullOrEmpty(data[3])))
                                            {
                                                // ドメイン情報
                                                if (domains.ContainsKey(domain))
                                                {
                                                    string[] data2 = domains[domain];
                                                    var itemtype = data2[1].Trim('"');

                                                    sw.WriteLine("alter table " + TableName2 + " alter column " + colname2 + " " + itemtype + notnull + ";");
                                                    sw.WriteLine("execute sp_updateextendedproperty N'MS_Description', N'" + colname + ":" + comment + "', N'SCHEMA', N'dbo', N'TABLE', N'" + TableName2 + "', N'COLUMN', N'" + colname2 + "';");
                                                }
                                            }
                                            else if (data.Length == 4)
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

                                        if (data[0] == "A")
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
                                        }
                                        else if (data[0] == "U")
                                        {
                                            if (data.Length == 3 || (data.Length == 4 && string.IsNullOrEmpty(data[3])))
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
                                            }
                                            else if (data.Length == 4)
                                            {
                                                // ドメイン情報
                                                if (domains.ContainsKey(domain))
                                                {
                                                    string[] data2 = domains[domain];
                                                    var itemtype = data2[1].Trim('"');

                                                    // 変更（物理名）
                                                    sw.WriteLine("alter table " + TableName2 + " add " + colname2 + " " + itemtype + notnull + ";");
                                                    sw.WriteLine("execute sp_addextendedproperty N'MS_Description', N'" + colname + ":" + comment + "', N'SCHEMA', N'dbo', N'TABLE', N'" + TableName2 + "', N'COLUMN', N'" + colname2 + "';");
                                                    sw.WriteLine("update " + TableName2 + " set " + colname2 + "=" + data[2] + ";");
                                                    sw.WriteLine("alter table " + TableName2 + " drop column " + data[2] + ";");
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

        private void btnProc12_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int zorder = 0;

                if (File.Exists(txtFileName.Text))
                {
                    // ファイル保存ダイアログを表示します。
                    String result = selectOutputFile("ファイル (*.a5er)|*.a5er");
                    if (String.IsNullOrEmpty(result))
                    {
                        // 終了します。
                        return;
                    }

                    List<TableDAO> datas = new List<TableDAO>();

                    StreamReader sr = new StreamReader(txtFileName.Text);

                    StreamWriter sw = new StreamWriter(result, false, Encoding.UTF8);


                    string strbuf = "";         // ファイル読み込みバッファ
                    bool IsEntity = false;      // [Entity]判断フラグ

                    TableDAO tableDao = null;

                    while (sr.EndOfStream == false)
                    {
                        strbuf = sr.ReadLine();
                        sw.WriteLine(strbuf);

                        if (strbuf.IndexOf("ZOrder=") == 0)
                        {
                            var z = int.Parse(strbuf.Replace("ZOrder=", ""));
                            if (zorder < z) zorder = z;
                        }

                        // ブロックを判定
                        if (strbuf != "" && strbuf[0] == '[')
                        {
                            IsEntity = false;
                            if (strbuf.IndexOf("[Entity]") != -1)
                            {
                                IsEntity = true;
                                tableDao = new TableDAO();
                                datas.Add(tableDao);
                            }
                        }
                        //-----------------------------------------------------
                        // [Entity]の場合
                        //-----------------------------------------------------
                        if (IsEntity)
                        {
                            if (strbuf.IndexOf("PName=") == 0)
                            {
                                // テーブル名を取得
                                tableDao.Table.Name = strbuf.Replace("PName=", "");
                            }
                            else if (strbuf.IndexOf("LName=") == 0)
                            {
                                // テーブル名を取得
                                tableDao.Table.TableName = strbuf.Replace("LName=", "");
                            }
                            else if (strbuf.IndexOf("Comment=") == 0)
                            {
                                // コメントを取得
                                tableDao.Table.Comment = strbuf.Replace("Comment=", "");
                            }
                            else if (strbuf.IndexOf("Page=") == 0)
                            {
                                // ページを取得
                                tableDao.Table.Page = strbuf.Replace("Page=", "");
                            }
                            else if (strbuf.IndexOf("Tag=") == 0)
                            {
                                // タグを取得
                                tableDao.Table.Tag = strbuf.Replace("Tag=", "");
                            }
                            else if (strbuf.IndexOf("Left=") == 0)
                            {
                                // LEFTを取得
                                tableDao.Table.Left = strbuf.Replace("Left=", "");
                            }
                            else if (strbuf.IndexOf("Top=") == 0)
                            {
                                // TOPを取得
                                tableDao.Table.Top = strbuf.Replace("Top=", "");
                            }
                            else if (strbuf.IndexOf("Field=") == 0)
                            {
                                var fields = reg.Split(strbuf.Replace("Field=", ""));

                                RecordEntity recordEntity = new RecordEntity()
                                {
                                    Name = fields[0].Trim('"'),
                                    ColName = fields[1].Trim('"'),
                                    Domain = fields[2].Trim('"'),
                                    IsNotNull = fields[3].Trim('"'),
                                    DataType = "",
                                    PrimaryKey = fields[4].Trim('"'),
                                    Comment = fields[6].Trim('"'),
                                };

                                tableDao.Records.Add(recordEntity);
                            }
                        }
                    }

                    // ファイルを閉じる
                    sr.Close();


                    String updateD = DateTime.Now.ToString("yyyyMMddHHmmss");

                    foreach (TableDAO info in datas)
                    {
                        if (!info.Table.Page.Equals("トランザクション（製造）")
                            && !info.Table.Page.Equals("トランザクション（在庫）")
                            && !info.Table.Page.Equals("トランザクション（計量）"))
                        {
                            continue;
                        }

                        sw.WriteLine("");
                        sw.WriteLine("[Entity]");
                        sw.WriteLine("PName=" + "h_" + info.Table.Name);
                        sw.WriteLine("LName=" + info.Table.TableName + "履歴");
                        sw.WriteLine("Comment=" + info.Table.Comment);
                        sw.WriteLine("TableOption=");
                        sw.WriteLine("Page=" + info.Table.Page.Replace("トランザクション","履歴"));
                        sw.WriteLine("Left=" + info.Table.Left);
                        sw.WriteLine("Top=" + info.Table.Top);

                        RecordEntity rec = info.Records[0];
                        sw.WriteLine(String.Format("Field=\"{0}\",\"{1}\",\"{2}\",{3},{4},\"\",\"{5}\",$FFFFFFFF,\"\"",
                                                    rec.Name, rec.ColName, rec.Domain, rec.IsNotNull, rec.PrimaryKey, rec.Comment));

                        sw.WriteLine("Field=\"履歴番号\",\"rev\",\"*履歴番号\",NOT NULL,1,\"\",\"【システム項目】履歴テーブルのレコード生成時に自動付与される。\",$FFFFFFFF,\"\"");
                        sw.WriteLine("Field=\"履歴区分\",\"revtype\",\"*履歴区分\",NOT NULL,,\"\",\"【システム項目】履歴テーブルに記録される変更の種類。　0:登録, 1:更新, 2:削除\",$FFFFFFFF,\"\"");

                        for (int i=1; i < info.Records.Count; i++)
                        {
                            rec = info.Records[i];

                            sw.WriteLine(String.Format("Field=\"{0}\",\"{1}\",\"{2}\",{3},{4},\"\",\"{5}\",$FFFFFFFF,\"\"",
                                                        rec.Name, rec.ColName, rec.Domain, rec.IsNotNull, rec.PrimaryKey, rec.Comment));
                        }

                        sw.WriteLine("EffectMode=Gradation");
                        sw.WriteLine("Tag=履歴（トランザクション）");
                        sw.WriteLine("Color=$000000");
                        sw.WriteLine("BkColor=$FFFFFF");
                        sw.WriteLine("ModifiedDateTime=" + updateD);

                        zorder++;
                        sw.WriteLine("ZOrder=" + zorder.ToString());
                    }

                    sw.Close();
                    sw.Dispose();

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
