using System.Diagnostics;
using Avalonia.CpuLimiter.Models;

namespace TestDemo;

internal class Program
{
    private static void Main(string[] args)
    {
        using (TextWriter tw = new StreamWriter("output.txt"))
        {
            Console.SetOut(tw);

            Process process = Process.GetCurrentProcess();

            Console.WriteLine(ProcessHelper.GetProcessAffinityBitMask(process));
        }
    }
}