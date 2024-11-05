using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.CpuLimiter.Models
{
    public class AdminRunner
    {
        public static bool IsRunAsAdmin()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new(identity);

                return principal.IsInRole(WindowsBuiltInRole.Administrator); 
            }
            else
            {
                throw new PlatformNotSupportedException();
            }
        }

        public static void RunElevated()
        {
            ProcessStartInfo procInfo = new ProcessStartInfo();
            procInfo.UseShellExecute = true;
            procInfo.FileName = Environment.ProcessPath;
            procInfo.Verb = "runas";
            procInfo.CreateNoWindow = true;
            // procInfo.WindowStyle = ProcessWindowStyle.Hidden;

            try
            {
                Process.Start(procInfo);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: " + ex.Message);
            }
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

            ProcessStartInfo startInfo = new ProcessStartInfo(path)
            {
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                // WindowStyle = ProcessWindowStyle.Hidden

            };

            Process process = new Process { StartInfo = startInfo };



            process.Start();
            Win32CpuAffinity win32CpuAffinity = new(cpuCoreNum);

            process = win32CpuAffinity.SetProcessWithLimitedCpu(process);

            Console.WriteLine($"Process started with PID: {process.Id}");

            process.WaitForExit();

            int exitCode = process.ExitCode;
            Console.WriteLine($"Process exited with exit code: {exitCode}");

            process.Close();
        }
    }
}
