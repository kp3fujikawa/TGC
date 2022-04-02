using System;
using System.Collections.Generic;
using System.Text;

namespace AddColumnTool
{
    class RecordEntity
    {
        public String Name { get; set; }
        public String ColName { get; set; }
        public String Domain { get; set; }
        public String DataType { get; set; }
        public String PrimaryKey { get; set; }
        public String Comment { get; set; }
    }
}
