using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;

namespace IS_Kompressors
{
    /// <summary>
    /// Логика взаимодействия для Authorize.xaml
    /// </summary>
    public partial class Authorize : MetroWindow
    {
        Entities msE = new Entities();
        ObservableCollection<string> ListUserLogins = new ObservableCollection<string>();
        ObservableCollection<string> ListUserPasswords = new ObservableCollection<string>();
        MD5 md5 = MD5.Create();
        bool isHashTrue = false;
        bool reg = false;
        private string name = "";

        public string Name
        {
            get
            {
                return this.name;
            }
        }


        public Authorize()
        {
            InitializeComponent();

        }

        private void authorize_Click(object sender, RoutedEventArgs e)
        {
            if (!reg)
            {
                var queryUsers =
                 from user in msE.Accounts
                 select user;

                foreach (Account user in queryUsers)
                {
                    ListUserLogins.Add(user.login);
                    ListUserPasswords.Add(user.password);
                }

                for (int i = 0; i < ListUserPasswords.Count; i++)
                {
                    isHashTrue = VerifyMd5Hash(md5, passwordBox.Password, ListUserPasswords.ElementAt(i));
                    if (isHashTrue)
                        break;
                }

                if (ListUserLogins.Contains(loginBox.Text) && isHashTrue)
                {
                    name = loginBox.Text;
                    //this.DialogResult = true;
                }
                else
                {
                    error.Visibility = Visibility.Visible;
                }
            }
            else
            {
                if (passwordBox.Password == passwordBoxRepeat.Password)
                {
                    Account newUser = new Account();
                    newUser.login = loginBox.Text;
                    string hash = GetMd5Hash(md5, passwordBox.Password);
                    newUser.password = hash;
                    msE.Accounts.Add(newUser);
                    msE.SaveChanges();
                    //DialogResult = true;
                }
                else
                    error.Content = "Пароли не совпадают";
            }
        }

        private void exit_Click(object sender, RoutedEventArgs e)
        {
            //this.DialogResult = false;
        }

        private void registrate_Click(object sender, RoutedEventArgs e)
        {
            main.Content = "Регистрация";
            passwordRepeat.Visibility = Visibility.Visible;
            passwordBoxRepeat.Visibility = Visibility.Visible;
            registrate.Visibility = Visibility.Hidden;
            reg = true;
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
    }
}
