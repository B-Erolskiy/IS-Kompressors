using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace IS_Kompressors
{
    /// <summary>
    /// Логика взаимодействия для AddEquipment.xaml
    /// </summary>
    public partial class AddEquipment : MetroWindow
    {
        ObservableCollection<string> ListCodes = new ObservableCollection<string>();
        ObservableCollection<Equipment> ListEquipment = new ObservableCollection<Equipment>();
        ObservableCollection<Attribute> ListAttributes = new ObservableCollection<Attribute>();
        ObservableCollection<Attribute> ListAttWas = new ObservableCollection<Attribute>();

        public Equipment equipment;
        Entities entities;
        public int fabricator, category;
        bool create = true;
        string name = "";
        int id = 0;

        public AddEquipment(ObservableCollection<Equipment> list,
            ObservableCollection<Attribute> listA,
            Entities en,
            ObservableCollection<Fabricator> ListFabricators,
            ObservableCollection<Сategories> ListСategories,
            ObservableCollection<string> ListCodes,
            Equipment editEquipment)
        {
            InitializeComponent();

            fabricatorEquipmentComboBox.ItemsSource = ListFabricators;
            categoriesEquipmentComboBox.ItemsSource = ListСategories;
            attributesEquipmentComboBox.ItemsSource = listA;
            this.entities = en;
            this.ListEquipment = list;
            this.ListAttributes = listA;

            if (editEquipment != null)
            {
                create = false;
                EquipmentTitle.Content = "Изменение оборудования";
                createEquipment.Content = "изменить";
                this.equipment = editEquipment;
                id = editEquipment.id_equipment;
                name = equipment.name_equipment;
                codeEquipmentTextBox.Text = equipment.code_equipment;
                nameEquipmentTextBox.Text = equipment.name_equipment;
                new TextRange(descriptionEquipmentTextBox.Document.ContentStart, descriptionEquipmentTextBox.Document.ContentEnd).Text = equipment.description_equipment;
                fabricatorEquipmentComboBox.SelectedValue = equipment.fabricator_equipment;
                categoriesEquipmentComboBox.SelectedValue = (int)equipment.category_equipment;
                priceEquipmentTextBox.Text = equipment.price_equipment.ToString();

                for(int i=0;i<equipment.Attributes_Values.Count;i++)
                {
                    Attributes_Values av = equipment.Attributes_Values.ElementAt(i);
                    Attribute att = av.Attribute;
                    ListAttWas.Add(att);
                    WrapPanel wp = new WrapPanel
                    {
                        Margin = new System.Windows.Thickness(10),
                    };
                    attributesPanel.Children.Add(wp);
                    Label label = new Label
                    {
                        Content = att.name_attribute + ": ",
                    };
                    TextBox txB = new TextBox
                    {
                        Name = att.name_attribute,
                        Width = 100,
                        Text = av.value_aValues,
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Right,
                    };
                    wp.Children.Add(label);
                    wp.Children.Add(txB);
                }
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

        private void cancelEquipment_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void createEquipment_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (create)
            {
                if (codeEquipmentTextBox.Text != null &&
                ListCodes.Contains(codeEquipmentTextBox.Text) == false &&
                nameEquipmentTextBox.Text != null &&
                fabricatorEquipmentComboBox.SelectedIndex != -1 &&
                categoriesEquipmentComboBox.SelectedIndex != -1)
                    Do();

                else
                {
                    this.ShowMessageAsync("Ошибка", "Возможно, вы не заполнили некоторые обязательные поля, или введеный артикул уже используется");
                }
            }
            else {
                if (codeEquipmentTextBox.Text != null &&
                nameEquipmentTextBox.Text != null &&
                fabricatorEquipmentComboBox.SelectedIndex != -1 &&
                categoriesEquipmentComboBox.SelectedIndex != -1)
                    Do();
                else
                {
                    this.ShowMessageAsync("Ошибка", "Возможно, вы не заполнили некоторые обязательные поля");
                }
            }
        }

        private void AddAttribute_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if(attributesEquipmentComboBox.SelectedIndex != -1 &&
                ListAttWas.Contains(attributesEquipmentComboBox.SelectedItem) == false)
            {
                Attribute att = attributesEquipmentComboBox.SelectedItem as Attribute;
                ListAttWas.Add(att);
                WrapPanel wp = new WrapPanel
                {
                    Margin = new System.Windows.Thickness(10),
                };
                attributesPanel.Children.Add(wp);
                Label label = new Label
                {
                    Content = att.name_attribute + ": ",
                };
                TextBox txB = new TextBox
                {
                    Name = att.name_attribute,
                    Width = 100,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Right,
                };
                wp.Children.Add(label);
                wp.Children.Add(txB);
            }
        }

        public void Do()
        {
            try
            {
                equipment = new Equipment();
                equipment.code_equipment = codeEquipmentTextBox.Text;
                equipment.name_equipment = nameEquipmentTextBox.Text;
                equipment.description_equipment = new TextRange(descriptionEquipmentTextBox.Document.ContentStart, descriptionEquipmentTextBox.Document.ContentEnd).Text;
                Fabricator fabricator = fabricatorEquipmentComboBox.SelectedItem as Fabricator;
                equipment.fabricator_equipment = fabricator.id_fabricator;
                Сategories category = categoriesEquipmentComboBox.SelectedItem as Сategories;
                equipment.category_equipment = category.id_category;
                equipment.price_equipment = Convert.ToDecimal(priceEquipmentTextBox.Text);

                WrapPanel[] w = attributesPanel.Children.OfType<WrapPanel>().ToArray();
                TextBox[] t = new TextBox[w.Count()];
                AttributeArray[] array = new AttributeArray[w.Count()];
                for (int i=0;i<w.Count();i++)
                {
                   t = w[i].Children.OfType<TextBox>().ToArray();
                   array[i] = new AttributeArray(t[0].Name, t[0].Text);
                }

                foreach(Attribute item in ListAttributes)
                {
                    for(int i=0;i<array.Count();i++)
                    {
                        if (item.name_attribute == array[i].name)
                        {
                            Attributes_Values av = new Attributes_Values();
                            av.value_aValues = array[i].value;
                            av.attribute_aValues = item.id_attribute;
                            if (id != 0)
                                av.equipment_aValues = id;
                            else
                                av.equipment_aValues = equipment.id_equipment;
                            equipment.Attributes_Values.Add(av);
                            item.Attributes_Values.Add(av);
                        }
                            
                    }
                }

                if (create)
                {
                    equipment.col_equipment = 0;
                    entities.Equipments.Add(equipment);
                }
                else
                {
                    foreach (Equipment eq in ListEquipment)
                    {
                        if (eq.name_equipment == name)
                        {
                            eq.name_equipment = equipment.name_equipment;
                            eq.code_equipment = equipment.code_equipment;
                            eq.description_equipment = equipment.description_equipment;
                            eq.fabricator_equipment = equipment.fabricator_equipment;
                            eq.category_equipment = equipment.category_equipment;
                            eq.price_equipment = equipment.price_equipment;
                            eq.Attributes_Values = equipment.Attributes_Values;
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

    public class AttributeArray
    {
        public string name;
        public string value;

        public AttributeArray()
        {
            this.name = "";
            this.value = "";
        }

        public AttributeArray(string n, string v)
        {
            this.name = n;
            this.value = v;
        }
    }
}
