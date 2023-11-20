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
using System.IO;

namespace VPet_Simulator.Core.Display {
    /// <summary>
    /// Interaction logic for CalorieBar.xaml
    /// </summary>
    public partial class CalorieBar : Window {

        Main m;
        public CalorieBar(Main m) {
            InitializeComponent();
            this.m = m;
            calorieBarDataGrid.ItemsSource = CalorieRecords.CalorieList;
        }
    }

    public class CalorieRecords {

        public static string FilePath = @".\calories_record.csv";
        public static List<CalorieRecord> CalorieList { get => GetCalorieRecords(); set { } }

        public static List<CalorieRecord> GetCalorieRecords() {
            var lines = File.ReadAllLines(FilePath);
            var list = new List<CalorieRecord>();

            for (int i = 0; i < lines.Length; i++) {
                if (lines[i].Length == 0) continue;

                var line = lines[i].Split(',');
                var calorieRecord = new CalorieRecord() {
                    Date = DateTime.Parse(line[0]).ToShortDateString(),
                    FoodName = line[1],
                    FoodCalorie = line[2]
                };
                list.Add(calorieRecord);
            }

            return list;
        }
    }

    public class CalorieRecord {
        public string Date { get; set; }
        public string FoodName { get; set; }
        public string FoodCalorie { get; set; }
    }
}
