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
    /// Логика взаимодействия для ChangeFontStyle.xaml
    /// </summary>
    public partial class ChangeFontStyle : MetroWindow
    {
        bool ready = false;
        public ChangeFontStyle()
        {
            InitializeComponent();

            ready = true;
            sample.FontStretch = Properties.Settings.Default.FontStretch;
            sample.FontFamily = Properties.Settings.Default.FontFamily;
            sample.FontWeight = Properties.Settings.Default.FontWeight;
            sample.FontStyle = Properties.Settings.Default.FontStyle;
            sample.FontSize = Convert.ToDouble(Properties.Settings.Default.FontSize);
            changeFontSize.Value = Convert.ToDouble(Properties.Settings.Default.FontSize);
        }

        private void submit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Properties.Settings.Default.FontSize = changeFontSize.Value.ToString();
                Properties.Settings.Default.FontStyle = sample.FontStyle;
                Properties.Settings.Default.FontWeight = sample.FontWeight;
                Properties.Settings.Default.FontStretch = sample.FontStretch;
                Properties.Settings.Default.FontFamily = sample.FontFamily;
                Properties.Settings.Default.Save();

                this.DialogResult = true;
            }
            catch(Exception ex)
            {
                this.ShowMessageAsync("Ошибка!",ex.ToString());
            }
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void typefaceSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                FamilyTypeface f = typefaceSelector.SelectedItem as FamilyTypeface;
                sample.FontStretch = f.Stretch;
                sample.FontStyle = f.Style;
                sample.FontWeight = f.Weight;
            }
            catch
            {

            }
        }

        private void fontSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FontFamily f = fontSelector.SelectedItem as FontFamily;
            sample.FontFamily = f;
        }

        private void changeFontSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (ready)
                sample.FontSize = changeFontSize.Value;
                
        }
    }
}
