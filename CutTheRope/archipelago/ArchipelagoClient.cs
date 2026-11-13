using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.Models;
using Archipelago.MultiClient.Net.Packets;
using CutTheRope.iframework;

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
                
                // TODO: Datastorage if needed? Scouting if needed?
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

    private void UpdateHasWon(ArchipelagoClientState state)
    {
        HasWon = state == ArchipelagoClientState.ClientGoal;
        //APTVManager.FlagTvNeedsUpdate();
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
            default:
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