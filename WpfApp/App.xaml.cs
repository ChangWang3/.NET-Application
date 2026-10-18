using Microsoft.Data.Sqlite;
using System.Windows;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace WpfApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                // Create the existing user database before the login screen opens.
                using (UserDataContext context = new UserDataContext())
                {
                    context.Database.EnsureCreated();
                    if (!context.Users.Any(user => user.Name == "assignment2"))
                    {
                        context.Users.Add(new User
                        {
                            Name = "assignment2",
                            Password = "1111"
                        });

                        context.SaveChanges();
                    }
                }
            }
            catch (Exception exception) when (exception is SqliteException || exception is DbUpdateException)
            {
                MessageBox.Show("Could not open the user database. " + exception.Message,
                    "Database error");
                Shutdown(1);
                return;
            }

            base.OnStartup(e);
        }
        
    }

}
