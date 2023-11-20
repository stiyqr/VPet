using LinePutScript.Localization.WPF;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO;
using System.Text;

using VPet_Simulator.Core;
using VPet_Simulator.Core.Display;
using Panuon.WPF.UI;

namespace VPet_Simulator.Windows
{
    /// <summary>
    /// MessageBar.xaml 的交互逻辑
    /// </summary>
    public partial class TalkBoxAPI : UserControl
    {
        Main m;
        MainWindow mw;
        public TalkBoxAPI(MainWindow mw)
        {
            InitializeComponent();
            this.m = mw.Main;
            this.mw = mw;
            TextBoxHelper.SetWatermark(tbTalk, "hehe");
        }

        private void SendMessage_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(tbTalk.Text))
            {
                return;
            }
            var cont = tbTalk.Text;
            tbTalk.Text = "";
            //Task.Run(() => OPENAI(cont));

            mw.Main.ToolBar.Visibility = Visibility.Collapsed;
            //Task.Run(() => PrintTextBoxMsg(cont) );
            //WriteToExpenses(cont);
            WriteToSelection(cont);
        }

        public void PrintTextBoxMsg(string content) {
            if (string.IsNullOrEmpty(content)) {
                return;
            }
            Dispatcher.Invoke(() => this.IsEnabled = false);

            m.SayRnd(content);

            Dispatcher.Invoke(() => this.IsEnabled = true);
        }

        public void WriteToSelection(string content) {
            if (SelectionBox.SelectedType == SelectionBox.SelectionType.Expenses) {
                WriteToExpenses(content);
            }
            else if (SelectionBox.SelectedType == SelectionBox.SelectionType.Calories) {
                WriteToCalories(content);
            } 
            else {
                Task.Run(() => OPENAI(content));
            }
        }

        public void WriteToCalories(string content) {
            string path = CalorieRecords.FilePath;
            char delimiter = ',';
            bool isValidInput = false;
            content = content.Trim();

            if (content.ToLower() == "clear") {
                File.WriteAllText(path, "");
                PrintFeedbackMsg("File cleared!");
            }
            else if (content.ToLower() == "print") {
                if (File.ReadAllText(path) == "") {
                    PrintFeedbackMsg("File is empty!");
                }
                else {
                    m.DisplayCalorieTable();
                }
            }
            else if (content.ToLower() == "active") {
                m.PrintActiveWindow();
            }
            else if (content.ToLower() == "timer") {
                m.ToggleScreenTimer();
            }
            else if (!content.Contains(delimiter)) {
                PrintFeedbackMsg($"Wrong input format.\nFormat: [food name]{delimiter}[calories]");
            }
            else {
                isValidInput = true;
            }

            if (!isValidInput) return;

            int delimiterIndex = -1;
            for (int i = 0; i < content.Length; i++) {
                if (content[i] == delimiter) {
                    if (delimiterIndex > -1 || i == 0) {
                        PrintFeedbackMsg($"Wrong input format.\nFormat: [food name]{delimiter}[calories]");
                        return;
                    }
                    else {
                        delimiterIndex = i;
                    }
                }
            }

            if (delimiterIndex < 0) {
                PrintFeedbackMsg($"Wrong input format.\nFormat: [food name]{delimiter}[calories]");
                return;
            }

            string foodName = content.Substring(0, delimiterIndex).Trim();
            string foodCalorie = content.Substring(delimiterIndex + 1).Trim();

            string date = DateTime.Today.ToShortDateString();
            string entry = date + "," + foodName + "," + foodCalorie;

            File.AppendAllText(path, entry + Environment.NewLine);

            Task.Run(() => PrintTextBoxMsg($"Recorded: {date} {foodName} {foodCalorie}"));
        }

        public void WriteToExpenses(string content) {
            string path = Expenses.FilePath;
            char delimiter = ',';
            bool isValidInput = false;
            content = content.Trim();

            if (content.ToLower() == "clear") {
                File.WriteAllText(path, "");
                PrintFeedbackMsg("File cleared!");
            }
            else if (content.ToLower() == "print") {
                if (File.ReadAllText(path) == "") {
                    PrintFeedbackMsg("File is empty!");
                }
                else {
                    //Task.Run(() => PrintTextBoxMsg(File.ReadAllText(path)));

                    //ExpenseTable0 expenseTable = new ExpenseTable0();
                    //Task.Run(() => expenseTable.Show());
                    m.DisplayExpenseTable();
                }
            }
            else if (content.ToLower() == "active") {
                m.PrintActiveWindow();
            }
            else if (content.ToLower() == "timer") {
                m.ToggleScreenTimer();
            }
            else if (!content.Contains(delimiter)) {
                PrintFeedbackMsg($"Wrong input format.\nFormat: [item name]{delimiter}[price]");
            }
            else {
                isValidInput = true;
            }

            if (!isValidInput) return;

            int delimiterIndex = -1;
            for (int i = 0; i < content.Length; i++) {
                if (content[i] == delimiter) {
                    if (delimiterIndex > -1 || i == 0) {
                        PrintFeedbackMsg($"Wrong input format.\nFormat: [item name]{delimiter}[price]");
                        return;
                    }
                    else {
                        delimiterIndex = i;
                    }
                }
            }

            if (delimiterIndex < 0) {
                PrintFeedbackMsg($"Wrong input format.\nFormat: [item name]{delimiter}[price]");
                return;
            }

            string itemName = content.Substring(0, delimiterIndex).Trim();
            string itemPrice = content.Substring(delimiterIndex + 1).Trim();

            string date = DateTime.Today.ToShortDateString();
            string entry = date + "," + itemName + "," + itemPrice;

            File.AppendAllText(path, entry + Environment.NewLine);

            Task.Run(() => PrintTextBoxMsg($"Recorded: {date} {itemName} {itemPrice}"));
        }

        private void PrintFeedbackMsg(string msg) {
            Task.Run(() => PrintTextBoxMsg(msg));
        }

        /// <summary>
        /// 使用OPENAI API进行回复
        /// </summary>
        /// <param name="content">内容 说话内容</param>
        public void OPENAI(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return;
            }
            if (mw.CGPTClient == null)
            {
                m.SayRnd("请先前往设置中设置 ChatGPT API".Translate());
                return;
            }
            Dispatcher.Invoke(() => this.IsEnabled = false);
            try
            {
                if (mw.CGPTClient.Completions.TryGetValue("vpet", out var vpetapi))
                {
                    var last = vpetapi.messages.LastOrDefault();
                    if (last != null)
                    {
                        if(last.role == ChatGPT.API.Framework.Message.RoleType.user)
                        {
                            vpetapi.messages.Remove(last);
                        }
                    }
                }

                var resp = mw.CGPTClient.Ask("vpet", content);
                var reply = resp.GetMessageContent();
                if (resp.choices[0].finish_reason == "length")
                {
                    reply += " ...";
                }
                var showtxt = "当前Token使用".Translate() + ": " + resp.usage.total_tokens;
                Dispatcher.Invoke(() =>
                {
                    m.MsgBar.MessageBoxContent.Children.Add(new TextBlock() { Text = showtxt, FontSize = 20, ToolTip = showtxt, HorizontalAlignment = HorizontalAlignment.Right });
                });
                m.SayRnd(reply);
            }
            catch (Exception exp)
            {
                var e = exp.ToString();
                string str = "请检查设置和网络连接".Translate();
                if (e.Contains("401"))
                {
                    str = "请检查API token设置".Translate();
                }
                m.SayRnd("API调用失败".Translate() + $",{str}\n{e}");//, GraphCore.Helper.SayType.Serious);
            }
            Dispatcher.Invoke(() => this.IsEnabled = true);
        }
        private void tbTalk_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Control))
            {
                SendMessage_Click(sender, e);
                e.Handled = true;
            }
        }

        private void tbTalk_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbTalk.Text.Length > 0)
            {
                //mw.Main.ToolBar.MenuPanel_MouseEnter();
            }
            else
            {
                mw.Main.ToolBar.MenuPanel_MouseLeave();
            }
        }
    }
}
