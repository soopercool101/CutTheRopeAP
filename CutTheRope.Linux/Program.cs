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

ArchipelagoClient.ServerData.Uri = Environment.GetEnvironmentVariable("AP_HOST") ?? "localhost:38281";
ArchipelagoClient.ServerData.SlotName = Environment.GetEnvironmentVariable("AP_SLOT") ?? "Victor";
ArchipelagoClient.ServerData.Password = Environment.GetEnvironmentVariable("AP_PASSWORD") ?? string.Empty;
Game1.GameSpeedMult = (double)66 / 100.0;
Global.ApClient.Connect();

using var game = new Game1(platform);
game.Run();