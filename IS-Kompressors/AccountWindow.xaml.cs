using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;
using System;
using System.Windows.Controls;
using Microsoft.Win32;
using MahApps.Metro.Controls;
using System.Windows.Media.Imaging;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro;
using Microsoft.Office.Interop.Word;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace IS_Kompressors
{
    /// <summary>
    /// Логика взаимодействия для AccountWindow.xaml
    /// </summary>
    public partial class AccountWindow : MetroWindow
    {
        MD5 md5 = MD5.Create();
        bool reg = false;
        Entities entities;
        ObservableCollection<string> ListLogins = new ObservableCollection<string>();

        public AccountWindow(Entities dataEntities)
        {
            InitializeComponent();
            this.entities = dataEntities;

            var queryAcc = from item in entities.Accounts
                           orderby item.login
                           select item;
            foreach (Account item in queryAcc)
            {
                ListLogins.Add(item.login);
            }
        }

        static string GetMd5Hash(MD5 md5Hash, string input)
        {
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }

        static bool VerifyMd5Hash(MD5 md5Hash, string input, string hash)
        {
            string hashOfInput = GetMd5Hash(md5Hash, input);

            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            if(reg)
            {
                if (!ListLogins.Contains(login.Text))
                {
                    Account acc = new Account();
                    acc.login = login.Text;
                    acc.password = GetMd5Hash(md5, password.Password);
                    acc.FIO = fio.Text;
                    entities.Accounts.Add(acc);
                    entities.SaveChanges();
                    this.DialogResult = true;
                }
                else
                {
                    this.ShowMessageAsync("Такой логин уже существует", "Пожалуйста, попробуйте придумать другой логин");
                }
            }
            else
            {
                var queryAcc = from item in entities.Accounts
                                     orderby item.login
                                     select item;
                foreach (Account item in queryAcc)
                {
                    if(item.login == login.Text && VerifyMd5Hash(md5, password.Password,item.password))
                    {
                        this.DialogResult = true;
                    }
                }
                this.ShowMessageAsync("Неверные данные пользователя","Введенные Вами данные неверны. Для регистрации нажмите кнопку Регистрация");
            }
        }

        private void regButton_Click(object sender, RoutedEventArgs e)
        {
            regButton.Visibility = Visibility.Hidden;
            passwordRepeat.Visibility = Visibility.Visible;
            repeatLabel.Visibility = Visibility.Visible;
            fio.Visibility = Visibility.Visible;
            fioLabel.Visibility = Visibility.Visible;
            reg = true;
            title.Content = "Регистрация";
        }

        private void cancelbutton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
