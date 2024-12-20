﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Media;
using System.ComponentModel;
using System.Data;

namespace SearchSample
{
    /// <summary>
    /// データ抽出画面用クラス
    /// </summary>
    public class DataExtractClass
    {
        #region "DataGrid用クラス"
        /// <summary>
        /// 表示条件用クラス
        /// </summary>
        public class DataExtractConditionItem : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            private string colCondition;
            private string colItem_n;
            private string colConditional;
            private string colCon_value;
            private string colLogical_exp;
            private string colItem_code;

            // 条件ｎ
            public string ColCondition { get { return this.colCondition; } set { this.colCondition = value; this.OnPropertyChanged(nameof(ColCondition)); } }
            // 項目
            public string ColItem_n { get { return this.colItem_n; } set { this.colItem_n = value; this.OnPropertyChanged(nameof(ColItem_n)); } }
            // 条件式
            public string ColConditional { get { return this.colConditional; } set { this.colConditional = value; this.OnPropertyChanged(nameof(ColConditional)); } }
            // 値
            public string ColCon_value { get { return this.colCon_value; } set { this.colCon_value = value; this.OnPropertyChanged(nameof(ColCon_value)); } }
            // 論理式
            public string ColLogical_exp { get { return this.colLogical_exp; } set { this.colLogical_exp = value; this.OnPropertyChanged(nameof(ColLogical_exp)); } }
            // item_code
            public string ColItem_code { get { return this.colItem_code; } set { this.colItem_code = value; this.OnPropertyChanged(nameof(ColItem_code)); } }

            /// <summary>
            /// コンストラクター
            /// </summary>
            public DataExtractConditionItem()
            {
                colCondition = null;
                colItem_n = null;
                colConditional = null;
                colCon_value = null;
                colLogical_exp = null;
                colItem_code = null;
            }

            /// <summary>
            /// PropertyChanged イベント
            /// </summary>
            /// <param name="info"></param>
            public void OnPropertyChanged(string info)
            {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
            }
        }

        /// <summary>
        /// お気に入りリスト用クラス
        /// </summary>
        public class DataExtractFavoriteListItem
        {
            public event PropertyChangedEventHandler PropertyChanged;

            private string colFavorite_n;
            private bool colSelect;
            private bool colModify;
            private bool colDelete;
            private string colFavorite_id;
            private string colTable_n;

            // お気に入り
            public string ColFavorite_n { get { return this.colFavorite_n; } set { this.colFavorite_n = value; this.OnPropertyChanged(nameof(ColFavorite_n)); } }
            // 選択ボタン
            public bool ColSelect { get { return this.colSelect; } set { this.colSelect = value; } }
            // 名前変更ボタン
            public bool ColModify { get { return this.colModify; } set { this.colModify = value; } }
            // 削除ボタン
            public bool ColDelete { get { return this.colDelete; } set { this.colDelete = value; } }
            // Favorite_id
            public string ColFavorite_id { get { return this.colFavorite_id; } set { this.colFavorite_id = value; this.OnPropertyChanged(nameof(ColFavorite_id)); } }
            // Table_n
            public string ColTable_n { get { return this.colTable_n; } set { this.colTable_n = value; this.OnPropertyChanged(nameof(ColTable_n)); } }

            /// <summary>
            /// コンストラクター
            /// </summary>
            public DataExtractFavoriteListItem()
            {
                colFavorite_n = null;
                colSelect = true;
                colModify = true;
                colDelete = true;
                colFavorite_id = null;
                colTable_n = null;
            }

            /// <summary>
            /// PropertyChanged イベント
            /// </summary>
            /// <param name="info"></param>
            public void OnPropertyChanged(string info)
            {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
            }
        }

        /// <summary>
        /// 表示項目選択用クラス
        /// </summary>
        public class DataExtractDispItem
        {
            public event PropertyChangedEventHandler PropertyChanged;

            private string colItem_n;
            private string colDisp_flg;
            private string colSort_no;

            // 項目
            public string ColItem_n { get { return this.colItem_n; } set { this.colItem_n = value; this.OnPropertyChanged(nameof(ColItem_n)); } }
            // 表示選択
            public string ColDisp_flg { get { return this.colDisp_flg; } set { this.colDisp_flg = value; this.OnPropertyChanged(nameof(ColDisp_flg)); } }
            // 表示順
            public string ColSort_no { get { return this.colSort_no; } set { this.colSort_no = value; this.OnPropertyChanged(nameof(ColSort_no)); } }

            /// <summary>
            /// コンストラクター
            /// </summary>
            public DataExtractDispItem()
            {
                colItem_n = null;
                colDisp_flg = null;
                colSort_no = null;
            }

            /// <summary>
            /// PropertyChanged イベント
            /// </summary>
            /// <param name="info"></param>
            public void OnPropertyChanged(string info)
            {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
            }
        }

        /// <summary>
        /// 並び替え設定用クラス
        /// </summary>
        public class DataExtractSortItem
        {
            public event PropertyChangedEventHandler PropertyChanged;

            private string colPriority;
            private string colItem_n;
            private string colOrder_flg;

            // Priority
            public string ColPriority { get { return this.colPriority; } set { this.colPriority = value; this.OnPropertyChanged(nameof(ColPriority)); } }
            // キー項目
            public string ColItem_n { get { return this.colItem_n; } set { this.colItem_n = value; this.OnPropertyChanged(nameof(ColItem_n)); } }
            // 順番
            public string ColOrder_flg { get { return this.colOrder_flg; } set { this.colOrder_flg = value; this.OnPropertyChanged(nameof(ColOrder_flg)); } }

            /// <summary>
            /// コンストラクター
            /// </summary>
            public DataExtractSortItem()
            {
                colPriority = null;
                colItem_n = null;
                colOrder_flg = null;
            }

            /// <summary>
            /// PropertyChanged イベント
            /// </summary>
            /// <param name="info"></param>
            public void OnPropertyChanged(string info)
            {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
            }
        }

        /// <summary>
        /// コンボボックス用クラス
        /// </summary>
        public class DataExtractComboBoxItem
        {
            // 表示用のラベル
            public string Label { get; set; }
            // 値
            public string Value { get; set; }

            /// <summary>
            /// コンストラクター
            /// </summary>
            public DataExtractComboBoxItem()
            {
                Label = null;
                Value = null;
            }
            public DataExtractComboBoxItem(string label, string value)
            {
                Label = label;
                Value = value;
            }
        }

        /// <summary>
        /// セル選択補助用クラス
        /// </summary>
        /// <remarks>
        /// https://social.msdn.microsoft.com/Forums/en-US/0cdf2968-00e8-4ea5-aea0-ffd0e8230110/wpf-datagrid-current-row-index?forum=wpf
        /// </remarks>
        public static class DataGridHelper
        {
            public static DataGridCell GetCell(DataGridCellInfo dataGridCellInfo)
            {
                if (!dataGridCellInfo.IsValid)
                {
                    return null;
                }

                var cellContent = dataGridCellInfo.Column.GetCellContent(dataGridCellInfo.Item);
                if (cellContent != null)
                {
                    return (DataGridCell)cellContent.Parent;
                }
                else
                {
                    return null;
                }
            }
            public static int GetRowIndex(DataGridCell dataGridCell)
            {
                // Use reflection to get DataGridCell.RowDataItem property value.
                PropertyInfo rowDataItemProperty = dataGridCell.GetType().GetProperty("RowDataItem", BindingFlags.Instance | BindingFlags.NonPublic);

                DataGrid dataGrid = GetDataGridFromChild(dataGridCell);

                return dataGrid.Items.IndexOf(rowDataItemProperty.GetValue(dataGridCell, null));
            }
            public static DataGrid GetDataGridFromChild(DependencyObject dataGridPart)
            {
                if (VisualTreeHelper.GetParent(dataGridPart) == null)
                {
                    throw new NullReferenceException("Control is null.");
                }
                if (VisualTreeHelper.GetParent(dataGridPart) is DataGrid)
                {
                    return (DataGrid)VisualTreeHelper.GetParent(dataGridPart);
                }
                else
                {
                    return GetDataGridFromChild(VisualTreeHelper.GetParent(dataGridPart));
                }
            }
        }

        /// <summary>
        /// ページング用クラス
        /// </summary>
        /// <remarks>
        /// https://www.codeproject.com/Articles/1257860/Paging-WPF-DataGrid
        /// </remarks>
        public class Paging
        {
            public int PageIndex { get; set; }

            DataTable PagedList = new DataTable();

            public DataTable SetPaging(IList<DataRow> ListToPage, int RecordsPerPage)
            {
                int PageGroup = PageIndex * RecordsPerPage;

                IList<DataRow> PagedList = new List<DataRow>();

                PagedList = ListToPage.Skip(PageGroup).Take(RecordsPerPage).ToList();
                //DataTable FinalPaging = PagedTable(PagedList);
                DataTable FinalPaging = PagedList.CopyToDataTable();

                return FinalPaging;
            }

            private DataTable PagedTable<T>(IList<T> SourceList)
            {
                Type columnType = typeof(T);
                DataTable TableToReturn = new DataTable();

                foreach (var Column in columnType.GetProperties())
                {
                    TableToReturn.Columns.Add(Column.Name, Column.PropertyType);
                }

                foreach (object item in SourceList)
                {
                    DataRow ReturnTableRow = TableToReturn.NewRow();
                    foreach (var Column in columnType.GetProperties())
                    {
                        ReturnTableRow[Column.Name] = Column.GetValue(item);
                    }
                    TableToReturn.Rows.Add(ReturnTableRow);
                }
                return TableToReturn;
            }

            public DataTable Next(IList<DataRow> ListToPage, int RecordsPerPage)
            {
                PageIndex++;
                if (PageIndex >= ListToPage.Count / RecordsPerPage)
                {
                    PageIndex = ListToPage.Count / RecordsPerPage;
                }
                PagedList = SetPaging(ListToPage, RecordsPerPage);
                return PagedList;
            }

            public DataTable Previous(IList<DataRow> ListToPage, int RecordsPerPage)
            {
                PageIndex--;
                if (PageIndex <= 0)
                {
                    PageIndex = 0;
                }
                PagedList = SetPaging(ListToPage, RecordsPerPage);
                return PagedList;
            }

            public DataTable First(IList<DataRow> ListToPage, int RecordsPerPage)
            {
                PageIndex = 0;
                PagedList = SetPaging(ListToPage, RecordsPerPage);
                return PagedList;
            }

            public DataTable Last(IList<DataRow> ListToPage, int RecordsPerPage)
            {
                PageIndex = ListToPage.Count / RecordsPerPage;
                PagedList = SetPaging(ListToPage, RecordsPerPage);
                return PagedList;
            }
        }
        #endregion
    }
}
