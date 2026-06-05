#nullable enable
using AC_SexRobotController.Helpers;
using AC_SexRobotController.Plugin;
using H;
using ILLGAMES.Extensions;
using ILLGAMES.Unity.BoneSwayCtrl;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using static H.HScene;

// List of items to implement/test:
// - Increasement of movement
// - 3P: 2F1M
// - 3P: 1F2M
// - 5P: ???
// - VR (may not work, due to the BepinEx currently not working there) => v2.0 Feature

namespace AC_SexRobotController.RobotController
{
    public sealed class SexRobotController : MonoBehaviour
    {
        internal static SexRobotController? Instance { get; private set; }

        private int _loopType;
        private HScene? HScene;
        private Stopwatch _sw = Stopwatch.StartNew();
        private static RobotMovement? _robotMovement;

        internal void SetHScene(HScene _hScene)
        {
            HScene = _hScene;

            if (HScene.NowAnimationInfo.NameAnimation != null
                && HScene.NowAnimationInfo.NameAnimation != ""
                && HScene.NowAnimationInfo.NameAnimation != RobotMovement.GetInstance().AnimationName)
            {
                UpdateRobotMovement(HScene.NowAnimationInfo.NameAnimation);
            }

            // since a new HScene is being created, check if the button has already been added to the current Scene Object
            Transform? btnLimiter = null;
            try
            {
                btnLimiter = HScene.transform.FindLoop(StringConstants.ButtonStrokeLengthLimiter_Name).transform;
            }
            catch (System.Exception)
            {
            }
            // if it doesn't exist, add it to the scene Object
            if (btnLimiter == null)
                AC_SexRobotControllerPlugin.CreateSceneButton(HScene.transform.FindLoop("btnConfig").transform);
        }

        private static void UpdateRobotMovement(string animationName)
        {
            _robotMovement = RobotMovement.GetInstance();
            _robotMovement.PrevAnimationName = _robotMovement.AnimationName;
            _robotMovement.AnimationName = animationName;
            _robotMovement.AnimationChanged = true;
            _robotMovement.UpdatePosition = false;
            _robotMovement.SpeedChanged = false;
        }

        internal void ReadBodyPosition()
        {
            if (HScene == null)
                return;
            RobotMovement.GetInstance().SetActorObjects(
                HScene.Player,
                HScene.GetActors(ActorType.Receivers).ToArray(),
                HScene.GetActors(ActorType.Attackers).ToArray()
                );
        }

        internal static void CheckAnimationName()
        {
            _robotMovement = RobotMovement.GetInstance();
            if (_robotMovement.AnimationName == null || _robotMovement.AnimationName == "")
                return;
            // check current animation name (for finding unregistered sex-animations)
            // verify that position doesn't exist and isn't already printed
            if (AC_SexRobotControllerPlugin.WriteAnimationsToFile.Value &&
                _robotMovement.AnimationName != _robotMovement.PrevAnimationName &&
                !BoneAnimationDefiner.animationFemaleTargetDictionary.ContainsKey(_robotMovement.AnimationName))
            {
                // set previous to the current to avoid multiple rewrites on current animation refresh
                _robotMovement.PrevAnimationName = _robotMovement.AnimationName;
                FileHandler.WriteToFile(_robotMovement.PrevAnimationName);
                AC_SexRobotControllerPlugin.LogInfo("The animation name '" + _robotMovement.AnimationName + "' was written to file!");
            }
        }

        internal static void UpdateAnimationDictionary()
        {
            //check if positions should be read from file
            if (AC_SexRobotControllerPlugin.ReadAnimationsFromFile.Value
                && !AC_SexRobotControllerPlugin.fileIsRead)
            {
                try
                {
                    // read positions from file
                    FileHandler.ReadPositionsFromFile();
                }
                catch (System.Exception ex)
                {
                    AC_SexRobotControllerPlugin.LogDebug("An error occurred when attempting to read animations from file: " + ex.ToString());
                }
                // regardless of result, consider file to be read (no point re-reading file with errors)
                AC_SexRobotControllerPlugin.fileIsRead = true;
            }
            else if (!AC_SexRobotControllerPlugin.ReadAnimationsFromFile.Value)
            {
                // if disabled, set read to false, 
                // to enable the possibility to reload file content with new animations without restarting the game
                AC_SexRobotControllerPlugin.fileIsRead = false;
            }
        }

        internal bool IsHSceneSet()
        {
            return (HScene != null);
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }
        }

        internal void HsceneEnding()
        {
            Destroy(this);
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                RobotMovement.GetInstance().AnimationChanged = false;
                RobotMovement.GetInstance().SpeedChanged = false;
                RobotMovement.GetInstance().UpdatePosition = false;
                Instance = null;
                HScene = null;
                _sw.Stop();
            }
        }

        private void Update()
        {
            try
            {
                _robotMovement = RobotMovement.GetInstance();
                SerialPortConnection.GetInstance().CheckButtonAndSerialConnState();

                // Return if not in an HScene
                if (HScene == null)
                    return;

                // Get ms elapsed since current stopwatch interval
                float msElapsed = _sw.ElapsedMilliseconds;

                if (msElapsed >= (1000.0 / AC_SexRobotControllerPlugin.SexRobotUpdateFrequencyConfig.Value))
                {

                    _sw.Stop();

                    // check if the animation has changed
                    if (_robotMovement.AnimationName != HScene.NowAnimationInfo.NameAnimation)
                    {
                        UpdateRobotMovement(HScene.NowAnimationInfo.NameAnimation);
                    }
                    // check here if the speed was changed - this is required to account for:
                    // - continuing after last finish
                    // - pulling out and re-inserting
                    // - moving faster/slower, which might change the the depth
                    else if (_loopType != HScene.CtrlFlag.LoopType && !_robotMovement.AnimationChanged)
                    {
                        _robotMovement.SpeedChanged = true;
                        _loopType = HScene.CtrlFlag.LoopType;
                    }

                    _robotMovement.UpdateAnimationStatus();
                    _sw = Stopwatch.StartNew();
                }
            }
            catch (System.Exception ex)
            {
                AC_SexRobotControllerPlugin.LogDebug("ERROR (UPDATE()): " + ex.ToString());
            }
        }
    }
}
