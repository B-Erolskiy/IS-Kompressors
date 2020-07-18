using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace IS_Kompressors
{
    /// <summary>
    /// Логика взаимодействия для PartWindow.xaml
    /// </summary>
    public partial class PartWindow : MetroWindow
    {
        ObservableCollection<Part> ListParts = new ObservableCollection<Part>();
        ObservableCollection<string> ListCodes = new ObservableCollection<string>();
        public Part part;
        Entities entities;
        public int part_id;
        bool create = true;

        public PartWindow(ObservableCollection<Equipment> list,
            ObservableCollection<Part> listParts,
            ObservableCollection<string> ListCodes,
            Entities en,
            Part editPart)
        {
            InitializeComponent();

            equipmentPartsComboBox.ItemsSource = list;

            this.entities = en;
            this.ListParts = listParts;
            if (editPart != null)
            {
                create = false;
                PartsTitle.Content = "Изменение запчасти";
                createParts.Content = "изменить";
                this.part = editPart;
                this.part_id = editPart.id_part;
                codePartsTextBox.Text = part.code_part;
                namePartsTextBox.Text = part.name_part;
                new TextRange(descriptionPartsTextBox.Document.ContentStart, descriptionPartsTextBox.Document.ContentEnd).Text = part.description_part;
                equipmentPartsComboBox.SelectedValue = part.equipment_part;
                pricePartsTextBox.Text = part.price_part.ToString();
            }
            this.ListCodes = ListCodes;
        }


        public int DS_Count(string s)
        {
            string substr = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0].ToString();
            int count = (s.Length - s.Replace(substr, "").Length) / substr.Length;
            return count;
        }

        private void priceEquipmentTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !((Char.IsDigit(e.Text, 0) || ((e.Text == System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0].ToString()) && (DS_Count(((TextBox)sender).Text) < 1))));
        }

        private void cancelParts_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void createParts_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (create)
            {
                if (codePartsTextBox.Text != null &&
                ListCodes.Contains(codePartsTextBox.Text) == false &&
                namePartsTextBox.Text != null &&
                equipmentPartsComboBox.SelectedIndex != -1)
                    Do();
                else
                {
                    this.ShowMessageAsync("Ошибка", "Возможно, вы не заполнили некоторые обязательные поля, или данный артикул уже используется");
                }
            }
            else
            {
                if (codePartsTextBox.Text != null &&
                namePartsTextBox.Text != null &&
                equipmentPartsComboBox.SelectedIndex != -1)
                    Do();
                else
                {
                    this.ShowMessageAsync("Ошибка", "Возможно, вы не заполнили некоторые обязательные поля");
                }
            }
        }

        public void Do()
        {
            try
            {
                part = new Part();
                part.code_part = codePartsTextBox.Text;
                part.name_part = namePartsTextBox.Text;
                part.description_part = new TextRange(descriptionPartsTextBox.Document.ContentStart, descriptionPartsTextBox.Document.ContentEnd).Text;
                Equipment equipment = equipmentPartsComboBox.SelectedItem as Equipment;
                part.equipment_part = equipment.id_equipment;
                part.price_part = Convert.ToDecimal(pricePartsTextBox.Text);

                if (create)
                {
                    part.col_part = 0;
                    entities.Parts.Add(part);
                }
                else
                {
                    foreach (Part pr in ListParts)
                    {
                        if (pr.id_part == part_id)
                        {
                            pr.name_part = part.name_part;
                            pr.code_part = part.code_part;
                            pr.description_part = part.description_part;
                            pr.equipment_part = part.equipment_part;
                            pr.price_part = part.price_part;
                        }
                    }
                }
                entities.SaveChanges();
                this.DialogResult = true;
            }
            catch (Exception ex)
            {

                this.ShowMessageAsync("Ошибка", ex.ToString());
            }
        }
    }
}
