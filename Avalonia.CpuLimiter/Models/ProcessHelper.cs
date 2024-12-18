﻿using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Avalonia.CpuLimiter.Models;

public class ProcessHelper
{
    public static int NativeCPUCoreLimit = Environment.ProcessorCount;

    // 写一个利用c#原生 api 限制当前程序调用CPU核心的类

    public static void SetProcessAffinity(Process process, int coreNum = 4)
    {
        if (coreNum > NativeCPUCoreLimit)
        {
            App.Current.logger.Error("Error: Logical Core number is out of range: 0 - {NativeCPUCoreLimit}.",
                NativeCPUCoreLimit);
            throw new InvalidOperationException("Logical Core number is out of range.");
        }

        // 设置进程的CPU亲和性
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ||
            RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            process.ProcessorAffinity = Int32TonintBitMask(coreNum);
        }
        else
        {
            App.Current.logger.Error("not supported on this platform");
            throw new PlatformNotSupportedException("not supported on this platform");
        }
    }

    // overload
    public static void SetProcessAffinity(int processId, int coreNum = 4)
    {
        Process process = Process.GetProcessById(processId);

        SetProcessAffinity(process, coreNum);
    }

    public static void SetProcessAffinity(Process process, uint bitMask)
    {
        if (nintBitMaskToInt32((nint)bitMask) > NativeCPUCoreLimit)
        {
            App.Current.logger.Error("Error: Logical Core number is out of range: 0 - {NativeCPUCoreLimit}.",
                NativeCPUCoreLimit);
            throw new InvalidOperationException("Logical Core number is out of range.");
        }

        // 设置进程的CPU亲和性
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ||
            RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            process.ProcessorAffinity = (nint)bitMask;
        }
        else
        {
            App.Current.logger.Error("not supported on this platform");
            throw new PlatformNotSupportedException("not supported on this platform");
        }
    }

    public static int GetProcessAffinity(Process process)
    {
        // 获取进程的CPU亲和性
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ||
            RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            return nintBitMaskToInt32(process.ProcessorAffinity);
        App.Current.logger.Error("{method} not supported on this platform", "GetProcessAffinity");
        throw new PlatformNotSupportedException("not supported on this platform");
    }

    public static int GetProcessAffinity(int processId)
    {
        // 获取进程的CPU亲和性
        Process process = Process.GetProcessById(processId);

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            return nintBitMaskToInt32(process.ProcessorAffinity);
        App.Current.logger.Error("{method} not supported on this platform", "GetProcessAffinity");
        throw new PlatformNotSupportedException("not supported on this platform");
    }

    public static string GetProcessAffinityBitMask(int processId)
    {
        // 获取进程的CPU亲和性
        Process process = Process.GetProcessById(processId);

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ||
            RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            return Convert.ToString(process.ProcessorAffinity, 2);
        App.Current.logger.Error("{method} not supported on this platform", "GetProcessAffinityBitMask");
        throw new PlatformNotSupportedException("not supported on this platform");
    }

    public static string GetProcessAffinityBitMask(Process process)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ||
            RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            return Convert.ToString(process.ProcessorAffinity, 2);
        App.Current.logger.Error("{method} not supported on this platform", "GetProcessAffinityBitMask");
        throw new PlatformNotSupportedException("not supported on this platform");
    }

    public static nint Int32TonintBitMask(int value)
    {
        // int BitNum = BitOperations.PopCount((uint)value);

        // 生成一个含有指定为1位数的nint数值

        nint BitMask = 0;

        for (var i = 0; i < value; i++) BitMask |= (nint)1 << i;

        return BitMask;
    }

    public static void PrintnintBitMask(nint value)
    {
        App.Current.logger.Information(Convert.ToString(value, 2));
    }


    public static int nintBitMaskToInt32(nint bitMask)
    {
        return BitOperations.PopCount((ulong)bitMask);
    }
}