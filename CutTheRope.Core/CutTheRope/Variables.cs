using System.IO;
using System.Reflection;

namespace CutTheRope;

public class Variables
{
    public static string GameDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    public static string LoginDetailsFile = Path.Combine(GameDirectory, "apconnectioninfo.txt");
    public static string ClientVersion = $"v0.1.0";

    public static bool? DeathLinkOverride;
}