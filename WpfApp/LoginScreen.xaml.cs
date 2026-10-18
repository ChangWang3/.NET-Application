using System;
using System.Linq;
using System.Windows;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace WpfApp
{
    /// <summary>
    /// Interaction logic for LoginScreen.xaml
    /// </summary>
    public partial class LoginScreen : Window
    {
        public LoginScreen()
        {
            InitializeComponent();
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string username = txtUsername.Text.Trim();
                string password = txtPassword.Password;

                using UserDataContext context = new UserDataContext();

                bool found = context.Users.Any(user =>
                    user.Name == username && user.Password == password);

                if (!found)
                {
                    LoginErrorText.Text = "Incorrect username or password.";
                    return;
                }

                EnterApplication();
                Close();
            }
            catch (SqliteException exception)
            {
                LoginErrorText.Text = "Database error. " + exception.Message;
            }
        }

        private void EnterApplication()
        {
            MainWindow main = new MainWindow();
            main.Show();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
