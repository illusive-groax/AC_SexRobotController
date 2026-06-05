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
        internal static ConfigEntry<string> SerialPortConfig { get; set; }
        internal static ConfigEntry<bool> SerialPortConnected { get; set; }
        internal static ConfigEntry<string> SerialPortStatus { get; set; }
        internal static ConfigEntry<KeyboardShortcut> ToggleSerialPortConnection { get; set; }
        internal static ConfigEntry<KeyboardShortcut> StrokeLengthMultiplierIncrease { get; set; }
        internal static ConfigEntry<KeyboardShortcut> StrokeLengthMultiplierDecrease { get; set; }
        internal static ConfigEntry<KeyboardShortcut> TogglelimitRobotStrokeLength { get; set; }

        internal static ConfigEntry<float> SexRobotUpdateFrequencyConfig { get; set; }
        internal static ConfigEntry<bool> DiagnosticsConfig { get; set; }
        internal static ConfigEntry<bool> ReadAnimationsFromFile { get; set; }
        internal static ConfigEntry<bool> WriteAnimationsToFile { get; set; }
        internal static ConfigEntry<bool> LimitRobotL0Length { get; set; }
        internal static ConfigEntry<float> LimitRobotL0Multiplier { get; set; }
        internal static ConfigEntry<float> RobotL0Multiplier { get; set; }
        internal static ConfigEntry<float> RobotL0MultiplierStepValue { get; set; }
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
        internal static TextMeshProUGUI buttonDisconnectRobotText;
        internal static TextMeshProUGUI buttonStrokeMultiplierIncreaseText;
        internal static TextMeshProUGUI buttonStrokeMultiplierDecreaseText;
        internal static TextMeshProUGUI buttonLimitRobotStrokeLengthText;
        internal static bool fileIsRead = false;
        internal static bool buttonConnectRobotClicked = false;
        internal static bool buttonDisconnectRobotClicked = false;
        internal static bool buttonLimitRobotStrokeLengthClicked = false;
        internal static bool buttonStrokeMultiplierIncreaseClicked = false;
        internal static bool buttonStrokeMultiplierDecreaseClicked = false;

        internal static Transform buttonConnectRobot;
        internal static Transform buttonDisconnectRobot;
        internal static Transform buttonStrokeMultiplierIncrease;
        internal static Transform buttonStrokeMultiplierDecrease;
        internal static Transform buttonLimitRobotStrokeLength;

        private static SerialPortConnection _serialPortConnection;

        private void SetupPluginConfigurations()
        {
            _serialPortConnection = SerialPortConnection.GetInstance();
            // Setup config file entries used in the in game menu
            // Creates a config file in BepInEx/config named AC_SexRobotControllerPlugin.cfg
            // general
            DiagnosticsConfig = Config.Bind(StringConstants.SexRobotGeneralSection, StringConstants.BepinExDebugOutput, false);
            ReadAnimationsFromFile = Config.Bind(StringConstants.SexRobotGeneralSection, StringConstants.ReadAnimationsFromFile, false, new ConfigDescription(StringConstants.ReadAnimationsFromFile_Tooltip));
            WriteAnimationsToFile = Config.Bind(StringConstants.SexRobotGeneralSection, StringConstants.WriteNotFoundPositionsToFile, false, new ConfigDescription(StringConstants.WriteNotFoundPositionsToFile_Tooltip));
            // connection
            ToggleSerialPortConnection = Config.Bind(StringConstants.SexRobotConnectionSection, StringConstants.ToggleSerialPortConnection, new KeyboardShortcut(KeyCode.S, KeyCode.LeftShift));
            (SerialPortConfig = Config.Bind(StringConstants.SexRobotConnectionSection, StringConstants.SerialPortConfig, StringConstants.SerialPorts[0], new ConfigDescription(StringConstants.SerialPortConfig_Tooltip, new AcceptableValueList<string>(StringConstants.SerialPorts)))).SettingChanged += (s, e) =>
            {
                _serialPortConnection.UpdateSerialPort();
            };
            SexRobotUpdateFrequencyConfig = Config.Bind(StringConstants.SexRobotConnectionSection, StringConstants.SexRobotUpdateFrequencyConfig, 30.0f, new ConfigDescription(StringConstants.SexRobotUpdateFrequencyConfig_Tooltip, new AcceptableValueRange<float>(1.0f, 120.0f)));
            SerialPortStatus = Config.Bind(StringConstants.SexRobotConnectionSection, StringConstants.SerialPortStatus, StringConstants.SerialPortStatus_Tooltip);
            SerialPortStatus.Value = SerialPortConfig.Value + StringConstants.SerialPortStatus_Disconnected;
            (SerialPortConnected = Config.Bind(StringConstants.SexRobotConnectionSection, StringConstants.SerialPortConnected, true)).SettingChanged += (s, e) =>
            {
                _serialPortConnection.UpdateSerialPortConnection();
            };
            //multipliers
            StrokeLengthMultiplierIncrease = Config.Bind(StringConstants.SexRobotLimitsSection, StringConstants.IncreaseStrokeMultiplierKey, new KeyboardShortcut(KeyCode.U));
            StrokeLengthMultiplierDecrease = Config.Bind(StringConstants.SexRobotLimitsSection, StringConstants.DecreaseStrokeMultiplierKey, new KeyboardShortcut(KeyCode.T));
            RobotL0Multiplier = Config.Bind(StringConstants.SexRobotLimitsSection, StringConstants.RobotL0Multiplier, 1.0f, new ConfigDescription(StringConstants.RobotL0Multiplier_Tooltip, new AcceptableValueRange<float>(0.25f, 5.0f)));
            RobotL0MultiplierStepValue = Config.Bind(StringConstants.SexRobotLimitsSection, StringConstants.RobotL0MultiplierStepValue, 0.25f, new ConfigDescription(StringConstants.RobotL0MultiplierStepValue_Tooltip, new AcceptableValueRange<float>(0.01f, 1.0f)));
            RobotL0Min = Config.Bind(StringConstants.SexRobotLimitsSection, StringConstants.RobotL0Min, 0.0f, new ConfigDescription(StringConstants.RobotL0Min_Tooltip, new AcceptableValueRange<float>(0.0f, 0.5f)));
            RobotL0Max = Config.Bind(StringConstants.SexRobotLimitsSection, StringConstants.RobotL0Max, 1.0f, new ConfigDescription(StringConstants.RobotL0Max_Tooltip, new AcceptableValueRange<float>(0.5f, 1.0f)));
            RobotL1Min = Config.Bind(StringConstants.SexRobotLimitsSection, StringConstants.RobotL1Min, 0.0f, new ConfigDescription(StringConstants.RobotL1Min_Tooltip, new AcceptableValueRange<float>(0.0f, 0.5f)));
            RobotL1Max = Config.Bind(StringConstants.SexRobotLimitsSection, StringConstants.RobotL1Max, 1.0f, new ConfigDescription(StringConstants.RobotL1Max_Tooltip, new AcceptableValueRange<float>(0.5f, 1.0f)));
            RobotL2Min = Config.Bind(StringConstants.SexRobotLimitsSection, StringConstants.RobotL2Min, 0.0f, new ConfigDescription(StringConstants.RobotL2Min_Tooltip, new AcceptableValueRange<float>(0.0f, 0.5f)));
            RobotL2Max = Config.Bind(StringConstants.SexRobotLimitsSection, StringConstants.RobotL2Max, 1.0f, new ConfigDescription(StringConstants.RobotL2Max_Tooltip, new AcceptableValueRange<float>(0.5f, 1.0f)));
            RobotR0Min = Config.Bind(StringConstants.SexRobotLimitsSection, StringConstants.RobotR0Min, 0.0f, new ConfigDescription(StringConstants.RobotR0Min_Tooltip, new AcceptableValueRange<float>(0.0f, 0.5f)));
            RobotR0Max = Config.Bind(StringConstants.SexRobotLimitsSection, StringConstants.RobotR0Max, 1.0f, new ConfigDescription(StringConstants.RobotR0Max_Tooltip, new AcceptableValueRange<float>(0.5f, 1.0f)));
            RobotR1Min = Config.Bind(StringConstants.SexRobotLimitsSection, StringConstants.RobotR1Min, 0.0f, new ConfigDescription(StringConstants.RobotR1Min_Tooltip, new AcceptableValueRange<float>(0.0f, 0.5f)));
            RobotR1Max = Config.Bind(StringConstants.SexRobotLimitsSection, StringConstants.RobotR1Max, 1.0f, new ConfigDescription(StringConstants.RobotR1Max_Tooltip, new AcceptableValueRange<float>(0.5f, 1.0f)));
            RobotR2Min = Config.Bind(StringConstants.SexRobotLimitsSection, StringConstants.RobotR2Min, 0.0f, new ConfigDescription(StringConstants.RobotR2Min_Tooltip, new AcceptableValueRange<float>(0.0f, 0.5f)));
            RobotR2Max = Config.Bind(StringConstants.SexRobotLimitsSection, StringConstants.RobotR2Max, 1.0f, new ConfigDescription(StringConstants.RobotR2Max_Tooltip, new AcceptableValueRange<float>(0.5f, 1.0f)));
            //limiter
            TogglelimitRobotStrokeLength = Config.Bind(StringConstants.SexRobotLimiterSection, StringConstants.ToggleStrokeLengthLimiter, new KeyboardShortcut(KeyCode.Space));
            LimitRobotL0Length = Config.Bind(StringConstants.SexRobotLimiterSection, StringConstants.StrokeLengthLimiter, false, new ConfigDescription(StringConstants.StrokeLengthLimiter_Tooltip));
            LimitRobotL0Multiplier = Config.Bind(StringConstants.SexRobotLimiterSection, StringConstants.StrokeLengthLimiterMultiplierValue, 1.0f, new ConfigDescription(StringConstants.StrokeLengthLimiterMultiplierValue_Tooltip, new AcceptableValueRange<float>(0.25f, 5.0f)));

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
                buttonLimitRobotStrokeLength = Object.Instantiate(btnOriginal, btnOriginal.parent).transform;
                buttonLimitRobotStrokeLength.name = StringConstants.ButtonStrokeLengthLimiter_Name;
                buttonLimitRobotStrokeLengthText = buttonLimitRobotStrokeLength.GetComponentInChildren<TextMeshProUGUI>();
                Button newButton = buttonLimitRobotStrokeLength.GetComponentInChildren<Button>();
                newButton.onClick.RemoveAllListeners();
                newButton.interactable = true;
                (newButton.onClick ??= new()).AddListener((UnityAction)new System.Action(() =>
                {
                    buttonLimitRobotStrokeLengthClicked = true;
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
                        buttonDisconnectRobotClicked = true;
                    }
                    else
                    {
                        buttonConnectRobotClicked = true;
                    }
                }));

                // Create robot stroke multiplier increase button
                buttonStrokeMultiplierIncrease = Object.Instantiate(btnController, btnController.parent).transform;
                buttonStrokeMultiplierIncrease.name = StringConstants.ButtonIncreaseStrokeLength_Name;
                buttonStrokeMultiplierIncreaseText = buttonStrokeMultiplierIncrease.GetComponentInChildren<TextMeshProUGUI>();
                buttonStrokeMultiplierIncreaseText.text = StringConstants.ButtonIncreaseStrokeLength_Text;
                buttonStrokeMultiplierIncreaseText.fontSize = 18;
                newButton = buttonStrokeMultiplierIncrease.GetComponentInChildren<Button>();
                newButton.onClick.RemoveAllListeners();
                newButton.interactable = true;
                (newButton.onClick ??= new()).AddListener((UnityAction)new System.Action(() =>
                {
                    buttonStrokeMultiplierIncreaseClicked = true;
                }));

                // Create robot stroke multiplier decrease button 
                buttonStrokeMultiplierDecrease = Object.Instantiate(btnController, btnController.parent).transform;
                buttonStrokeMultiplierDecrease.name = StringConstants.ButtonDecreaseStrokeLength_Name;
                buttonStrokeMultiplierDecreaseText = buttonStrokeMultiplierDecrease.GetComponentInChildren<TextMeshProUGUI>();
                buttonStrokeMultiplierDecreaseText.text = StringConstants.ButtonDecreaseStrokeLength_Text;
                buttonStrokeMultiplierDecreaseText.fontSize = 18;
                newButton = buttonStrokeMultiplierDecrease.GetComponentInChildren<Button>();
                newButton.onClick.RemoveAllListeners();
                newButton.interactable = true;
                (newButton.onClick ??= new()).AddListener((UnityAction)new System.Action(() =>
                {
                    buttonStrokeMultiplierDecreaseClicked = true;
                }));

                //create button for speed limitation
                buttonLimitRobotStrokeLength = Object.Instantiate(btnController, btnController.parent).transform;
                buttonLimitRobotStrokeLength.name = StringConstants.ButtonStrokeLengthLimiter_Name;
                buttonLimitRobotStrokeLengthText = buttonLimitRobotStrokeLength.GetComponentInChildren<TextMeshProUGUI>();
                buttonLimitRobotStrokeLengthText.text = StringConstants.ButtonStrokeLengthLimiter_Text;
                buttonLimitRobotStrokeLengthText.fontSize = 20;
                newButton = buttonLimitRobotStrokeLength.GetComponentInChildren<Button>();
                newButton.interactable = true;
                (newButton.onClick ??= new()).AddListener((UnityAction)new System.Action(() =>
                {
                    buttonLimitRobotStrokeLengthClicked = true;
                }));
            }
            catch (System.Exception ex)
            {
                LogDebug("An error occurred upon attempting to add Buttons to the Settings menu: " + ex.ToString());
            }
        }
    }
}
