using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

public class Utils
{
    public static void Write(ConsoleColor cc, string mess)
    {
        ConsoleColor oCC = Console.ForegroundColor;

        Console.ForegroundColor = cc;
        Console.WriteLine(mess);
        Console.ForegroundColor = oCC;
    }
    public static void Info(string message)
    {
        Write(ConsoleColor.Cyan, message);
    }
    public static void Warn(string message)
    {
        Write(ConsoleColor.Yellow, message);
    }
    public static void Error(string message)
    {
        Write(ConsoleColor.Red, message);
    }

    public static void HandleError(Exception e)
    {
        string caller = Assembly.GetCallingAssembly().GetName().Name;
        Error($"Got an error from '{caller}' error: {e.Message}");
    }
}
