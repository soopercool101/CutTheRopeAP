using System;
using System.Collections.Generic;
using System.Linq;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.Models;
using Archipelago.MultiClient.Net.Packets;
using CutTheRope.iframework;
using Newtonsoft.Json.Linq;

namespace CutTheRope.archipelago;

public class ArchipelagoClient
{
    public const string APVersion = "0.6.7";
    private const string Game = "Cut the Rope";

    public static bool Authenticated;
    public bool AttemptingConnection { get; private set; }

    public static ArchipelagoData ServerData = new();
    public static DeathLinkHandler DeathLinkHandler;
    public string Player => Session?.Players.ActivePlayer.Alias ?? string.Empty;
    public ArchipelagoSession Session { get; private set; }

    /// <summary>
    /// call to connect to an Archipelago session. Connection info should already be set up on ServerData
    /// </summary>
    /// <returns></returns>
    public bool Connect()
    {
        if (Authenticated || AttemptingConnection) return false;

        FrameworkTypes._LOG("Connecting");
        AttemptingConnection = true;

        try
        {
            Session = ArchipelagoSessionFactory.CreateSession(ServerData.Uri);
            SetupSession();
        }
        catch (Exception e)
        {
            FrameworkTypes._LOG("Failed to setup session");
        }

        FrameworkTypes._LOG("Attempting Connection");
        return TryConnect();
    }

    /// <summary>
    /// add handlers for Archipelago events
    /// </summary>
    private void SetupSession()
    {
        //Session.MessageLog.OnMessageReceived += message => ArchipelagoConsole.LogMessage(message);
        Session.Items.ItemReceived += OnItemReceived;
        Session.Socket.ErrorReceived += OnSessionErrorReceived;
        Session.Socket.SocketClosed += OnSessionSocketClosed;
    }

    /// <summary>
    /// attempt to connect to the server with our connection info
    /// </summary>
    private bool TryConnect()
    {
        try
        {
            // For now, do this fully synchronously
            //ThreadPool.QueueUserWorkItem(_ =>
            //{
                try
                {
                    FrameworkTypes._LOG("Beginning TryConnect (Inner)");
                    HandleConnectResult(Session.TryConnectAndLogin(
                        Game,
                        ServerData.SlotName,
                        ItemsHandlingFlags.AllItems, // TODO make sure to change this line
                        new Version(APVersion),
                        password: ServerData.Password,
                        requestSlotData: true // ServerData.NeedSlotData
                    ));
                    FrameworkTypes._LOG("Finishing TryConnect (Inner)");
                    return Authenticated && SlotData.Loaded;
                }
                catch (Exception e)
                {
                    FrameworkTypes._LOG("Failed connection in TryConnect (Inner)");
                    AttemptingConnection = false;
                    FrameworkTypes._LOG(e.Message);
                    HandleConnectResult(new LoginFailure(e.ToString()));
                }
            //});
        }
        catch (Exception e)
        {
            AttemptingConnection = false;
            FrameworkTypes._LOG("Failed connection in TryConnect");
            FrameworkTypes._LOG(e.Message);
            HandleConnectResult(new LoginFailure(e.ToString()));
        }

        return false;
    }

    /// <summary>
    /// handle the connection result and do things
    /// </summary>
    /// <param name="result"></param>
    private void HandleConnectResult(LoginResult result)
    {
        try
        {
            string outText;
            if (result.Successful)
            {
                var success = (LoginSuccessful) result;

                SlotData.LoadSlotData(success.SlotData);
                ServerData.SetupSession(success.SlotData, Session.RoomState.Seed);
                Authenticated = true;

                //var scouting = Session.Locations.ScoutLocationsAsync(HintCreationPolicy.None,
                //    AllLocations.Where(LocationNeedsScouting).ToArray());
                Session.DataStorage.TrackClientStatus(UpdateHasWon);

                FrameworkTypes._LOG($"SlotData logging ({ServerData.SlotData.Count} values)");
                foreach (var key in ServerData.SlotData.Keys)
                {
                    FrameworkTypes._LOG($"SlotData: {key} | {ServerData.SlotData[key]}");
                }

                var tags = new List<string>();
                if (SlotData.DeathLink)
                {
                    tags.Add("DeathLink");
                }
                //if (Plugin.SlotData.RingLink)
                //{
                //    tags.Add("RingLink");
                //}
                //if (Plugin.SlotData.TrapLink)
                //{
                //    tags.Add("TrapLink");
                //}

                Session.ConnectionInfo.UpdateConnectionOptions(Session.ConnectionInfo.Tags
                    .Concat(tags.Where(tag => Array.IndexOf(Session.ConnectionInfo.Tags, tag) == -1)).ToArray());

                DeathLinkHandler = new(Session.CreateDeathLinkService(), ServerData.SlotName, false);
                Session.Socket.PacketReceived += Socket_PacketReceived;

                //Session.Locations.CompleteLocationChecksAsync(ServerData.CheckedLocations.ToArray());

                for (var i = 0; i < SlotData.GoalBox; i++)
                {
                    var box = i;
                    Session.DataStorage[Scope.Slot, $"Box{box}Level"].GetAsync().ContinueWith(x =>
                    {
                        try
                        {
                            SetLevelUnlocked(box, x.Result.ToObject<int>());
                        }
                        catch
                        {
                            SetLevelUnlocked(box, 0);
                        }
                    });
                    Session.DataStorage[Scope.Slot, $"Box{box}Level"].OnValueChanged += UpdateBoxData[box];
                }
            }
            else
            {
                var failure = (LoginFailure) result;
                outText = $"Failed to connect to {ServerData.Uri} as {ServerData.SlotName}.";
                outText = failure.Errors.Aggregate(outText, (current, error) => current + $"\n    {error}");

                FrameworkTypes._LOG(outText);

                Authenticated = false;
                Disconnect();
            }

            //ArchipelagoConsole.LogMessage(outText);
        }
        catch (Exception ex)
        {
            FrameworkTypes._LOG(ex.Message);
            Authenticated = false;
        }
        finally
        {
            FrameworkTypes._LOG("Finalized connection in HandleConnectResult");
            AttemptingConnection = false;
        }
    }

    private static Dictionary<int, int> LevelsCompletePerBox = new();

    private object LevelUnlockLock = new();
    public bool GetLevelUnlocked(int box, int level)
    {
        lock (LevelUnlockLock)
        {
            if (LevelsCompletePerBox.TryGetValue(box, out var value))
            {
                return value >= level;
            }

            return false;
        }
    }

    public void SetLevelUnlocked(int box, int level)
    {
        lock (LevelUnlockLock)
        {
            if (LevelsCompletePerBox.TryGetValue(box, out var oldLevel) && oldLevel >= level)
            {
                return;
            }
            LevelsCompletePerBox[box] = level;
            Session.DataStorage[Scope.Slot, $"Box{box}Level"] = level;
        }
    }

    private static readonly DataStorageHelper.DataStorageUpdatedHandler[] UpdateBoxData = [
        Box0_OnLevelCountChanged,
        Box1_OnLevelCountChanged,
        Box2_OnLevelCountChanged,
        Box3_OnLevelCountChanged,
        Box4_OnLevelCountChanged,
        Box5_OnLevelCountChanged,
        Box6_OnLevelCountChanged,
        Box7_OnLevelCountChanged,
        Box8_OnLevelCountChanged,
        Box9_OnLevelCountChanged,
        Box10_OnLevelCountChanged,
    ];

    public static void Box_OnLevelCountChanged(int box, int newValue)
    {
        LevelsCompletePerBox[box] = newValue;
    }

    private static void Box0_OnLevelCountChanged(JToken originalValue, JToken newValue, Dictionary<string, JToken> additionalArguments)
    {
        Box_OnLevelCountChanged(0, newValue.ToObject<int>());
    }

    private static void Box1_OnLevelCountChanged(JToken originalValue, JToken newValue, Dictionary<string, JToken> additionalArguments)
    {
        Box_OnLevelCountChanged(1, newValue.ToObject<int>());
    }

    private static void Box2_OnLevelCountChanged(JToken originalValue, JToken newValue, Dictionary<string, JToken> additionalArguments)
    {
        Box_OnLevelCountChanged(2, newValue.ToObject<int>());
    }

    private static void Box3_OnLevelCountChanged(JToken originalValue, JToken newValue, Dictionary<string, JToken> additionalArguments)
    {
        Box_OnLevelCountChanged(3, newValue.ToObject<int>());
    }

    private static void Box4_OnLevelCountChanged(JToken originalValue, JToken newValue, Dictionary<string, JToken> additionalArguments)
    {
        Box_OnLevelCountChanged(4, newValue.ToObject<int>());
    }

    private static void Box5_OnLevelCountChanged(JToken originalValue, JToken newValue, Dictionary<string, JToken> additionalArguments)
    {
        Box_OnLevelCountChanged(5, newValue.ToObject<int>());
    }

    private static void Box6_OnLevelCountChanged(JToken originalValue, JToken newValue, Dictionary<string, JToken> additionalArguments)
    {
        Box_OnLevelCountChanged(6, newValue.ToObject<int>());
    }

    private static void Box7_OnLevelCountChanged(JToken originalValue, JToken newValue, Dictionary<string, JToken> additionalArguments)
    {
        Box_OnLevelCountChanged(7, newValue.ToObject<int>());
    }

    private static void Box8_OnLevelCountChanged(JToken originalValue, JToken newValue, Dictionary<string, JToken> additionalArguments)
    {
        Box_OnLevelCountChanged(8, newValue.ToObject<int>());
    }

    private static void Box9_OnLevelCountChanged(JToken originalValue, JToken newValue, Dictionary<string, JToken> additionalArguments)
    {
        Box_OnLevelCountChanged(9, newValue.ToObject<int>());
    }

    private static void Box10_OnLevelCountChanged(JToken originalValue, JToken newValue, Dictionary<string, JToken> additionalArguments)
    {
        Box_OnLevelCountChanged(10, newValue.ToObject<int>());
    }

    private void UpdateHasWon(ArchipelagoClientState state)
    {
        HasWon = state == ArchipelagoClientState.ClientGoal;
    }

    private void Socket_PacketReceived(ArchipelagoPacketBase packet)
    {
        try
        {
            switch (packet)
            {
                case BouncedPacket trap when trap.Tags.Contains("TrapLink"):
                    // TODO: Traplink
                    break;
            }
        }
        catch (Exception e)
        {
            FrameworkTypes._LOG(e.Message);
        }
    }

    public void SendRingLink(int amount)
    {
        //if (Plugin.SlotData.RingLink && amount != 0)
        //{
        //    BouncePacket packet = new()
        //    {
        //        Tags = ["RingLink"],
        //        Data = new()
        //        {
        //            { "time", (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds },
        //            { "source", Session.ConnectionInfo.Slot },
        //            { "amount", amount }
        //        }
        //    };
        //    Session.Socket.SendPacket(packet);
        //}
    }

    public void SendTrapLink(string trap)
    {
        //if (Plugin.SlotData.TrapLink && !string.IsNullOrEmpty(trap))
        //{
        //    BouncePacket packet = new()
        //    {
        //        Tags = ["TrapLink"],
        //        Data = new()
        //        {
        //            { "trap", (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds },
        //            { "source", Player },
        //            { "trap_name", trap }
        //        }
        //    };
        //    Session.Socket.SendPacket(packet);
        //}
    }

    /// <summary>
    /// something we wrong or we need to properly disconnect from the server. cleanup and re null our session
    /// </summary>
    private void Disconnect()
    {
        FrameworkTypes._LOG("disconnecting from server...");
        Session?.Socket.DisconnectAsync();
        Session = null;
        Authenticated = false;
        FrameworkTypes._LOG("Disconnected");
        AttemptingConnection = false;
    }

    public void SendMessage(string message)
    {
        Session.Socket.SendPacketAsync(new SayPacket { Text = message });
    }

    /// <summary>
    /// we received an item so reward it here
    /// </summary>
    /// <param name="helper">item helper which we can grab our item from</param>
    private void OnItemReceived(ReceivedItemsHelper helper)
    {
        var receivedItem = helper.DequeueItem();

        if (helper.Index < ServerData.Index) return;

        ServerData.Index++;

        switch (receivedItem.ItemId)
        {
            case 1:
                // star
                ReceivedItems.StarCount++;
                break;
            case 2:
            case 3:
                // filler, do nothing
                break;
            case 100:
                ReceivedItems.Tutorials = true;
                break;
            case 101:
                ReceivedItems.Bubble = true;
                break;
            case 102:
                ReceivedItems.AutomaticGrab = true;
                break;
            case 103:
                ReceivedItems.AirCushion = true;
                break;
            case 104:
                ReceivedItems.Slider = true;
                break;
            case 105:
                // Don't go down if electro timer is cheated in, but don't go to always off territory if multiple cheated in
                ReceivedItems.ElectroTimer = (ReceivedItems.ElectroTimerState)Math.Max((byte)ReceivedItems.ElectroTimer, (byte)ReceivedItems.ElectroTimerState.Normal);
                break;
            case 205:
                if (ReceivedItems.ElectroTimer < ReceivedItems.ElectroTimerState.AlwaysOff)
                    ReceivedItems.ElectroTimer++;
                break;
            case 106:
                ReceivedItems.Hat = true;
                break;
            case 107:
                ReceivedItems.Bouncer = true;
                break;
            case 108:
                ReceivedItems.Wheel = true;
                break;
            case 109:
                ReceivedItems.GravityButton = true;
                break;
            case 110:
                // Don't go down if blade is cheated in, but don't go to always off territory if multiple cheated in
                ReceivedItems.Blade = (ReceivedItems.BladeState)Math.Max((byte)ReceivedItems.Blade, (byte)ReceivedItems.BladeState.RotateAllowed);
                break;
            case 210:
                if (ReceivedItems.Blade < ReceivedItems.BladeState.Disabled)
                    ReceivedItems.Blade++;
                break;
            case 111:
                ReceivedItems.Bee = true;
                break;
            case 112:
                ReceivedItems.Record = true;
                break;
            case 200:
                ReceivedItems.SpikesDisable = true;
                break;
            case 201:
                ReceivedItems.TimedStarDisable = true;
                break;
            case 202:
                ReceivedItems.SpiderDisable = true;
                break;
            default:
                // Box
                if (receivedItem.ItemId is > 10_00 and < 10_99)
                {
                    ReceivedItems.ReceivedBoxItems.Add((int)(receivedItem.ItemId - 10_01));
                    break;
                }
                FrameworkTypes._LOG($"Error: Unknown item ID: {receivedItem.ItemId}");
                throw new ArgumentOutOfRangeException();
        }
    }

    public static readonly string[] Traps = [

    ];

    /// <summary>
    /// something went wrong with our socket connection
    /// </summary>
    /// <param name="e">thrown exception from our socket</param>
    /// <param name="message">message received from the server</param>
    private void OnSessionErrorReceived(Exception e, string message)
    {
        FrameworkTypes._LOG(e.Message);
        //ArchipelagoConsole.LogMessage(message);
    }

    /// <summary>
    /// something went wrong closing our connection. disconnect and clean up
    /// </summary>
    /// <param name="reason"></param>
    private void OnSessionSocketClosed(string reason)
    {
        FrameworkTypes._LOG($"Connection to Archipelago lost: {reason}");
        Disconnect();
    }

    public void SendLocation(long id)
    {
        FrameworkTypes._LOG($"Sending location #{id}");
        Session.Locations.CompleteLocationChecks(id);
    }

    public void Win()
    {
        Session.SetGoalAchieved();
        HasWon = true;
    }

    public bool HasWon { get; private set; }

    public void SendLocations(long[] ids)
    {
        Session.Locations.CompleteLocationChecks(ids);
    }

    public bool LocationUncleared(long location)
    {
        return AllLocations.Contains(location) && !AllClearedLocations.Contains(location);
    }

    public System.Collections.ObjectModel.ReadOnlyCollection<long> AllClearedLocations => Session.Locations.AllLocationsChecked;
    public System.Collections.ObjectModel.ReadOnlyCollection<long> AllLocations => Session.Locations.AllLocations;
    public Dictionary<long, ScoutedItemInfo> ScoutedLocations { get; private set; }
    public ILocationCheckHelper Locations => Session.Locations;
}