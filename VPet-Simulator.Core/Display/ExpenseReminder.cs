using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VPet_Simulator.Core.Display;

namespace VPet_Simulator.Core {
    public class ExpenseReminder {

        Main m;

        public ExpenseReminder(Main m) {
            this.m = m;
            HasExpenseRecordToday();
            m.TimeUIHandle += M_TimeUIHandle;
        }

        public bool hasReminded1 = false;
        public bool hasReminded2 = false;

        private void M_TimeUIHandle(Main m) {
            if (hasReminded1 && hasReminded2) return;

            if (!hasReminded1) {
                m.SayRnd("Have you record your expenses today?");
                hasReminded1 = true;
            }
            else if (!hasReminded2) {
                string msg;
                double expense = GetThisWeekExpenses();

                if (expense == 0) {
                    msg = "You don't have any recorded expenses this week.";
                }
                else {
                    msg = $"You have spent {expense} this week.";
                }

                m.SayRnd(msg);
                hasReminded2 = true;
            }
        }

        private double GetThisWeekExpenses() {
            double sum = 0;

            foreach (var line in Expenses.ExpenseList) {
                if (DateTime.Compare(DateTime.Parse(line.Date), DateTime.Now.AddDays(-7)) >= 0) {
                    sum += Convert.ToDouble(line.ItemPrice);
                }
            }

            return sum;
        }

        private void HasExpenseRecordToday() {
            if (Expenses.ExpenseList.Count > 0 && DateTime.Compare(DateTime.Parse(Expenses.ExpenseList[0].Date), DateTime.Now.Date) >= 0) {
                // user has recorded an expense today
                hasReminded1 = true;
            }
            else { hasReminded1 = false; }
        }
    }
}
