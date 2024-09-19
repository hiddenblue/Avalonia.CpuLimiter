using System.Runtime.InteropServices;
using System;
using System.IO;
using System.Diagnostics;
using System.Security.Principal;
using Win32CpuAffinity;

namespace CpuLimiter
{
    class Program
    {
        public static void Main(string[] args)
        {
            if (!IsRunAsAdmin())
            {
                RunElevated();
            }
            else
            {
                RunAsAdmin();
            }
        }

        static bool IsRunAsAdmin()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);

            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        static void RunElevated()
        {
            ProcessStartInfo procInfo = new ProcessStartInfo();
            procInfo.UseShellExecute = true;
            procInfo.FileName = Environment.ProcessPath;
            procInfo.Verb = "runas";
            procInfo.CreateNoWindow = true;
            procInfo.WindowStyle = ProcessWindowStyle.Hidden;

            try
            {
                Process.Start(procInfo);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: " + ex.Message);
            }
        }

        static void RunAsAdmin()
        {
            Console.WriteLine("Running as adminisitrator.");
            string executablePath = "D:\\prototype\\Prototype\\prototypef.exe";

            ProcessStartInfo startInfo = new ProcessStartInfo(executablePath)
            {
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
              
            };

            Process process = new Process { StartInfo = startInfo };

           

            process.Start();
            process = Win32CpuAffinity.Win32CpuAffinity.SetProcessWithLimitedCPU(process, 14);

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
