using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CutTheRope.iframework;

namespace CutTheRope.archipelago
{
    public static class SlotData
    {
        /// <summary>
        /// Backwards compatibility. Lowest x.y.0 version supported, where y is the lowest supported minor version
        /// </summary>
        public const int LowestSupportedMajorVersion = 0;
        /// <summary>
        /// Backwards compatibility. Lowest supported minor version in the lowest supported major version.
        /// </summary>
        public const int LowestSupportedMinorVersion = 0;
        /// <summary>
        /// Highest x.y.# version supported, where y is in the highest supported minor version
        /// Should be up to date with latest APWorld whenever a new version is released.
        /// </summary>
        public const int HighestSupportedMajorVersion = 0;
        /// <summary>
        /// Highest supported minor version in the highest supported major version.
        /// Should be up to date with latest APWorld whenever a new version is released.
        /// </summary>
        public const int HighestSupportedMinorVersion = 1;
        public static bool FailedValidation { get; private set; }
        public static long APWorldMajorVersion { get; set; }
        public static long APWorldMinorVersion { get; set; }
        public static long APWorldBuildVersion { get; set; }
        public static string APWorldVersionString { get; set; }
        public static bool Loaded { get; private set; }


        public static bool DeathLink { get; private set; }
        public static int DeathLinkAmnesty { get; private set; }

        public static byte GoalBox { get; private set; }
        public static int GoalBoxStarRequirement { get; private set; }

        public enum BoxLockingType
        {
            OpenWorld = 0,
            BoxItems = 1,
        }

        public static BoxLockingType BoxLockingMode { get; private set; }

        public enum StarsanityType
        {
            PerLevel = 1,
            Individual = 2,
        }

        public static StarsanityType StarsanityMode { get; private set; }

        public static bool LoadSlotData(Dictionary<string, object> slotData)
        {
            Loaded = false;
            if (slotData.TryGetValue("minor_version", out var minorVersion) && slotData.TryGetValue("major_version", out var majorVersion) && slotData.TryGetValue("build_version", out var buildVersion))
            {
                APWorldMajorVersion = (long)majorVersion;
                APWorldMinorVersion = (long)minorVersion;
                APWorldBuildVersion = (long)buildVersion;
                APWorldVersionString = $"{APWorldMajorVersion}.{APWorldMinorVersion}.{APWorldBuildVersion}";
                if (APWorldMajorVersion < LowestSupportedMajorVersion || (APWorldMajorVersion == LowestSupportedMajorVersion && APWorldMinorVersion < LowestSupportedMinorVersion))
                {
                    ArchipelagoClient.Authenticated = false;
                    FrameworkTypes._LOG($"ERROR: Game was generated on version {APWorldVersionString} which is lower than lowest supported APWorld version {LowestSupportedMajorVersion}.{LowestSupportedMinorVersion}.0. Please update your APWorld or use an older version of the mod.");
                    FailedValidation = true;
                    return false;
                }
                if (APWorldMajorVersion > HighestSupportedMajorVersion || (APWorldMajorVersion == HighestSupportedMajorVersion && APWorldMinorVersion > HighestSupportedMinorVersion))
                {
                    ArchipelagoClient.Authenticated = false;
                    FrameworkTypes._LOG($"ERROR: Game was generated on version {APWorldVersionString} which is higher than highest supported APWorld version {HighestSupportedMajorVersion}.{HighestSupportedMinorVersion}.X. Please update your game mod.");
                    FailedValidation = true;
                    return false;
                }
            }
            else
            {
                APWorldVersionString = "Unknown";
                FrameworkTypes._LOG("No slot data for version found");
                ArchipelagoClient.Authenticated = false;
                FrameworkTypes._LOG("ERROR: Game was generated on an unknown version, make sure your APWorld and game mod are up-to-date!");
                FailedValidation = true;
                return false;
            }

            DeathLink = (bool)slotData["death_link"];
            DeathLinkAmnesty = (int)(long)slotData["death_link_amnesty"];
            GoalBox = (byte)(long)slotData["goal_box"];
            GoalBoxStarRequirement = (int)(long)slotData["goal_box_star_requirement"];
            BoxLockingMode = (BoxLockingType)(long)slotData["box_locking_mode"];
            StarsanityMode = (StarsanityType)(long)slotData["starsanity_mode"];

            Loaded = true;
            return true;
        }

    }
}
