﻿using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;      // ファイル選択ダイアログ

namespace AddColumnTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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

                    StreamWriter sw = new StreamWriter(OutFileName);

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
                                var checkField = strbuf.Split(',');
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
                                        var arrField = strbuf.Split(',');

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
                    // 出力ファイル名
                    int loc = txtFileName.Text.IndexOf("a5er");
                    string OutFileName = String.Format("{0}_Field１と３.csv",
                                                       txtFileName.Text[0..(loc-1)]);

                    StreamReader sr = new StreamReader(txtFileName.Text);

                    StreamWriter sw = new StreamWriter(OutFileName);

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
                                bool IsSameKey = false;        // 値比較フラグ
                                var checkField = strbuf.Split(',');
                                int pos = checkField[0].IndexOf("=");
                                var colname = checkField[0].Substring(pos+1);
                                if (colname.Equals(checkField[2])) IsSameKey = true;

                                // Fieldの1番目と3番目の値が同じ場合
                                if (IsSameKey)
                                {
                                    // Fieldの1番目と3番目の値を書き出す
                                    string addline = String.Format("{0},{1}",
                                        colname, checkField[2]);
                                    sw.WriteLine(addline);
                                }

                                // Fieldのデータ行を処理
                                while (sr.EndOfStream == false)
                                {
                                    // Fieldの1番目と3番目の値が同じ場合
                                    if (IsSameKey)
                                    {
                                        // 1レコードのみ出力する為、読み飛ばす
                                    }
                                    // Fieldの1番目と3番目の値が異なる場合
                                    else
                                    {
                                        // 文字列を分割して配列に格納
                                        var arrField = strbuf.Split(',');

                                        // Fieldの1番目の値を取得
                                        pos = arrField[0].IndexOf("=");

                                        // Fieldの1番目と3番目の値を書き出す
                                        string addline = String.Format("{0},{1}",
                                            arrField[0].Substring(pos + 1), arrField[2]);
                                        sw.WriteLine(addline);
                                    }

                                    strbuf = sr.ReadLine();

                                    // Field以外なら抜ける
                                    if (strbuf.IndexOf("Field") == -1)
                                    {
                                        // 出力せずに読み飛ばす
                                        break;
                                    }
                                }
                            }
                            // Field以外の場合
                            else
                            {
                                // 出力せずに読み飛ばす
                            }
                        }
                        //-----------------------------------------------------
                        // [Entity]以外の場合
                        //-----------------------------------------------------
                        else
                        {
                            // 出力せずに読み飛ばす
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
                    // 出力ファイル名
                    int loc = txtFileName.Text.IndexOf("a5er");
                    string OutFileName = txtFileName.Text.Insert(loc - 1, "_Field３設定");

                    StreamReader sr = new StreamReader(txtFileName.Text);

                    StreamWriter sw = new StreamWriter(OutFileName);

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
                                    var arrField = strbuf.Split(',');

                                    // Fieldの1番目の値を取得
                                    int pos = arrField[0].IndexOf("=");
                                    var colname = arrField[0].Substring(pos + 1);

                                    // Fieldの3番目の値を設定
                                    arrField[2] = colname.Insert(1, "*");

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

        }
    }
}
