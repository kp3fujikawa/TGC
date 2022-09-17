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
    public partial class SelectUser : Form
    {
        public String User { get; set; }

        public SelectUser()
        {
            InitializeComponent();
        }

        private void Form_Load(object sender, EventArgs e)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(@"user.xml");
            var workplace = xmlDoc.SelectNodes("user_list/user");

            foreach (XmlNode place in workplace)
            {
                var name = place.Attributes["name"].InnerText;
                listBox1.Items.Add(name);

                if (User == name)
                {
                    listBox1.SelectedItems.Add(name);
                }
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.User = (String)listBox1.SelectedItem;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
