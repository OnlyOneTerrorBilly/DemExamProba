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
    /// Логика взаимодействия для AdminPanel.xaml
    /// </summary>
    public partial class AdminPanel : Page
    {
        private Polzovateli _selectedUser;
        public AdminPanel()
        {
            InitializeComponent();
            LoadUsers();
        }
        private void LoadUsers()
        {
            using (var db = new DataBaseEntities())
            {
                DgUsers.ItemsSource = db.Polzovateli
                    .Include("Roli")
                    .ToList();
            }
        }
        private void ClearForm()
        {
            TxtLogin.Text = "";
            TxtPassword.Password = "";
            CmbRole.SelectedIndex = 1; // Пользователь по умолчанию
            _selectedUser = null;
            BtnAdd.IsEnabled = true;
            BtnUpdate.IsEnabled = true;
        }

        private void DgUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedUser = DgUsers.SelectedItem as Polzovateli;

            if (_selectedUser != null)
            {
                TxtLogin.Text = _selectedUser.Login;
                TxtPassword.Password = _selectedUser.Password;

                // Выбираем роль в комбобоксе
                foreach (ComboBoxItem item in CmbRole.Items)
                {
                    if (item.Tag.ToString() == _selectedUser.Roli_Polzovateli.ToString())
                    {
                        CmbRole.SelectedItem = item;
                        break;
                    }
                }

                BtnAdd.IsEnabled = false;
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            string login = TxtLogin.Text.Trim();
            string password = TxtPassword.Password;
            int roleId = int.Parse(((ComboBoxItem)CmbRole.SelectedItem).Tag.ToString());

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Заполните логин и пароль", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var db = new DataBaseEntities())
            {
                // Проверка на существование логина
                if (db.Polzovateli.Any(u => u.Login == login))
                {
                    MessageBox.Show("Пользователь с таким логином уже существует", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var newUser = new Polzovateli
                {
                    Login = login,
                    Password = password,
                    Roli_Polzovateli = roleId,
                    Blocked = false,
                    Popitki_Vhoda = 0
                };

                db.Polzovateli.Add(newUser);
                db.SaveChanges();
            }

            MessageBox.Show("Пользователь добавлен", "Успех",
                MessageBoxButton.OK, MessageBoxImage.Information);

            LoadUsers();
            ClearForm();
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedUser == null)
            {
                MessageBox.Show("Выберите пользователя для редактирования", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string login = TxtLogin.Text.Trim();
            string password = TxtPassword.Password;
            int roleId = int.Parse(((ComboBoxItem)CmbRole.SelectedItem).Tag.ToString());

            using (var db = new DataBaseEntities())
            {
                var user = db.Polzovateli.Find(_selectedUser.Id_Polzovateli);

                if (user != null)
                {
                    // Проверка на уникальность логина (если меняем)
                    if (user.Login != login && db.Polzovateli.Any(u => u.Login == login))
                    {
                        MessageBox.Show("Пользователь с таким логином уже существует", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    user.Login = login;
                    user.Password = password;
                    user.Roli_Polzovateli = roleId;

                    db.SaveChanges();
                }
            }

            MessageBox.Show("Данные пользователя обновлены", "Успех",
                MessageBoxButton.OK, MessageBoxImage.Information);

            LoadUsers();
            ClearForm();
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void BtnUnblock_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null) return;

            int userId = (int)btn.Tag;

            using (var db = new DataBaseEntities())
            {
                var user = db.Polzovateli.Find(userId);
                if (user != null)
                {
                    user.Blocked = false;
                    user.Popitki_Vhoda = 0;
                    db.SaveChanges();
                }
            }

            LoadUsers(); // Обновляем таблицу

            MessageBox.Show("Пользователь разблокирован", "Успех",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnNazad_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Navigation());
        }
    }
}
