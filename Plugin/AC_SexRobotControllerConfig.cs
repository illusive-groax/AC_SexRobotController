using AC_SexRobotController.Helpers;
using AC_SexRobotController.RobotController;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AC_SexRobotController.Plugin
{
    internal partial class AC_SexRobotControllerPlugin : BasePlugin
    {
        internal static ConfigEntry<bool> DisablePlugin { get; set; }
        internal static ConfigEntry<bool> HideMultiplierButtonsInSettings { get; set; }
        internal static ConfigEntry<string> SerialPortConfig { get; set; }
        internal static ConfigEntry<string> SerialPortStatus { get; set; }
        internal static ConfigEntry<bool> SerialPortConnected { get; set; }
        internal static ConfigEntry<KeyboardShortcut> ToggleSerialPortConnection { get; set; }
        internal static ConfigEntry<KeyboardShortcut> StrokeLengthMultiplierIncrease { get; set; }
        internal static ConfigEntry<KeyboardShortcut> StrokeLengthMultiplierDecrease { get; set; }
        internal static ConfigEntry<KeyboardShortcut> StrokeSpeedMultiplierIncrease { get; set; }
        internal static ConfigEntry<KeyboardShortcut> StrokeSpeedMultiplierDecrease { get; set; }
        internal static ConfigEntry<KeyboardShortcut> TogglelimitRobotStrokeLength { get; set; }
        internal static ConfigEntry<float> SexRobotUpdateFrequencyConfig { get; set; }
        internal static ConfigEntry<bool> DiagnosticsConfig { get; set; }
        internal static ConfigEntry<bool> ReadAnimationsFromFile { get; set; }
        internal static ConfigEntry<bool> WriteAnimationsToFile { get; set; }
        internal static ConfigEntry<bool> LimitRobotL0Multipliers { get; set; }
        internal static ConfigEntry<float> RobotL0LengthMultiplier { get; set; }
        internal static ConfigEntry<float> RobotL0SpeedMultiplier { get; set; }
        internal static ConfigEntry<float> RobotL0MultiplierStepValue { get; set; }
        internal static ConfigEntry<float> LimitRobotL0LengthMultiplier { get; set; }
        internal static ConfigEntry<float> LimitRobotL0SpeedMultiplier { get; set; }
        internal static ConfigEntry<float> RobotOrgasmSpeedMultiplier { get; set; }
        internal static ConfigEntry<float> RobotL0Min { get; set; }
        internal static ConfigEntry<float> RobotL0Max { get; set; }
        internal static ConfigEntry<float> RobotL1Min { get; set; }
        internal static ConfigEntry<float> RobotL1Max { get; set; }
        internal static ConfigEntry<float> RobotL2Min { get; set; }
        internal static ConfigEntry<float> RobotL2Max { get; set; }
        internal static ConfigEntry<float> RobotR0Min { get; set; }
        internal static ConfigEntry<float> RobotR0Max { get; set; }
        internal static ConfigEntry<float> RobotR1Min { get; set; }
        internal static ConfigEntry<float> RobotR1Max { get; set; }
        internal static ConfigEntry<float> RobotR2Min { get; set; }
        internal static ConfigEntry<float> RobotR2Max { get; set; }

        internal static TextMeshProUGUI buttonConnectRobotText;
        internal static TextMeshProUGUI buttonLimitStrokeMultipliersText;
        internal static TextMeshProUGUI buttonStrokeLengthMultiplierIncreaseText;
        internal static TextMeshProUGUI buttonStrokeLengthMultiplierDecreaseText;
        internal static TextMeshProUGUI buttonStrokeSpeedMultiplierIncreaseText;
        internal static TextMeshProUGUI buttonStrokeSpeedMultiplierDecreaseText;

        internal static Transform buttonConnectRobot;
        internal static Transform buttonLimitStrokeMultipliers;
        internal static Transform buttonStrokeLengthMultiplierIncrease;
        internal static Transform buttonStrokeLengthMultiplierDecrease;
        internal static Transform buttonStrokeSpeedMultiplierIncrease;
        internal static Transform buttonStrokeSpeedMultiplierDecrease;

        internal static bool fileIsRead = false;
        internal static bool btnConnectRobotClicked = false;
        internal static bool btnDisconnectRobotClicked = false;
        internal static bool btnStrokeLengthMultiplierIncreaseClicked = false;
        internal static bool btnStrokLengthMultiplierDecreaseClicked = false;
        internal static bool btnStrokeSpeedMultiplierIncreaseClicked = false;
        internal static bool btnStrokeSpeedMultiplierDecreaseClicked = false;
        internal static bool btnLimitRobotStrokeLengthClicked = false;

        private static SerialPortConnection _serialPortConnection;

        private void SetupPluginConfigurations()
        {
            _serialPortConnection = SerialPortConnection.GetInstance();
            // Creates a config file in BepInEx/config named AC_SexRobotControllerPlugin.cfg
            // GENERAL
            DisablePlugin = Config.Bind(StringConstants.SexRobotGeneralSection, StringConstants.DisablePlugin, false, new ConfigDescription(StringConstants.DisablePlugin_Tooltip));
            HideMultiplierButtonsInSettings = Config.Bind(StringConstants.SexRobotGeneralSection, StringConstants.HideSettingsMultiplierButtons, false, new ConfigDescription(StringConstants.HideSettingsMultiplierButtons_Tooltip));
            DiagnosticsConfig = Config.Bind(StringConstants.SexRobotGeneralSection, StringConstants.BepinExDebugOutput, false);
            //ReadAnimationsFromFile = Config.Bind(StringConstants.SexRobotGeneralSection, StringConstants.ReadAnimationsFromFile, false, new ConfigDescription(StringConstants.ReadAnimationsFromFile_Tooltip));
            WriteAnimationsToFile = Config.Bind(StringConstants.SexRobotGeneralSection, StringConstants.WriteNotFoundPositionsToFile, false, new ConfigDescription(StringConstants.WriteNotFoundPositionsToFile_Tooltip));

            // KEYBOARD SHORTCUTS
            ToggleSerialPortConnection = Config.Bind(StringConstants.SexRobotKeyboardShortcutsSection, StringConstants.ToggleSerialPortConnectionKey, new KeyboardShortcut(KeyCode.S, KeyCode.LeftShift));
            StrokeLengthMultiplierIncrease = Config.Bind(StringConstants.SexRobotKeyboardShortcutsSection, StringConstants.IncreaseStrokeLengthMultiplierKey, new KeyboardShortcut(KeyCode.U));
            StrokeLengthMultiplierDecrease = Config.Bind(StringConstants.SexRobotKeyboardShortcutsSection, StringConstants.DecreaseStrokeLengthMultiplierKey, new KeyboardShortcut(KeyCode.T));
            StrokeSpeedMultiplierIncrease = Config.Bind(StringConstants.SexRobotKeyboardShortcutsSection, StringConstants.IncreaseStrokeSpeedMultiplierKey, new KeyboardShortcut(KeyCode.U, KeyCode.LeftShift));
            StrokeSpeedMultiplierDecrease = Config.Bind(StringConstants.SexRobotKeyboardShortcutsSection, StringConstants.DecreaseStrokeSpeedMultiplierKey, new KeyboardShortcut(KeyCode.T, KeyCode.LeftShift));
            TogglelimitRobotStrokeLength = Config.Bind(StringConstants.SexRobotKeyboardShortcutsSection, StringConstants.ToggleStrokeLengthLimiterKey, new KeyboardShortcut(KeyCode.Space));

            // CONNECTION
            SexRobotUpdateFrequencyConfig = Config.Bind(StringConstants.SexRobotConnectionSection, StringConstants.SexRobotUpdateFrequencyConfig, 30.0f, new ConfigDescription(StringConstants.SexRobotUpdateFrequencyConfig_Tooltip, new AcceptableValueRange<float>(1.0f, 120.0f)));
            SerialPortStatus = Config.Bind(StringConstants.SexRobotConnectionSection, StringConstants.SerialPortStatus, StringConstants.SerialPortStatus_Tooltip);
            (SerialPortConfig = Config.Bind(StringConstants.SexRobotConnectionSection, StringConstants.SerialPortConfig, StringConstants.SerialPorts[0], new ConfigDescription(StringConstants.SerialPortConfig_Tooltip, new AcceptableValueList<string>(StringConstants.SerialPorts)))).SettingChanged += (s, e) =>
            {
                _serialPortConnection.UpdateSerialPort();
            };
            SerialPortStatus.Value = SerialPortConfig.Value + StringConstants.SerialPortStatus_Disconnected;
            (SerialPortConnected = Config.Bind(StringConstants.SexRobotConnectionSection, StringConstants.SerialPortConnected, true)).SettingChanged += (s, e) =>
            {
                _serialPortConnection.UpdateSerialPortConnection();
            };

            // L0 MULTIPLIERS
            RobotL0LengthMultiplier = Config.Bind(StringConstants.SexRobotLimitsSection, StringConstants.RobotL0IntensityMultiplier, RobotMovement.L0_DEFAULT_INTENSITY,
                new ConfigDescription(StringConstants.RobotL0IntensityMultiplier_Tooltip, new AcceptableValueRange<float>(RobotMovement.L0_INTENSITY_MIN, RobotMovement.L0_INTENSITY_MAX)));
            RobotL0SpeedMultiplier = Config.Bind(StringConstants.SexRobotLimitsSection, StringConstants.RobotL0SpeedMultiplier, RobotMovement.L0_DEFAULT_MAXSPEED,
                new ConfigDescription(StringConstants.RobotL0AmplifierMultiplier_Tooltip, new AcceptableValueRange<float>(RobotMovement.L0_MAXSPEED_MIN, RobotMovement.L0_MAXSPEED_MAX)));
            RobotL0MultiplierStepValue = Config.Bind(StringConstants.SexRobotLimitsSection, StringConstants.RobotL0MultiplierStepValue, 0.25f, new ConfigDescription(StringConstants.RobotL0MultiplierStepValue_Tooltip, new AcceptableValueRange<float>(0.01f, 1.0f)));
            RobotOrgasmSpeedMultiplier = Config.Bind(StringConstants.SexRobotLimitsSection, StringConstants.RobotOrgasmSpeedMultiplier, 0.25f, new ConfigDescription(StringConstants.RobotOrgasmSpeedMultiplier_Tooltip, new AcceptableValueRange<float>(0.0f, 1.0f)));

            // L0 LIMITER (OPTIONAL)
            LimitRobotL0Multipliers = Config.Bind(StringConstants.SexRobotLimiterSection, StringConstants.StrokeLengthLimiter, false, new ConfigDescription(StringConstants.StrokeLengthLimiter_Tooltip));
            LimitRobotL0LengthMultiplier = Config.Bind(StringConstants.SexRobotLimiterSection, StringConstants.LimitRobotL0IntensityMultiplierValue, RobotMovement.L0_DEFAULT_INTENSITY,
                new ConfigDescription(StringConstants.LimitRobotL0IntensityMultiplierValue_Tooltip, new AcceptableValueRange<float>(RobotMovement.L0_INTENSITY_MIN, RobotMovement.L0_INTENSITY_MAX)));
            LimitRobotL0SpeedMultiplier = Config.Bind(StringConstants.SexRobotLimiterSection, StringConstants.LimitRobotL0SpeedMultiplierValue, RobotMovement.L0_DEFAULT_MAXSPEED,
                new ConfigDescription(StringConstants.LimitRobotL0SpeedMultiplierValue_Tooltip, new AcceptableValueRange<float>(RobotMovement.L0_MAXSPEED_MIN, RobotMovement.L0_MAXSPEED_MAX)));

            // MIN/MAX: L0, L1, L2, R0, R1, R2
            RobotL0Min = Config.Bind(StringConstants.SexRobotMinMaxSection, StringConstants.RobotL0Min, 0.0f, new ConfigDescription(StringConstants.RobotL0Min_Tooltip, new AcceptableValueRange<float>(0.0f, 0.5f)));
            RobotL0Max = Config.Bind(StringConstants.SexRobotMinMaxSection, StringConstants.RobotL0Max, 1.0f, new ConfigDescription(StringConstants.RobotL0Max_Tooltip, new AcceptableValueRange<float>(0.5f, 1.0f)));
            RobotL1Min = Config.Bind(StringConstants.SexRobotMinMaxSection, StringConstants.RobotL1Min, 0.0f, new ConfigDescription(StringConstants.RobotL1Min_Tooltip, new AcceptableValueRange<float>(0.0f, 0.5f)));
            RobotL1Max = Config.Bind(StringConstants.SexRobotMinMaxSection, StringConstants.RobotL1Max, 1.0f, new ConfigDescription(StringConstants.RobotL1Max_Tooltip, new AcceptableValueRange<float>(0.5f, 1.0f)));
            RobotL2Min = Config.Bind(StringConstants.SexRobotMinMaxSection, StringConstants.RobotL2Min, 0.0f, new ConfigDescription(StringConstants.RobotL2Min_Tooltip, new AcceptableValueRange<float>(0.0f, 0.5f)));
            RobotL2Max = Config.Bind(StringConstants.SexRobotMinMaxSection, StringConstants.RobotL2Max, 1.0f, new ConfigDescription(StringConstants.RobotL2Max_Tooltip, new AcceptableValueRange<float>(0.5f, 1.0f)));
            RobotR0Min = Config.Bind(StringConstants.SexRobotMinMaxSection, StringConstants.RobotR0Min, 0.0f, new ConfigDescription(StringConstants.RobotR0Min_Tooltip, new AcceptableValueRange<float>(0.0f, 0.5f)));
            RobotR0Max = Config.Bind(StringConstants.SexRobotMinMaxSection, StringConstants.RobotR0Max, 1.0f, new ConfigDescription(StringConstants.RobotR0Max_Tooltip, new AcceptableValueRange<float>(0.5f, 1.0f)));
            RobotR1Min = Config.Bind(StringConstants.SexRobotMinMaxSection, StringConstants.RobotR1Min, 0.0f, new ConfigDescription(StringConstants.RobotR1Min_Tooltip, new AcceptableValueRange<float>(0.0f, 0.5f)));
            RobotR1Max = Config.Bind(StringConstants.SexRobotMinMaxSection, StringConstants.RobotR1Max, 1.0f, new ConfigDescription(StringConstants.RobotR1Max_Tooltip, new AcceptableValueRange<float>(0.5f, 1.0f)));
            RobotR2Min = Config.Bind(StringConstants.SexRobotMinMaxSection, StringConstants.RobotR2Min, 0.0f, new ConfigDescription(StringConstants.RobotR2Min_Tooltip, new AcceptableValueRange<float>(0.0f, 0.5f)));
            RobotR2Max = Config.Bind(StringConstants.SexRobotMinMaxSection, StringConstants.RobotR2Max, 1.0f, new ConfigDescription(StringConstants.RobotR2Max_Tooltip, new AcceptableValueRange<float>(0.5f, 1.0f)));

            // if enabled, attempt to automatically connect when game is started
            if (SerialPortConnected.Value)
            {
                _serialPortConnection.UpdateSerialPortConnection();
            }
        }

        internal static void CreateSceneButton(Transform btnOriginal)
        {
            _serialPortConnection = SerialPortConnection.GetInstance();
            if (btnOriginal == null)
                return;
            try
            {
                // Create robot stroke multiplier increase button
                buttonLimitStrokeMultipliers = Object.Instantiate(btnOriginal, btnOriginal.parent).transform;
                buttonLimitStrokeMultipliers.name = StringConstants.ButtonLimitStrokeMultiplier_Name;
                buttonLimitStrokeMultipliersText = buttonLimitStrokeMultipliers.GetComponentInChildren<TextMeshProUGUI>();
                Button newButton = buttonLimitStrokeMultipliers.GetComponentInChildren<Button>();
                newButton.onClick.RemoveAllListeners();
                newButton.interactable = true;
                (newButton.onClick ??= new()).AddListener((UnityAction)new System.Action(() =>
                {
                    btnLimitRobotStrokeLengthClicked = true;
                }));

            }
            catch (System.Exception ex)
            {
                LogDebug("An error occurred when attempting to create Scene Button: " + ex.ToString());
            }
        }

        internal static void CreateControllerButtons(Transform btnConnect, Transform btnController)
        {
            _serialPortConnection = SerialPortConnection.GetInstance();
            try
            {
                if (btnController == null || btnConnect == null)
                    return;
                // Create a button for connecting / disconnecting the robot by copying one of the original config menu buttons
                buttonConnectRobot = Object.Instantiate(btnConnect, btnConnect.parent).transform;
                buttonConnectRobot.name = StringConstants.ButtonConnectRobot_Name;
                buttonConnectRobotText = buttonConnectRobot.GetComponentInChildren<TextMeshProUGUI>();
                buttonConnectRobotText.text = StringConstants.ButtonConnectRobot_Text;
                buttonConnectRobotText.fontSize = 20;
                Button newButton = buttonConnectRobot.GetComponentInChildren<Button>();
                newButton.onClick.RemoveAllListeners();
                newButton.interactable = true;
                (newButton.onClick ??= new()).AddListener((UnityAction)new System.Action(() =>
                {
                    if (SerialPortConnected.Value)
                    {
                        btnDisconnectRobotClicked = true;
                    }
                    else
                    {
                        btnConnectRobotClicked = true;
                    }
                }));

                if (!HideMultiplierButtonsInSettings.Value)
                {
                    // Create robot stroke length multiplier increase button
                    buttonStrokeLengthMultiplierIncrease = Object.Instantiate(btnController, btnController.parent).transform;
                    buttonStrokeLengthMultiplierIncrease.name = StringConstants.ButtonIncreaseStrokeLength_Name;
                    buttonStrokeLengthMultiplierIncreaseText = buttonStrokeLengthMultiplierIncrease.GetComponentInChildren<TextMeshProUGUI>();
                    buttonStrokeLengthMultiplierIncreaseText.text = StringConstants.ButtonIncreaseStrokeLength_Text;
                    buttonStrokeLengthMultiplierIncreaseText.fontSize = 18;
                    newButton = buttonStrokeLengthMultiplierIncrease.GetComponentInChildren<Button>();
                    newButton.onClick.RemoveAllListeners();
                    newButton.interactable = true;
                    (newButton.onClick ??= new()).AddListener((UnityAction)new System.Action(() =>
                    {
                        btnStrokeLengthMultiplierIncreaseClicked = true;
                    }));

                    // Create robot stroke length multiplier decrease button 
                    buttonStrokeLengthMultiplierDecrease = Object.Instantiate(btnController, btnController.parent).transform;
                    buttonStrokeLengthMultiplierDecrease.name = StringConstants.ButtonDecreaseStrokeLength_Name;
                    buttonStrokeLengthMultiplierDecreaseText = buttonStrokeLengthMultiplierDecrease.GetComponentInChildren<TextMeshProUGUI>();
                    buttonStrokeLengthMultiplierDecreaseText.text = StringConstants.ButtonDecreaseStrokeLength_Text;
                    buttonStrokeLengthMultiplierDecreaseText.fontSize = 18;
                    newButton = buttonStrokeLengthMultiplierDecrease.GetComponentInChildren<Button>();
                    newButton.onClick.RemoveAllListeners();
                    newButton.interactable = true;
                    (newButton.onClick ??= new()).AddListener((UnityAction)new System.Action(() =>
                    {
                        btnStrokLengthMultiplierDecreaseClicked = true;
                    }));

                    // Create robot stroke speed multiplier increase button
                    buttonStrokeSpeedMultiplierIncrease = Object.Instantiate(btnController, btnController.parent).transform;
                    buttonStrokeSpeedMultiplierIncrease.name = StringConstants.ButtonIncreaseStrokeSpeed_Name;
                    buttonStrokeSpeedMultiplierIncreaseText = buttonStrokeSpeedMultiplierIncrease.GetComponentInChildren<TextMeshProUGUI>();
                    buttonStrokeSpeedMultiplierIncreaseText.text = StringConstants.ButtonIncreaseStrokeSpeed_Text;
                    buttonStrokeSpeedMultiplierIncreaseText.fontSize = 18;
                    newButton = buttonStrokeSpeedMultiplierIncrease.GetComponentInChildren<Button>();
                    newButton.onClick.RemoveAllListeners();
                    newButton.interactable = true;
                    (newButton.onClick ??= new()).AddListener((UnityAction)new System.Action(() =>
                    {
                        btnStrokeSpeedMultiplierIncreaseClicked = true;
                    }));

                    // Create robot stroke speed multiplier decrease button 
                    buttonStrokeSpeedMultiplierDecrease = Object.Instantiate(btnController, btnController.parent).transform;
                    buttonStrokeSpeedMultiplierDecrease.name = StringConstants.ButtonDecreaseStrokeSpeed_Name;
                    buttonStrokeSpeedMultiplierDecreaseText = buttonStrokeSpeedMultiplierDecrease.GetComponentInChildren<TextMeshProUGUI>();
                    buttonStrokeSpeedMultiplierDecreaseText.text = StringConstants.ButtonDecreaseStrokeSpeed_Text;
                    buttonStrokeSpeedMultiplierDecreaseText.fontSize = 18;
                    newButton = buttonStrokeSpeedMultiplierDecrease.GetComponentInChildren<Button>();
                    newButton.onClick.RemoveAllListeners();
                    newButton.interactable = true;
                    (newButton.onClick ??= new()).AddListener((UnityAction)new System.Action(() =>
                    {
                        btnStrokeSpeedMultiplierDecreaseClicked = true;
                    }));
                }

                // Create button for switching L0 multipliers (dynamic switching between softer and intenser animations)
                buttonLimitStrokeMultipliers = Object.Instantiate(btnConnect, btnConnect.parent).transform;
                buttonLimitStrokeMultipliers.name = StringConstants.ButtonLimitStrokeMultiplier_Name;
                buttonLimitStrokeMultipliersText = buttonLimitStrokeMultipliers.GetComponentInChildren<TextMeshProUGUI>();
                buttonLimitStrokeMultipliersText.text = StringConstants.ButtonLimitStrokeMultiplier_Text;
                buttonLimitStrokeMultipliersText.fontSize = 20;
                newButton = buttonLimitStrokeMultipliers.GetComponentInChildren<Button>();
                newButton.interactable = true;
                (newButton.onClick ??= new()).AddListener((UnityAction)new System.Action(() =>
                {
                    btnLimitRobotStrokeLengthClicked = true;
                }));
            }
            catch (System.Exception ex)
            {
                LogDebug("An error occurred upon attempting to add Buttons to the Settings menu: " + ex.ToString());
            }
        }
    }
}
