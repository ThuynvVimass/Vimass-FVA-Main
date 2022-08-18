using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VimassFVA
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
            this.txtPass.Text = "toandh87";
            this.txtUser.Text = "01677249552";
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string user = txtUser.Text;
            string pass = txtPass.Text;
            string url = "https://vimass.vn/vmbank/services/account/login1";
            User u = new User();            
            u.pass = pass;
            u.VimassMH = 0;
            u.appId = 5;
            u.user = user;
            u.type = 1;
            var json = JsonConvert.SerializeObject(u);
            String res = Service.SendWebrequest_POST_Method(json, url);
            Response response = JsonConvert.DeserializeObject<Response>(res);
            if (response!= null&& response.msgCode == 1)
            {
                Register reg = new Register(user);
                reg.Show();                
                this.Dispose();
            }
            else {
                lbError.Text = response.msgContent;
            }
        }
    }
}
