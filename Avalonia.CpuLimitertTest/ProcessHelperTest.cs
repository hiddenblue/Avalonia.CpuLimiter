using System.Diagnostics;
using System.Runtime.InteropServices;
using Avalonia.CpuLimiter.Models;

namespace Avalonia.CpuLimitertTest;

public class ProcessHelperTest
{
    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(2, 3)]
    [InlineData(3, 7)]
    [InlineData(4, 15)]
    [InlineData(5, 31)]
    [InlineData(6, 63)]
    [InlineData(7, 127)]
    [InlineData(8, 255)]
    public void TestInt32TonintBitMask(int value, nint expectedBitMask)
    {
        // Act
        nint bitMask = ProcessHelper.Int32TonintBitMask(value);

        // Assert
        Assert.Equal(expectedBitMask, bitMask);
    }

    [Theory]
    [InlineData(0, "0")]
    [InlineData(1, "1")]
    [InlineData(2, "10")]
    [InlineData(3, "11")]
    [InlineData(4, "100")]
    [InlineData(5, "101")]
    [InlineData(6, "110")]
    [InlineData(7, "111")]
    [InlineData(8, "1000")]
    [InlineData(-1, "1111111111111111111111111111111111111111111111111111111111111111")] // 64-bit negative number
    public void TestnintBitMask(nint value, string expectedOutput)
    {
        // Arrange
        StringWriter? consoleOutput = new();
        Console.SetOut(consoleOutput);

        // Act
        ProcessHelper.PrintnintBitMask(value);
        string actualOutput = consoleOutput.ToString().Trim();

        // Assert
        Assert.Equal(expectedOutput, actualOutput);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(3, 2)]
    [InlineData(7, 3)]
    [InlineData(15, 4)]
    [InlineData(31, 5)]
    [InlineData(63, 6)]
    [InlineData(127, 7)]
    [InlineData(255, 8)]
    [InlineData(-1, 64)] // 64-bit negative number with all bits set to 1
    public void TestnintBitMaskToInt32(nint bitMask, int expectedCount)
    {
        // Act
        int count = ProcessHelper.nintBitMaskToInt32(bitMask);

        // Assert
        Assert.Equal(expectedCount, count);
    }

    [Fact]
    public void TestGetProcessAffinity_Windows1()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            // Skip the test if not running on Windows
            return;

        // Arrange
        Process currentProcess = Process.GetCurrentProcess();

        // Act
        int affinity = ProcessHelper.GetProcessAffinity(currentProcess);

        // Assert
        Assert.True(affinity == 16);
    }

    [Fact]
    public void TestGetProcessAffinity_Windows2()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            // Skip the test if not running on Windows
            return;

        // Act
        int affinity = ProcessHelper.GetProcessAffinity(17892);

        // Assert
        Assert.True(affinity > 0);
    }

    [Fact]
    public void TestGetProcessAffinity_Linux()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            // Skip the test if not running on Linux
            return;

        // Arrange
        Process currentProcess = Process.GetCurrentProcess();

        // Act
        int affinity = ProcessHelper.GetProcessAffinity(currentProcess);

        // Assert
        Assert.True(affinity > 0);
    }

    [Fact]
    public void TestGetProcessAffinity_UnsupportedPlatform()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ||
            RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            // Skip the test if running on a supported platform
            return;

        // Arrange
        Process currentProcess = Process.GetCurrentProcess();

        // Act & Assert
        Assert.Throws<PlatformNotSupportedException>(() => ProcessHelper.GetProcessAffinity(currentProcess));
    }

    [Fact]
    public void TestGetProcessAffinityBitMask_Windows()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            // Skip the test if not running on Windows
            return;

        // Arrange
        int processId = Process.GetCurrentProcess().Id;

        // Act
        string bitMask = ProcessHelper.GetProcessAffinityBitMask(processId);

        // Assert
        Assert.True(bitMask.Length > 0);
    }

    [Fact]
    public void TestGetProcessAffinityBitMask_Linux()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            // Skip the test if not running on Linux
            return;

        // Arrange
        int processId = Process.GetCurrentProcess().Id;

        // Act
        string bitMask = ProcessHelper.GetProcessAffinityBitMask(processId);

        // Assert
        Assert.True(bitMask.Length > 0);
    }

    [Fact]
    public void TestGetProcessAffinityBitMask_UnsupportedPlatform()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ||
            RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            // Skip the test if running on a supported platform
            return;

        // Arrange
        int processId = Process.GetCurrentProcess().Id;

        // Act & Assert
        Assert.Throws<PlatformNotSupportedException>(() => ProcessHelper.GetProcessAffinityBitMask(processId));
    }

    [Fact]
    public void TestGetProcessAffinityBitMask_InvalidProcessId()
    {
        // Arrange
        int invalidProcessId = -1;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => ProcessHelper.GetProcessAffinityBitMask(invalidProcessId));
    }
}