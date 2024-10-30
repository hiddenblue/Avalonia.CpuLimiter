using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Avalonia.CpuLimiter.Models
{
    class NativeCPUAffinity
    {
        // 写一个利用c#原生 api 限制当前程序调用CPU核心的类

        public static void SetProcessAffinity(int processId, int cpuId)
        {
            // 获取当前进程
            Process process = System.Diagnostics.Process.GetCurrentProcess();

            // 设置进程的CPU亲和性
            process.ProcessorAffinity = 1 << cpuId;

        }

        public static void SetProcessAffinity(int cpuId)
        {
            // 获取当前进程
            Process process = System.Diagnostics.Process.GetCurrentProcess();
            // 设置进程的CPU亲和性
            process.ProcessorAffinity = 1 << cpuId;

        }

        public static int GetProcessAffinity(int processId)
        {
            // 获取当前进程
            System.Diagnostics.Process process = System.Diagnostics.Process.GetCurrentProcess();

            // 获取进程的CPU亲和性
            return process.ProcessorAffinity.ToInt32();
        }


    }
}
