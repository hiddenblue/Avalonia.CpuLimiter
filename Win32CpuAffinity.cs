using System;
using System.Diagnostics;
using System.Runtime.InteropServices;


namespace Win32CpuAffinity
{
    class Win32CpuAffinity
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetProcessAffinityMask(IntPtr hProcess, IntPtr dwProcessAffinityMask);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetProcessAffinityMask(IntPtr hProcess, IntPtr dwProcessAffinityMask, IntPtr lpSystemAffinityMask);

        public static Process SetProcessWithLimitedCPU(Process process, int number)
        {
            if( number <= 0)
            {
                throw new AbandonedMutexException();
            }

            if (process == null)
            {
                throw new ArgumentNullException(
                    nameof(process));
            }

            IntPtr affinityMask = new IntPtr(number); // set the local number of process;
            SetProcessAffinityMask(process.Handle, affinityMask);

            return process;
                
        }
    }
}
