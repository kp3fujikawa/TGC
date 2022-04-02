using System;
using System.Collections.Generic;
using System.Text;

namespace AddColumnTool
{
    class TableDAO
    {
        public TableEntity Table { get; set; }
        public List<RecordEntity> Records { get; set; }

        public TableDAO()
        {
            Table = new TableEntity();
            Records = new List<RecordEntity>();
        }
    }
}
