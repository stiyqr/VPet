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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Header;
using System.Xml.Linq;
using Timer = System.Timers.Timer;

namespace VPet_Simulator.Core.Display {
    /// <summary>
    /// Interaction logic for ExpenseBar.xaml
    /// </summary>
    public partial class ExpenseBar : Window {

        Main m;
        public ExpenseBar(Main m) {
            InitializeComponent();

            //expenseBarDataGrid.ItemsSource = Expenses.GetExpenses();
            expenseBarDataGrid.ItemsSource = Expenses.ExpenseList;
            //DataContext = new Expenses();
            this.m = m;
        }
    }

    public class Expenses {

        public static string FilePath = @".\expenses_record.csv";
        public static List<Expense> ExpenseList { get => GetExpenses(); set { }  }

        public static List<Expense> GetExpenses() {
            var lines = File.ReadAllLines(FilePath);
            var list = new List<Expense>();

            for (int i = 0; i < lines.Length; i++) {
                if (lines[i].Length == 0) continue;

                var line = lines[i].Split(',');
                var expense = new Expense() {
                    Date = DateTime.Parse(line[0]).ToShortDateString(),
                    ItemName = line[1],
                    ItemPrice = line[2]
                };
                list.Add(expense);
            }

            return list;
        }
    }

    public class Expense {
        public string Date { get; set; }
        public string ItemName { get; set; }
        public string ItemPrice { get; set; }
    }
}
