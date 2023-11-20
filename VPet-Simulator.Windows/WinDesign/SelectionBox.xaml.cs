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
using System.IO;

using VPet_Simulator.Core;
using VPet_Simulator.Core.Display;
using System.Windows.Media;
using System.Data;
using System.Windows.Media.Imaging;

namespace VPet_Simulator.Windows {
    /// <summary>
    /// Interaction logic for SelectionBox.xaml
    /// </summary>
    public partial class SelectionBox : UserControl {

        Main m;
        MainWindow mw;

        
        public SelectionBox(MainWindow mw) {
            InitializeComponent();
            this.m = mw.Main;
            this.mw = mw;
        }

        public enum SelectionType {
            Expenses,
            Calories,
            Chat
        }
        
        public static SelectionType SelectedType = SelectionType.Expenses;

        /*
        private void ExpensesBtn_Click(object sender, RoutedEventArgs e) {
            SelectedType = SelectionType.Expenses;
            ExpensesBtn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffffff"));
            CaloriesBtn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#aaaaaa"));
        }

        private void CaloriesBtn_Click(object sender, RoutedEventArgs e) {
            SelectedType = SelectionType.Calories;
            ExpensesBtn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#aaaaaa"));
            CaloriesBtn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffffff"));
        }
        */

        private void ExpensesIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            SelectedType = SelectionType.Expenses;
            ExpensesCircle.Visibility = Visibility.Visible;
            CaloriesCircle.Visibility = Visibility.Hidden;
            ChatCircle.Visibility = Visibility.Hidden;
        }

        private void CaloriesIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            SelectedType = SelectionType.Calories;
            ExpensesCircle.Visibility = Visibility.Hidden;
            CaloriesCircle.Visibility = Visibility.Visible;
            ChatCircle.Visibility = Visibility.Hidden;
        }

        private void ChatIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            SelectedType = SelectionType.Chat;
            ExpensesCircle.Visibility = Visibility.Hidden;
            CaloriesCircle.Visibility = Visibility.Hidden;
            ChatCircle.Visibility = Visibility.Visible;
        }
    }
}
