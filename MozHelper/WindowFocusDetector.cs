using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Lavasoft.WebBar.UI.AppCore.Utilities
{
    /// <summary>
    /// This class listens for windows that are brought into focus and checks
    /// if they are associated to a procees we are interested in
    /// </summary>
    class WindowFocusDetector
    {
        [DllImport("user32.dll")]
        private static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

        private const uint WINEVENT_OUTOFCONTEXT = 0;
        private const uint EVENT_SYSTEM_FOREGROUND = 3;

        [DllImport("user32.dll")]
        private static extern IntPtr UnhookWinEvent(IntPtr hWinEventHook);

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("psapi.dll")]
        private static extern uint GetModuleFileNameEx(IntPtr hWnd, IntPtr hModule, StringBuilder lpFileName, int nSize);

        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(UInt32 dwDesiredAccess, Int32 bInheritHandle, UInt32 dwProcessId);

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr hObject);

        private IntPtr m_hhook;

        delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);
        WinEventDelegate dele = null;

        public delegate void WindowFocusedDelegate();

        // The pairing of process names we are interested in and a delegate to call when they are brought into focus
        Dictionary<string, WindowFocusedDelegate> windowCheckList = new Dictionary<string, WindowFocusedDelegate>();


        public WindowFocusDetector()
        {
            dele = new WinEventDelegate(WinEventProc);

            // Setup the hook event
            m_hhook = SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, dele, 0, 0, WINEVENT_OUTOFCONTEXT);
        }

        ~WindowFocusDetector()
        {
            // Remove the hook
            UnhookWinEvent(m_hhook);
        }

        public void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            // Check to see if the window that was brought into focus is one of the ones we are listening for
            string processName = GetProcessNameFromHandle(hwnd);
            if(windowCheckList.Keys.Contains(processName))
            {
                windowCheckList[processName].Invoke();
            }
        }

        /// <summary>
        /// Add a process for which we will listen for windows becoming into focus
        /// </summary>
        /// <param name="processName">The process' filename</param>
        /// <param name="handler">A delegate to call when the process' window comes into focus</param>
        public void AddListener(string processName, WindowFocusedDelegate handler)
        {
            windowCheckList.Add(processName, handler);
        }

        /// <summary>
        /// Get the process file name for a windows handle
        /// </summary>
        private string GetProcessNameFromHandle(IntPtr hWnd)
        {
            uint processId = 0;
            const int nChars = 1024;
            StringBuilder filename = new StringBuilder(nChars);
            GetWindowThreadProcessId(hWnd, out processId);
            IntPtr hProcess = OpenProcess(1040, 0, processId);
            GetModuleFileNameEx(hProcess, IntPtr.Zero, filename, nChars);
            CloseHandle(hProcess);
            return (Path.GetFileName(filename.ToString()));
        }
    }
}
