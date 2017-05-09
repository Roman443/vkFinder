using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace vkFinder
{
    public partial class Form2 : Form
    {
        private bool _authificated;
        public MainForm MainForm = new MainForm();

        public Form2()
        {
            InitializeComponent();
        }

        private void Button1Click(object sender, EventArgs e)
        {
            if (loginBox.Text == "" || passwordBox.Text == "") return;
            AppSettings.Default.login = loginBox.Text;
            AppSettings.Default.password = passwordBox.Text;
            AppSettings.Default.Save();
            AuthorizeWorker.RunWorkerAsync();
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

        private void AuthorizeWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                MainForm.Authorize(AppSettings.Default.login, AppSettings.Default.password);
                _authificated = true;
            }
            catch (Exception)
            {
                MessageBox.Show(@"Ошибка авторизации", @"Статус авторизации");
            }
        }

        private void AuthorizeWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!_authificated) return;
            MessageBox.Show(@"Успешная авторизация", @"Статус авторизации");
            MainForm.AuthorizationSuccess();
            Close();
        }
    }
}