using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Avalonia.CpuLimiter.Models
{
    class NativeCpuAffinity
    {
        // 写一个利用c#原生 api 限制当前程序调用CPU核心的类

        public static void SetProcessAffinity(int processId, int cpuId)
        {
            // 获取当前进程
            Process process = System.Diagnostics.Process.GetCurrentProcess();

            // 设置进程的CPU亲和性
            if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                process.ProcessorAffinity = 1 << cpuId;
            else
            {
                throw new PlatformNotSupportedException("not supported on this platform");
            }

        }

        public static void SetProcessAffinity(int cpuId)
        {
            // 获取当前进程
            Process process = System.Diagnostics.Process.GetCurrentProcess();
            // 设置进程的CPU亲和性
            
            if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                process.ProcessorAffinity = 1 << cpuId;
            else
            {
                throw new PlatformNotSupportedException("not supported on this platform");
            }

        }

        public static int GetProcessAffinity(int processId)
        {
            // 获取当前进程
            System.Diagnostics.Process process = System.Diagnostics.Process.GetCurrentProcess();

            // 获取进程的CPU亲和性
            if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return process.ProcessorAffinity.ToInt32();
            else
            {
                throw new PlatformNotSupportedException("not supported on this platform");
            }
            
        }


    }
}
