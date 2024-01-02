namespace CutTheRope.archipelago
{
    public static class EnabledElements
    {
        // Progression
        public static bool AutomaticGrab { get; set; } = false;
        public static bool Bubble { get; set; } = false;
        public static bool SpikesDisable { get; set; } = false;
        public static bool AirCushion { get; set; } = false;
        public static bool Slider { get; set; } = false;
        public enum ElectroTimerState
        {
            AlwaysOn = 0,
            Normal = 1,
            AlwaysOff = 2
        }
        public static ElectroTimerState ElectroTimer { get; set; } = ElectroTimerState.AlwaysOn;
        public static bool Hat { get; set; } = false;
        public static bool Bouncer { get; set; } = false;
        public static bool Wheel { get; set; } = false;
        public static bool GravityButton { get; set; } = false;
        public enum BladeState
        {
            NoRotate = 0,
            RotateAllowed = 1,
            Disabled = 2
        }
        public static BladeState Blade { get; set; } = BladeState.NoRotate;
        public static bool Bee { get; set; } = false;
        public static bool Record { get; set; } = false;

        // Useful
        public static bool TimedStarDisable { get; set; } = false;
        public static bool SpiderDisable { get; set; } = false;

        // Filler
        public static bool Tutorials { get; set; } = false;
    }
}
