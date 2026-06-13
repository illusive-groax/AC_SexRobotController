namespace AC_SexRobotController.Helpers
{
    internal sealed class StringConstants
    {
        internal const string GAME_NAME = "Aicomi";
        internal const string GAME_VR_NAME = "AicomiVR";

        internal const string PLUGIN_VERSION = "1.2.1";
        internal const string PLUGIN_NAME = "AC_SexRobotController";
        internal const string PLUGIN_GUID = "AC_SexRobotController";

        /* Name of Buttons to look for (to add Plugin Buttons to GUI */
        internal const string SCENE_CONFIG_BUTTON = "btnConfig";
        internal const string CONFIG_MENU_RESET_BUTTON = "btnInit";
        internal const string CONFIG_MENU_TITLE_BUTTON = "btnTitle";

        /** Path to file containing the names of the animations and positions */
        internal const string ANIMATION_FILE_PATH = ".\\BepinEx\\Plugins\\AC_SexRobotController\\SexRobotController.txt";
        internal const string UNKNOWN_ANIMATIONS_FILE_PATH = ".\\BepinEx\\Plugins\\AC_SexRobotController\\animations.txt";

        /** BepinEx: Plugin Settings Menu **/
        // General settings
        internal const string SexRobotGeneralSection = "General";
        internal const string BepinExDebugOutput = "BepInEx Debug: Console Output";
        internal const string ReadAnimationsFromFile = "Read animations from file?";
        internal const string ReadAnimationsFromFile_Tooltip = "Reads animations and their mapping from file, allowing to include animations currently not implemented in the Plugin.";
        internal const string WriteNotFoundPositionsToFile = "Write animation names to file?";
        internal const string WriteNotFoundPositionsToFile_Tooltip = "Writes the name of the animations which currently are not available in the Plugin to a file.";
        // WIP: Disable options
        internal const string DisablePlugin = "Disable Plugin";
        internal const string DisablePlugin_Tooltip = "Disables the plugin temporarily. Scene reload required for SR6 to connect and move according to scene animations(?).";
        internal const string HideSettingsMultiplierButtons = "Hide multiplier buttons:";
        internal const string HideSettingsMultiplierButtons_Tooltip = "Enabling this option ensures that no multiplier buttons are added to the Settings menu. Game restart required(?).";

        // Connection settings
        internal const string SexRobotConnectionSection = "Sex Robot: Connection";
        internal const string SerialPortConfig = "Serial Port For Sex Robot";
        internal const string SerialPortConfig_Tooltip = "Available Serial ports";
        // how often the physical device should be updated
        internal const string SexRobotUpdateFrequencyConfig = "Sex Robot Update Frequency";
        internal const string SexRobotUpdateFrequencyConfig_Tooltip = "Sex Robot Update Frequencies";
        internal const string SerialPortStatus = "Serial Port Status Information";
        internal const string SerialPortStatus_Tooltip = "Serial Port is not connected";
        internal const string SerialPortStatus_Disconnected = " port is disconnected.";
        internal const string SerialPortConnected = "Connect via Serial Port";

        // Multiplier settings
        internal const string SexRobotLimitsSection = "Sex Robot: L0 Control";
        // length
        internal const string RobotL0IntensityMultiplier = "Sex Robot (L0) Length Multiplier";
        internal const string RobotL0IntensityMultiplier_Tooltip = "Sex Robot (L0) Length Multiplier: How far does the L0 move?";
        internal const string RobotL0MultiplierStepValue = "Sex Robot (L0) Step Value";
        internal const string RobotL0MultiplierStepValue_Tooltip = "Increase or decrease the amount of Sex Robot (L0) multiplier steps";
        // speed
        internal const string RobotL0SpeedMultiplier = "Sex Robot (L0) Speed Multiplier";
        internal const string RobotL0AmplifierMultiplier_Tooltip = "Sex Robot (L0) Speed Multiplier: How fast should it respond to the tracked motion?";
        // min/max for the Axes
        internal const string SexRobotMinMaxSection = "Sex Robot: Min/Max Values";
        internal const string RobotL0Min = "Sex Robot (L0) Up/Down Min";
        internal const string RobotL0Min_Tooltip = "Sex Robot (L0) Up/Down Min";
        internal const string RobotL0Max = "Sex Robot (L0) Up/Down Max";
        internal const string RobotL0Max_Tooltip = "Sex Robot (L0) Up/Down Max";
        internal const string RobotL1Min = "Sex Robot (L1) Forward/Backward Min";
        internal const string RobotL1Min_Tooltip = "Sex Robot (L1) Forward/Backward Min";
        internal const string RobotL1Max = "Sex Robot (L1) Forward/Backward Max";
        internal const string RobotL1Max_Tooltip = "Sex Robot (L1) Forward/Backward Max";
        internal const string RobotL2Min = "Sex Robot (L2) Left/Right Min";
        internal const string RobotL2Min_Tooltip = "Sex Robot (L2) Left/Right Min";
        internal const string RobotL2Max = "Sex Robot (L2) Left/Right Max";
        internal const string RobotL2Max_Tooltip = "Sex Robot (L2) Left/Right Max";
        internal const string RobotR0Min = "Sex Robot (R0) Twist Min";
        internal const string RobotR0Min_Tooltip = "Sex Robot (R0) Twist Min";
        internal const string RobotR0Max = "Sex Robot (R0) Twist Max";
        internal const string RobotR0Max_Tooltip = "Sex Robot (R0) Twist Max";
        internal const string RobotR1Min = "Sex Robot (R1) Roll Min";
        internal const string RobotR1Min_Tooltip = "Sex Robot (R1) Roll Min";
        internal const string RobotR1Max = "Sex Robot (R1) Roll Max";
        internal const string RobotR1Max_Tooltip = "Sex Robot (R1) Roll Max";
        internal const string RobotR2Min = "Sex Robot (R2) Pitch Min";
        internal const string RobotR2Min_Tooltip = "Sex Robot (R2) Pitch Min";
        internal const string RobotR2Max = "Sex Robot (R2) Pitch Max";
        internal const string RobotR2Max_Tooltip = "Sex Robot (R2) Pitch Max";

        // Limiter settings
        internal const string SexRobotLimiterSection = "Sex Robot L0 Limiter";
        internal const string StrokeLengthLimiter = "Enable Limiter for L0?";
        internal const string StrokeLengthLimiter_Tooltip = "Sets a limiter that overrides the default value. Useful when switching positions, where the default multiplier causes the stroking speed to be too fast/hard.";
        internal const string LimitRobotL0IntensityMultiplierValue = "Sex Robot (L0) Length Limiter (Optional)";
        internal const string LimitRobotL0IntensityMultiplierValue_Tooltip = "Sex Robot (L0) Length multiplier enabling the reduction of the L0 movement.";
        internal const string LimitRobotL0SpeedMultiplierValue = "Sex Robot (L0) Speed Limiter (Optional)";
        internal const string LimitRobotL0SpeedMultiplierValue_Tooltip = "Sex Robot (L0) Speed multiplier to limit the speed for animations where the tracking leads to too fast movement.";

        // Keyboard shortcuts
        internal const string SexRobotKeyboardShortcutsSection = "Keyboard shortcuts";
        internal const string ToggleSerialPortConnectionKey = "Connect/Disconnect Sex Robot";
        internal const string ToggleStrokeLengthLimiterKey = "Stroke: Enable/Disable L0 Limiter";
        internal const string IncreaseStrokeLengthMultiplierKey = "Stroke Length: Increase multiplier";
        internal const string DecreaseStrokeLengthMultiplierKey = "Stroke Length: Decrease multiplier";
        internal const string IncreaseStrokeSpeedMultiplierKey = "Stroke Speed: Increase multiplier";
        internal const string DecreaseStrokeSpeedMultiplierKey = "Stroke Speed: Decrease multiplier";

        /** Buttons **/
        // connect robot
        internal const string ButtonConnectRobot_Name = "btnConnectRobot";
        internal const string ButtonConnectRobot_Text = "Connect Robot";
        internal const string ButtonConnectRobot_Connected = "Connected";
        internal const string ButtonConnectRobot_NotConnected = "Can't Connect";
        // disconnect robot
        internal const string ButtonDisconnectRobot_Text = "Disconnect Robot";
        internal const string ButtonDisconnectRobot_Disconnected = "Disconnected";
        internal const string ButtonDisconnectRobot_NotDisconnected = "Can't Disconnect";
        // increase length
        internal const string ButtonIncreaseStrokeLength_Name = "btnIncreaseStrokeLength";
        internal const string ButtonIncreaseStrokeLength_Text = "Length Multiplier +";
        // decrease length
        internal const string ButtonDecreaseStrokeLength_Name = "btnDecreaseStrokeLength";
        internal const string ButtonDecreaseStrokeLength_Text = "Length Multiplier -";
        // increase speed
        internal const string ButtonIncreaseStrokeSpeed_Name = "btnIncreaseStrokSpeed";
        internal const string ButtonIncreaseStrokeSpeed_Text = "Speed Multiplier +";
        // decrease speed
        internal const string ButtonDecreaseStrokeSpeed_Name = "btnDecreaseStrokSpeed";
        internal const string ButtonDecreaseStrokeSpeed_Text = "Speed Multiplier -";
        // speed limiter
        internal const string ButtonLimitStrokeMultiplier_Name = "btnLimitStroke";
        internal const string ButtonLimitStrokeMultiplier_Text = "Use L0 limiter";
        internal const string ButtonLimitStrokeMultiplier_Enabled = "Activated";
        internal const string ButtonLimitStrokeMultiplier_Disabled = "Deactivated";

        /** Status messages **/
        internal const string Status_CurrentStrokeMultiplierValue = "Stroke multiplier: ";
        internal const string Status_LimitStroke = "L0 Limiter enabled: ";

        internal static readonly string[] SerialPorts = [
            "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9", "COM10",
            "COM11", "COM12", "COM13", "COM14", "COM15", "COM16", "COM17", "COM18", "COM19",
            "COM20", "COM21", "COM22", "COM23", "COM24", "COM25", "COM26", "COM27", "COM28",
            "COM29", "COM30", "COM31", "COM32", "COM33"
        ];
    }
}
