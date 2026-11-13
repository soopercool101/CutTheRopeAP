using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace CutTheRope.archipelago;

public class ArchipelagoData
{
    public static string GameDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    public static string LoginDetailsFile = Path.Combine(GameDirectory, "apconnectioninfo.txt");

    private string _uri;
    public string Uri
    {
        get => _uri;
        set
        {
            if (_uri == null)
            {
                _uri = value;
                return;
            }
            if (_uri.Equals(value)) return;
            //FrameworkTypes._LOG("Uri changed");
            //ArchipelagoRenderer.AttemptedConnectionOnce = false;
            _uri = value;
        }
    }

    private string _slotName;
    public string SlotName
    {
        get => _slotName;
        set
        {
            if (_slotName == null)
            {
                _slotName = value;
                return;
            }
            if (_slotName.Equals(value)) return;
            //FrameworkTypes._LOG("SlotName changed");
            //ArchipelagoRenderer.AttemptedConnectionOnce = false;
            _slotName = value;
        }
    }

    private string _password;
    public string Password
    {
        get => _password;
        set
        {
            if (_password == null)
            {
                _password = value;
                return;
            }
            if (_password.Equals(value)) return;
            //FrameworkTypes._LOG("Password changed");
            //ArchipelagoRenderer.AttemptedConnectionOnce = false;
            _password = value;
        }
    }
    public int Index;

    //public List<long> CheckedLocations;

    /// <summary>
    /// seed for this archipelago data. Can be used when loading a file to verify the session the player is trying to
    /// load is valid to the room it's connecting to.
    /// </summary>
    private string seed;

    internal Dictionary<string, object> SlotData;

    public bool NeedSlotData => SlotData == null;

    public ArchipelagoData()
    {
        Uri = "archipelago.gg:";
        SlotName = "Player1";
        Password = string.Empty;

        try
        {
            if (File.Exists(LoginDetailsFile))
            {
                var lines = File.ReadAllLines(LoginDetailsFile);
                Uri = lines[0];
                SlotName = lines[1];
                Password = lines.Length > 2 ? lines[2] : string.Empty;
            }
            else
            {
                File.WriteAllLines(LoginDetailsFile, [Uri, SlotName, Password]);
            }
        }
        catch
        {
            // ignored
        }

        //CheckedLocations = [];
    }

    public ArchipelagoData(string uri, string slotName, string password)
    {
        Uri = uri;
        SlotName = slotName;
        Password = password;
        //CheckedLocations = [];
    }

    /// <summary>
    /// assigns the slot data and seed to our data handler. any necessary setup using this data can be done here.
    /// </summary>
    /// <param name="roomSlotData">slot data of your slot from the room</param>
    /// <param name="roomSeed">seed name of this session</param>
    public void SetupSession(Dictionary<string, object> roomSlotData, string roomSeed)
    {
        SlotData = roomSlotData;
        seed = roomSeed;
    }

    /// <summary>
    /// returns the object as a json string to be written to a file which you can then load
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}