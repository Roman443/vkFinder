using System;
using System.Windows.Forms;
using VkNet;

namespace vkFinder
{
    public partial class Form2 : Form
    {
        public static VkApi Vk = new VkApi();
        public Form2()
        {
            InitializeComponent();
        }

        private void Button1Click(object sender, EventArgs e)
        {
            var mf = new MainForm();
            if (!((loginBox.Text != "") & (passwordBox.Text != ""))) return;
            AppSettings.Default.login = loginBox.Text;
            AppSettings.Default.password = passwordBox.Text;
            AppSettings.Default.Save();
            try
            {
                mf.Authorize(AppSettings.Default.login, AppSettings.Default.password);
                mf.AuthorizationSuccess();
                Close();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
            
        }

        private void Form2Load(object sender, EventArgs e)
        {
            loginBox.Text = AppSettings.Default.login;
            passwordBox.Text = AppSettings.Default.password;
        }

        private void CheckBox1CheckedChanged(object sender, EventArgs e)
        {
            passwordBox.UseSystemPasswordChar = !checkBox1.Checked;
        }
    }
}