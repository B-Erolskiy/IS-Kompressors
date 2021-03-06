﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MahApps.Metro;
using MahApps.Metro.Controls;

namespace IS_Kompressors
{       /// <summary>
        /// Interaction logic for AccentStyleWindow.xaml
        /// </summary>
        public partial class AccentStyleWindow : MetroWindow
        {
            public string accent = "";
            public static readonly DependencyProperty ColorsProperty
                = DependencyProperty.Register("Colors",
                                              typeof(List<KeyValuePair<string, Color>>),
                                              typeof(AccentStyleWindow),
                                              new PropertyMetadata(default(List<KeyValuePair<string, Color>>)));

            public List<KeyValuePair<string, Color>> Colors
            {
                get { return (List<KeyValuePair<string, Color>>)GetValue(ColorsProperty); }
                set { SetValue(ColorsProperty, value); }
            }

            public AccentStyleWindow()
            {
                InitializeComponent();

                this.DataContext = this;

                this.Colors = typeof(Colors)
                    .GetProperties()
                    .Where(prop => typeof(Color).IsAssignableFrom(prop.PropertyType))
                    .Select(prop => new KeyValuePair<String, Color>(prop.Name, (Color)prop.GetValue(null)))
                    .ToList();

                var theme = ThemeManager.DetectAppStyle(Application.Current);
                ThemeManager.ChangeAppStyle(this, theme.Item2, theme.Item1);
            }

            private void ChangeWindowThemeButtonClick(object sender, RoutedEventArgs e)
            {
                var theme = ThemeManager.DetectAppStyle(this);
                ThemeManager.ChangeAppStyle(this, theme.Item2, ThemeManager.GetAppTheme("Base" + ((Button)sender).Content));
            }

            private void ChangeWindowAccentButtonClick(object sender, RoutedEventArgs e)
            {
                var theme = ThemeManager.DetectAppStyle(this);
                ThemeManager.ChangeAppStyle(this, ThemeManager.GetAccent(((Button)sender).Content.ToString()), theme.Item1);
            }

            private void ChangeAppThemeButtonClick(object sender, RoutedEventArgs e)
            {
                var theme = ThemeManager.DetectAppStyle(Application.Current);
                ThemeManager.ChangeAppStyle(Application.Current, theme.Item2, ThemeManager.GetAppTheme("Base" + ((Button)sender).Content));
                Properties.Settings.Default.AppTheme = ((Button)sender).Content.ToString();
                Properties.Settings.Default.Save();
            }

            private void ChangeAppAccentButtonClick(object sender, RoutedEventArgs e)
            {
                var theme = ThemeManager.DetectAppStyle(Application.Current);
                ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent(((Button)sender).Content.ToString()), theme.Item1);
                Properties.Settings.Default.NewColor = false;
                Properties.Settings.Default.Accent = ((Button)sender).Content.ToString();
                Properties.Settings.Default.Save();
        }

            private void AccentSelectionChanged(object sender, SelectionChangedEventArgs e)
            {
                var selectedAccent = AccentSelector.SelectedItem as Accent;
                if (selectedAccent != null)
                {
                    var theme = ThemeManager.DetectAppStyle(Application.Current);
                    ThemeManager.ChangeAppStyle(Application.Current, selectedAccent, theme.Item1);
                    Application.Current.MainWindow.Activate();
                Properties.Settings.Default.NewColor = false;
                Properties.Settings.Default.Accent = selectedAccent.Name;
                    Properties.Settings.Default.Save();
            }
        }

            private void ColorsSelectorOnSelectionChanged(object sender, SelectionChangedEventArgs e)
            {
                var selectedColor = this.ColorsSelector.SelectedItem as KeyValuePair<string, Color>?;
                if (selectedColor.HasValue)
                {
                    var theme = ThemeManager.DetectAppStyle(Application.Current);
                    ThemeManagerHelper.CreateAppStyleBy(selectedColor.Value.Value, true);
                    Application.Current.MainWindow.Activate();
                    Properties.Settings.Default.Color = selectedColor.Value.Value;
                    Properties.Settings.Default.NewColor = true;
                    Properties.Settings.Default.Save();
                }
            }
        }
    }
