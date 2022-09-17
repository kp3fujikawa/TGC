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
    public partial class MainWindow : Form
    {
        private List<PresParam> presParams = new List<PresParam>();
        private int activeParam = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void txtUser_Click(object sender, EventArgs e)
        {
            var dlg = new SelectUser();
            dlg.User = txtUser.Text;
            dlg.ShowDialog();
            txtUser.Text = dlg.User;
        }

        private void txtWorkSpace_Click(object sender, EventArgs e)
        {
            var dlg = new SelectWorkPlace();
            dlg.WorkPalce = txtWorkSpace.Text;
            dlg.ShowDialog();
            if (txtWorkSpace.Text != dlg.WorkPalce)
            {
                txtWorkSpace.Text = dlg.WorkPalce;
                ReloadDisp();
            }
        }

        private void Form_Load(object sender, EventArgs e)
        {
            ReloadDisp();
        }
        private void ReloadDisp()
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(txtWorkSpace.Text + @".xml");
            var ope = xmlDoc.SelectNodes("operation/ope");

            this.presParams.Clear();
            this.activeParam = -1;
            foreach (XmlNode emp in ope)
            {
                var message = emp.Attributes["message"].InnerText;
                var confirm_message = emp.Attributes["confirm_message"].InnerText;
                var value = emp.Attributes["value"].InnerText;
                var result = emp.Attributes["result"].InnerText;
                var link = emp.Attributes["link"].InnerText;

                presParams.Add(new PresParam()
                {
                    ConfirmMessage = confirm_message,
                    Message = message,
                    Value = value,
                    Result = result,
                    Link = link
                });

                if (!String.IsNullOrEmpty(result))
                {
                    this.activeParam = presParams.Count;
                }
            }

            RefreshDisp();
        }

        private void RefreshDisp()
        {

            var controls = new TextBox[] { txtParam1,
                                            txtParam2,
                                            txtParam3,
                                            txtParam4,
                                            txtParam5,
                                            txtParam6,
                                            txtParam7,
                                            txtParam8,
            };
            var controls2 = new TextBox[] { txtResult1,
                                            txtResult2,
                                            txtResult3,
                                            txtResult4,
                                            txtResult5,
                                            txtResult6,
                                            txtResult7,
                                            txtResult8,
            };
            var controls3 = new Button[] { btnActive1,
                                            btnActive2,
                                            btnActive3,
                                            btnActive4,
                                            btnActive5,
                                            btnActive6,
                                            btnActive7,
                                            btnActive8,
            };

            for (int i = 0; i < controls.Length; i++)
            {
                controls3[i].Visible = false;
                controls[i].Text = "";
                controls2[i].Text = "";
            }
            btnParamLink.Visible = false;
            btnParamLink.Tag = "";
            txtResult.Tag = "";
            txtMessage.Text = "";
            btnConfirm.Visible = false;

            int index = 0;
            int active = this.activeParam;

            foreach (PresParam presParam in presParams)
            {
                controls[index].Text = presParam.Message;
                controls2[index].Text = presParam.Result;
                if (index == active)
                {
                    controls3[index].Visible = true;
                    txtMessage.Text = presParam.ConfirmMessage;
                    txtResult.Tag = presParam.Value;
                    if (!String.IsNullOrEmpty(presParam.Link))
                    {
                        btnParamLink.Visible = true;
                        btnParamLink.Tag = presParam.Link;
                    }

                    btnConfirm.Visible = true;
                }

                index++;
                if (index > 7) break;
            }

            txtResult.Focus();
        }

        private void btnParamLink_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start((String)btnParamLink.Tag);
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            this.presParams[this.activeParam].Result = txtResult.Text;
            txtResult.Text = "";
            this.activeParam++;

            RefreshDisp();
        }

        private void btnMes_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.google.com");
        }

        private void btnInput_Click(object sender, EventArgs e)
        {
            var dlg = new InputResult();
            dlg.InputValue = txtResult.Text;
            dlg.ShowDialog();
            
            txtResult.Text = dlg.InputValue;

        }

        private void btnLink_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(@"D:\");
        }

        private void txtCheck_Click(object sender, EventArgs e)
        {
            txtResult.Text = (String)txtResult.Tag;
        }

        private void txtResult_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
