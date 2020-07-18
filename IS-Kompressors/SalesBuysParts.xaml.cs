using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace IS_Kompressors
{
    /// <summary>
    /// Логика взаимодействия для SalesBuysParts.xaml
    /// </summary>
    public partial class SalesBuysParts : MetroWindow
    {
        public Actions_Parts action;
        public bool typeAction;
        Entities entities;
        ObservableCollection<Part> ListParts = new ObservableCollection<Part>();

        public SalesBuysParts(Entities en,
            ObservableCollection<Part> ListP,
            bool type)
        {
            InitializeComponent();
            this.entities = en;
            this.typeAction = type;
            this.ListParts = ListP;

            partPartsSalesComboBox.ItemsSource = ListParts;

            if (type)
            {
                SalesBuysTitle.Content = "Добавление продажи";
                personSalesBuysLabel.Content = "Покупатель";
            }
            else
            {
                SalesBuysTitle.Content = "Добавление поставки";
                personSalesBuysLabel.Content = "Поставщик";
            }
        }


        public int DS_Count(string s)
        {
            string substr = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0].ToString();
            int count = (s.Length - s.Replace(substr, "").Length) / substr.Length;
            return count;
        }

        private void colSalesBuysTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !(Char.IsDigit(e.Text, 0));
        }

        private void cancelParts_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void createParts_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (colSalesBuysTextBox.Text != null &&
                partPartsSalesComboBox.SelectedIndex != -1)
                try
                {
                    action = new Actions_Parts();
                    action.col_aParts = Convert.ToDecimal(colSalesBuysTextBox.Text);
                    action.date_aParts = DateTime.Today;
                    action.person_aParts = personSalesBuysTextBox.Text;
                    Part part = partPartsSalesComboBox.SelectedItem as Part;
                    action.price_aParts = action.col_aParts * part.price_part;
                    action.part_aParts = part.id_part;
                    action.type_aParts = typeAction;
                    if (typeAction)
                        if (part.col_part >= action.col_aParts)
                        {
                            entities.Actions_Parts.Add(action);
                            entities.SaveChanges();
                            this.DialogResult = true;
                        }
                        else
                            this.ShowMessageAsync("Отмена добавления продажи", "Невозможно продать запчасти, так как на складе нет достаточного количества ");
                    else
                    {
                        entities.Actions_Parts.Add(action);
                        entities.SaveChanges();
                        this.DialogResult = true;
                    }
                }
                catch (Exception ex)
                {

                    this.ShowMessageAsync("Ошибка", ex.ToString());
                }
            else
            {
                this.ShowMessageAsync("Ошибка", "Возможно, вы не заполнили некоторые обязательные поля");
            }
    }
}
}
