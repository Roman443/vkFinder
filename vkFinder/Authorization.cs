using System;
using System.Windows.Forms;

namespace vkFinder
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            var mf = new MainForm();
            if (!((loginBox.Text != "") & (passwordBox.Text != ""))) return;
            AppSettings.Default.login = loginBox.Text;
            AppSettings.Default.password = passwordBox.Text;
            AppSettings.Default.Save();
            mf.Authorize(AppSettings.Default.login, AppSettings.Default.password);
            Close();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            loginBox.Text = AppSettings.Default.login;
            passwordBox.Text = AppSettings.Default.password;
        }

        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            passwordBox.UseSystemPasswordChar = !checkBox1.Checked;
        }
    }
}