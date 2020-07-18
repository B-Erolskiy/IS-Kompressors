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
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Collections.ObjectModel;

namespace IS_Kompressors
{
    /// <summary>
    /// Логика взаимодействия для SalesBuys.xaml
    /// </summary>
    public partial class SalesBuys : MetroWindow
    {
        public Actions_Equipment action;
        public bool typeAction;
        Entities entities;
        ObservableCollection<Equipment> ListEquipment = new ObservableCollection<Equipment>();

        public SalesBuys(Entities en,
            ObservableCollection<Equipment> ListEquipments,
            bool type)
        {
            InitializeComponent();
            this.entities = en;
            this.typeAction = type;
            this.ListEquipment = ListEquipments;

            equipmentEquipmentSalesComboBox.ItemsSource = ListEquipments;

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

        private void cancelEquipment_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void createEquipment_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (colSalesBuysTextBox.Text != null &&
                equipmentEquipmentSalesComboBox.SelectedIndex != -1)
                    try
                    {
                        action = new Actions_Equipment();
                        action.col_aEquipment = Convert.ToDecimal(colSalesBuysTextBox.Text);
                        action.date_aEquipment = DateTime.Today;
                        action.person_aEquipment = personSalesBuysTextBox.Text;
                        Equipment equipment = equipmentEquipmentSalesComboBox.SelectedItem as Equipment;
                        action.price_aEquipment = action.col_aEquipment * equipment.price_equipment;
                        action.equipment_aEquipment = equipment.id_equipment;
                        action.type_aEquipment = typeAction;
                        if(typeAction)
                            if(equipment.col_equipment >= action.col_aEquipment)
                            {
                                entities.Actions_Equipment.Add(action);
                                entities.SaveChanges();
                                this.DialogResult = true;
                            }
                            else
                                this.ShowMessageAsync("Отмена добавления продажи", "Невозможно продать оборудование, так как на складе нет достаточного количества ");
                    else
                    {
                        entities.Actions_Equipment.Add(action);
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
