using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Principal;
using Serilog;

namespace Avalonia.CpuLimiter.Models;

public class AdminRunner
{
    private static readonly ILogger _logger = App.Current.logger;

    [DllImport("libc")]
    private static extern uint getuid();

    public static bool IsRunAsAdmin()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Console.WriteLine("Running in Windows");
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new(identity);

            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ||
            RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ||
            RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
        {
            Console.WriteLine("Running in unix-like System: Linux / macos / unix");
            return getuid() == 0;
        }

        return false;
    }

    public static void RunElevated()
    {
        ProcessStartInfo procInfo = CreateProcessStartInfo(Environment.ProcessPath, new string[] { });
        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process? process = Process.Start(procInfo);
            }

            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process? process = Process.Start(procInfo);

                while (!process!.StandardOutput.EndOfStream)
                {
                    string? standardOutput = process.StandardOutput.ReadLine();

                    if (standardOutput!.Contains("Password:"))
                    {
                        string? passwd = Console.ReadLine();
                        StreamWriter myStreamWriter = process.StandardInput;
                        myStreamWriter.WriteLineAsync(passwd);
                        break;
                    }
                }
            }

            Console.WriteLine("AdminRunner: Continue to run");
        }

        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }


    private static ProcessStartInfo CreateProcessStartInfo(string programPath, string[] args)
    {
        ProcessStartInfo procstartInfo = new()
        {
            UseShellExecute = true
        };

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            ConfigureProcStartInfoForWindows(procstartInfo, programPath, args);
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            ConfigureProcStartInfoForLinux(procstartInfo, programPath, args);
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            ConfigureProcStartInfoForMacOS(procstartInfo, programPath, args);
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
            ConfigureProcStartInfoForLinux(procstartInfo, programPath, args);

        return procstartInfo;
    }

    private static void ConfigureProcStartInfoForWindows(ProcessStartInfo procStartInfo, string programPath,
        string[] args)
    {
        procStartInfo.Verb = "runas";
        procStartInfo.FileName = programPath;
        procStartInfo.CreateNoWindow = true;

        foreach (string arg in args) procStartInfo.ArgumentList.Add(arg);
    }

    private static void ConfigureProcStartInfoForLinux(ProcessStartInfo procStartInfo, string programPath,
        string[] args)
    {
        procStartInfo.FileName = "sudo";
        procStartInfo.UseShellExecute = false;
        procStartInfo.ArgumentList.Add("-E");
        procStartInfo.ArgumentList.Add(programPath);
        procStartInfo.RedirectStandardInput = true;
        procStartInfo.RedirectStandardOutput = true;
        procStartInfo.RedirectStandardError = true;

        foreach (string arg in args) procStartInfo.ArgumentList.Add(arg);
    }

    private static void ConfigureProcStartInfoForMacOS(ProcessStartInfo procStartInfo, string programPath,
        string[] args)
    {
        procStartInfo.FileName = "osascript";
        procStartInfo.ArgumentList.Add("-e");
        procStartInfo.ArgumentList.Add(
            $"do shell script \"{programPath} {string.Join(' ', args)}\" with prompt \"MyProgram\" with administrator privileges");
    }


    public static void RunAsAdmin(int cpuCoreNum, string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new InvalidOperationException("The executable file Path cannot be empty");

        if (IsRunAsAdmin())
            _logger.Information("AdminRunner: Run as machine administrator");
        else
            _logger.Warning("Running without administrator privileges.");

        ProcessStartInfo startInfo = null;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            startInfo = new ProcessStartInfo(path)
            {
                UseShellExecute = false,
                WorkingDirectory = Path.GetDirectoryName(path),
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
                // WindowStyle = ProcessWindowStyle.Hidden
            };
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            startInfo = new ProcessStartInfo(path)
            {
                UseShellExecute = true,
                // UserName = App.Current.UserName,

                WorkingDirectory = Path.GetDirectoryName(path)
                // RedirectStandardInput = true,
                // RedirectStandardOutput = true,
                // RedirectStandardError = true,
            };
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            _logger.Error("Not supported on OS X in this moment");
            throw new PlatformNotSupportedException("This platform is not supported");
        }


        try
        {
            Process process = new() { StartInfo = startInfo };

            process.Start();

            ProcessHelper.SetProcessAffinity(process, cpuCoreNum);

            _logger.Information($"Process started with Name: {process.ProcessName}");

            _logger.Information($"Process started with PID: {process.Id}");

            string bitMask = ProcessHelper.GetProcessAffinityBitMask(process);

            _logger.Information($"Process started with PID: {process.Id}");
            process.WaitForExit();

            int exitCode = process.ExitCode;
            _logger.Information($"Process exited with exit code: {exitCode}");

            process.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
            throw;
        }
    }
}