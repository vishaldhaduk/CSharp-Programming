using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace FirefoxHelper
{
    public class DllInjector
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dw_DesiredAccess, bool b_InheritHandle, int dw_ProcessId);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(string lp_ModuleName);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr h_Module, string procName);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        private static extern IntPtr VirtualAllocEx(IntPtr h_Process, IntPtr lp_Address, uint dw_Size, uint fl_AllocationType, uint fl_Protect);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool WriteProcessMemory(IntPtr h_Process, IntPtr lp_BaseAddress, byte[] lp_Buffer, uint n_Size, out UIntPtr lp_NumberOfBytesWriten);

        [DllImport("kernel32.dll")]
        private static extern IntPtr CreateRemoteThread(IntPtr h_Process, IntPtr lp_ThreadAttributes, uint dw_StackSize, IntPtr lp_StartAddress, IntPtr lp_Parameter, uint dw_CreationFlags, IntPtr lp_ThreadId);

        // privileges
        const int PROCESS_CREATE_THREAD = 0x0002;
        const int PROCESS_QUERY_INFORMATION = 0x0400;
        const int PROCESS_VM_OPERATION = 0x0008;
        const int PROCESS_VM_WRITE = 0x0020;
        const int PROCESS_VM_READ = 0x0010;
        // memory allocation
        const uint MEM_COMMIT = 0x00001000;
        const uint MEM_RESERVE = 0x00002000;
        const uint PAGE_READWRITE = 4;

        public static int Inject(string targetApplication, string dllName)
        {
            Process targetProcess = Process.GetProcessesByName(targetApplication)[0];

            const int procPerms = PROCESS_CREATE_THREAD | PROCESS_QUERY_INFORMATION | PROCESS_VM_OPERATION | PROCESS_VM_WRITE | PROCESS_VM_READ;
            IntPtr procHandle = OpenProcess(procPerms, false, targetProcess.Id);

            // Get pointer to function LoadLibraryA in target process
            IntPtr loadLibraryAddr = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");

            // Save DLL name in process, to pass to LoadLibraryA
            const uint memPerms = MEM_COMMIT | MEM_RESERVE;
            IntPtr allocMemAddress = VirtualAllocEx(procHandle, IntPtr.Zero, (uint)((dllName.Length + 1) * Marshal.SizeOf(typeof(char))), memPerms, PAGE_READWRITE);
            UIntPtr bytesWritten;
            WriteProcessMemory(procHandle, allocMemAddress, Encoding.Default.GetBytes(dllName), (uint)((dllName.Length + 1) * Marshal.SizeOf(typeof(char))), out bytesWritten);

            // TODO: Check the signature of the DLL we're injecting

            // Remote thread that calls LoadLibraryA, and runs DLL from target memory space.
            CreateRemoteThread(procHandle, IntPtr.Zero, 0, loadLibraryAddr, allocMemAddress, 0, IntPtr.Zero);

            return 0;
        }
    }
}