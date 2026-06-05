using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace KKS_SexRobotController
{
    [BepInProcess("KoikatsuSunshine")]
    [BepInProcess("KoikatsuSunshine_VR")]
    [BepInPlugin(pluginGUID, pluginName, pluginVersion)]

    class KKS_SexRobotController : BaseUnityPlugin
    {
        const string pluginGUID = "KKSrobotics.KKSSexRobotController";
        const string pluginName = "KKS_SexRobotController";
        const string pluginVersion = "1.9";

        private HFlag Flags;
        private Stopwatch sw = Stopwatch.StartNew();
        private bool hSceneEnded = false;
        private bool fileIsRead = false;
        private RobotMovement robotMovement;
        private SerialPortConnection serialPortConnection;
        //for retrieving animations that aren't listed
        private string prevAnimationName = "";
        //check if button is created
        private bool guiButtonCreated = false;

        // the path in normal differs from VR
        private const string ButtonPath_MainGame = "Canvas/SubMenu/ClothCategory/ClothFemale/Button";
        private const string ButtonPath_VR = "Canvas/MainHSceneWindow/SubMenu/ClothCategory/ClothFemale/Button";

        //called only on scene load/initialization
        internal void onHSceneLoad(HSceneProc __instance)
        {
            try
            {
                // if previously an H-Scene was played and ended 
                // and a new one is now being started, clear previous values
                if (hSceneEnded)
                {
                    robotMovement.player = null;
                    robotMovement.females = null;
                    hSceneEnded = false;
                }
                if (robotMovement.player == null)
                    robotMovement.player = __instance.male;
                if (robotMovement.females == null)
                    robotMovement.females = __instance.lstFemale.FindAll(female => female != null).ToArray();
                robotMovement.updatePosition = false;
                robotMovement.speedChanged = false;
            }
            catch (Exception e)
            {
                Logger.LogInfo("ERROR in onHSceneLoad(): " + e.ToString());
            }
        }
        private void onHSceneUpdate(HSprite _hSprite)
        {
            try
            {
                if (_hSprite == null)
                    return;
                // if previously an H-Scene was played and ended 
                // and a new one is now being started, clear previous values
                if (hSceneEnded)
                {
                    robotMovement.player = null;
                    robotMovement.females = null;
                    hSceneEnded = false;
                    //guiButtonCreated = false;
                }
                if (robotMovement.females == null && _hSprite.females != null)
                {
                    robotMovement.females = _hSprite.females.FindAll(female => female != null).ToArray();
                }
                onHSceneUpdate(_hSprite.flags);
            }
            catch (Exception e)
            {
                Logger.LogInfo("ERROR in onHSceneUpdate(): " + e.ToString());
            }
        }

        private void onHSceneUpdate(HFlag _flag)
        {
            try
            {
                if (_flag != null)
                {
                    Flags = _flag;
                    // check if the animation or the animation speed has changed
                    //if so, update the animation values
                    if (robotMovement.animationName != Flags.nowAnimationInfo.nameAnimation)
                    {
                        robotMovement.animationChanged = true;
                        robotMovement.animationName = Flags.nowAnimationInfo.nameAnimation;
                    }

                    // in VR, the robotMovement.player doesn't get set
                    // therefore, check here if robotMovement.player is set
                    if (robotMovement.player == null)
                        robotMovement.player = Flags.player.chaCtrl;

                    //check if positions should be read from file
                    if (serialPortConnection.readPositionsFromFile.Value && !fileIsRead)
                    {
                        try
                        {
                            // read positions from file
                            FileHandler.readPositionsFromFile();
                        }
                        catch (Exception e)
                        {
                            Logger.LogInfo("Error updating Animation dictionary: " + e.ToString());

                        }
                        fileIsRead = true;
                    }
                    else if (!serialPortConnection.readPositionsFromFile.Value)
                    {
                        // if disabled, set read to false, to enable live updates
                        fileIsRead = false;
                    }

                    // get current animation name (for finding unregistered sex-animations)
                    // verify that position doesn't exist and isn't already printed
                    if (serialPortConnection.printSceneName.Value &&
                        Flags.nowAnimationInfo.nameAnimation != prevAnimationName
                        && !BoneAnimationDefiner.animationFemaleTargetDictionary.ContainsKey(Flags.nowAnimationInfo.nameAnimation))
                    {
                        prevAnimationName = Flags.nowAnimationInfo.nameAnimation;
                        Logger.LogInfo("Current Animation: " + prevAnimationName);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogInfo("ERROR in onHSceneUpdate(): " + e.ToString());
            }
        }

        //OnInitHeroine: always called
        internal void OnInitHeroine(ref HSprite hSprite)
        {
            if (hSprite != null)
                onHSceneUpdate(hSprite);
        }

        //called on speed change
        internal void OnSpeedChange(HFlag hFlag)
        {
            onHSceneUpdate(hFlag);
        }

        //HandlePause: called before/after sex  (e.g. pos. select, initialize)
        internal void HandlePause(ref HSprite hSprite)
        {
            if (hSprite != null)
                onHSceneUpdate(hSprite);
        }

        internal void printLog(List<string> logList)
        {
            if (logList != null)
            {
                foreach (string log in logList)
                {
                    Logger.LogInfo(log);
                }
            }
        }

        private void Start()
        {
            serialPortConnection = SerialPortConnection.GetInstance();
            robotMovement = RobotMovement.GetInstance();
            Hooks.InstallHooks();
            Harmony.CreateAndPatchAll(typeof(KKS_SexRobotController));
        }

        private void Awake()
        {
            serialPortConnection = SerialPortConnection.GetInstance();
            robotMovement = RobotMovement.GetInstance();
            //list containing log to print to console
            List<string> logList = new List<string>();
            // Setup config file entries used in the in game menu
            // Creates a config file in BepInEx/config named KKSrobotics.KKSSexRobotController.cfg
            // general
            serialPortConnection.diagnosticsConfig = Config.Bind(StringConstants.SexRobotGeneralSection, StringConstants.BepinExDebugOutput, false);
            serialPortConnection.printSceneName = Config.Bind(StringConstants.SexRobotGeneralSection, StringConstants.BepinExPrintPosition, false, new ConfigDescription(StringConstants.BepinExPrintPosition_Tooltip));
            serialPortConnection.readPositionsFromFile = Config.Bind(StringConstants.SexRobotGeneralSection, StringConstants.ReadPositionNamesFromFile, false, new ConfigDescription(StringConstants.ReadPositionNamesFromFile_Tooltip));
            // connection
            serialPortConnection.toggleSerialPortConnection = Config.Bind(StringConstants.SexRobotConnectionSection, StringConstants.ToggleSerialPortConnection, new KeyboardShortcut(KeyCode.S, KeyCode.LeftShift));
            (serialPortConnection.serialPortConfig = Config.Bind(StringConstants.SexRobotConnectionSection, StringConstants.SerialPortConfig, SerialPortConnection.SerialPorts[0], new ConfigDescription(StringConstants.SerialPortConfig_Tooltip, new AcceptableValueList<string>(SerialPortConnection.SerialPorts)))).SettingChanged += (s, e) =>
            {
                logList.AddRange(serialPortConnection.UpdateSerialPort());
            };
            serialPortConnection.sexRobotUpdateFrequencyConfig = Config.Bind(StringConstants.SexRobotConnectionSection, StringConstants.SexRobotUpdateFrequencyConfig, 30.0f, new ConfigDescription(StringConstants.SexRobotUpdateFrequencyConfig_Tooltip, new AcceptableValueRange<float>(1.0f, 120.0f)));
            serialPortConnection.serialPortStatus = Config.Bind(StringConstants.SexRobotConnectionSection, StringConstants.SerialPortStatus, StringConstants.SerialPortStatus_Tooltip);
            serialPortConnection.serialPortStatus.Value = serialPortConnection.serialPortConfig.Value + StringConstants.SerialPortStatus_Disconnected;
            (serialPortConnection.serialPortConnected = Config.Bind(StringConstants.SexRobotConnectionSection, StringConstants.SerialPortConnected, true)).SettingChanged += (s, e) =>
            {
                logList.AddRange(serialPortConnection.UpdateSerialPortConnection());
            };
            //multipliers
            serialPortConnection.strokeLengthMultiplierIncrease = Config.Bind(StringConstants.SexRobotLimitsSection, StringConstants.IncreaseStrokeMultiplierKey, new KeyboardShortcut(KeyCode.U));
            serialPortConnection.strokeLengthMultiplierDecrease = Config.Bind(StringConstants.SexRobotLimitsSection, StringConstants.DecreaseStrokeMultiplierKey, new KeyboardShortcut(KeyCode.T));
            serialPortConnection.robotL0Multiplier = Config.Bind(StringConstants.SexRobotLimitsSection, StringConstants.RobotL0Multiplier, 1.0f, new ConfigDescription(StringConstants.RobotL0Multiplier_Tooltip, new AcceptableValueRange<float>(0.25f, 5.0f)));
            serialPortConnection.robotL0MultiplierStepValue = Config.Bind(StringConstants.SexRobotLimitsSection, StringConstants.RobotL0MultiplierStepValue, 0.25f, new ConfigDescription(StringConstants.RobotL0MultiplierStepValue_Tooltip, new AcceptableValueRange<float>(0.01f, 1.0f)));
            serialPortConnection.robotL0Min = Config.Bind(StringConstants.SexRobotLimitsSection, StringConstants.RobotL0Min, 0.0f, new ConfigDescription(StringConstants.RobotL0Min_Tooltip, new AcceptableValueRange<float>(0.0f, 0.5f)));
            serialPortConnection.robotL0Max = Config.Bind(StringConstants.SexRobotLimitsSection, StringConstants.RobotL0Max, 1.0f, new ConfigDescription(StringConstants.RobotL0Max_Tooltip, new AcceptableValueRange<float>(0.5f, 1.0f)));
            serialPortConnection.robotL1Min = Config.Bind(StringConstants.SexRobotLimitsSection, StringConstants.RobotL1Min, 0.0f, new ConfigDescription(StringConstants.RobotL1Min_Tooltip, new AcceptableValueRange<float>(0.0f, 0.5f)));
            serialPortConnection.robotL1Max = Config.Bind(StringConstants.SexRobotLimitsSection, StringConstants.RobotL1Max, 1.0f, new ConfigDescription(StringConstants.RobotL1Max_Tooltip, new AcceptableValueRange<float>(0.5f, 1.0f)));
            serialPortConnection.robotL2Min = Config.Bind(StringConstants.SexRobotLimitsSection, StringConstants.RobotL2Min, 0.0f, new ConfigDescription(StringConstants.RobotL2Min_Tooltip, new AcceptableValueRange<float>(0.0f, 0.5f)));
            serialPortConnection.robotL2Max = Config.Bind(StringConstants.SexRobotLimitsSection, StringConstants.RobotL2Max, 1.0f, new ConfigDescription(StringConstants.RobotL2Max_Tooltip, new AcceptableValueRange<float>(0.5f, 1.0f)));
            serialPortConnection.robotR0Min = Config.Bind(StringConstants.SexRobotLimitsSection, StringConstants.RobotR0Min, 0.0f, new ConfigDescription(StringConstants.RobotR0Min_Tooltip, new AcceptableValueRange<float>(0.0f, 0.5f)));
            serialPortConnection.robotR0Max = Config.Bind(StringConstants.SexRobotLimitsSection, StringConstants.RobotR0Max, 1.0f, new ConfigDescription(StringConstants.RobotR0Max_Tooltip, new AcceptableValueRange<float>(0.5f, 1.0f)));
            serialPortConnection.robotR1Min = Config.Bind(StringConstants.SexRobotLimitsSection, StringConstants.RobotR1Min, 0.0f, new ConfigDescription(StringConstants.RobotR1Min_Tooltip, new AcceptableValueRange<float>(0.0f, 0.5f)));
            serialPortConnection.robotR1Max = Config.Bind(StringConstants.SexRobotLimitsSection, StringConstants.RobotR1Max, 1.0f, new ConfigDescription(StringConstants.RobotR1Max_Tooltip, new AcceptableValueRange<float>(0.5f, 1.0f)));
            serialPortConnection.robotR2Min = Config.Bind(StringConstants.SexRobotLimitsSection, StringConstants.RobotR2Min, 0.0f, new ConfigDescription(StringConstants.RobotR2Min_Tooltip, new AcceptableValueRange<float>(0.0f, 0.5f)));
            serialPortConnection.robotR2Max = Config.Bind(StringConstants.SexRobotLimitsSection, StringConstants.RobotR2Max, 1.0f, new ConfigDescription(StringConstants.RobotR2Max_Tooltip, new AcceptableValueRange<float>(0.5f, 1.0f)));
            //limiter
            serialPortConnection.togglelimitRobotStrokeLength = Config.Bind(StringConstants.SexRobotLimiterSection, StringConstants.ToggleStrokeLengthLimiter, new KeyboardShortcut(KeyCode.Space));
            serialPortConnection.limitRobotL0Length = Config.Bind(StringConstants.SexRobotLimiterSection, StringConstants.StrokeLengthLimiter, false, new ConfigDescription(StringConstants.StrokeLengthLimiter_Tooltip));
            serialPortConnection.limitRobotL0Multiplier = Config.Bind(StringConstants.SexRobotLimiterSection, StringConstants.StrokeLengthLimiterMultiplierValue, RobotMovement.LimitStrokeLengthMultiplier, new ConfigDescription(StringConstants.StrokeLengthLimiterMultiplierValue_Tooltip, new AcceptableValueRange<float>(0.25f, 5.0f)));

            if (serialPortConnection.serialPortConnected.Value)
            {
                logList.AddRange(serialPortConnection.UpdateSerialPortConnection());
            }
            printLog(logList);
        }

        internal void SetupUIButtons()
        {
            createSceneButton();
            if (!guiButtonCreated)
                createConfigButton();
        }

        private void createSceneButton()
        {
            bool isVR = false;
            List<string> logList = new List<string>();
            serialPortConnection = SerialPortConnection.GetInstance();
            try
            {
                GameObject original = GameObject.Find(ButtonPath_MainGame);
                if (original == null)
                {
                    original = GameObject.Find(ButtonPath_VR);
                    if (original == null)
                        return;
                    isVR = true;
                }

                serialPortConnection.buttonLimitRobotStrokeLength = Instantiate(original, original.transform.parent).transform;
                //currently set to be below the female clothing button, so avoid overlap by setting position - values from Main and VR differs!
                if (!isVR)
                    serialPortConnection.buttonLimitRobotStrokeLength.position = new Vector3(-1.28f, 0.68f, 0.0f);
                if (isVR)
                    serialPortConnection.buttonLimitRobotStrokeLength.localPosition = new Vector3(78.0f, -50.00f, 0.0f);
                serialPortConnection.buttonLimitRobotStrokeLength.name = StringConstants.ButtonStrokeLengthLimiter_Name;
                serialPortConnection.buttonLimitRobotStrokeLengthText = serialPortConnection.buttonLimitRobotStrokeLength.GetComponentInChildren<TextMeshProUGUI>();
                serialPortConnection.buttonLimitRobotStrokeLengthText.text = StringConstants.ButtonStrokeLengthLimiter_Text;
                var button = serialPortConnection.buttonLimitRobotStrokeLength.GetComponentInChildren<Button>();
                button.onClick = new Button.ButtonClickedEvent();
                button.onClick.AddListener(() =>
                {
                    serialPortConnection.buttonLimitRobotStrokeLengthClicked = true;
                });
            }
            catch (Exception e)
            {
                logList.Add("---> Error attempting to create GUI Button: " + e.ToString());
            }
            printLog(logList);
        }

        private void createConfigButton()
        {
            List<string> logList = new List<string>();
            serialPortConnection = SerialPortConnection.GetInstance();
            string path = "ConfigScene(Clone)/Canvas/Node ShortCut/ShortCutButton(Clone)";
            try
            {
                GameObject original = GameObject.Find(path);
                if (original == null)
                    return;
                // Create connect robot button by instantiating main button, changing it's name, text label, and adding a new listener to handle click events
                serialPortConnection.buttonConnectRobot = Instantiate(original, original.transform.parent).transform;
                serialPortConnection.buttonConnectRobot.name = StringConstants.ButtonConnectRobot_Name;
                serialPortConnection.buttonConnectRobotText = serialPortConnection.buttonConnectRobot.GetComponentInChildren<TextMeshProUGUI>();
                serialPortConnection.buttonConnectRobotText.text = StringConstants.ButtonConnectRobot_Text;
                Button newButton = serialPortConnection.buttonConnectRobot.GetComponentInChildren<Button>();
                newButton.onClick.RemoveAllListeners();
                newButton.onClick.AddListener(delegate
                {
                    serialPortConnection.buttonConnectRobotClicked = true;
                });

                // Create disconnect robot button by instantiating main button, changing it's name, text label, and adding a new listener to handle click events
                serialPortConnection.buttonDisconnectRobot = Instantiate(original, original.transform.parent).transform;
                serialPortConnection.buttonDisconnectRobot.name = StringConstants.ButtonDisconnectRobot_Name;
                serialPortConnection.buttonDisconnectRobotText = serialPortConnection.buttonDisconnectRobot.GetComponentInChildren<TextMeshProUGUI>();
                serialPortConnection.buttonDisconnectRobotText.text = StringConstants.ButtonDisconnectRobot_Text;
                newButton = serialPortConnection.buttonDisconnectRobot.GetComponentInChildren<Button>();
                newButton.onClick.RemoveAllListeners();
                newButton.onClick.AddListener(delegate
                {
                    serialPortConnection.buttonDisconnectRobotClicked = true;
                });

                //create button for speed limitation
                serialPortConnection.buttonLimitRobotStrokeLength = Instantiate(original, original.transform.parent).transform;
                serialPortConnection.buttonLimitRobotStrokeLength.name = StringConstants.ButtonStrokeLengthLimiter_Name;
                serialPortConnection.buttonLimitRobotStrokeLengthText = serialPortConnection.buttonLimitRobotStrokeLength.GetComponentInChildren<TextMeshProUGUI>();
                serialPortConnection.buttonLimitRobotStrokeLengthText.text = StringConstants.ButtonStrokeLengthLimiter_Text;
                newButton = serialPortConnection.buttonLimitRobotStrokeLength.GetComponentInChildren<Button>();
                newButton.onClick.RemoveAllListeners();
                newButton.onClick.AddListener(delegate
                {
                    serialPortConnection.buttonLimitRobotStrokeLengthClicked = true;
                });
                guiButtonCreated = true;
            }
            catch (Exception e)
            {
                logList.Add("---> Error upon creating Settings Button: " + e.ToString());
            }
            printLog(logList);
        }

        private void Update()
        {
            try
            {
                printLog(serialPortConnection.checkButtonAndSerialConnState());

                // Return if not in an HScene
                if (Flags == null)
                {
                    return;
                }

                if (Flags.isHSceneEnd)
                {
                    // H-Scene is ending, set flag and return
                    hSceneEnded = true;
                    return;
                }

                // Get ms elapsed since current stopwatch interval
                float msElapsed = sw.ElapsedMilliseconds;

                // If the ms elapsed is greater than the period based on the robot's update frequency then
                // stop the stopwatch, call the robot update function, and restart the stopwatch
                if (msElapsed >= (1000.0 / serialPortConnection.sexRobotUpdateFrequencyConfig.Value))
                {
                    sw.Stop();

                    // check here if the speed needs to be updated, as updates only handle loops and not speed adjustment
                    if (robotMovement.nowAnimStateName != Flags.nowAnimStateName && !robotMovement.animationChanged)
                    {
                        robotMovement.speedChanged = true;
                        robotMovement.nowAnimStateName = Flags.nowAnimStateName;
                    }

                    printLog(robotMovement.updateAnimationStatus());
                    sw = Stopwatch.StartNew();
                }
            }
            catch (Exception ex)
            {
                Logger.LogInfo("ERROR (UPDATE()): " + ex.ToString());
            }
        }
    }
}
