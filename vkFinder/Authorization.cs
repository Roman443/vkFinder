using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace vkFinder
{
    public partial class Form2 : Form
    {
        private bool _authificated;

        public Form2()
        {
            InitializeComponent();
        }

        private void AuthorizeButtonClick(object sender, EventArgs e)
        {
            if (loginBox.Text == "" || passwordBox.Text == "") return;
            AppSettings.Default.login = loginBox.Text;
            AppSettings.Default.password = passwordBox.Text;
            AppSettings.Default.Save();
            AuthorizeButton.Enabled = false;
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
                var mainForm = Owner as MainForm;
                mainForm?.Authorize(AppSettings.Default.login, AppSettings.Default.password);
                _authificated = true;
            }
            catch (Exception)
            {
                MessageBox.Show(@"Ошибка авторизации", @"Статус авторизации");
            }
        }

        private void AuthorizeWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            AuthorizeButton.Enabled = true;
            if (!_authificated) return;
            MessageBox.Show(@"Успешная авторизация", @"Статус авторизации");
            if (Owner is MainForm mainForm)
            {
                mainForm.label1.ForeColor = Color.Green;
                mainForm.label1.Text = @"Статус: Авторизован";
            }
            Close();
        }
    }
}