using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.CpuLimiter.Models
{
    public class AdminRunner
    {
        public static bool IsRunAsAdmin()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new(identity);

            return principal.IsInRole(WindowsBuiltInRole.Administrator);
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

        public static void RunAsAdmin(int CPUCoreNum, string Path)
        {

            if (string.IsNullOrWhiteSpace(Path))
                throw new InvalidOperationException("The executable file Path cannot be empty");

            Console.WriteLine("Running as adminisitrator.");

            ProcessStartInfo startInfo = new ProcessStartInfo(Path)
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
            Win32CpuAffinity win32CpuAffinity = new(CPUCoreNum);

            process = win32CpuAffinity.SetProcessWithLimitedCPU(process);

            if (process != null)
            {
                Console.WriteLine($"Process started with PID: {process.Id}");

                process.WaitForExit();

                int exitCode = process.ExitCode;
                Console.WriteLine($"Process exited with exit code: {exitCode}");

                process.Close();

            }
            else
            {
                Console.WriteLine("Failed to start process.");

            }

        }
    }
}
