using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchSample.Model
{
    public class OutputItem
    {
        public string output_item { get; set; }
        public bool display { get; set; }
        public string sort { get; set; }
        public string sort_dir { get; set; }
        public bool sort_dir_asc { get; set; }
        public bool sort_dir_desc { get; set; }
        public string line_no { get; set; }

    }
}
