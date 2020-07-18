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
    /// Логика взаимодействия для Fabricators.xaml
    /// </summary>
    public partial class Fabricators : MetroWindow
    {
        public Fabricator fabricator;
        Entities entities;
        ObservableCollection<Fabricator> listFabricators;
        bool create = true;
        string name = "";

        public Fabricators(ObservableCollection<Fabricator> list, Fabricator fabr, Entities dataEntities)
        {
            InitializeComponent();

            this.entities = dataEntities;
            this.listFabricators = list;
            if (fabr != null)
            {
                create = false;
                FabricatorTitle.Content = "Изменение производителя";
                createFabricator.Content = "изменить";
                this.fabricator = fabr;
                name = fabr.name_fabricator;
                nameFabricatorTextBox.Text = fabr.name_fabricator;
                telFabricatorTextBox.Text = fabr.tel_fabricator.ToString();
                new TextRange(descriptionFabricatorTextBox.Document.ContentStart, descriptionFabricatorTextBox.Document.ContentEnd).Text = fabr.description_fabricator;
                innFabricatorTextBox.Text = fabr.inn_fabricator.ToString();
                sphereFabricatorTextBox.Text = fabr.doing_fabricator;
                contacteFabricatorTextBox.Text = fabr.contactName_fabricator;
            }
        }

        private void createFabricator_Click(object sender, RoutedEventArgs e)
        {
            if (nameFabricatorTextBox.Text != null)
                try
                {
                    fabricator = new Fabricator();
                    fabricator.name_fabricator = nameFabricatorTextBox.Text;
                    fabricator.tel_fabricator = Convert.ToDecimal(telFabricatorTextBox.Text);
                    fabricator.inn_fabricator = Convert.ToDecimal(innFabricatorTextBox.Text);
                    fabricator.description_fabricator = new TextRange(descriptionFabricatorTextBox.Document.ContentStart, descriptionFabricatorTextBox.Document.ContentEnd).Text;
                    fabricator.doing_fabricator = sphereFabricatorTextBox.Text;
                    fabricator.contactName_fabricator = contacteFabricatorTextBox.Text;
                    

                    if(create)
                    {
                        entities.Fabricators.Add(fabricator);
                    }
                    else
                    {
                        foreach(Fabricator f in listFabricators)
                        {
                            if (f.name_fabricator == name)
                            {
                                f.name_fabricator = fabricator.name_fabricator;
                                f.inn_fabricator = fabricator.inn_fabricator;
                                f.tel_fabricator = fabricator.tel_fabricator;
                                f.description_fabricator = fabricator.description_fabricator;
                                f.doing_fabricator = fabricator.doing_fabricator;
                                f.contactName_fabricator = fabricator.contactName_fabricator;
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

        private void cancelFabricator_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void innFabricatorTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !(Char.IsDigit(e.Text, 0));
        }
    }
}
