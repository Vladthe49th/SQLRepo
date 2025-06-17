using System;
using System.Runtime.InteropServices;
using System.Text;

class Program
{
    //  STARTUPINFO structure
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct STARTUPINFO
    {
        public int cb;
        public string lpReserved;
        public string lpDesktop;
        public string lpTitle;
        public int dwX, dwY, dwXSize, dwYSize;
        public int dwXCountChars, dwYCountChars;
        public int dwFillAttribute;
        public int dwFlags;
        public short wShowWindow;
        public short cbReserved2;
        public IntPtr lpReserved2;
        public IntPtr hStdInput, hStdOutput, hStdError;
    }

    // Process information structure
    [StructLayout(LayoutKind.Sequential)]
    public struct PROCESS_INFORMATION
    {
        public IntPtr hProcess;
        public IntPtr hThread;
        public uint dwProcessId;
        public uint dwThreadId;
    }

    // WinAPI funcs
    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    static extern bool CreateProcess(
        string lpApplicationName,
        string lpCommandLine,
        IntPtr lpProcessAttributes,
        IntPtr lpThreadAttributes,
        bool bInheritHandles,
        uint dwCreationFlags,
        IntPtr lpEnvironment,
        string lpCurrentDirectory,
        ref STARTUPINFO lpStartupInfo,
        out PROCESS_INFORMATION lpProcessInformation
    );

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern uint WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool GetExitCodeProcess(IntPtr hProcess, out uint lpExitCode);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool CloseHandle(IntPtr hObject);

    const uint INFINITE = 0xFFFFFFFF;

    static void Main(string[] args)
    {
        Console.WriteLine("Daughter process loading...");

        var si = new STARTUPINFO();
        si.cb = Marshal.SizeOf(si);
        var pi = new PROCESS_INFORMATION();

        // Executive file
        string commandLine = "notepad.exe";

        bool result = CreateProcess(
            null,
            commandLine,
            IntPtr.Zero,
            IntPtr.Zero,
            false,
            0,
            IntPtr.Zero,
            null,
            ref si,
            out pi
        );

        if (!result)
        {
            Console.WriteLine("Process error: " + Marshal.GetLastWin32Error());
            return;
        }

        // Wait for process to finish
        WaitForSingleObject(pi.hProcess, INFINITE);

        // Получаем код завершения
        if (GetExitCodeProcess(pi.hProcess, out uint exitCode))
        {
            Console.WriteLine("Process finished with code: " + exitCode);
        }
        else
        {
            Console.WriteLine("No completion code has been received!.");
        }


        CloseHandle(pi.hProcess);
        CloseHandle(pi.hThread);
    }
}

