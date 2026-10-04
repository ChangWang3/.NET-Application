//using System;
//using System.Collections.Generic;
//using System.Text;
using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Data;
//using System.Windows.Documents;
//using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
//using System.Windows.Shapes;

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
            string Username = txtUsername.Text;
            string Password = txtPassword.Password;

            using (UserDataContext context = new UserDataContext())
            {
                bool userFound = context.Users.Any(user => user.Name == Username && user.Password == Password);

                if (userFound)
                {
                    EnterApplication();
                    Close();
                } else
                {
                    MessageBox.Show("User not found");
                }
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
