using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;


namespace Avalonia.CpuLimiter.Models;

class Win32CpuAffinity
{
    public Win32CpuAffinity(int cpuLogicalCoreNum)
    {
        CpuLogicalCoreNum = cpuLogicalCoreNum;
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool SetProcessAffinityMask(nint hProcess, nint dwProcessAffinityMask);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool GetProcessAffinityMask(nint hProcess, nint dwProcessAffinityMask, nint lpSystemAffinityMask);

    public Process SetProcessWithLimitedCpu(Process process)
    {
        if (CpuLogicalCoreNum <= 0)
        {
            throw new AbandonedMutexException();
        }

        if (process == null)
        {
            throw new ArgumentNullException(
                nameof(process));
        }

        nint affinityMask = new nint(CpuLogicalCoreNum); // set the local number of process;
        SetProcessAffinityMask(process.Handle, affinityMask);

        return process;

    }

    private int CpuLogicalCoreNum { get; set; }


}
