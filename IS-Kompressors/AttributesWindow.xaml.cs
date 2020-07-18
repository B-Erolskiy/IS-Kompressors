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
    /// Логика взаимодействия для AttributesWindow.xaml
    /// </summary>
    public partial class AttributesWindow : MetroWindow
    {
        public Attribute attribute;
        Entities entities;
        ObservableCollection<Attribute> listAttributes;
        bool create = true;
        string name = "";

        public AttributesWindow(ObservableCollection<Attribute> list, Attribute att, Entities dataEntities)
        {
            InitializeComponent();

            this.entities = dataEntities;
            this.listAttributes = list;
            if (att != null)
            {
                create = false;
                AttributesTitle.Content = "Изменение  характеристики";
                createAttributes.Content = "изменить";
                this.attribute = att;
                name = att.name_attribute;
                nameAttributesTextBox.Text = att.name_attribute;
                new TextRange(descriptionAttributesTextBox.Document.ContentStart, descriptionAttributesTextBox.Document.ContentEnd).Text = att.description_attribute;
            }
        }

        private void createAttributes_Click(object sender, RoutedEventArgs e)
        {
            if (nameAttributesTextBox.Text != null)
                try
                {
                    attribute = new Attribute();
                    attribute.name_attribute = nameAttributesTextBox.Text;
                    attribute.description_attribute = new TextRange(descriptionAttributesTextBox.Document.ContentStart, descriptionAttributesTextBox.Document.ContentEnd).Text;


                    if (create)
                    {
                        entities.Attributes.Add(attribute);
                    }
                    else
                    {
                        foreach (Attribute a in listAttributes)
                        {
                            if (a.name_attribute == name)
                            {
                                a.name_attribute = attribute.name_attribute;
                                a.description_attribute = attribute.description_attribute;
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
            else
            {
                this.ShowMessageAsync("Ошибка", "Возможно, вы не заполнили некоторые обязательные поля");
            }
        }

        private void cancelAttributes_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
}
}
