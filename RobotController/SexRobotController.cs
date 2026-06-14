#nullable enable
using AC_SexRobotController.Helpers;
using AC_SexRobotController.Plugin;
using H;
using ILLGAMES.Extensions;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using static H.HScene;

namespace AC_SexRobotController.RobotController
{
    internal sealed class SexRobotController : MonoBehaviour
    {
        internal static SexRobotController? Instance { get; private set; }

        private HScene? HScene;
        private static RobotMovement? _robotMovement;
        private readonly Stopwatch _sw = Stopwatch.StartNew();

        internal void SetHScene(HScene _hScene)
        {
            this.HScene = _hScene;

            if (this.HScene.NowAnimationInfo.NameAnimation != null
                && this.HScene.NowAnimationInfo.NameAnimation != ""
                && this.HScene.NowAnimationInfo.NameAnimation != RobotMovement.GetInstance().AnimationName)
                UpdateRobotMovement(this.HScene);

            // since a new HScene is being created, check if the button has already been added to the current Scene Object
            Transform? btnLimiter = null;
            try
            {
                btnLimiter = HScene.transform.FindLoop(StringConstants.ButtonLimitStrokeMultiplier_Name).transform;
            }
            catch (System.Exception)
            {
            }
            // if it doesn't exist, add it to the scene Object
            if (btnLimiter == null)
                AC_SexRobotControllerPlugin.CreateSceneButton(HScene.transform.FindLoop(StringConstants.SCENE_CONFIG_BUTTON).transform);
        }

        private static void UpdateRobotMovement(HScene hScene)
        {
            _robotMovement = RobotMovement.GetInstance();
            _robotMovement.LoopType = hScene.CtrlFlag.LoopType;
            _robotMovement.AnimationSpeed = hScene.CtrlFlag.Speed;
            _robotMovement.PrevAnimationName = _robotMovement.AnimationName;
            _robotMovement.AnimationName = hScene.NowAnimationInfo.NameAnimation;
            _robotMovement.UpdatePosition = false;
            _robotMovement.AnimationChanged = true;
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
                AC_SexRobotControllerPlugin.LogInfo($"The animation name '{_robotMovement.AnimationName}' was written to file!");
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
                _sw.Reset();
                this.HScene = null;
                RobotMovement.GetInstance().HSceneEnding();
                Instance = null;
            }
        }

        private void Update()
        {
            try
            {
                _robotMovement = RobotMovement.GetInstance();
                SerialPortConnection.GetInstance().CheckButtonAndSerialConnState();

                // Return if not in an HScene
                if (this.HScene == null)
                    return;

                // Get ms elapsed since current stopwatch interval
                float msElapsed = _sw.ElapsedMilliseconds;

                if (msElapsed >= (1000.0 / AC_SexRobotControllerPlugin.SexRobotUpdateFrequencyConfig.Value))
                {

                    _sw.Stop();

                    // check if the animation has changed
                    if (_robotMovement.AnimationName != this.HScene.NowAnimationInfo.NameAnimation)
                        UpdateRobotMovement(this.HScene);
                    _robotMovement.LoopType = this.HScene.CtrlFlag.LoopType;
                    _robotMovement.IsNowOrgasm = this.HScene.CtrlFlag.IsNowOrgasm;
                    _robotMovement.AnimationSpeed = (this.HScene.CtrlFlag.IsNowOrgasm) ? AC_SexRobotControllerPlugin.RobotOrgasmSpeedMultiplier.Value : this.HScene.CtrlFlag.Speed;
                    _robotMovement.UpdateAnimationStatus();
                    _sw.Restart();
                }
            }
            catch (System.Exception ex)
            {
                AC_SexRobotControllerPlugin.LogDebug("Error (Update()): " + ex.ToString());
            }
        }
    }
}
