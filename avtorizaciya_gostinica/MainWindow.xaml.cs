using avtorizaciya_gostinica.ApplicationData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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

namespace avtorizaciya_gostinica
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<User> users = new List<User>();
        private User currentUser;

        public MainWindow()
        {
            InitializeComponent();
            // Создал пользователей для теста
            users.Add(new User { Id = 1, Login = "admin", Password = ComputeMD5Hash("admin123"), Role = "Администратор" });
            users.Add(new User { Id = 2, Login = "user", Password = ComputeMD5Hash("user123"), Role = "Пользователь" });
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text;
            string password = PasswordBox.Password;

            // Ищем пользователя с введённым логином
            currentUser = users.FirstOrDefault(u => u.Login == login);

            // Проверяем, существует ли пользователь
            if (currentUser == null)
            {
                MessageTextBlock.Text = "Неверный логин или пароль. Попробуйте ещё раз.";
                return;
            }

            // Проверяем, заблокирована ли учетная запись
            if (currentUser.IsBlocked)
            {
                MessageTextBlock.Text = "Ваша учетная запись заблокирована. Обратитесь к администратору.";
                return;
            }

            // Хешируем введённый пароль с помощью MD5
            string hashedInputPassword = ComputeMD5Hash(password);

            // Проверяем правильность пароля
            if (hashedInputPassword == currentUser.Password)
            {
                MessageTextBlock.Text = "Вы успешно авторизовались!";
                currentUser.LoginAttempts = 0; // Сбрасываем счетчик попыток входа

                // Если это администратор, открываем форму управления пользователями
                if (currentUser.Role == "Администратор")
                {
                    MainPage.AdminWindow adminWindow = new MainPage.AdminWindow(currentUser);
                    adminWindow.Show();
                    this.Close(); // Закрываем окно авторизации
                }
            }
            else
            {
                // Увеличиваем счетчик неудачных попыток
                currentUser.LoginAttempts++;

                // Проверяем, не превышен ли лимит попыток
                if (currentUser.LoginAttempts >= 3)
                {
                    currentUser.IsBlocked = true; // Блокируем учетную запись
                    MessageTextBlock.Text = "Ваша учетная запись заблокирована после трех неудачных попыток.";
                }
                else
                {
                    MessageTextBlock.Text = "Неверный логин или пароль. Попробуйте ещё раз.";
                }
            }
        }

        // Метод для хеширования пароля с использованием MD5
        private string ComputeMD5Hash(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                // Преобразуем строку в массив байтов и вычисляем хеш
                byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(input));

                // Преобразуем массив байтов в строку
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    sb.Append(data[i].ToString("x2")); // Преобразуем в шестнадцатеричный формат
                }
                return sb.ToString();
            }
        }
    }
}
