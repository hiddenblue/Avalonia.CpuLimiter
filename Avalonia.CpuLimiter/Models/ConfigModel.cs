using System.Collections.ObjectModel;
using Avalonia.CpuLimiter.ViewModels;
using Avalonia.Styling;

namespace Avalonia.CpuLimiter.Models;

public class MyConfigModel
{
   public MyConfigModel()
   {
   }

   public MyConfigModel(MyConfigModel myConfig)
   {
      HistoryLimit = myConfig.HistoryLimit;
      UserColor = myConfig.UserColor;
      StartupDecorationConfig = myConfig.StartupDecorationConfig;
      StartupCultureConfig = myConfig.StartupCultureConfig;
      ThemeVariantConfig = myConfig.ThemeVariantConfig;
   }

   public int HistoryLimit;
   public CustomColor UserColor;
   public StartupDecoration StartupDecorationConfig;
   public string StartupCultureConfig;
   public  ThemeVariant ThemeVariantConfig;

   public AboutInfo AboutInfo;

   public Collection<CustomColor> CustomColorCollection;




}

public class AboutInfo
{
   public static readonly string AppName = "Avalonia.CpuLimiter";
   public static readonly string AppVersion = "1.0";
   public static readonly string Author = "Hiddenblue";
   public static readonly string DotnetVersion = "8.0";
   public static readonly string AvaloniaUiVersion = "11.20";
   public static readonly string License = "GPLV2";
}

public enum StartupDecoration
{
   Fluent,
   Simple,
}