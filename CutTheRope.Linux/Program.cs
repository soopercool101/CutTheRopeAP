using CutTheRope;
using CutTheRope.archipelago;
using CutTheRope.Linux;
using CutTheRope.windows;

var platform = new LinuxPlatform();
Global.ScreenSizeManager = new ScreenSizeManager(2560, 1440, platform);

if (!ContentValidator.Validate(out var message))
{
    Console.WriteLine(message);
    return;
}

var host = Environment.GetEnvironmentVariable("AP_HOST");
var slot = Environment.GetEnvironmentVariable("AP_SLOT");
var password = Environment.GetEnvironmentVariable("AP_PASSWORD") ?? string.Empty;

if (host is null || slot is null)
{
    Console.WriteLine("Please set the environment variables to set your connection settings: AP_HOST, AP_SLOT, AP_PASSWORD");
    return;
}

ArchipelagoClient.ServerData.Uri = host;
ArchipelagoClient.ServerData.SlotName = slot;
ArchipelagoClient.ServerData.Password = password;
Game1.GameSpeedMult = (double)66 / 100.0;

Console.WriteLine($"Connecting to {ArchipelagoClient.ServerData.Uri} as {ArchipelagoClient.ServerData.SlotName}...");

var connectResult = await Global.ApClient.Connect();
if (!connectResult.Successful)
{
    Console.WriteLine(ArchipelagoClient.ConnectionError);
    return;
}

using var game = new Game1(platform);
game.Run();