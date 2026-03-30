using System.Diagnostics;
using System.Runtime.InteropServices;
using Imperium.Engine.Data.EntityModels.Character;

namespace Imperium.Engine.Utilities
{
    public static class ApplicationUtilities
    {
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        static Process[] ImpProc = Process.GetProcessesByName("Imperium");

        public static Player CurrentPlayer  = new Player();

        private const int SW_HIDE = 0;
        private const int SW_SHOWNORMAL = 1;
        private const int SW_SHOWMINIMIZED = 2;
        private const int SW_SHOWMAXIMIZED = 3;
        private const int SW_SHOWNOACTIVATE = 4;
        private const int SW_SHOW = 5;
        private const int SW_MINIMIZE = 6;
        private const int SW_SHOWMINNOACTIVE = 7;
        private const int SW_SHOWNA = 8;
        private const int SW_RESTORE = 9;
        private const int SW_SHOWDEFAULT = 10;
        private const int SW_FORCEMINIMIZE = 11;

        private static void ChangeWindowState(int processId, int command)
        {
            try
            {
                Process proc = Process.GetProcessById(processId);
                IntPtr hWnd = proc.MainWindowHandle;
                if (hWnd == IntPtr.Zero)
                {
                    Console.WriteLine("No main window handle found for process " + processId);
                    
                    return;
                }

                ShowWindow(hWnd, command);

                // Only call SetForegroundWindow on Maximize, Restore, ShowNormal, Show
                if (command == SW_SHOWMAXIMIZED ||
                    command == SW_RESTORE ||
                    command == SW_SHOWNORMAL ||
                    command == SW_SHOW)
                {
                    SetForegroundWindow(hWnd);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error changing window state for process {processId}: {ex.Message}");
            }
        }

       

        /// <summary>
        /// Hides the window of the specified process.
        /// </summary>
        public static void HideByProcessId(int processId)
        {
            ChangeWindowState(processId, SW_HIDE);
        }

        /// <summary>
        /// Shows the window in normal (restored) state and brings it to the foreground.
        /// </summary>
        public static void ShowNormalByProcessId(int processId)
        {
            ChangeWindowState(processId, SW_SHOWNORMAL);
        }

        /// <summary>
        /// Minimizes the window and activates the next top-level window.
        /// </summary>
        public static void ShowMinimizedByProcessId(int processId)
        {
            ChangeWindowState(processId, SW_SHOWMINIMIZED);
        }

        /// <summary>
        /// Maximizes the window and brings it to the foreground.
        /// </summary>
        public static void ShowMaximizedByProcessId(int processId)
        {
            ChangeWindowState(processId, SW_SHOWMAXIMIZED);
        }

        /// <summary>
        /// Shows the window in normal state without activating it.
        /// </summary>
        public static void ShowNoActivateByProcessId(int processId)
        {
            ChangeWindowState(processId, SW_SHOWNOACTIVATE);
        }

        /// <summary>
        /// Shows the window in its current size and position, and brings it to the foreground.
        /// </summary>
        public static void ShowCurrentByProcessId(int processId)
        {
            ChangeWindowState(processId, SW_SHOW);
        }

        /// <summary>
        /// Minimizes the window and activates the next top-level window.
        /// </summary>
        public static void MinimizeByProcessId(int processId)
        {
            ChangeWindowState(processId, SW_MINIMIZE);
        }

        /// <summary>
        /// Minimizes the window without activating it.
        /// </summary>
        public static void ShowMinNoActivateByProcessId(int processId)
        {
            ChangeWindowState(processId, SW_SHOWMINNOACTIVE);
        }

        /// <summary>
        /// Shows the window in its current state without activating it.
        /// </summary>
        public static void ShowNAByProcessId(int processId)
        {
            ChangeWindowState(processId, SW_SHOWNA);
        }

        /// <summary>
        /// Restores the window from minimized or maximized state and brings it to the foreground.
        /// </summary>
        public static void RestoreByProcessId(int processId)
        {
            ChangeWindowState(processId, SW_RESTORE);
        }

        /// <summary>
        /// Sets the window display state based on the default setting of the program that started it.
        /// </summary>
        public static void ShowDefaultByProcessId(int processId)
        {
            ChangeWindowState(processId, SW_SHOWDEFAULT);
        }

        /// <summary>
        /// Forces the window to minimize, even if the application is not responding.
        /// </summary>
        public static void ForceMinimizeByProcessId(int processId)
        {
            ChangeWindowState(processId, SW_FORCEMINIMIZE);
        }
    }
}
