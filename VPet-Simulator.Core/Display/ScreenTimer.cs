using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HWND = System.IntPtr;
using System.Windows.Input;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using LinePutScript.Localization.WPF;

namespace VPet_Simulator.Core {
    public class ScreenTimer {
        
        Main m;

        public ScreenTimer(Main m) {
            this.m = m;
            //m.TimeUIHandle += M_TimeUIHandle;
        }

        public DateTime StartTime;
        public DateTime LastNotifTime;
        public KeyValuePair<HWND, string> CurrentActiveWindow;
        public KeyValuePair<HWND, string> PrevActiveWindow;
        public bool isActive = false;

        private void M_TimeUIHandle(Main m) {
            if (!isActive) return;

            TimeSpan ts = DateTime.Now - LastNotifTime;
            TimeSpan notifInterval = TimeSpan.FromSeconds(30);
            CurrentActiveWindow = GetActiveWindow(GetOpenWindows());

            if (CurrentActiveWindow.Key != PrevActiveWindow.Key && PrevActiveWindow.Value != null) {
                StartTime = DateTime.Now;
                LastNotifTime = StartTime;
                ts = TimeSpan.Zero;
                m.SayRnd($"Window changed to {CurrentActiveWindow.Value}.");
            }

            if (ts > notifInterval) {
                string msg = $"You have been opening {CurrentActiveWindow.Value} for {FormatTimeSpan(DateTime.Now - StartTime)}.";
                m.SayRnd(msg);
                LastNotifTime = DateTime.Now;
            }

            PrevActiveWindow = CurrentActiveWindow;
        }

        private string FormatTimeSpan(TimeSpan ts) {
            string result = "";

            if (ts.Hours > 0) {
                result += ts.Hours.ToString() + " hour";
                if (ts.Hours > 1) result += "s";
            }
            if (ts.Minutes > 0) {
                if (result.Length > 0) result += " ";
                result += ts.Minutes.ToString() + " minute";
                if (ts.Minutes > 1) result += "s";
            }
            if (ts.Seconds > 0) {
                if (result.Length > 0) result += " ";
                result += ts.Seconds.ToString() + " second";
                if (ts.Seconds > 1) result += "s";
            }

            return result;
        }

        public void Start() {
            m.SayRnd("Start monitoring screen time!");
            m.TimeUIHandle += M_TimeUIHandle;
            StartTime = DateTime.Now;
            LastNotifTime = StartTime;
            M_TimeUIHandle(m);
        }

        public void Stop() {
            m.SayRnd("Stop monitoring screen time!");
            m.TimeUIHandle -= M_TimeUIHandle;
            PrevActiveWindow = new KeyValuePair<HWND, string>();
        }


        private delegate bool EnumWindowsProc(HWND hWnd, int lParam);

        [DllImport("USER32.DLL")]
        private static extern bool EnumWindows(EnumWindowsProc enumFunc, int lParam);

        [DllImport("USER32.DLL")]
        private static extern int GetWindowText(HWND hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("USER32.DLL")]
        private static extern int GetWindowTextLength(HWND hWnd);

        [DllImport("USER32.DLL")]
        private static extern bool IsWindowVisible(HWND hWnd);

        [DllImport("USER32.DLL")]
        private static extern IntPtr GetShellWindow();

        public static IDictionary<HWND, string> GetOpenWindows() {
            HWND shellWindow = GetShellWindow();
            Dictionary<HWND, string> windows = new Dictionary<HWND, string>();

            EnumWindows(delegate (HWND hWnd, int lParam) {
                if (hWnd == shellWindow) return true;
                if (!IsWindowVisible(hWnd)) return true;

                int length = GetWindowTextLength(hWnd);
                if (length == 0) return true;

                StringBuilder builder = new StringBuilder(length);
                GetWindowText(hWnd, builder, length + 1);

                windows[hWnd] = builder.ToString();
                return true;

            }, 0);

            return windows;
        }

        public static KeyValuePair<IntPtr, string> GetActiveWindow(IDictionary<HWND, string> windows) {
            KeyValuePair<IntPtr, string> result = new KeyValuePair<IntPtr, string>();

            foreach (KeyValuePair<IntPtr, string> window in windows) {
                //IntPtr handle = window.Key;
                //string title = window.Value;
                //Console.WriteLine("{0}: {1}", handle, title);

                if (window.Value == "MainWindow" || window.Value == "Your Expenses") continue;
                else {
                    result = window;
                    break;
                }
            }

            return result;
        }
    }
}
