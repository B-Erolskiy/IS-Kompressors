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
using System.Windows.Controls.DataVisualization.Charting;
using System.Collections.Generic;
using System.Windows.Media;
using Microsoft.Office.Interop.Excel;
using System.Windows.Controls.DataVisualization;
using System.Windows.Documents;

namespace IS_Kompressors
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        //вся БД
        public static Entities dataEntities = new Entities();

        //сохранение в word
        Microsoft.Office.Interop.Word._Application oWord = new Microsoft.Office.Interop.Word.Application();

        //сохранение в excel
        Microsoft.Office.Interop.Excel.Application excelA = new Microsoft.Office.Interop.Excel.Application();
        string uri = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "templateExcel.xlsx";

        //фигуры
        ObservableCollection<KeyValuePair<string, decimal>> ListPerEquipmentSales = new ObservableCollection<KeyValuePair<string, decimal>>();
        ObservableCollection<KeyValuePair<string, decimal>> ListPerEquipmentPosts = new ObservableCollection<KeyValuePair<string, decimal>>();
        ObservableCollection<KeyValuePair<string, decimal>> ListPerPartsSales = new ObservableCollection<KeyValuePair<string, decimal>>();
        ObservableCollection<KeyValuePair<string, decimal>> ListPerPartsPosts = new ObservableCollection<KeyValuePair<string, decimal>>();
        ObservableCollection<KeyValuePair<string, decimal>> ListPerFabricatorEquipment = new ObservableCollection<KeyValuePair<string, decimal>>();
        ObservableCollection<KeyValuePair<string, decimal>> ListPerCategoryEquipment = new ObservableCollection<KeyValuePair<string, decimal>>();
        Komp[] komp = new Komp[5];
        Komp[] part = new Komp[5];

        //окно редактирования стилей
        private MetroWindow accentThemeTestWindow;

        //коллекции для DataGrid
        ObservableCollection<Equipment> ListEquipment = new ObservableCollection<Equipment>();
        ObservableCollection<Attribute> ListAttributes = new ObservableCollection<Attribute>();
        ObservableCollection<Part> ListParts = new ObservableCollection<Part>();
        ObservableCollection<Fabricator> ListFabricators = new ObservableCollection<Fabricator>();
        ObservableCollection<Сategories> ListСategories = new ObservableCollection<Сategories>();
        ObservableCollection<Actions_Equipment> ListEquipmentSales = new ObservableCollection<Actions_Equipment>();
        ObservableCollection<Actions_Equipment> ListEquipmentBuys = new ObservableCollection<Actions_Equipment>();
        ObservableCollection<Actions_Parts> ListPartsSales = new ObservableCollection<Actions_Parts>();
        ObservableCollection<Actions_Parts> ListPartsBuys = new ObservableCollection<Actions_Parts>();
        ObservableCollection<Actions_Parts> ListActions_Parts = new ObservableCollection<Actions_Parts>();
        ObservableCollection<Attributes_Values> ListAttributes_Values = new ObservableCollection<Attributes_Values>();



        public MainWindow()
        {
            //цвет темы из настроек
            var application = System.Windows.Application.Current;
            var theme = ThemeManager.DetectAppStyle(application);
            if (Properties.Settings.Default.NewColor)
            {
                ThemeManagerHelper.CreateAppStyleBy(Properties.Settings.Default.Color, true);
                application.MainWindow.Activate();
            }
            else
                ThemeManager.ChangeAppStyle(application, ThemeManager.GetAccent(Properties.Settings.Default.Accent), theme.Item1);

            this.FontFamily = Properties.Settings.Default.FontFamily;
            this.FontSize = Convert.ToDouble(Properties.Settings.Default.FontSize);
            this.FontStyle = Properties.Settings.Default.FontStyle;
            this.FontWeight = Properties.Settings.Default.FontWeight;
            this.FontStretch = Properties.Settings.Default.FontStretch;

            InitializeComponent();

            //авторизация
            ShowLoginDialog();

            GetEquipment();
            GetParts();
            GetFabricators();
            GetСategories();
            GetActions_Equipment();
            GetActions_Parts();
            GetAttributes();

            /*ButtonAutomationPeer peer = new ButtonAutomationPeer();
            HamburgerMenuControl_OnItemClick(null, null);*/


            //изображения для управления БД
            imageSoursesAdd();
        }

        //
        //авторизация
        //
        private void ShowLoginDialog()
        {
            AccountWindow aw = new AccountWindow(dataEntities);
            aw.ShowDialog();
            if(aw.DialogResult != true)
            {
                this.Close();
            }
        }

        //
        //Главная страница
        //

        //Ссылка на оборудование
        private void tileEquipment_Click(object sender, RoutedEventArgs e)
        {
            HamburgerMenuControl.SelectedItem = menuEquipment;
            this.HamburgerMenuControl.Content = menuEquipment;
        }

        //Ссылка на запчасти
        private void tileParts_Click(object sender, RoutedEventArgs e)
        {
            HamburgerMenuControl.SelectedItem = menuParts;
            this.HamburgerMenuControl.Content = menuParts;
        }

        //Ссылка на клиенты
        private void tileKlients_Click(object sender, RoutedEventArgs e)
        {
            HamburgerMenuControl.SelectedItem = menuKlients;
            this.HamburgerMenuControl.Content = menuKlients;
        }

        //Ссылка на категории
        private void tileCategories_Click(object sender, RoutedEventArgs e)
        {
            HamburgerMenuControl.SelectedItem = menuCategories;
            this.HamburgerMenuControl.Content = menuCategories;
        }

        //Ссылка на настройки
        private void tileSettings_Click(object sender, RoutedEventArgs e)
        {
            HamburgerMenuControl.SelectedItem = menuSettings;
            this.HamburgerMenuControl.Content = menuSettings;
        }

        //Ссылка на настройки
        private void tileDiagrams_Click(object sender, RoutedEventArgs e)
        {
            HamburgerMenuControl.SelectedItem = menuGraphics;
            this.HamburgerMenuControl.Content = menuGraphics;
        }

        //Ссылка на о программе
        private void tileAbout_Click(object sender, RoutedEventArgs e)
        {
            HamburgerMenuControl.SelectedIndex = -1;
            this.HamburgerMenuControl.Content = menuAbout;
        }


        //
        //Получение данных из БД
        //


        private void GetEquipment()
        {
            ListEquipment.Clear();
            var queryEquipment = from item in dataEntities.Equipments
                                 orderby item.id_equipment
                                 select item;
            foreach (Equipment item in queryEquipment)
            {
                
                ListEquipment.Add(item);
            }
            int i = 0;
            komp = new Komp[ListEquipment.Count];
            dataGridEquipment.ItemsSource = ListEquipment;
            searchPartsEquipment.ItemsSource = ListEquipment;
            foreach (Equipment item in queryEquipment)
            {
                komp[i] = new Komp(item.code_equipment);
                i++;
            }
        }

        private void GetParts()
        {
            ListParts.Clear();
            var queryParts = from item in dataEntities.Parts
                                 orderby item.code_part
                                 select item;
            foreach (Part item in queryParts)
            {
                ListParts.Add(item);
            }
            dataGridParts.ItemsSource = ListParts;
            
            int i = 0;
            part = new Komp[ListParts.Count];
            foreach (Part item in queryParts)
            {
                part[i] = new Komp(item.code_part);
                i++;
            }
        }

        private void GetFabricators()
        {
            ListFabricators.Clear();
            ListPerFabricatorEquipment.Clear();
            var queryFabricators = from item in dataEntities.Fabricators
                                 orderby item.id_fabricator
                                 select item;
            foreach (Fabricator item in queryFabricators)
            {
                ListFabricators.Add(item);

                string name = item.name_fabricator;
                decimal col = item.Equipments.Count;

                ListPerFabricatorEquipment.Add(new KeyValuePair<string, decimal>(name, col));
            }
            dataGridFabricators.ItemsSource = ListFabricators;
            searchEquipmentFabricator.ItemsSource = ListFabricators;
            chartPerFabricatorEquipment.DataContext = ListPerFabricatorEquipment;
            searchPartsFabricator.ItemsSource = ListFabricators;
        }

        private void GetAttributes()
        {
            ListAttributes.Clear();
            var queryAttributes = from item in dataEntities.Attributes
                                  orderby item.id_attribute
                                   select item;
            foreach (Attribute item in queryAttributes)
            {
                ListAttributes.Add(item);
            }
            dataGridAttributes.ItemsSource = ListAttributes;
        }

        private void GetAttributes_Values()
        {
            var queryA = from item in dataEntities.Attributes_Values
                                         orderby item.id_aValues
                                         select item;
            foreach (Attributes_Values item in queryA)
            {
                if (item.equipment_aValues == null && item.Equipment != null)
                    item.equipment_aValues = item.Equipment.id_equipment;
                ListAttributes_Values.Add(item);
            }
        }

        private void GetСategories()
        {
            ListСategories.Clear();
            ListPerCategoryEquipment.Clear();
            var queryCategories = from item in dataEntities.Сategories
                                 orderby item.id_category
                                 select item;
            foreach (Сategories item in queryCategories)
            {
                ListСategories.Add(item);

                string name = item.name_category;
                decimal col = item.Equipments.Count;

                ListPerCategoryEquipment.Add(new KeyValuePair<string, decimal>(name, col));
            }
            dataGridCategories.ItemsSource = ListСategories;
            searchEquipmentCategories.ItemsSource = ListСategories;
            chartPerCategoryEquipment.DataContext = ListPerCategoryEquipment;
            searchPartsCategories.ItemsSource = ListСategories;
        }

        //БД действия по оборудованию + диаграммы
        private void GetActions_Equipment()
        {
            ListPerEquipmentSales.Clear();
            ListPerEquipmentPosts.Clear();

            ListEquipmentSales.Clear();
            ListEquipmentBuys.Clear();
            var queryActions_Equipment = from item in dataEntities.Actions_Equipment
                                         orderby item.id_aEquipment
                                 select item;
            
            foreach (Actions_Equipment item in queryActions_Equipment)
            {
                if (item.type_aEquipment == true)
                {
                    if (item.equipment_aEquipment == null && item.Equipment != null)
                        item.equipment_aEquipment = item.Equipment.id_equipment;
                    ListEquipmentSales.Add(item);
                    for(int i=0;i<komp.Count();i++)
                    {
                        if (item.Equipment != null)
                            if (komp[i].name == item.Equipment.code_equipment)
                                komp[i].sale += item.price_aEquipment;
                    }
                }
                else
                {
                    if (item.equipment_aEquipment == null && item.Equipment != null)
                        item.equipment_aEquipment = item.Equipment.id_equipment;
                    ListEquipmentBuys.Add(item);
                    for (int i = 0; i < komp.Count(); i++)
                    {
                        if(item.Equipment != null)
                            if (komp[i].name == item.Equipment.code_equipment)
                                komp[i].post += item.price_aEquipment;
                    }
                }
            }
            decimal max = 0;
            decimal sumPost = 0;
            decimal sumSale = 0;
            for(int i=0;i<komp.Count();i++)
            {
                if (komp[i].sale > max)
                    max = komp[i].sale;
                if (komp[i].post > max)
                    max = komp[i].post;
                ListPerEquipmentPosts.Add(new KeyValuePair<string, decimal>(komp[i].name, komp[i].post));
                sumPost += komp[i].post;
                ListPerEquipmentSales.Add(new KeyValuePair<string, decimal>(komp[i].name, komp[i].sale));
                sumSale += komp[i].sale;
            }

            sumPostsLabel.Text = "Всего за данный период поставлено на " + String.Format("{0:0.00}", sumPost) + " рублей";
            sumSalesLabel.Text = "Всего за данный период продано на " + String.Format("{0:0.00}", sumSale) + " рублей";
            decimal result = sumSale - sumPost;
            if(result>0)
            {
                topPolygon.Visibility = Visibility.Visible;
                centerLine.Visibility = Visibility.Hidden;
                bottomPolygon.Visibility = Visibility.Hidden;
                resultPostSalesLabel.Content = "+ " + String.Format("{0:0}", result) + " рублей";
                resultPostSalesLabel.Foreground = new SolidColorBrush(Colors.Green);
            }
            else if (result < 0)
            {
                topPolygon.Visibility = Visibility.Hidden;
                centerLine.Visibility = Visibility.Hidden;
                bottomPolygon.Visibility = Visibility.Visible;
                resultPostSalesLabel.Content = String.Format("{0:0}", result) + " рублей";
                resultPostSalesLabel.Foreground = new SolidColorBrush(Colors.Red);
            }
            else
            {
                topPolygon.Visibility = Visibility.Hidden;
                centerLine.Visibility = Visibility.Visible;
                bottomPolygon.Visibility = Visibility.Hidden;
                resultPostSalesLabel.Content = String.Format("{0:0}", result) + " рублей";
                resultPostSalesLabel.Foreground = new SolidColorBrush(Colors.Gray);
            }
            equipmentBuysEq.ItemsSource = ListEquipment;
            equipmentSalesEq.ItemsSource = ListEquipment;
            dataGridEquipmentSales.ItemsSource = ListEquipmentSales;
            dataGridEquipmentBuys.ItemsSource = ListEquipmentBuys;

            //гистограмма
            max = max + max/10;
            lin1.Maximum = Convert.ToDouble(max);
            lin2.Maximum = Convert.ToDouble(max);
            lin1.Visibility = Visibility.Hidden;
            chartPerEquipmentSales.DataContext = ListPerEquipmentSales;
            chartPerEquipmentPost.DataContext = ListPerEquipmentPosts;
        }

        //БД действия по запчастям + диаграммы
        private void GetActions_Parts()
        {
            ListPerPartsSales.Clear();
            ListPerPartsPosts.Clear();

            ListPartsSales.Clear();
            ListPartsBuys.Clear();
            var queryActions_Equipment = from item in dataEntities.Actions_Parts
                                         orderby item.id_aParts
                                         select item;

            foreach (Actions_Parts item in queryActions_Equipment)
            {
                if (item.type_aParts == true)
                {
                    if (item.part_aParts == null && item.Part != null)
                        item.part_aParts = item.Part.id_part;
                    ListPartsSales.Add(item);
                    for (int i = 0; i < part.Count(); i++)
                    {
                        if (item.Part != null)
                            if (part[i].name == item.Part.code_part)
                                part[i].sale += item.price_aParts;
                    }
                }
                else
                {
                    if (item.part_aParts == null && item.Part != null)
                        item.part_aParts = item.Part.id_part;
                    ListPartsBuys.Add(item);
                    for (int i = 0; i < part.Count(); i++)
                    {
                        if (item.Part != null)
                            if (part[i].name == item.Part.code_part)
                                part[i].post += item.price_aParts;
                    }
                }
            }
            decimal max = 0;
            decimal sumPost = 0;
            decimal sumSale = 0;
            for (int i = 0; i < part.Count(); i++)
            {
                if (part[i].sale > max)
                    max = part[i].sale;
                if (part[i].post > max)
                    max = part[i].post;
                ListPerPartsPosts.Add(new KeyValuePair<string, decimal>(part[i].name, part[i].post));
                sumPost += part[i].post;
                ListPerPartsSales.Add(new KeyValuePair<string, decimal>(part[i].name, part[i].sale));
                sumSale += part[i].sale;
            }

            sumPartsPostsLabel.Text = "Всего за данный период поставлено на " + String.Format("{0:0.00}", sumPost) + " рублей";
            sumPartsSalesLabel.Text = "Всего за данный период продано на " + String.Format("{0:0.00}", sumSale) + " рублей";
            decimal result = sumSale - sumPost;
            if (result > 0)
            {
                topPartsPolygon.Visibility = Visibility.Visible;
                centerPartsLine.Visibility = Visibility.Hidden;
                bottomPartsPolygon.Visibility = Visibility.Hidden;
                resultPartsPostSalesLabel.Content = "+ " + String.Format("{0:0}", result) + " рублей";
                resultPartsPostSalesLabel.Foreground = new SolidColorBrush(Colors.Green);
            }
            else if (result < 0)
            {
                topPartsPolygon.Visibility = Visibility.Hidden;
                centerPartsLine.Visibility = Visibility.Hidden;
                bottomPartsPolygon.Visibility = Visibility.Visible;
                resultPartsPostSalesLabel.Content = String.Format("{0:0}", result) + " рублей";
                resultPartsPostSalesLabel.Foreground = new SolidColorBrush(Colors.Red);
            }
            else
            {
                topPartsPolygon.Visibility = Visibility.Hidden;
                centerPartsLine.Visibility = Visibility.Visible;
                bottomPartsPolygon.Visibility = Visibility.Hidden;
                resultPartsPostSalesLabel.Content = String.Format("{0:0}", result) + " рублей";
                resultPartsPostSalesLabel.Foreground = new SolidColorBrush(Colors.Gray);
            }
            PartsBuysEq.ItemsSource = ListParts;
            PartsSalesEq.ItemsSource = ListParts;
            dataGridPartsSales.ItemsSource = ListPartsSales;
            dataGridPartsBuys.ItemsSource = ListPartsBuys;

            //гистограмма
            max = max + max / 10;
            lin3.Maximum = Convert.ToDouble(max);
            lin4.Maximum = Convert.ToDouble(max);
            lin3.Visibility = Visibility.Hidden;
            chartPerPartsSales.DataContext = ListPerPartsSales;
            chartPerPartsPost.DataContext = ListPerPartsPosts;
        }

        //
        // Оборудование
        //       
        
        
        //редактирование оборудования
        private void EditEquipment_Click(object sender, RoutedEventArgs e)
        {
            Equipment equipment = dataGridEquipment.SelectedItem as Equipment;
            ObservableCollection<string> fabricators = new ObservableCollection<string>();
            ObservableCollection<string> categories = new ObservableCollection<string>();
            ObservableCollection<string> codes = new ObservableCollection<string>();

            codes.Clear();
            categories.Clear();
            fabricators.Clear();

            //все категории
            var queryCategories = from item in dataEntities.Сategories
                                  orderby item.id_category
                                  select item.name_category;
            foreach (String item in queryCategories)
            {
                categories.Add(item);
            }
            //все производители
            var queryFabricators = from item in dataEntities.Fabricators
                                   orderby item.id_fabricator
                                   select item.name_fabricator;
            foreach (String item in queryFabricators)
            {
                fabricators.Add(item);
            }
            //все существующие артикулы оборудования
            var queryCodes = from item in dataEntities.Equipments
                             orderby item.code_equipment
                             select item.code_equipment;
            foreach (String item in queryCodes)
            {
                codes.Add(item);
            }
            GetFabricators();
            GetСategories();
            ApplyEffect(this);
            AddEquipment window = new AddEquipment(ListEquipment, ListAttributes, dataEntities, ListFabricators, ListСategories, codes, equipment);
            window.ShowDialog();
            if (window.DialogResult == true)
            {
                dataGridEquipment.SelectedIndex = -1;
                GetEquipment();
                GetActions_Equipment();
                GetFabricators();
                GetСategories();
                this.ShowMessageAsync("Оборудование изменено", "Вы можете найти его во вкладке Оборудование->Склад ");
            }
            else
            {
                this.ShowMessageAsync("Отмена изменения оборудования", "Вы можете изменить товар во вкладке Оборудование->Склад->Изменить оборудование ");
            }
            ClearEffect(this);
        }

        //добавление оборудования
        private void AddEquipment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ObservableCollection<string> fabricators = new ObservableCollection<string>();
                ObservableCollection<string> codes = new ObservableCollection<string>();

                codes.Clear();
                fabricators.Clear();
                //все производители
                var queryFabricators = from item in dataEntities.Fabricators
                                       orderby item.id_fabricator
                                       select item.name_fabricator;
                foreach (string item in queryFabricators)
                {
                    fabricators.Add(item);
                }
                //все существующие артикулы оборудования
                var queryCodes = from item in dataEntities.Equipments
                                       orderby item.code_equipment
                                       select item.code_equipment;
                foreach (string item in queryCodes)
                {
                    codes.Add(item);
                }
                GetFabricators();
                GetСategories();
                ApplyEffect(this);
                AddEquipment window = new AddEquipment(ListEquipment, ListAttributes, dataEntities, ListFabricators, ListСategories, codes, null);
                window.ShowDialog();
                if (window.DialogResult == true)
                {
                    dataGridEquipment.SelectedIndex = -1;
                    GetEquipment();
                    GetActions_Equipment();
                    GetFabricators();
                    GetСategories();
                    this.ShowMessageAsync("Оборудование добавлено", "Вы можете найти его во вкладке Оборудование->Склад ");
                }
                else
                    this.ShowMessageAsync("Отмена добавления оборудования", "Вы можете добавить товар во вкладке Оборудование->Склад->Добавить оборудование ");
                ClearEffect(this);
            }
            catch (Exception ex)
            {
                this.ShowMessageAsync("Ошибка!", ex.ToString());
                ClearEffect(this);
            }
        }

        //удаление оборудования
        private void DeleteEquipment_Click(object sender, RoutedEventArgs e)
        {
            Equipment equipment = dataGridEquipment.SelectedItem as Equipment;
            if (equipment != null)
            {
                var s = MessageBox.Show("Удалить оборудование с кодом " + equipment.code_equipment + " ?", "Вы уверены?", MessageBoxButton.YesNo);
                if (s == MessageBoxResult.Yes)
                {
                    dataEntities.Equipments.Remove(equipment);
                    dataEntities.SaveChanges();
                    GetEquipment();
                    GetActions_Equipment();
                    GetFabricators();
                    GetСategories();
                    this.ShowMessageAsync("Оборудование удалено", "");
                    dataGridEquipment.SelectedIndex = -1;
                }
            }
            else
            {
                this.ShowMessageAsync("Ошибка!", "Выберите строку для удаления");
            }
        }

        //загрузка списка оборудования
        private void DownloadEquipment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "XLSX files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
                if (saveFileDialog.ShowDialog() == true)
                {
                    Workbook excelB = excelA.Workbooks.Add(uri);
                    Worksheet excelS = (Worksheet)excelB.Sheets[1];
                    excelS.Columns.AutoFit();
                    ObservableCollection<string> Headers = new ObservableCollection<string>
                    { "Артикул","Наименование","Описание","Производитель","Категория","Количество на складе","Цена","Запчасти", "Характеристики"};

                    for (int i = 0; i < Headers.Count; i++)
                    {
                        Microsoft.Office.Interop.Excel.Range myRange = (Microsoft.Office.Interop.Excel.Range)excelS.Cells[1, i + 1];
                        excelS.Cells[1, i + 1].Font.Bold = true;
                        excelS.Columns[i + 1].ColumnWidth = 25;
                        myRange.Value2 = Headers.ElementAt(i);
                    }
                    for (int i = 0; i < ListEquipment.Count; i++)
                    {
                        int j = 0;
                        dataGridEquipment.SelectedIndex = i;
                        Equipment eq = dataGridEquipment.SelectedItem as Equipment;

                        Microsoft.Office.Interop.Excel.Range myRange;
                        myRange = (Microsoft.Office.Interop.Excel.Range)excelS.Cells[i + 2, j + 1];
                        myRange.Value2 = eq.code_equipment;
                        j++;
                        myRange = (Microsoft.Office.Interop.Excel.Range)excelS.Cells[i + 2, j + 1];
                        myRange.Value2 = eq.name_equipment;
                        j++;
                        myRange = (Microsoft.Office.Interop.Excel.Range)excelS.Cells[i + 2, j + 1];
                        myRange.Value2 = eq.description_equipment;
                        j++;
                        myRange = (Microsoft.Office.Interop.Excel.Range)excelS.Cells[i + 2, j + 1];
                        myRange.Value2 = eq.Fabricator.name_fabricator;
                        j++;
                        myRange = (Microsoft.Office.Interop.Excel.Range)excelS.Cells[i + 2, j + 1];
                        myRange.Value2 = eq.Сategories.name_category;
                        j++;
                        myRange = (Microsoft.Office.Interop.Excel.Range)excelS.Cells[i + 2, j + 1];
                        myRange.Value2 = eq.col_equipment;
                        j++;
                        myRange = (Microsoft.Office.Interop.Excel.Range)excelS.Cells[i + 2, j + 1];
                        myRange.Value2 = eq.price_equipment;
                        j++;
                        myRange = (Microsoft.Office.Interop.Excel.Range)excelS.Cells[i + 2, j + 1];
                        for (int x=0; x < eq.Parts.Count; x++)
                        {
                            Part p = eq.Parts.ElementAt(x) as Part;
                            myRange.Value2 += p.name_part + "\n";
                        }
                        j++;
                        myRange = (Microsoft.Office.Interop.Excel.Range)excelS.Cells[i + 2, j + 1];
                        for (int x = 0; x < eq.Attributes_Values.Count; x++)
                        {
                            Attributes_Values p = eq.Attributes_Values.ElementAt(x) as Attributes_Values;
                            myRange.Value2 += p.Attribute.name_attribute + ": " + p.value_aValues + "\n";
                        }
                        j++;
                    }
                    excelB.SaveAs(saveFileDialog.FileName);
                    excelA.Quit();

                    string message = "Данные успешно записались в Excel файл по этому пути: " + saveFileDialog.FileName;
                    string caption = "Файл создан";
                    this.ShowMessageAsync(caption, message);

                }
            }
            catch (Exception ex)
            {
                this.ShowMessageAsync("Ошибка!",ex.ToString());
            }
        }

        //поиск оборудования по имени
        private void searchEquipment_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (searchEquipmentName.Text != "")
            {
                ListEquipment.Clear();
                var queryEquipment = from item in dataEntities.Equipments
                                     orderby item.code_equipment
                                     select item;
                foreach (Equipment item in queryEquipment)
                {
                    if (item.name_equipment.ToLower().Contains(searchEquipmentName.Text) ||
                        item.name_equipment.ToUpper().Contains(searchEquipmentName.Text) ||
                        item.name_equipment.Contains(searchEquipmentName.Text))
                        ListEquipment.Add(item);
                }
            }
            else
            { GetEquipment(); GetActions_Equipment(); }
        }

        //поиск оборудования по артикулу
        private void searchEquipmentCode_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (searchEquipmentCode.Text != "")
            {
                ListEquipment.Clear();
                var queryEquipment = from item in dataEntities.Equipments
                                     orderby item.code_equipment
                                     select item;
                foreach (Equipment item in queryEquipment)
                {
                    if (item.code_equipment.ToLower().Contains(searchEquipmentCode.Text) ||
                        item.code_equipment.ToUpper().Contains(searchEquipmentCode.Text) ||
                        item.code_equipment.Contains(searchEquipmentCode.Text))
                        ListEquipment.Add(item);
                }
            }
            else
            { GetEquipment(); GetActions_Equipment(); }
        }

        //поиск оборудования по цене
        private void searchEquipmentPrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (searchEquipmentPrice.Text != "")
            {
                ListEquipment.Clear();
                var queryEquipment = from item in dataEntities.Equipments
                                     orderby item.code_equipment
                                     select item;
                foreach (Equipment item in queryEquipment)
                {
                    if (item.price_equipment.ToString().Contains(searchEquipmentPrice.Text))
                        ListEquipment.Add(item);
                }
            }
            else { GetEquipment(); GetActions_Equipment(); }
        }

        //поиск оборудованию по количеству на складе
        private void searchEquipmentCol_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (searchEquipmentCol.Text != "")
            {
                ListEquipment.Clear();
                var queryEquipment = from item in dataEntities.Equipments
                                     orderby item.code_equipment
                                     select item;
                foreach (Equipment item in queryEquipment)
                {
                    if (item.col_equipment.ToString().Contains(searchEquipmentCol.Text))
                        ListEquipment.Add(item);
                }
            }
            else { GetEquipment(); GetActions_Equipment(); }
        }

        //поиск оборудования по категории
        private void searchEquipmentCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (searchEquipmentCategories.SelectedIndex != -1)
            {
                ListEquipment.Clear();
                var queryEquipment = from item in dataEntities.Equipments
                                     orderby item.code_equipment
                                     select item;
                foreach (Equipment item in queryEquipment)
                {
                    Equipment equ = new Equipment();
                    if (item.category_equipment == Convert.ToInt32(searchEquipmentCategories.SelectedValue))
                        ListEquipment.Add(item);
                }
            }
            else { GetEquipment(); GetActions_Equipment(); }
        }

        //поиск оборудования по производителю
        private void searchEquipmentFabricator_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (searchEquipmentFabricator.SelectedIndex != -1)
            {
                ListEquipment.Clear();
                var queryEquipment = from item in dataEntities.Equipments
                                     orderby item.code_equipment
                                     select item;
                foreach (Equipment item in queryEquipment)
                {
                    if (item.fabricator_equipment == Convert.ToInt32(searchEquipmentFabricator.SelectedValue))
                        ListEquipment.Add(item);
                }
            }
            else { GetEquipment(); GetActions_Equipment(); }
        }

        //выбор критерия поиска
        private void searchEquipmentChoise_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GetEquipment();
            GetActions_Equipment();

            //скрываем весь поиск
            searchEquipmentCode.Visibility = Visibility.Hidden;
            searchEquipmentName.Visibility = Visibility.Hidden;
            searchEquipmentFabricator.Visibility = Visibility.Hidden;
            searchEquipmentCategories.Visibility = Visibility.Hidden;
            searchEquipmentCol.Visibility = Visibility.Hidden;
            searchEquipmentPrice.Visibility = Visibility.Hidden;

            if (searchEquipmentChoise.SelectedIndex == 0)
            {
                searchEquipmentCode.Visibility = Visibility.Visible;
            }
            if (searchEquipmentChoise.SelectedIndex == 1)
            {
                searchEquipmentName.Visibility = Visibility.Visible;
            }
            if (searchEquipmentChoise.SelectedIndex == 2)
            {
                searchEquipmentFabricator.Visibility = Visibility.Visible;
            }
            if (searchEquipmentChoise.SelectedIndex == 3)
            {
                searchEquipmentCategories.Visibility = Visibility.Visible;
            }
            if (searchEquipmentChoise.SelectedIndex == 4)
            {
                searchEquipmentCol.Visibility = Visibility.Visible;
            }
            if (searchEquipmentChoise.SelectedIndex == 5)
            {
                searchEquipmentPrice.Visibility = Visibility.Visible;
            }

        }

        //ввод цены в поле (валидация)
        private void searchEquipmentPrice_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !((Char.IsDigit(e.Text, 0) || ((e.Text == System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0].ToString()) && (DS_Count(((System.Windows.Controls.TextBox)sender).Text) < 1))));
        }

        //подсчет количество разделителей для decimal
        public int DS_Count(string s)
        {
            string substr = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0].ToString();
            int count = (s.Length - s.Replace(substr, "").Length) / substr.Length;
            return count;
        }

        //ввод количества оборудования на складе (валидация)
        private void searchEquipmentCol_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !(Char.IsDigit(e.Text, 0));
        }

        //поиск оборудования - сброс
        private void searchEquipmentClear_Click(object sender, RoutedEventArgs e)
        {
            searchEquipmentCode.Text = "";
            searchEquipmentName.Text = "";
            searchEquipmentFabricator.SelectedIndex = -1;
            searchEquipmentCategories.SelectedIndex = -1;
            searchEquipmentCol.Text = "";
            searchEquipmentPrice.Text = "";
        }

        //вывод информации об оборудовании
        private void dataGridEquipment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (dataGridEquipment.SelectedIndex != -1)
                {
                    Equipment item = dataGridEquipment.SelectedItem as Equipment;
                    nameEquipmentLabel.Content = item.name_equipment;
                    codeEquipmentLabel.Content = "Артикул: " + item.code_equipment;
                    descriptionEquipmentLabel.Text = item.description_equipment;
                    priceEquipmentLabel.Content = "Цена: " + String.Format("{0:0.00}", item.price_equipment) + " рублей";
                    colEquipmentLabel.Content = "Количество на складе: " + item.col_equipment;
                    fabricatorEquipmentLabel.Content = "Производитель: ";
                    categoryEquipmentLabel.Content = "Категория: ";
                    foreach (Fabricator f in ListFabricators)
                    {
                        if (f.id_fabricator == item.fabricator_equipment)
                            fabricatorEquipmentLabel.Content = "Производитель: " + f.name_fabricator;
                    }
                    foreach (Сategories f in ListСategories)
                    {
                        if (f.id_category == item.category_equipment)
                            categoryEquipmentLabel.Content = "Категория: " + item.Сategories.name_category;
                    }
                    partsEquipmentLabel.Text = "";
                    attributesEquipmentLabel.Text = "";
                    var queryEquipment = from equipment in dataEntities.Parts
                                         orderby equipment.code_part
                                         select equipment;
                    foreach (Part part in queryEquipment)
                    {
                        if (part.equipment_part == item.id_equipment)
                            partsEquipmentLabel.Text += part.name_part + "\n";
                    }
                    
                    var queryAtt = from equipment in dataEntities.Attributes_Values
                                         orderby equipment.id_aValues
                                         select equipment;
                    foreach (Attributes_Values av in queryAtt)
                    {
                        if (av.equipment_aValues == item.id_equipment)
                            attributesEquipmentLabel.Text += av.Attribute.name_attribute + ": " + av.value_aValues + "\n";
                    }
                    attributesEquipment.Visibility = Visibility.Visible;
                    partsEquipment.Visibility = Visibility.Visible;
                    descriptionEquipment.Visibility = Visibility.Visible;
                    uploadEquipment.IsEnabled = true;
                    copyEquipment.IsEnabled = true;
                }
                else
                {
                    nameEquipmentLabel.Content = "";
                    codeEquipmentLabel.Content = "";
                    descriptionEquipmentLabel.Text = "";
                    attributesEquipmentLabel.Text = "";
                    partsEquipmentLabel.Text = "";
                    priceEquipmentLabel.Content = "";
                    colEquipmentLabel.Content = "";
                    fabricatorEquipmentLabel.Content = "";
                    categoryEquipmentLabel.Content = "";
                    partsEquipment.Visibility = Visibility.Hidden;
                    descriptionEquipment.Visibility = Visibility.Hidden;
                    attributesEquipment.Visibility = Visibility.Hidden;
                    uploadEquipment.IsEnabled = false;
                    copyEquipment.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                this.ShowMessageAsync("Ошибка!", ex.ToString());
            }
        }

        //копирование информации об оборудовании в буфер обмена
        private void copyEquipment_Click(object sender, RoutedEventArgs e)
        {
            string textForCopy = nameEquipmentLabel.Content + "\n"
                + codeEquipmentLabel.Content + "\n"
                + categoryEquipmentLabel.Content + "\n"
                + fabricatorEquipmentLabel.Content + "\n"
                + descriptionEquipment.Content + "\n"
                + descriptionEquipmentLabel.Text + "\n"
                + priceEquipmentLabel.Content + "\n"
                + colEquipmentLabel.Content;

            Clipboard.SetText(textForCopy);
        }

        //загрузка одного оборудования в Word
        private void uploadEquipment_Click(object sender, RoutedEventArgs e)
        {
            string path = "";
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Word files (*.docx)|*.docx|All files (*.*)|*.*";
                if (saveFileDialog.ShowDialog() == true)
                {
                    path = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "templateWord.dot";
                    _Document oDoc = GetDoc(path, "equipment");
                    oDoc.SaveAs(saveFileDialog.FileName);
                    oDoc.Close();
                    this.ShowMessageAsync("Успешное сохранение", "Ваш файл сохранен по следующему пути: " + saveFileDialog.FileName);
                }
            }
            catch (Exception ex)
            {
                this.ShowMessageAsync(path, ex.ToString());
            }
        }

        //
        //продажи
        //

        //добавление продажи оборудования
        private void EquipmentSalesAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ApplyEffect(this);
                SalesBuys salesbuysWindow = new SalesBuys(dataEntities, ListEquipment, true);
                salesbuysWindow.ShowDialog();
                if (salesbuysWindow.DialogResult == true)
                {
                    dataGridEquipmentSales.SelectedIndex = -1;
                    GetEquipment();
                    GetActions_Equipment();
                    ListEquipment.Clear();
                    var queryEquipment = from item in dataEntities.Equipments
                                         orderby item.id_equipment
                                         select item;
                    foreach (Equipment item in queryEquipment)
                    {
                        if (item.id_equipment == salesbuysWindow.action.equipment_aEquipment)
                        {
                                item.col_equipment -= salesbuysWindow.action.col_aEquipment;
                        }
                        ListEquipment.Add(item);
                    }
                    dataEntities.SaveChanges();
                    GetEquipment();
                    GetActions_Equipment();
                    this.ShowMessageAsync("Продажа добавлена", "Вы можете найти ее во вкладке Оборудование->Продажи ");
                }
                else
                    this.ShowMessageAsync("Отмена добавления продажи", "Вы можете добавить продажу во вкладке Оборудование->Продажи->Добавить продажу ");
                ClearEffect(this);
            }
            catch(Exception ex)
            {
                this.ShowMessageAsync("Ошибка", ex.ToString());
                ClearEffect(this);
            }
        }

        //удаление продажи
        private void EquipmentSalesDelete_Click(object sender, RoutedEventArgs e)
        {
            Actions_Equipment sale = dataGridEquipmentSales.SelectedItem as Actions_Equipment;
            if (sale != null)
            {
                var s = MessageBox.Show("Удалить продажу номер " + sale.id_aEquipment + " ?", "Вы уверены?", MessageBoxButton.YesNo);
                if (s == MessageBoxResult.Yes)
                {
                    dataEntities.Actions_Equipment.Remove(sale);
                    dataEntities.SaveChanges();
                    GetActions_Equipment();
                    this.ShowMessageAsync("Продажа удалена", "");
                    dataGridEquipmentSales.SelectedIndex = -1;
                }
            }
            else
            {
                this.ShowMessageAsync("Ошибка!", "Выберите строку для удаления");
            }
        }

        //вывод информации о продаже
        private void dataGridEquipmentSales_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (dataGridEquipmentSales.SelectedIndex != -1)
                {
                    Actions_Equipment item = dataGridEquipmentSales.SelectedItem as Actions_Equipment;
                    if (item.person_aEquipment != null)
                        personEquipmentSalesLabel.Content = "Покупатель: " + item.person_aEquipment;
                    else
                        personEquipmentSalesLabel.Content = "Покупатель: ";
                    dateEquipmentSalesLabel.Content = "Дата продажи: " + string.Format("{0:dd/MM/yyyy}", item.date_aEquipment);
                    if (item.Equipment != null)
                        equipmentEquipmentSalesLabel.Content = "Оборудование: " + item.Equipment.name_equipment;
                    else
                        equipmentEquipmentSalesLabel.Content = "Оборудование: ";
                    colEquipmentSalesLabel.Content = "Количество: " + item.col_aEquipment;
                    priceEquipmentSalesLabel.Content = "Сумма продажи: " + string.Format("{0:0.00}", item.price_aEquipment) + " руб.";
                    pricePerEquipmentSalesLabel.Content = "(За каждое оборудование: " + String.Format("{0:0.00}", item.price_aEquipment / item.col_aEquipment) + " руб.)";
                    uploadEquipmentSales.IsEnabled = true;
                    copyEquipmentSales.IsEnabled = true;
                }
                else
                {
                    dateEquipmentSalesLabel.Content = "";
                    equipmentEquipmentSalesLabel.Content = "";
                    colEquipmentSalesLabel.Content = "";
                    priceEquipmentSalesLabel.Content = "";
                    pricePerEquipmentSalesLabel.Content = "";
                    uploadEquipmentSales.IsEnabled = false;
                    copyEquipmentSales.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                this.ShowMessageAsync("Ошибка!", ex.ToString());
            }
        }

        //
        //поставки
        //

        //добавление поставки
        private void EquipmentBuysAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ApplyEffect(this);
                SalesBuys salesbuysWindow = new SalesBuys(dataEntities, ListEquipment, false);
                salesbuysWindow.ShowDialog();
                if (salesbuysWindow.DialogResult == true)
                {
                    dataGridEquipmentBuys.SelectedIndex = -1;
                    GetEquipment();
                    GetActions_Equipment();
                    ListEquipment.Clear();
                    var queryEquipment = from item in dataEntities.Equipments
                                         orderby item.id_equipment
                                         select item;
                
                    foreach (Equipment item in queryEquipment)
                    {
                        if (item.id_equipment == salesbuysWindow.action.equipment_aEquipment)
                            {
                                item.col_equipment += salesbuysWindow.action.col_aEquipment;
                            }
                        ListEquipment.Add(item);
                    }
                    dataEntities.SaveChanges();
                    GetEquipment();
                    GetActions_Equipment();
                    this.ShowMessageAsync("Поставка добавлена", "Вы можете найти ее во вкладке Оборудование->Поставки ");
                }
                else
                    this.ShowMessageAsync("Отмена добавления поставки", "Вы можете добавить поставку во вкладке Оборудование->Поставки->Добавить поставку ");
                ClearEffect(this);
            }
            catch (Exception ex)
            {
                this.ShowMessageAsync("Ошибка", ex.ToString());
                ClearEffect(this);
            }
        }

        //удаление поставки
        private void EquipmentBuysDelete_Click(object sender, RoutedEventArgs e)
        {
            Actions_Equipment post = dataGridEquipmentBuys.SelectedItem as Actions_Equipment;
            if (post != null)
            {
                var s = MessageBox.Show("Удалить поставку номер " + post.id_aEquipment + " ?", "Вы уверены?", MessageBoxButton.YesNo);
                if (s == MessageBoxResult.Yes)
                {
                    dataEntities.Actions_Equipment.Remove(post);
                    dataEntities.SaveChanges();
                    GetActions_Equipment();
                    this.ShowMessageAsync("Поставка удалена", "");
                    dataGridEquipmentBuys.SelectedIndex = -1;
                }
            }
            else
            {
                this.ShowMessageAsync("Ошибка!", "Выберите строку для удаления");
            }
        }

        //вывод информации о поставке
        private void dataGridEquipmentBuys_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (dataGridEquipmentBuys.SelectedIndex != -1)
                {
                    Actions_Equipment item = dataGridEquipmentBuys.SelectedItem as Actions_Equipment;
                    if(item.person_aEquipment!=null)
                        personEquipmentBuysLabel.Content = "Поставщик: " + item.person_aEquipment;
                    else
                        personEquipmentBuysLabel.Content = "Поставщик: ";
                    dateEquipmentBuysLabel.Content = "Дата поставки: " + String.Format("{0:dd/MM/yyyy}", item.date_aEquipment);
                    if (item.Equipment != null)
                        equipmentEquipmentBuysLabel.Content = "Оборудование: " + item.Equipment.name_equipment;
                    else
                        equipmentEquipmentBuysLabel.Content = "Оборудование: ";
                    colEquipmentBuysLabel.Content = "Количество: " + item.col_aEquipment;
                    priceEquipmentBuysLabel.Content = "Сумма поставки: " + String.Format("{0:0.00}", item.price_aEquipment) + " руб.";
                    pricePerEquipmentBuysLabel.Content = "(За каждое оборудование: " + String.Format("{0:0.00}", item.price_aEquipment / item.col_aEquipment) + " руб.)";
                    uploadEquipmentBuys.IsEnabled = true;
                    copyEquipmentBuys.IsEnabled = true;
                }
                else
                {
                    dateEquipmentBuysLabel.Content = "";
                    equipmentEquipmentBuysLabel.Content = "";
                    colEquipmentBuysLabel.Content = "";
                    priceEquipmentBuysLabel.Content = "";
                    pricePerEquipmentBuysLabel.Content = "";
                    uploadEquipmentBuys.IsEnabled = false;
                    copyEquipmentBuys.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                this.ShowMessageAsync("Ошибка!", ex.ToString());
            }
        }


        private void EquipmentBuysDownload_Click(object sender, RoutedEventArgs e)
        {

        }

        //
        //конец раздела "Оборудование"
        //

        //
        // Запчасти
        //       


        //редактирование запчасти
        private void EditParts_Click(object sender, RoutedEventArgs e)
        {
            Part part = dataGridParts.SelectedItem as Part;
            ObservableCollection<string> equipment = new ObservableCollection<string>();
            ObservableCollection<string> codes = new ObservableCollection<string>();

            codes.Clear();
            equipment.Clear();

            //все запчасти
            var queryFabricators = from item in dataEntities.Equipments
                                   orderby item.id_equipment
                                   select item.name_equipment;
            foreach (String item in queryFabricators)
            {
                equipment.Add(item);
            }
            //все существующие артикулы запчасти
            var queryCodes = from item in dataEntities.Parts
                             orderby item.code_part
                             select item.code_part;
            foreach (String item in queryCodes)
            {
                codes.Add(item);
            }
            GetEquipment();
            PartWindow window = new PartWindow(ListEquipment, ListParts, codes, dataEntities, part);
            ApplyEffect(this);
            window.ShowDialog();
            if (window.DialogResult == true)
            {
                dataGridParts.SelectedIndex = -1;
                GetParts();
                GetActions_Parts();
                GetEquipment();
                this.ShowMessageAsync("Данные о запчасти изменены", "Вы можете найти их во вкладке Запчасти->Склад ");
            }
            else
            {
                this.ShowMessageAsync("Отмена изменения данных о запчасти", "Вы можете изменить эти данные во вкладке Запчасти->Склад->Изменить запчасть ");
            }
            ClearEffect(this);
        }

        //добавление запчасти
        private void AddParts_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ObservableCollection<string> equipment = new ObservableCollection<string>();
                ObservableCollection<string> codes = new ObservableCollection<string>();

                codes.Clear();
                equipment.Clear();

                //все запчасти
                var queryFabricators = from item in dataEntities.Equipments
                                       orderby item.id_equipment
                                       select item.name_equipment;
                foreach (String item in queryFabricators)
                {
                    equipment.Add(item);
                }
                //все существующие артикулы запчасти
                var queryCodes = from item in dataEntities.Parts
                                 orderby item.code_part
                                 select item.code_part;
                foreach (String item in queryCodes)
                {
                    codes.Add(item);
                }
                GetEquipment();
                GetActions_Equipment();
                PartWindow window = new PartWindow(ListEquipment, ListParts, codes, dataEntities, null);
                ApplyEffect(this);
                window.ShowDialog();
                if (window.DialogResult == true)
                {
                    dataGridParts.SelectedIndex = -1;
                    GetParts();
                    GetActions_Parts();
                    GetEquipment();
                    GetActions_Equipment();
                    this.ShowMessageAsync("Запчасть добавлена", "Вы можете найти ее во вкладке Запчасти->Склад ");
                }
                else
                {
                    this.ShowMessageAsync("Отмена добавления запчасти", "Вы можете добавить ее во вкладке Запчасти->Склад->Добавить запчасть ");
                }
                ClearEffect(this);
            }
            catch (Exception ex)
            {
                this.ShowMessageAsync("Ошибка!", ex.ToString());
                ClearEffect(this);
            }
        }

        //удаление запчасти
        private void DeleteParts_Click(object sender, RoutedEventArgs e)
        {
            Part part = dataGridParts.SelectedItem as Part;
            if (part != null)
            {
                var s = MessageBox.Show("Удалить запчасть с кодом " + part.code_part + " ?", "Вы уверены?", MessageBoxButton.YesNo);
                if (s == MessageBoxResult.Yes)
                {
                    dataEntities.Parts.Remove(part);
                    dataEntities.SaveChanges();
                    GetParts();
                    GetActions_Parts();
                    GetEquipment();
                    GetActions_Parts();
                    this.ShowMessageAsync("Запчасть удалена", "");
                    dataGridParts.SelectedIndex = -1;
                }
            }
            else
            {
                this.ShowMessageAsync("Ошибка!", "Выберите строку для удаления");
            }
        }

        //загрузка списка запчастей
        private void DownloadParts_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "XLSX files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
                if (saveFileDialog.ShowDialog() == true)
                {

                }
            }
            catch (Exception ex)
            {
                this.ShowMessageAsync("Ошибка!", ex.ToString());
            }
        }

        //поиск запчасти по имени
        private void searchParts_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (searchPartsName.Text != "")
            {
                ListParts.Clear();
                var queryEquipment = from item in dataEntities.Parts
                                     orderby item.code_part
                                     select item;
                foreach (Part item in queryEquipment)
                {
                    if (item.name_part.ToLower().Contains(searchPartsName.Text) ||
                        item.name_part.ToUpper().Contains(searchPartsName.Text) ||
                        item.name_part.Contains(searchPartsName.Text))
                        ListParts.Add(item);
                }
            }
            else
            { GetParts(); GetActions_Parts(); }
        }

        //поиск запчасти по артикулу
        private void searchPartsCode_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (searchPartsCode.Text != "")
            {
                ListParts.Clear();
                var queryEquipment = from item in dataEntities.Parts
                                     orderby item.code_part
                                     select item;
                foreach (Part item in queryEquipment)
                {
                    if (item.code_part.ToLower().Contains(searchPartsCode.Text) ||
                        item.code_part.ToUpper().Contains(searchPartsCode.Text) ||
                        item.code_part.Contains(searchPartsCode.Text))
                        ListParts.Add(item);
                }
            }
            else
            { GetParts(); GetActions_Parts(); }
        }

        //поиск запчасти по цене
        private void searchPartsPrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (searchPartsPrice.Text != "")
            {
                ListParts.Clear();
                var queryEquipment = from item in dataEntities.Parts
                                     orderby item.code_part
                                     select item;
                foreach (Part item in queryEquipment)
                {
                    if (item.price_part.ToString().Contains(searchPartsPrice.Text))
                        ListParts.Add(item);
                }
            }
            else { GetParts(); GetActions_Parts(); }
        }

        //поиск запчасти по количеству на складе
        private void searchPartsCol_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (searchPartsCol.Text != "")
            {
                ListParts.Clear();
                var queryEquipment = from item in dataEntities.Parts
                                     orderby item.code_part
                                     select item;
                foreach (Part item in queryEquipment)
                {
                    if (item.col_part.ToString().Contains(searchPartsCol.Text))
                        ListParts.Add(item);
                }
            }
            else { GetParts(); GetActions_Parts(); }
        }

        //поиск запчасти по категории
        private void searchPartsCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (searchPartsCategories.SelectedIndex != -1)
            {
                ListParts.Clear();
                var queryEquipment = from item in dataEntities.Parts
                                     orderby item.code_part
                                     select item;
                foreach (Part item in queryEquipment)
                {
                    if (item.Equipment.Сategories.id_category == Convert.ToInt32(searchPartsCategories.SelectedValue))
                        ListParts.Add(item);
                }
            }
            else { GetParts(); GetActions_Parts(); }
        }


        //поиск запчасти по оборудованию
        private void searchPartsEquipment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (searchPartsEquipment.SelectedIndex != -1)
            {
                ListParts.Clear();
                var queryEquipment = from item in dataEntities.Parts
                                     orderby item.code_part
                                     select item;
                foreach (Part item in queryEquipment)
                {
                    if (item.Equipment.id_equipment == Convert.ToInt32(searchPartsEquipment.SelectedValue))
                        ListParts.Add(item);
                }
            }
            else { GetParts(); GetActions_Parts(); }
        }

        //поиск запчасти по производителю
        private void searchPartsFabricator_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (searchPartsFabricator.SelectedIndex != -1)
            {
                ListParts.Clear();
                var queryEquipment = from item in dataEntities.Parts
                                     orderby item.code_part
                                     select item;
                foreach (Part item in queryEquipment)
                {
                    if (item.Equipment.Fabricator.id_fabricator == Convert.ToInt32(searchPartsFabricator.SelectedValue))
                        ListParts.Add(item);
                }
            }
            else { GetParts(); GetActions_Parts(); }
        }

        //выбор критерия поиска
        private void searchPartsChoise_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GetParts();
            GetActions_Parts();

            //скрываем весь поиск
            searchPartsCode.Visibility = Visibility.Hidden;
            searchPartsName.Visibility = Visibility.Hidden;
            searchPartsFabricator.Visibility = Visibility.Hidden;
            searchPartsCategories.Visibility = Visibility.Hidden;
            searchPartsEquipment.Visibility = Visibility.Hidden;
            searchPartsCol.Visibility = Visibility.Hidden;
            searchPartsPrice.Visibility = Visibility.Hidden;

            if (searchPartsChoise.SelectedIndex == 0)
            {
                searchPartsCode.Visibility = Visibility.Visible;
            }
            if (searchPartsChoise.SelectedIndex == 1)
            {
                searchPartsName.Visibility = Visibility.Visible;
            }
            if (searchPartsChoise.SelectedIndex == 2)
            {
                searchPartsFabricator.Visibility = Visibility.Visible;
            }
            if (searchPartsChoise.SelectedIndex == 3)
            {
                searchPartsCategories.Visibility = Visibility.Visible;
            }
            if (searchPartsChoise.SelectedIndex == 4)
            {
                searchPartsEquipment.Visibility = Visibility.Visible;
            }
            if (searchPartsChoise.SelectedIndex == 5)
            {
                searchPartsCol.Visibility = Visibility.Visible;
            }
            if (searchPartsChoise.SelectedIndex == 6)
            {
                searchPartsPrice.Visibility = Visibility.Visible;
            }
        }
        

        //вывод информации об оборудовании
        private void dataGridParts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (dataGridParts.SelectedIndex != -1)
                {
                    Part item = dataGridParts.SelectedItem as Part;
                    namePartsLabel.Content = item.name_part;
                    codePartsLabel.Content = "Артикул: " + item.code_part;
                    descriptionPartsLabel.Text = item.description_part;
                    pricePartsLabel.Content = "Цена: " + String.Format("{0:0.00}", item.price_part) + " рублей";
                    colPartsLabel.Content = "Количество на складе: " + item.col_part;
                    if (item.Equipment != null)
                        equipmentPartsLabel.Content = "Предназначена для: " + item.Equipment.name_equipment;
                    else
                        equipmentPartsLabel.Content = "Предназначена для: ";
                    if (item.Equipment != null)
                        fabricatorPartsLabel.Content = "Производитель: " + item.Equipment.Fabricator.name_fabricator;
                    else
                        fabricatorPartsLabel.Content = "Производитель: ";
                    if (item.Equipment != null)
                        categoriesPartsLabel.Content = "Категория: " + item.Equipment.Сategories.name_category;
                    else
                        categoriesPartsLabel.Content = "Категория: ";
                    /*fabricatorEquipmentLabel.Content = "Производитель: ";
                    categoryEquipmentLabel.Content = "Категория: ";
                    foreach (Fabricator f in ListFabricators)
                    {
                        if (f.id_fabricator == item.fabricator_equipment)
                            fabricatorEquipmentLabel.Content = "Производитель: " + f.name_fabricator;
                    }
                    foreach (Сategories f in ListСategories)
                    {
                        if (f.id_category == item.category_equipment)
                            categoryEquipmentLabel.Content = "Категория: " + item.Сategories.name_category;
                    }*/

                    descriptionParts.Visibility = Visibility.Visible;
                    uploadParts.IsEnabled = true;
                    copyParts.IsEnabled = true;
                }
                else
                {
                    namePartsLabel.Content = "";
                    codePartsLabel.Content = "";
                    descriptionPartsLabel.Text = "";
                    pricePartsLabel.Content = "";
                    colPartsLabel.Content = "";
                    equipmentPartsLabel.Content = "";
                    descriptionParts.Visibility = Visibility.Hidden;
                    uploadParts.IsEnabled = false;
                    copyParts.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                this.ShowMessageAsync("Ошибка!", ex.ToString());
            }
        }

        //копирование информации об оборудовании в буфер обмена
        private void copyParts_Click(object sender, RoutedEventArgs e)
        {
            string textForCopy = namePartsLabel.Content + "\n"
                + codePartsLabel.Content + "\n"
                + equipmentPartsLabel.Content + "\n"
                + fabricatorPartsLabel.Content + "\n"
                + categoriesPartsLabel.Content + "\n"
                + descriptionParts.Content + "\n"
                + descriptionPartsLabel.Text + "\n"
                + pricePartsLabel.Content + "\n"
                + colPartsLabel.Content;

            Clipboard.SetText(textForCopy);
        }

        //загрузка одного оборудования в Word
        private void uploadParts_Click(object sender, RoutedEventArgs e)
        {
            string path = "";
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Word files (*.docx)|*.docx|All files (*.*)|*.*";
                if (saveFileDialog.ShowDialog() == true)
                {
                    path = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "templateWord.dot";
                    _Document oDoc = GetDoc(path, "equipment");
                    oDoc.SaveAs(saveFileDialog.FileName);
                    oDoc.Close();
                    this.ShowMessageAsync("Успешное сохранение", "Ваш файл сохранен по следующему пути: " + saveFileDialog.FileName);
                }
            }
            catch (Exception ex)
            {
                this.ShowMessageAsync(path, ex.ToString());
            }
        }

        //
        //продажи
        //

        //добавление продажи оборудования
        private void PartsSalesAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ApplyEffect(this);
                SalesBuysParts salesbuysWindow = new SalesBuysParts(dataEntities, ListParts, true);
                salesbuysWindow.ShowDialog();
                if (salesbuysWindow.DialogResult == true)
                {
                    dataGridPartsSales.SelectedIndex = -1;
                    GetParts();
                    GetActions_Parts();
                    ListParts.Clear();
                    var queryEquipment = from item in dataEntities.Parts
                                         orderby item.id_part
                                         select item;
                    foreach (Part item in queryEquipment)
                    {
                        if (item.id_part == salesbuysWindow.action.part_aParts)
                        {
                            item.col_part -= salesbuysWindow.action.col_aParts;
                        }
                        ListParts.Add(item);
                    }
                    dataEntities.SaveChanges();
                    GetParts();
                    GetActions_Parts();
                    this.ShowMessageAsync("Продажа добавлена", "Вы можете найти ее во вкладке Запчасти->Продажи ");
                }
                else
                    this.ShowMessageAsync("Отмена добавления продажи", "Вы можете добавить продажу во вкладке Запчасти->Продажи->Добавить продажу ");
                ClearEffect(this);
            }
            catch (Exception ex)
            {
                this.ShowMessageAsync("Ошибка", ex.ToString());
                ClearEffect(this);
            }
        }

        //удаление продажи
        private void PartsSalesDelete_Click(object sender, RoutedEventArgs e)
        {
            Actions_Parts sale = dataGridPartsSales.SelectedItem as Actions_Parts;
            if (sale != null)
            {
                var s = MessageBox.Show("Удалить продажу номер " + sale.id_aParts + " ?", "Вы уверены?", MessageBoxButton.YesNo);
                if (s == MessageBoxResult.Yes)
                {
                    dataEntities.Actions_Parts.Remove(sale);
                    dataEntities.SaveChanges();
                    GetActions_Parts();
                    this.ShowMessageAsync("Продажа удалена", "");
                    dataGridPartsSales.SelectedIndex = -1;
                }
            }
            else
            {
                this.ShowMessageAsync("Ошибка!", "Выберите строку для удаления");
            }
        }

        //вывод информации о продаже
        private void dataGridPartsSales_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (dataGridPartsSales.SelectedIndex != -1)
                {
                    Actions_Parts item = dataGridPartsSales.SelectedItem as Actions_Parts;
                    if (item.person_aParts != null)
                        personPartsSalesLabel.Content = "Покупатель: " + item.person_aParts;
                    else
                        personPartsSalesLabel.Content = "Покупатель: ";
                    datePartsSalesLabel.Content = "Дата продажи: " + string.Format("{0:dd/MM/yyyy}", item.date_aParts);
                    if (item.Part != null)
                        partPartsSalesLabel.Content = "Запчасть: " + item.Part.name_part;
                    else
                        partPartsSalesLabel.Content = "Запчасть: ";
                    colPartsSalesLabel.Content = "Количество: " + item.col_aParts;
                    pricePartsSalesLabel.Content = "Сумма продажи: " + string.Format("{0:0.00}", item.price_aParts) + " руб.";
                    pricePerPartsSalesLabel.Content = "(За каждую запчасть: " + String.Format("{0:0.00}", item.price_aParts / item.col_aParts) + " руб.)";
                    uploadPartsSales.IsEnabled = true;
                    copyPartsSales.IsEnabled = true;
                }
                else
                {
                    datePartsSalesLabel.Content = "";
                    partPartsSalesLabel.Content = "";
                    colPartsSalesLabel.Content = "";
                    pricePartsSalesLabel.Content = "";
                    pricePerPartsSalesLabel.Content = "";
                    uploadPartsSales.IsEnabled = false;
                    copyPartsSales.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                this.ShowMessageAsync("Ошибка!", ex.ToString());
            }
        }

        //
        //поставки
        //

        //добавление поставки
        private void PartsBuysAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ApplyEffect(this);
                SalesBuysParts salesbuysWindow = new SalesBuysParts(dataEntities, ListParts, false);
                salesbuysWindow.ShowDialog();
                if (salesbuysWindow.DialogResult == true)
                {
                    dataGridPartsBuys.SelectedIndex = -1;
                    GetParts();
                    GetActions_Parts();
                    ListParts.Clear();
                    var queryEquipment = from item in dataEntities.Parts
                                         orderby item.id_part
                                         select item;

                    foreach (Part item in queryEquipment)
                    {
                        if (item.id_part == salesbuysWindow.action.part_aParts)
                        {
                            item.col_part += salesbuysWindow.action.col_aParts;
                        }
                        ListParts.Add(item);
                    }
                    dataEntities.SaveChanges();
                    GetParts();
                    GetActions_Parts();
                    this.ShowMessageAsync("Поставка добавлена", "Вы можете найти ее во вкладке Запчасти->Поставки ");
                }
                else
                    this.ShowMessageAsync("Отмена добавления поставки", "Вы можете добавить поставку во вкладке Запчасти->Поставки->Добавить поставку ");
                ClearEffect(this);
            }
            catch (Exception ex)
            {
                this.ShowMessageAsync("Ошибка", ex.ToString());
                ClearEffect(this);
            }
        }

        //удаление поставки
        private void PartsBuysDelete_Click(object sender, RoutedEventArgs e)
        {
            Actions_Parts post = dataGridPartsBuys.SelectedItem as Actions_Parts;
            if (post != null)
            {
                var s = MessageBox.Show("Удалить поставку номер " + post.id_aParts + " ?", "Вы уверены?", MessageBoxButton.YesNo);
                if (s == MessageBoxResult.Yes)
                {
                    dataEntities.Actions_Parts.Remove(post);
                    dataEntities.SaveChanges();
                    GetActions_Parts();
                    this.ShowMessageAsync("Поставка удалена", "");
                    dataGridPartsBuys.SelectedIndex = -1;
                }
            }
            else
            {
                this.ShowMessageAsync("Ошибка!", "Выберите строку для удаления");
            }
        }

        //вывод информации о поставке
        private void dataGridPartsBuys_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (dataGridPartsBuys.SelectedIndex != -1)
                {
                    Actions_Parts item = dataGridPartsBuys.SelectedItem as Actions_Parts;
                    if (item.person_aParts != null)
                        personPartsBuysLabel.Content = "Поставщик: " + item.person_aParts;
                    else
                        personPartsBuysLabel.Content = "Поставщик: ";
                    datePartsBuysLabel.Content = "Дата поставки: " + String.Format("{0:dd/MM/yyyy}", item.date_aParts);
                    if (item.Part != null)
                        partPartsBuysLabel.Content = "Запчасть: " + item.Part.name_part;
                    else
                        partPartsBuysLabel.Content = "Запчасть: ";
                    colPartsBuysLabel.Content = "Количество: " + item.col_aParts;
                    pricePartsBuysLabel.Content = "Сумма поставки: " + String.Format("{0:0.00}", item.price_aParts) + " руб.";
                    pricePerPartsBuysLabel.Content = "(За каждую запчасть: " + String.Format("{0:0.00}", item.price_aParts / item.col_aParts) + " руб.)";
                    uploadPartsBuys.IsEnabled = true;
                    copyPartsBuys.IsEnabled = true;
                }
                else
                {
                    datePartsBuysLabel.Content = "";
                    partPartsBuysLabel.Content = "";
                    colPartsBuysLabel.Content = "";
                    pricePartsBuysLabel.Content = "";
                    pricePerPartsBuysLabel.Content = "";
                    uploadPartsBuys.IsEnabled = false;
                    copyPartsBuys.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                this.ShowMessageAsync("Ошибка!", ex.ToString());
            }
        }


        private void PartsBuysDownload_Click(object sender, RoutedEventArgs e)
        {

        }
        
        //
        //конец раздела "Оборудование"
        //

        //нажатие на элемент бокового меню
        private void HamburgerMenuControl_OnItemClick(object sender, ItemClickEventArgs e)
        {
            if (e == null)
                HamburgerMenuControl.SelectedItem = menuMain;
            else
                // set the content
                this.HamburgerMenuControl.Content = e.ClickedItem;
            // close the pane
            this.HamburgerMenuControl.IsPaneOpen = false;
        }
        

        //
        //Настройки
        //


        //изменение цвета темы
        private void ChangeAppStyleButtonClick(object sender, RoutedEventArgs e)
        {
            if (accentThemeTestWindow != null)
            {
                accentThemeTestWindow.Activate();
                return;
            }
            ApplyEffect(this);
            accentThemeTestWindow = new AccentStyleWindow();
            accentThemeTestWindow.Owner = this;
            accentThemeTestWindow.Closed += (o, args) => accentThemeTestWindow = null;
            accentThemeTestWindow.Left = this.Left + this.ActualWidth / 2.0;
            accentThemeTestWindow.Top = this.Top + this.ActualHeight / 2.0;
            accentThemeTestWindow.Show();
            ClearEffect(this);
        }


        //изменение шрифта темы
        private void ChangeFontStyleButtonClick(object sender, RoutedEventArgs e)
        {
            if (accentThemeTestWindow != null)
            {
                accentThemeTestWindow.Activate();
                return;
            }
            ApplyEffect(this);
            ChangeFontStyle window = new ChangeFontStyle();
            window.ShowDialog();
            this.FontFamily = Properties.Settings.Default.FontFamily;
            this.FontSize = Convert.ToDouble(Properties.Settings.Default.FontSize);
            this.FontStyle = Properties.Settings.Default.FontStyle;
            this.FontWeight = Properties.Settings.Default.FontWeight;
            this.FontStretch = Properties.Settings.Default.FontStretch;
            ClearEffect(this);
        }

        //добавление шаблона
        private _Document GetDoc(string path, string type)
        {
            _Document oDoc = oWord.Documents.Add(path);
            SetTemplate(oDoc, type);
            return oDoc;
        }

        //заполнение шаблона Word
        private void SetTemplate(_Document oDoc, string type)
        {
            if(type == "equipment")
                oDoc.Bookmarks["about"].Range.Text = "об оборудовании";

            oDoc.Bookmarks["name"].Range.Text = nameEquipmentLabel.Content.ToString();
            oDoc.Bookmarks["code"].Range.Text = codeEquipmentLabel.Content.ToString();
            oDoc.Bookmarks["category"].Range.Text = categoryEquipmentLabel.Content.ToString();
            oDoc.Bookmarks["fabricator"].Range.Text = fabricatorEquipmentLabel.Content.ToString();
            oDoc.Bookmarks["description"].Range.Text = descriptionEquipmentLabel.Text.ToString();
            oDoc.Bookmarks["price"].Range.Text = priceEquipmentLabel.Content.ToString();
            oDoc.Bookmarks["col"].Range.Text = colEquipmentLabel.Content.ToString();
            oDoc.Bookmarks["attributes"].Range.Text = attributesEquipmentLabel.Text.ToString();
            oDoc.Bookmarks["parts"].Range.Text = partsEquipmentLabel.Text.ToString();
        }


        //установка локальных путей на картинки
        private void imageSoursesAdd()
        {
            //оборудование
            BitmapImage bi1 = new BitmapImage();
            bi1.BeginInit();
            bi1.UriSource = new Uri("Images/add.png", UriKind.Relative);
            bi1.EndInit();
            imageEquipmentAdd.Source = bi1;

            BitmapImage bi2 = new BitmapImage();
            bi2.BeginInit();
            bi2.UriSource = new Uri("Images/delete.png", UriKind.Relative);
            bi2.EndInit();
            imageEquipmentDelete.Source = bi2;

            BitmapImage bi3 = new BitmapImage();
            bi3.BeginInit();
            bi3.UriSource = new Uri("Images/download.png", UriKind.Relative);
            bi3.EndInit();
            imageEquipmentDownload.Source = bi3;

            BitmapImage bi6 = new BitmapImage();
            bi6.BeginInit();
            bi6.UriSource = new Uri("Images/appStyle.png", UriKind.Relative);
            bi6.EndInit();
            imageAppSlyle.Source = bi6;

            BitmapImage bi7 = new BitmapImage();
            bi7.BeginInit();
            bi7.UriSource = new Uri("Images/edit.png", UriKind.Relative);
            bi7.EndInit();
            imageEquipmentEdit.Source = bi7;

            //продажи оборудования
            BitmapImage bi9 = new BitmapImage();
            bi9.BeginInit();
            bi9.UriSource = new Uri("Images/add.png", UriKind.Relative);
            bi9.EndInit();
            imageEquipmentSalesAdd.Source = bi9;

            BitmapImage bi10 = new BitmapImage();
            bi10.BeginInit();
            bi10.UriSource = new Uri("Images/delete.png", UriKind.Relative);
            bi10.EndInit();
            imageEquipmentSalesDelete.Source = bi10;

            BitmapImage bi11 = new BitmapImage();
            bi11.BeginInit();
            bi11.UriSource = new Uri("Images/download.png", UriKind.Relative);
            bi11.EndInit();
            imageEquipmentSalesDownload.Source = bi11;

            //поставки оборудования
            BitmapImage bi13 = new BitmapImage();
            bi13.BeginInit();
            bi13.UriSource = new Uri("Images/add.png", UriKind.Relative);
            bi13.EndInit();
            imageEquipmentBuysAdd.Source = bi13;

            BitmapImage bi14 = new BitmapImage();
            bi14.BeginInit();
            bi14.UriSource = new Uri("Images/delete.png", UriKind.Relative);
            bi14.EndInit();
            imageEquipmentBuysDelete.Source = bi14;

            BitmapImage bi15 = new BitmapImage();
            bi15.BeginInit();
            bi15.UriSource = new Uri("Images/download.png", UriKind.Relative);
            bi15.EndInit();
            imageEquipmentBuysDownload.Source = bi15;

            //производители
            BitmapImage bi16 = new BitmapImage();
            bi16.BeginInit();
            bi16.UriSource = new Uri("Images/edit.png", UriKind.Relative);
            bi16.EndInit();
            imageFabricatorEdit.Source = bi16;

            BitmapImage bi17 = new BitmapImage();
            bi17.BeginInit();
            bi17.UriSource = new Uri("Images/add.png", UriKind.Relative);
            bi17.EndInit();
            imageFabricatorAdd.Source = bi17;

            BitmapImage bi18 = new BitmapImage();
            bi18.BeginInit();
            bi18.UriSource = new Uri("Images/delete.png", UriKind.Relative);
            bi18.EndInit();
            imageFabricatorDelete.Source = bi18;

            BitmapImage bi19 = new BitmapImage();
            bi19.BeginInit();
            bi19.UriSource = new Uri("Images/download.png", UriKind.Relative);
            bi19.EndInit();
            imageFabricatorDownload.Source = bi19;

            //категории
            BitmapImage bi20 = new BitmapImage();
            bi20.BeginInit();
            bi20.UriSource = new Uri("Images/edit.png", UriKind.Relative);
            bi20.EndInit();
            imageCategoryEdit.Source = bi20;

            BitmapImage bi21 = new BitmapImage();
            bi21.BeginInit();
            bi21.UriSource = new Uri("Images/add.png", UriKind.Relative);
            bi21.EndInit();
            imageCategoryAdd.Source = bi21;

            BitmapImage bi22 = new BitmapImage();
            bi22.BeginInit();
            bi22.UriSource = new Uri("Images/delete.png", UriKind.Relative);
            bi22.EndInit();
            imageCategoryDelete.Source = bi22;

            BitmapImage bi23 = new BitmapImage();
            bi23.BeginInit();
            bi23.UriSource = new Uri("Images/download.png", UriKind.Relative);
            bi23.EndInit();
            imageCategoryDownload.Source = bi23;

            BitmapImage bi24 = new BitmapImage();
            bi24.BeginInit();
            bi24.UriSource = new Uri("Images/fontStyle.png", UriKind.Relative);
            bi24.EndInit();
            imageChangeFont.Source = bi24;

            //запчасти
            BitmapImage bi25 = new BitmapImage();
            bi25.BeginInit();
            bi25.UriSource = new Uri("Images/add.png", UriKind.Relative);
            bi25.EndInit();
            imagePartsAdd.Source = bi25;

            BitmapImage bi26 = new BitmapImage();
            bi26.BeginInit();
            bi26.UriSource = new Uri("Images/delete.png", UriKind.Relative);
            bi26.EndInit();
            imagePartsDelete.Source = bi26;

            BitmapImage bi27 = new BitmapImage();
            bi27.BeginInit();
            bi27.UriSource = new Uri("Images/download.png", UriKind.Relative);
            bi27.EndInit();
            imagePartsDownload.Source = bi27;

            BitmapImage bi29 = new BitmapImage();
            bi29.BeginInit();
            bi29.UriSource = new Uri("Images/edit.png", UriKind.Relative);
            bi29.EndInit();
            imagePartsEdit.Source = bi29;

            //продажи запчастей
            BitmapImage bi30 = new BitmapImage();
            bi30.BeginInit();
            bi30.UriSource = new Uri("Images/add.png", UriKind.Relative);
            bi30.EndInit();
            imagePartsSalesAdd.Source = bi30;

            BitmapImage bi31 = new BitmapImage();
            bi31.BeginInit();
            bi31.UriSource = new Uri("Images/delete.png", UriKind.Relative);
            bi31.EndInit();
            imagePartsSalesDelete.Source = bi31;

            BitmapImage bi32 = new BitmapImage();
            bi32.BeginInit();
            bi32.UriSource = new Uri("Images/download.png", UriKind.Relative);
            bi32.EndInit();
            imagePartsSalesDownload.Source = bi32;

            //поставки запчастей
            BitmapImage bi33 = new BitmapImage();
            bi33.BeginInit();
            bi33.UriSource = new Uri("Images/add.png", UriKind.Relative);
            bi33.EndInit();
            imagePartsBuysAdd.Source = bi33;

            BitmapImage bi34 = new BitmapImage();
            bi34.BeginInit();
            bi34.UriSource = new Uri("Images/delete.png", UriKind.Relative);
            bi34.EndInit();
            imagePartsBuysDelete.Source = bi34;

            BitmapImage bi35 = new BitmapImage();
            bi35.BeginInit();
            bi35.UriSource = new Uri("Images/download.png", UriKind.Relative);
            bi35.EndInit();
            imagePartsBuysDownload.Source = bi35;

            //продажи запчастей
            BitmapImage bi36 = new BitmapImage();
            bi36.BeginInit();
            bi36.UriSource = new Uri("Images/add.png", UriKind.Relative);
            bi36.EndInit();
            imageAttributesAdd.Source = bi36;

            BitmapImage bi37 = new BitmapImage();
            bi37.BeginInit();
            bi37.UriSource = new Uri("Images/delete.png", UriKind.Relative);
            bi37.EndInit();
            imageAttributesDelete.Source = bi37;

            BitmapImage bi38 = new BitmapImage();
            bi38.BeginInit();
            bi38.UriSource = new Uri("Images/download.png", UriKind.Relative);
            bi38.EndInit();
            imageAttributesDownload.Source = bi38;

        }

        //
        //производители
        //

        //вывод информации о производителе
        private void dataGridFabricators_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (dataGridFabricators.SelectedIndex != -1)
                {
                    Fabricator item = dataGridFabricators.SelectedItem as Fabricator;
                    nameFabricatorLabel.Content = item.name_fabricator;
                    innFabricatorLabel.Content = "ИНН: " + item.inn_fabricator;
                    descriptionFabricatorLabel.Text = "\n" + item.description_fabricator;
                    telFabricatorLabel.Content = "Контактный телефон: " + item.tel_fabricator;
                    contactFabricatorLabel.Content = "Контактное лицо: " + item.contactName_fabricator;
                    equipmentFabricatorLabel.Text = "";
                    ObservableCollection<string> ListEquipmentF = new ObservableCollection<string>();
                    var queryEquipment = from equipment in dataEntities.Equipments
                                         orderby equipment.code_equipment
                                         select equipment;
                    foreach (Equipment equipment in queryEquipment)
                    {
                        if (equipment.fabricator_equipment == item.id_fabricator)
                            equipmentFabricatorLabel.Text += "\n" + equipment.name_equipment;
                    }
                    equipmentFabricator.Visibility = Visibility.Visible;
                    descriptionFabricator.Visibility = Visibility.Visible;
                    uploadEquipment.IsEnabled = true;
                    copyEquipment.IsEnabled = true;
                }
                else
                {
                    nameFabricatorLabel.Content = "";
                    innFabricatorLabel.Content = "";
                    telFabricatorLabel.Content = "";
                    contactFabricatorLabel.Content = "";
                    equipmentFabricatorLabel.Text = "";
                    descriptionFabricatorLabel.Text = "";
                    equipmentFabricator.Visibility = Visibility.Hidden;
                    descriptionFabricator.Visibility = Visibility.Hidden;
                    uploadEquipment.IsEnabled = false;
                    copyEquipment.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                this.ShowMessageAsync("Ошибка!", ex.ToString());
            }
        }


        private void AddFabricator_Click(object sender, RoutedEventArgs e)
        {
            ApplyEffect(this);
            Fabricators window = new Fabricators(ListFabricators, null, dataEntities);
            window.ShowDialog();
            if (window.DialogResult == true)
            {
                dataGridFabricators.SelectedIndex = -1;
                GetFabricators();

                this.ShowMessageAsync("Производитель добавлен", "Вы можете найти его во вкладке Производители");
            }
            else
                this.ShowMessageAsync("Отмена добавления производителя", "Вы можете добавить его во вкладке Производители->Добавить производителя");
            ClearEffect(this);
        }



        private void EditFabricator_Click(object sender, RoutedEventArgs e)
        {
            Fabricator fabricator = dataGridFabricators.SelectedItem as Fabricator;
            ApplyEffect(this);
            Fabricators window = new Fabricators(ListFabricators, fabricator, dataEntities);
            window.ShowDialog();
            if (window.DialogResult == true)
            {
                dataGridFabricators.SelectedIndex = -1;
                GetFabricators();

                this.ShowMessageAsync("Производитель изменен", "Вы можете найти его во вкладке Производители");
            }
            else
            {
                this.ShowMessageAsync("Отмена изменения производителя", "Вы можете изменить его во вкладке Производители->Изменить производителя ");
            }
            ClearEffect(this);
        }



        private void DownloadFabricator_Click(object sender, RoutedEventArgs e)
        {
        }



        private void DeleteFabricator_Click(object sender, RoutedEventArgs e)
        {
            Fabricator fabricator = dataGridFabricators.SelectedItem as Fabricator;
            if (fabricator != null)
            {
                var s = MessageBox.Show("Удалить производителя  " + fabricator.name_fabricator + " ?", "Вы уверены?", MessageBoxButton.YesNo);
                if (s == MessageBoxResult.Yes)
                {
                    dataEntities.Fabricators.Remove(fabricator);
                    dataEntities.SaveChanges();
                    GetFabricators();
                    this.ShowMessageAsync("Производитель удален", "");
                    dataGridFabricators.SelectedIndex = -1;
                }
            }
            else
            {
                this.ShowMessageAsync("Ошибка!", "Выберите строку для удаления");
            }
        }

        //
        //категории
        //

        private void AddCategory_Click(object sender, RoutedEventArgs e)
        {
            ApplyEffect(this);
            CategoriesWindow window = new CategoriesWindow(ListСategories, null, dataEntities);
            window.ShowDialog();
            if (window.DialogResult == true)
            {
                dataGridCategories.SelectedIndex = -1;
                GetСategories();

                this.ShowMessageAsync("Категория добавлена", "Вы можете найти ее во вкладке Категории");
            }
            else
                this.ShowMessageAsync("Отмена добавления категории", "Вы можете добавить ее во вкладке Производители->Добавить категорию");
            ClearEffect(this);
        }

        private void EditCategory_Click(object sender, RoutedEventArgs e)
        {
            Сategories category = dataGridCategories.SelectedItem as Сategories;
            ApplyEffect(this);
            CategoriesWindow window = new CategoriesWindow(ListСategories, category, dataEntities);
            window.ShowDialog();
            if (window.DialogResult == true)
            {
                dataGridCategories.SelectedIndex = -1;
                GetСategories();

                this.ShowMessageAsync("Категория изменена", "Вы можете найти ее во вкладке Категории");
            }
            else
            {
                this.ShowMessageAsync("Отмена изменения категории", "Вы можете изменить ее во вкладке Оборудование->Изменить категорию ");
            }
            ClearEffect(this);
        }

        private void uploadCategory_Click(object sender, RoutedEventArgs e)
        {
        }

        private void copyCategory_Click(object sender, RoutedEventArgs e)
        {
        }

        private void DownloadCategory_Click(object sender, RoutedEventArgs e)
        {
        }



        private void DeleteCategory_Click(object sender, RoutedEventArgs e)
        {
            Сategories category = dataGridCategories.SelectedItem as Сategories;
            if (category != null)
            {
                var s = MessageBox.Show("Удалить категорию  " + category.name_category + " ?", "Вы уверены?", MessageBoxButton.YesNo);
                if (s == MessageBoxResult.Yes)
                {
                    dataEntities.Сategories.Remove(category);
                    dataEntities.SaveChanges();
                    GetEquipment();
                    this.ShowMessageAsync("Категория удалена", "");
                    dataGridEquipment.SelectedIndex = -1;
                }
            }
            else
            {
                this.ShowMessageAsync("Ошибка!", "Выберите строку для удаления");
            }
        }

        private void dataGridCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (dataGridCategories.SelectedIndex != -1)
                {
                    Сategories item = dataGridCategories.SelectedItem as Сategories;
                    nameCategoryLabel.Content = item.name_category;
                    descriptionCategoryLabel.Text = item.description_category;
                    descriptionCategory.Visibility = Visibility.Visible;
                    uploadEquipment.IsEnabled = true;
                    copyEquipment.IsEnabled = true;
                }
                else
                {
                    nameCategoryLabel.Content = "";
                    descriptionCategoryLabel.Text = "";
                    descriptionCategory.Visibility = Visibility.Hidden;
                    uploadEquipment.IsEnabled = false;
                    copyEquipment.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                this.ShowMessageAsync("Ошибка!", ex.ToString());
            }
        }

        //
        //характеристики
        //

        private void AddAttributes_Click(object sender, RoutedEventArgs e)
        {
            ApplyEffect(this);
            AttributesWindow window = new AttributesWindow(ListAttributes, null, dataEntities);
            window.ShowDialog();
            if (window.DialogResult == true)
            {
                dataGridAttributes.SelectedIndex = -1;
                GetAttributes_Values();
                GetAttributes();
                this.ShowMessageAsync("Характеристика добавлена", "Вы можете найти ее во вкладке Характеристики");
            }
            else
                this.ShowMessageAsync("Отмена добавления производителя", "Вы можете добавить ее во вкладке Характеристики->Добавить характеристику");
            ClearEffect(this);
        }

        private void EditAttributes_Click(object sender, RoutedEventArgs e)
        {
            Attribute category = dataGridAttributes.SelectedItem as Attribute;
            ApplyEffect(this);
            AttributesWindow window = new AttributesWindow(ListAttributes, category, dataEntities);
            window.ShowDialog();
            if (window.DialogResult == true)
            {
                dataGridAttributes.SelectedIndex = -1;
                GetAttributes_Values();
                GetAttributes();
                
                this.ShowMessageAsync("Характеристика изменена", "Вы можете найти ее во вкладке Характеристики ");
            }
            else
            {
                this.ShowMessageAsync("Отмена изменения характеристики", "Вы можете изменить ее во вкладке Характеристики->Изменить характеристику ");
            }
            ClearEffect(this);
        }

        private void DownloadAttributes_Click(object sender, RoutedEventArgs e)
        {
        }


        private void copyAttributes_Click(object sender, RoutedEventArgs e)
        {
        }

        

        private void DeleteAttributes_Click(object sender, RoutedEventArgs e)
        {
            Attribute category = dataGridAttributes.SelectedItem as Attribute;
            if (category != null)
            {
                var s = MessageBox.Show("Удалить характеристику  " + category.name_attribute + " ?", "Вы уверены?", MessageBoxButton.YesNo);
                if (s == MessageBoxResult.Yes)
                {
                    dataEntities.Attributes.Remove(category);
                    dataEntities.SaveChanges();
                    GetEquipment();
                    this.ShowMessageAsync("Характеристика удалена", "");
                    dataGridEquipment.SelectedIndex = -1;
                }
            }
            else
            {
                this.ShowMessageAsync("Ошибка!", "Выберите строку для удаления");
            }
        }

        private void dataGridAttributes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (dataGridAttributes.SelectedIndex != -1)
                {
                    Attribute item = dataGridAttributes.SelectedItem as Attribute;
                    nameAttributesLabel.Content = item.name_attribute;
                    descriptionAttributesLabel.Text = item.description_attribute;
                    descriptionAttributes.Visibility = Visibility.Visible;
                }
                else
                {
                    nameAttributesLabel.Content = "";
                    descriptionAttributesLabel.Text = "";
                    descriptionAttributes.Visibility = Visibility.Hidden;
                }
            }
            catch (Exception ex)
            {
                this.ShowMessageAsync("Ошибка!", ex.ToString());
            }
        }

        private void ApplyEffect(MetroWindow win)
        {
            System.Windows.Media.Effects.BlurEffect objBlur = new System.Windows.Media.Effects.BlurEffect();
            objBlur.Radius = 4;
            win.Effect = objBlur;
        }

        private void ClearEffect(MetroWindow win)
        {
            win.Effect = null;
        }

        //формирование отчета по запчастям(по времени)
        private void submitChartParts_Click(object sender, RoutedEventArgs e)
        {
            if (partsDatePickerFrom.Text != "" && partsDatePickerTo.Text != "")
            {
                //очистка от предыдущих значений
                for (int i = 0; i < part.Count(); i++)
                {
                    part[i].sale = 0;
                    part[i].post = 0;
                }
                ListPerPartsSales.Clear();
                ListPerPartsPosts.Clear();

                var queryActions_Equipment = from item in dataEntities.Actions_Parts
                                             orderby item.id_aParts
                                             select item;

                foreach (Actions_Parts item in queryActions_Equipment)
                {
                    if (item.type_aParts == true &&
                        item.Part != null &&
                        item.date_aParts >= Convert.ToDateTime(partsDatePickerFrom.Text) &&
                        item.date_aParts <= Convert.ToDateTime(partsDatePickerTo.Text))
                    {
                        item.part_aParts = item.Part.id_part;
                        for (int i = 0; i < part.Count(); i++)
                        {
                            if (part[i].name == item.Part.code_part)
                                    part[i].sale += item.price_aParts;
                        }
                    }
                    else if (item.part_aParts != null &&
                            item.Part != null &&
                            item.date_aParts >= Convert.ToDateTime(partsDatePickerFrom.Text) &&
                        item.date_aParts <= Convert.ToDateTime(partsDatePickerTo.Text))
                    {

                        item.part_aParts = item.Part.id_part;
                        for (int i = 0; i < part.Count(); i++)
                        {
                            if (part[i].name == item.Part.code_part)
                                    part[i].post += item.price_aParts;
                        }
                    }
                }
                decimal max = 0;
                decimal sumPost = 0;
                decimal sumSale = 0;
                for (int i = 0; i < part.Count(); i++)
                {
                    if (part[i].sale > max)
                        max = part[i].sale;
                    if (part[i].post > max)
                        max = part[i].post;
                    ListPerPartsPosts.Add(new KeyValuePair<string, decimal>(part[i].name, part[i].post));
                    sumPost += part[i].post;
                    ListPerPartsSales.Add(new KeyValuePair<string, decimal>(part[i].name, part[i].sale));
                    sumSale += part[i].sale;
                }

                sumPartsPostsLabel.Text = "Всего за данный период поставлено на " + String.Format("{0:0.00}", sumPost) + " рублей";
                sumPartsSalesLabel.Text = "Всего за данный период продано на " + String.Format("{0:0.00}", sumSale) + " рублей";
                decimal result = sumSale - sumPost;
                if (result > 0)
                {
                    topPartsPolygon.Visibility = Visibility.Visible;
                    centerPartsLine.Visibility = Visibility.Hidden;
                    bottomPartsPolygon.Visibility = Visibility.Hidden;
                    resultPartsPostSalesLabel.Content = "+ " + String.Format("{0:0}", result) + " рублей";
                    resultPartsPostSalesLabel.Foreground = new SolidColorBrush(Colors.Green);
                }
                else if (result < 0)
                {
                    topPartsPolygon.Visibility = Visibility.Hidden;
                    centerPartsLine.Visibility = Visibility.Hidden;
                    bottomPartsPolygon.Visibility = Visibility.Visible;
                    resultPartsPostSalesLabel.Content = String.Format("{0:0}", result) + " рублей";
                    resultPartsPostSalesLabel.Foreground = new SolidColorBrush(Colors.Red);
                }
                else
                {
                    topPartsPolygon.Visibility = Visibility.Hidden;
                    centerPartsLine.Visibility = Visibility.Visible;
                    bottomPartsPolygon.Visibility = Visibility.Hidden;
                    resultPartsPostSalesLabel.Content = String.Format("{0:0}", result) + " рублей";
                    resultPartsPostSalesLabel.Foreground = new SolidColorBrush(Colors.Gray);
                }
                //гистограмма
                max = max + max / 10;
                lin3.Maximum = Convert.ToDouble(max);
                lin4.Maximum = Convert.ToDouble(max);
                lin3.Visibility = Visibility.Hidden;
                chartPerPartsSales.DataContext = ListPerPartsSales;
                chartPerPartsPost.DataContext = ListPerPartsPosts;
            }
        }

        //формирование отчета по оборудованию(по времени)
        private void submitChartEquipment_Click(object sender, RoutedEventArgs e)
        {
            if (EquipmentDatePickerFrom.Text != "" && EquipmentDatePickerTo.Text != "")
            {
                //очистка от предыдущих значений
                for (int i = 0; i < komp.Count(); i++)
                {
                    komp[i].sale = 0;
                    komp[i].post = 0;
                }
                ListPerEquipmentSales.Clear();
                ListPerEquipmentPosts.Clear();

                var queryActions_Equipment = from item in dataEntities.Actions_Equipment
                                             orderby item.id_aEquipment
                                             select item;

                foreach (Actions_Equipment item in queryActions_Equipment)
                {
                    if (item.type_aEquipment == true &&
                        item.Equipment != null &&
                        item.date_aEquipment >= Convert.ToDateTime(EquipmentDatePickerFrom.Text) &&
                        item.date_aEquipment <= Convert.ToDateTime(EquipmentDatePickerTo.Text))
                    {
                        item.equipment_aEquipment = item.Equipment.id_equipment;
                        for (int i = 0; i < komp.Count(); i++)
                        {
                            if (item.Equipment != null)
                                if (komp[i].name == item.Equipment.code_equipment)
                                    komp[i].sale += item.price_aEquipment;
                        }
                    }
                    else if (item.equipment_aEquipment != null && 
                            item.Equipment != null &&
                            item.date_aEquipment >= Convert.ToDateTime(EquipmentDatePickerFrom.Text) &&
                        item.date_aEquipment <= Convert.ToDateTime(EquipmentDatePickerTo.Text))
                            {

                                item.equipment_aEquipment = item.Equipment.id_equipment;
                                for (int i = 0; i < komp.Count(); i++)
                                {
                                    if (item.Equipment != null)
                                        if (komp[i].name == item.Equipment.code_equipment)
                                            komp[i].post += item.price_aEquipment;
                                }
                            }
                }
                decimal max = 0;
                decimal sumPost = 0;
                decimal sumSale = 0;
                for (int i = 0; i < komp.Count(); i++)
                {
                    if (komp[i].sale > max)
                        max = komp[i].sale;
                    if (komp[i].post > max)
                        max = komp[i].post;
                    ListPerEquipmentPosts.Add(new KeyValuePair<string, decimal>(komp[i].name, komp[i].post));
                    sumPost += komp[i].post;
                    ListPerEquipmentSales.Add(new KeyValuePair<string, decimal>(komp[i].name, komp[i].sale));
                    sumSale += komp[i].sale;
                }

                sumPostsLabel.Text = "Всего за данный период поставлено на " + String.Format("{0:0.00}", sumPost) + " рублей";
                sumSalesLabel.Text = "Всего за данный период продано на " + String.Format("{0:0.00}", sumSale) + " рублей";
                decimal result = sumSale - sumPost;
                if (result > 0)
                {
                    topPolygon.Visibility = Visibility.Visible;
                    centerLine.Visibility = Visibility.Hidden;
                    bottomPolygon.Visibility = Visibility.Hidden;
                    resultPostSalesLabel.Content = "+ " + String.Format("{0:0}", result) + " рублей";
                    resultPostSalesLabel.Foreground = new SolidColorBrush(Colors.Green);
                }
                else if (result < 0)
                {
                    topPolygon.Visibility = Visibility.Hidden;
                    centerLine.Visibility = Visibility.Hidden;
                    bottomPolygon.Visibility = Visibility.Visible;
                    resultPostSalesLabel.Content = String.Format("{0:0}", result) + " рублей";
                    resultPostSalesLabel.Foreground = new SolidColorBrush(Colors.Red);
                }
                else
                {
                    topPolygon.Visibility = Visibility.Hidden;
                    centerLine.Visibility = Visibility.Visible;
                    bottomPolygon.Visibility = Visibility.Hidden;
                    resultPostSalesLabel.Content = String.Format("{0:0}", result) + " рублей";
                    resultPostSalesLabel.Foreground = new SolidColorBrush(Colors.Gray);
                }
                //гистограмма
                max = max + max / 10;
                lin1.Maximum = Convert.ToDouble(max);
                lin2.Maximum = Convert.ToDouble(max);
                lin1.Visibility = Visibility.Hidden;
                chartPerEquipmentSales.DataContext = ListPerEquipmentSales;
                chartPerEquipmentPost.DataContext = ListPerEquipmentPosts;
            }
        }

        //формирование отчета по запчастям(все)
        private void allChartParts_Click(object sender, RoutedEventArgs e)
        {
            GetParts();
            GetActions_Parts();
        }

        //формирование отчета по оборудованию(все)
        private void allChartEquipment_Click(object sender, RoutedEventArgs e)
        {
            GetEquipment();
            GetActions_Equipment();
        }

        //выгрузка диаграммы в Excel
        private void uploadChartEquipment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "XLSX files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
                if (saveFileDialog.ShowDialog() == true)
                {
                    Workbook excelB = excelA.Workbooks.Add(uri);
                    Worksheet excelS = (Worksheet)excelB.Sheets[1];
                    excelS.Columns.AutoFit();
                    ObservableCollection<string> Headers = new ObservableCollection<string>
                    {"Продажи","Поставки"};
                    int C = 0;
                    for (int i = 0; i < Headers.Count; i++)
                    {
                        Microsoft.Office.Interop.Excel.Range myRange = (Microsoft.Office.Interop.Excel.Range)excelS.Cells[1, i + 2];
                        excelS.Cells[1, i + 2].Font.Bold = true;
                        excelS.Columns[i + 2].ColumnWidth = 25;
                        myRange.Value2 = Headers.ElementAt(i);
                    }
                    for (int i = 0; i < komp.Count(); i++)
                    {
                        int j = 0;
                        Microsoft.Office.Interop.Excel.Range myRange;
                        myRange = (Microsoft.Office.Interop.Excel.Range)excelS.Cells[i + 2, j + 1];
                        myRange.Value2 = komp[i].name;
                        j++;
                        myRange = (Microsoft.Office.Interop.Excel.Range)excelS.Cells[i + 2, j + 1];
                        myRange.Value2 = komp[i].sale;
                        j++;
                        myRange = (Microsoft.Office.Interop.Excel.Range)excelS.Cells[i + 2, j + 1];
                        myRange.Value2 = komp[i].post;
                        j++;
                    }

                    Microsoft.Office.Interop.Excel.Range chartRange;
                    object misValue = System.Reflection.Missing.Value;
                    ChartObjects xlCharts = (ChartObjects)excelS.ChartObjects(Type.Missing);
                    ChartObject myChart = (ChartObject)xlCharts.Add(10, 80, 300, 250);
                    Microsoft.Office.Interop.Excel.Chart chartPage = myChart.Chart;
                    C = komp.Count() + 1;
                    chartRange = excelS.get_Range("A1","C"+ C);
                    chartPage.SetSourceData(chartRange, misValue);
                    chartPage.ChartType = Microsoft.Office.Interop.Excel.XlChartType.xlColumnClustered;

                    for (int i = 0; i < 3; i++)
                    {
                        if (i == 0)
                        {
                            int j = 5;
                            Microsoft.Office.Interop.Excel.Range myRange;
                            myRange = (Microsoft.Office.Interop.Excel.Range)excelS.Cells[i + 2, j];
                            myRange.Value2 = "Результат:";
                            j++;

                            myRange = (Microsoft.Office.Interop.Excel.Range)excelS.Cells[i + 2, j];
                            myRange.Value2 = resultPostSalesLabel.Content;
                            j++;
                        }
                        if (i == 1)
                        {
                            int j = 5;
                            Microsoft.Office.Interop.Excel.Range myRange;
                            myRange = (Microsoft.Office.Interop.Excel.Range)excelS.Cells[i + 2, j];
                            myRange.Value2 = sumSalesLabel.Text;
                            j++;
                        }
                        if (i == 2)
                        {
                            int j = 5;
                            Microsoft.Office.Interop.Excel.Range myRange;
                            myRange = (Microsoft.Office.Interop.Excel.Range)excelS.Cells[i + 2, j];
                            myRange.Value2 = sumPostsLabel.Text;
                            j++;
                        }
                    }

                    excelB.SaveAs(saveFileDialog.FileName);
                    excelA.Quit();

                    string message = "Данные успешно записались в Excel файл по этому пути: " + saveFileDialog.FileName;
                    string caption = "Файл создан";
                    this.ShowMessageAsync(caption, message);

                }
            }
            catch (Exception ex)
            {
                this.ShowMessageAsync("Ошибка!", ex.ToString());
            }
        }
    }

    public class Komp
    {
        public Komp()
        {
            this.sale = 0;
            this.post = 0;
            this.name = "";
        }

        public Komp(string name)
        {
            this.sale = 0;
            this.post = 0;
            this.name = name;
        }

        public decimal post { get; set; }
        public decimal sale { get; set; }
        public string name { get; set; }
    }
}
