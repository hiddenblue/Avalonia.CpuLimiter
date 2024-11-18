using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Avalonia.CpuLimiter.Models
{
    public class AdminRunner
    {
        [DllImport("libc")]
        private static extern uint getuid();
        
        public static bool IsRunAsAdmin()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new(identity);

                return principal.IsInRole(WindowsBuiltInRole.Administrator); 
            }
            else if(RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ||
                    RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
            {
                return getuid() == 0;
            }
            else
            {
                return false;
            }
        }

        public static async Task RunElevated()
        {
            ProcessStartInfo procInfo = CreateProcessStartInfo(Environment.ProcessPath, new string[]{});
            try
            {
                var process = Process.Start(procInfo);

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    while (!process!.StandardOutput.EndOfStream)
                    {
                        string? standardOutput = await process.StandardOutput.ReadLineAsync();
                        
                        if (standardOutput!.Contains("Password:"))
                        {
                            string? passwd = Console.ReadLine();
                            StreamWriter myStreamWriter = process.StandardInput;
                            await myStreamWriter.WriteLineAsync(passwd);
                            break;
                        }
                    }
                }
                Console.WriteLine("Continue to run");
            }
            
            catch (Exception ex)
            {
                Console.WriteLine($@"Error: " + ex.Message);
            }
        }


        private static ProcessStartInfo CreateProcessStartInfo(string programPath, string[] args)
        {
            ProcessStartInfo procstartInfo = new()
            {
                UseShellExecute = true
            };

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
               ConfigureProcStartInfoForWindows(procstartInfo, programPath, args); 
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                ConfigureProcStartInfoForLinux(procstartInfo, programPath, args);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                ConfigureProcStartInfoForMacOS(procstartInfo, programPath, args);
            }
            else if(RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
            {
                ConfigureProcStartInfoForLinux(procstartInfo, programPath, args);
            }
            return procstartInfo;
        }

        private static void ConfigureProcStartInfoForWindows(ProcessStartInfo procStartInfo, string programPath,
            string[] args)
        {
            procStartInfo.Verb = "runas";
            procStartInfo.FileName = programPath;
            procStartInfo.CreateNoWindow = true;
            
            foreach (string arg in args)
            {
                procStartInfo.ArgumentList.Add(arg);
            }
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
            
            foreach (string arg in args)
            {
                procStartInfo.ArgumentList.Add(arg);
            }
        }
        
        private static void ConfigureProcStartInfoForMacOS(ProcessStartInfo procStartInfo, string programPath,
            string[] args)
        {
            procStartInfo.FileName = "osascript";
            procStartInfo.ArgumentList.Add("-e");
            procStartInfo.ArgumentList.Add($"do shell script \"{programPath} {string.Join(' ', args)}\" with prompt \"MyProgram\" with administrator privileges");
        }
        
        

        public static void RunAsAdmin(int cpuCoreNum, string? path)
        {

            if (string.IsNullOrWhiteSpace(path))
                throw new InvalidOperationException("The executable file Path cannot be empty");

            if(IsRunAsAdmin())
                Console.WriteLine("Running as adminisitrator.");
            else
            {
                Console.WriteLine($"Running without administrator privileges.");
            }

            ProcessStartInfo startInfo;
            
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                startInfo = new ProcessStartInfo(path)
                {
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    // WindowStyle = ProcessWindowStyle.Hidden
                };
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                startInfo = new ProcessStartInfo(path)
                {
                    UseShellExecute = true,
                    // RedirectStandardInput = true,
                    // RedirectStandardOutput = true,
                    // RedirectStandardError = true,
                };
            }
            else
            {
                throw new PlatformNotSupportedException("This platform is not supported");
            }


            Process process = new() { StartInfo = startInfo };



            process.Start();

            ProcessHelper.SetProcessAffinity(process, cpuCoreNum);

            Console.WriteLine($"Process started with PID: {process.Id}");
            
            string bitMask = ProcessHelper.GetProcessAffinityBitMask(process);

            Console.WriteLine($"The program is running with affinity bitmask: {bitMask}");

            process.WaitForExit();

            int exitCode = process.ExitCode;
            Console.WriteLine($"Process exited with exit code: {exitCode}");

            process.Close();
        }
    }
}
