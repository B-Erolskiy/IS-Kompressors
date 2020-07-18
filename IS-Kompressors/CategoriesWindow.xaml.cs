using System;
using System.Windows;
using System.Windows.Documents;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Collections.ObjectModel;

namespace IS_Kompressors
{
    /// <summary>
    /// Логика взаимодействия для CategoriesWindow.xaml
    /// </summary>
    public partial class CategoriesWindow : MetroWindow
    {
        public Сategories category;
        Entities entities;
        ObservableCollection<Сategories> listCategories;
        bool create = true;
        string name = "";

        public CategoriesWindow(ObservableCollection<Сategories> list, Сategories cat, Entities dataEntities)
        {
            InitializeComponent();

            this.entities = dataEntities;
            this.listCategories = list;
            if (cat != null)
            {
                create = false;
                CategoriesTitle.Content = "Изменение  категории";
                createCategory.Content = "изменить";
                this.category = cat;
                name = cat.name_category;
                nameCategoryTextBox.Text = cat.name_category;
                new TextRange(descriptionCategoryTextBox.Document.ContentStart, descriptionCategoryTextBox.Document.ContentEnd).Text = cat.description_category;
            }
        }

        private void createCategory_Click(object sender, RoutedEventArgs e)
        {
            if (nameCategoryTextBox.Text != null)
                try
                {
                    category = new Сategories();
                    category.name_category = nameCategoryTextBox.Text;
                    category.description_category = new TextRange(descriptionCategoryTextBox.Document.ContentStart, descriptionCategoryTextBox.Document.ContentEnd).Text;


                    if (create)
                    {
                        entities.Сategories.Add(category);
                    }
                    else
                    {
                        foreach (Сategories c in listCategories)
                        {
                            if (c.name_category == name)
                            {
                                c.name_category = category.name_category;
                                c.description_category = category.description_category;
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

        private void cancelCategory_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
