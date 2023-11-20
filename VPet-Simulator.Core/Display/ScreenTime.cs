using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using HWND = System.IntPtr;
using System.Windows.Input;
using System.Runtime.CompilerServices;

namespace VPet_Simulator.Core {
    public partial class Main {

        public static IDictionary<HWND, string> GetOpenWindows() {
            HWND shellWindow = GetShellWindow();
            Dictionary<HWND, string> windows = new Dictionary<HWND, string>();

            EnumWindows(delegate (HWND hWnd, int lParam)
            {
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

                if (window.Value == "MainWindow") continue;
                else {
                    result = window;
                    break;
                }
            }

            return result;
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


        // Screen Time Timer
        public static DateTime ScreenTimeStartTime;
        public static TimeSpan ScreenTimeElapsed;
        public static string CurrentActiveWindow;

        public static string StartScreenTimeTimer() {
            TimeSpan screenTimeNotifInterval = TimeSpan.FromSeconds(10);
            RestartScreenTimeStats();

            while (ScreenTimeElapsed < screenTimeNotifInterval) {
                ScreenTimeElapsed = DateTime.Now - ScreenTimeStartTime;

                if (CurrentActiveWindow != GetActiveWindow(GetOpenWindows()).Value) {
                    RestartScreenTimeStats();
                }
            }

            return $"You have been opening {CurrentActiveWindow} for {(DateTime.Now - ScreenTimeStartTime).Seconds} seconds.";
        }

        private static void RestartScreenTimeStats() {
            ScreenTimeStartTime = DateTime.Now;
            ScreenTimeElapsed = TimeSpan.FromSeconds(0);
            CurrentActiveWindow = GetActiveWindow(GetOpenWindows()).Value;
        }
    }
}
