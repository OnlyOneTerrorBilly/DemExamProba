using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DemExam.Pages
{
    /// <summary>
    /// Логика взаимодействия для Avtorization.xaml
    /// </summary>
    public partial class Avtorization : Page
    {
        // Счетчик попыток для текущей сессии (если не хотим хранить в БД до блокировки)
        private int _localAttemptCount = 0;
        public Avtorization()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text.Trim();
            string password = PasswordBox.Password;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                ErrorTextBlock.Text = "Заполните логин и пароль";
                return;
            }

            using (var db = DataBaseEntities.GetContext())
            {
                var user = db.Polzovateli.FirstOrDefault(u => u.Login == login);

                if (user == null)
                {
                    ErrorTextBlock.Text = "Неверный логин или пароль";
                    return;
                }

                if (user.Blocked == true)
                {
                    ErrorTextBlock.Text = "Вы заблокированы. Обратитесь к администратору";
                    return;
                }

                if (user.Password == password)
                {
                    user.Popitki_Vhoda = 0;
                    db.SaveChanges();

                    MessageBox.Show("Вы успешно авторизовались", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    NavigationService.Navigate(new Navigation());
                }
                else
                {
                    _localAttemptCount++;

                    int newAttempts = user.Popitki_Vhoda + 1;
                    user.Popitki_Vhoda = newAttempts;

                    if (newAttempts >= 3 || _localAttemptCount >= 3)
                    {
                        user.Blocked = true;
                        db.SaveChanges();
                        ErrorTextBlock.Text = "Вы заблокированы. Обратитесь к администратору";
                    }
                    else
                    {
                        db.SaveChanges();
                        ErrorTextBlock.Text = $"Неверный логин или пароль. Осталось попыток: {3 - newAttempts}";
                    }
                }
            }
        }
    }
}
