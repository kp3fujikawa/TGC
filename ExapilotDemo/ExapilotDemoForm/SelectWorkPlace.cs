using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace ExapilotDemoForm
{
    public partial class SelectWorkPlace : Form
    {
        public String WorkPalce { get; set; }

        public SelectWorkPlace()
        {
            InitializeComponent();
        }

        private void Form_Load(object sender, EventArgs e)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(@"workplace.xml");
            var workplace = xmlDoc.SelectNodes("work_place_list/work_place");

            foreach (XmlNode place in workplace)
            {
                var name = place.Attributes["name"].InnerText;
                listBox1.Items.Add(name);

                if (WorkPalce == name)
                {
                    listBox1.SelectedItems.Add(name);
                }
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.WorkPalce = (String)listBox1.SelectedItem;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
