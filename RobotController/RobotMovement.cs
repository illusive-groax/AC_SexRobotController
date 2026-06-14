using AC_SexRobotController.Helpers;
using AC_SexRobotController.Plugin;
using H;
using System;
using System.Linq;
using UnityEngine;

namespace AC_SexRobotController.RobotController
{
    internal sealed class RobotMovement
    {
        internal const float L0_INTENSITY_MIN = 1.0f;
        internal const float L0_INTENSITY_MAX = 10.0f;
        internal const float L0_MAXSPEED_MIN = 0.5f;
        internal const float L0_MAXSPEED_MAX = 3.0f;
        internal const float L0_DEFAULT_INTENSITY = 6.0f;
        internal const float L0_DEFAULT_MAXSPEED = 1.5f;

        // check which Loop is currently playing
        internal int LoopType { get; set; }
        // get the current animation speed (0.0f -> 2.0f)
        internal float AnimationSpeed { get; set; }
        // orgasm sets LoopType to IDLE, therefore check if orgasm for that last shake
        internal bool IsNowOrgasm { get; set; }
        // if the animation has changed, the position needs to be updated
        internal bool UpdatePosition { get; set; }
        // the name of the current animation playing
        internal string AnimationName { get; set; }
        // check if the animation has changed, if so, update position
        internal bool AnimationChanged { get; set; }
        // previous animation name, to keep track of current and previous animation
        internal string PrevAnimationName { get; set; }

        private static SerialPortConnection _serialPortConnection;
        private static readonly Lazy<RobotMovement> _instance = new(() => new RobotMovement());

        // Transform for the Player
        private Transform PlayerTransform { get; set; }
        // Transform for the Female(s)
        private Transform[] FemaleTransform { get; set; }
        // Transform for other Male(s) -> unclear if needed, unused for now
        private Transform[] AttackerTransform { get; set; }
        // Transforms used to retrieve the position of the male bodyparts
        private Transform _malePenisBase;
        private Transform _malePenisMid;
        private Transform _malePenisTip;
        private Transform _malePenisLeftBall;
        private Transform _malePenisRightBall;
        // Transforms used to retrieve the position of the female bodyparts
        private Transform _femaleMouthLipsUpper;
        private Transform _femaleMouthLipsLower;
        private Transform _femaleMouthLeft;
        private Transform _femaleMouthRight;
        private Transform _femaleHip;
        private Transform _femaleVagina;
        private Transform _femaleAnus;
        private Transform _femaleMiddleBreastsLeft;
        private Transform _femaleMiddleBreastsRight;
        private Transform _femaleBreasts;
        private Transform _femaleMiddleFingerLeft;
        private Transform _femaleRingFingerLeft;
        private Transform _femaleHandLeft;
        private Transform _femaleMiddleFingerRight;
        private Transform _femaleRingFingerRight;
        private Transform _femaleHandRight;
        private Transform _femaleFootLeft;
        private Transform _femaleFootRight;
        private Transform _femaleToesLeft;
        private Transform _femaleToesRight;
        // which female body part should be tracked?
        private BoneAnimationDefiner.FemaleTargetType _currentFemaleTargetType;

        #region AI_GENERATED_VARIABLES
        private Vector3 _currPenisTip;
        private Vector3 _malePenisXAxis;

        private bool _l0Initialized;
        private float _l0Filtered;
        // normalized L0 (value between 0 and 1) after auto-range and gain
        private float _normalizedL0;
        private float _l0Reference;
        private float _l0SmoothVelocity;
        // controls the movement along the L0 (larger value = more movement)
        private float L0MovementIntensity = L0_DEFAULT_INTENSITY;
        // responsiveness / speed multiplier
        private float L0SpeedMultiplier = L0_DEFAULT_MAXSPEED;

        // keep track of the idle phase (avoid sudden movements/shakes)
        private float _idlePhase;
        // keep track of sent commands, don't send duplicate T-Code commands
        private string _lastCommand;
        #endregion

        private RobotMovement()
        {
            AnimationName = "";
            UpdatePosition = false;
            PrevAnimationName = "";
            LoopType = (int)BoneAnimationDefiner.LoopType.IDLE;
            _serialPortConnection = SerialPortConnection.GetInstance();
        }

        internal static RobotMovement GetInstance()
        {
            return _instance.Value;
        }

        internal void UpdateAnimationStatus()
        {
            try
            {
                if (AnimationName == null || AnimationName == "")
                    return;

                if (BoneAnimationDefiner.animationFemaleTargetDictionary.ContainsKey(AnimationName))
                {
                    if (AnimationChanged)
                        GetBonePositionData();

                    if (!UpdatePosition)
                        return;

                    bool limiterActive = AC_SexRobotControllerPlugin.LimitRobotL0Multipliers.Value;
                    L0MovementIntensity = limiterActive ? AC_SexRobotControllerPlugin.LimitRobotL0LengthMultiplier.Value : AC_SexRobotControllerPlugin.RobotL0LengthMultiplier.Value;
                    L0SpeedMultiplier = limiterActive ? AC_SexRobotControllerPlugin.LimitRobotL0SpeedMultiplier.Value : AC_SexRobotControllerPlugin.RobotL0SpeedMultiplier.Value;

                    UpdateRobotPosition();
                }
                else
                {
                    if (PrevAnimationName != AnimationName)
                    {
                        AC_SexRobotControllerPlugin.LogInfo("The animation name was not found in the plugin dictonary: '" + AnimationName + "'.'");
                        WriteUnknownAnimationsToFile();
                        // set previous to the current to avoid multiple rewrites
                        PrevAnimationName = AnimationName;
                        // send device home
                        SendTCodeHomeCommand();
                    }
                }
            }
            catch (Exception ex)
            {
                // send device home
                SendTCodeHomeCommand();
                AC_SexRobotControllerPlugin.LogDebug("An error occurred upon attempting to update the animation: " + ex.ToString());
                AC_SexRobotControllerPlugin.LogDebug(ex.StackTrace);
            }
        }

        private void WriteUnknownAnimationsToFile()
        {
            if (AC_SexRobotControllerPlugin.WriteAnimationsToFile.Value)
            {
                FileHandler.WriteToFile(AnimationName);
                AC_SexRobotControllerPlugin.LogInfo("The animation name '" + AnimationName + "' was written to file!");
            }
        }

        internal void SetActorObjects(HActor _player, HActor[] _females, HActor[] _attackers)
        {
            // get the transforms for calculating the position/movement
            FemaleTransform = GetTransforms(_females);
            AttackerTransform = GetTransforms(_attackers);
            PlayerTransform = _player.Human.gameObject.GetComponentInChildren<Transform>();
        }

        private static Transform[] GetTransforms(HActor[] actors)
        {
            int index = 0;
            Transform[] tArray = new Transform[actors.Length];
            foreach (HActor actor in actors)
            {
                tArray[index] = actor.Human.gameObject.GetComponentInChildren<Transform>();
                index++;
            }
            return tArray;
        }

        private void GetBonePositionData()
        {
            int girlIndex = 0;

            if (AC_SexRobotControllerPlugin.DiagnosticsConfig.Value)
                AC_SexRobotControllerPlugin.LogInfo($"For the animation '{AnimationName}', the amount of females are: {FemaleTransform.Length}.");

            if (PlayerTransform != null && FemaleTransform != null && FemaleTransform.Length > 0)
            {
                // get male transforms
                UpdateMaleTransforms();
                // use the current animation name to check which female should be tracked
                BoneAnimationDefiner.animationFemaleTargetDictionary.TryGetValue(AnimationName, out _currentFemaleTargetType);
                // check if transforms were successfully retrieved
                bool canUpdatePosition = false;
                switch (FemaleTransform.Length)
                {
                    case 1:
                    case 2:
                        {
                            // 3P -> check if SWAP is used
                            // TODO: Consider merging this with 4P later (if they both use swap)
                            if (AnimationIsFemaleSwap())
                            {
                                girlIndex = 1;
                            }
                            UpdateFemaleTransforms(girlIndex);
                            canUpdatePosition = true;
                        }
                        break;
                    case 4:
                        {
                            // 5P
                            // TODO: loop and set active? how to see which is active?
                            AC_SexRobotControllerPlugin.LogInfo($"Error: The plugin doesn't support the amount of females detected. Amount: {FemaleTransform.Length}.");
                        }
                        break;
                    default:
                        AC_SexRobotControllerPlugin.LogInfo($"Error: The plugin doesn't support the amount of females detected. Amount: {FemaleTransform.Length}.");
                        break;
                }
                AnimationChanged = false;
                UpdatePosition = canUpdatePosition;
            }
            else
            {
                UpdatePosition = false;
                // send device home
                SendTCodeHomeCommand();
                AC_SexRobotControllerPlugin.LogInfo("Error: The current HScene doesn't have 1 male and at least 1 female.");
            }
        }

        private bool AnimationIsFemaleSwap()
        {
            if (_currentFemaleTargetType == BoneAnimationDefiner.FemaleTargetType.VAGINAL
                || _currentFemaleTargetType == BoneAnimationDefiner.FemaleTargetType.ANAL
                || _currentFemaleTargetType == BoneAnimationDefiner.FemaleTargetType.ORAL
                || _currentFemaleTargetType == BoneAnimationDefiner.FemaleTargetType.BREASTS
                || _currentFemaleTargetType == BoneAnimationDefiner.FemaleTargetType.LEFTHAND
                || _currentFemaleTargetType == BoneAnimationDefiner.FemaleTargetType.RIGHTHAND
                || _currentFemaleTargetType == BoneAnimationDefiner.FemaleTargetType.INTERCRURAL_HIP
                || _currentFemaleTargetType == BoneAnimationDefiner.FemaleTargetType.LEFTFOOT
                || _currentFemaleTargetType == BoneAnimationDefiner.FemaleTargetType.RIGHTFOOT
                || _currentFemaleTargetType == BoneAnimationDefiner.FemaleTargetType.BOTH_HANDS
                || _currentFemaleTargetType == BoneAnimationDefiner.FemaleTargetType.BOTH_FEET)
                return false;
            return true;
        }

        #region UpdateTransforms
        private void UpdateMaleTransforms()
        {
            try
            {
                // required to get the current, the first attempt calling this directly when setting the HScene resulted in a NullException.
                var maleTransform = PlayerTransform.gameObject.GetComponentsInChildren<Transform>();

                // Get the base of the male's penis
                _malePenisBase = maleTransform.Where(x => x.name == BoneAnimationDefiner.bodyBoneDictionary[BoneAnimationDefiner.BodyBone.PENIS_BASE]).FirstOrDefault();

                // Get the center/mid of the penis
                _malePenisMid = maleTransform.Where(x => x.name == BoneAnimationDefiner.bodyBoneDictionary[BoneAnimationDefiner.BodyBone.PENIS_MID]).FirstOrDefault();

                // Get the tip of the male's penis
                _malePenisTip = maleTransform.Where(x => x.name == BoneAnimationDefiner.bodyBoneDictionary[BoneAnimationDefiner.BodyBone.PENIS_TIP]).FirstOrDefault();

                // Get the male's penis left ball
                _malePenisLeftBall = maleTransform.Where(x => x.name == BoneAnimationDefiner.bodyBoneDictionary[BoneAnimationDefiner.BodyBone.BALLS_L]).FirstOrDefault();

                // Get the male's penis right ball
                _malePenisRightBall = maleTransform.Where(x => x.name == BoneAnimationDefiner.bodyBoneDictionary[BoneAnimationDefiner.BodyBone.BALLS_R]).FirstOrDefault();
            }
            catch (Exception ex)
            {
                AC_SexRobotControllerPlugin.LogDebug("An error occurred upon attempting to update the male transforms: " + ex.ToString());
                throw new Exception("Error occurred in UpdateMaleTransforms(), cannot continue!");
            }
        }

        private void UpdateFemaleTransforms(int girlIndex)
        {
            try
            {
                var femaleTransform = FemaleTransform[girlIndex].gameObject.GetComponentsInChildren<Transform>();

                // Find/set all the female Transforms needed for the VAGINAL / ANAL / INTERCRURAL (HIP) calculations
                // Get the base of the female's hip
                _femaleHip = femaleTransform.Where(x => x.name == BoneAnimationDefiner.bodyBoneDictionary[BoneAnimationDefiner.BodyBone.FEMALE_HIPS]).FirstOrDefault();

                // Get the base of the female's vagina
                _femaleVagina = femaleTransform.Where(x => x.name == BoneAnimationDefiner.bodyBoneDictionary[BoneAnimationDefiner.BodyBone.VAGINA]).FirstOrDefault();

                // Get the base of the female's anus
                _femaleAnus = femaleTransform.Where(x => x.name == BoneAnimationDefiner.bodyBoneDictionary[BoneAnimationDefiner.BodyBone.ANUS]).FirstOrDefault();

                // Find/set all the female Transforms needed for the ORAL calculations
                // Get the female's mouth upper lips
                _femaleMouthLipsUpper = femaleTransform.Where(x => x.name == BoneAnimationDefiner.bodyBoneDictionary[BoneAnimationDefiner.BodyBone.FEMALE_MOUTH_UPPER_LIP]).FirstOrDefault();

                // Get the female's mouth lower lips
                _femaleMouthLipsLower = femaleTransform.Where(x => x.name == BoneAnimationDefiner.bodyBoneDictionary[BoneAnimationDefiner.BodyBone.FEMALE_MOUTH_LOWER_LIP]).FirstOrDefault();

                // Get the female's mouth left
                _femaleMouthLeft = femaleTransform.Where(x => x.name == BoneAnimationDefiner.bodyBoneDictionary[BoneAnimationDefiner.BodyBone.FEMALE_MOUTHL]).FirstOrDefault();

                // Get the female's mouth right
                _femaleMouthRight = femaleTransform.Where(x => x.name == BoneAnimationDefiner.bodyBoneDictionary[BoneAnimationDefiner.BodyBone.FEMALE_MOUTHR]).FirstOrDefault();

                // Find/set all the female Transforms needed for the BREASTS calculations
                // Get the female's middle of the breasts left
                _femaleMiddleBreastsLeft = femaleTransform.Where(x => x.name == BoneAnimationDefiner.bodyBoneDictionary[BoneAnimationDefiner.BodyBone.FEMALE_BREASTL]).FirstOrDefault();

                // Get the female's middle of the breasts right
                _femaleMiddleBreastsRight = femaleTransform.Where(x => x.name == BoneAnimationDefiner.bodyBoneDictionary[BoneAnimationDefiner.BodyBone.FEMALE_BREASTR]).FirstOrDefault();

                // Get the female's breasts center on the chest
                _femaleBreasts = femaleTransform.Where(x => x.name == BoneAnimationDefiner.bodyBoneDictionary[BoneAnimationDefiner.BodyBone.FEMALE_BREAST]).FirstOrDefault();

                // Find/set all the female Transforms needed for the LEFTHAND / RIGHTHAND calculations
                // Get the female's left hand's middle finger
                _femaleMiddleFingerLeft = femaleTransform.Where(x => x.name == BoneAnimationDefiner.bodyBoneDictionary[BoneAnimationDefiner.BodyBone.FEMALE_HAND_MIDDLEL]).FirstOrDefault();

                // Get the female's left hand's ring fingers
                _femaleRingFingerLeft = femaleTransform.Where(x => x.name == BoneAnimationDefiner.bodyBoneDictionary[BoneAnimationDefiner.BodyBone.FEMALE_HAND_RINGL]).FirstOrDefault();

                // Get the female's left hand's center
                _femaleHandLeft = femaleTransform.Where(x => x.name == BoneAnimationDefiner.bodyBoneDictionary[BoneAnimationDefiner.BodyBone.FEMALE_HANDL]).FirstOrDefault();

                // Get the female's right hand's middle finger
                _femaleMiddleFingerRight = femaleTransform.Where(x => x.name == BoneAnimationDefiner.bodyBoneDictionary[BoneAnimationDefiner.BodyBone.FEMALE_HAND_MIDDLER]).FirstOrDefault();

                // Get the female's right hand's ring fingers
                _femaleRingFingerRight = femaleTransform.Where(x => x.name == BoneAnimationDefiner.bodyBoneDictionary[BoneAnimationDefiner.BodyBone.FEMALE_HAND_RINGR]).FirstOrDefault();

                // Get the female's right hand's center
                _femaleHandRight = femaleTransform.Where(x => x.name == BoneAnimationDefiner.bodyBoneDictionary[BoneAnimationDefiner.BodyBone.FEMALE_HANDR]).FirstOrDefault();

                // Find/set all the female Transforms needed for the LEFTFOOT / RIGHTFOOT / BOTH_FEET calculations
                // Get the base of the female's left foot
                _femaleFootLeft = femaleTransform.Where(x => x.name == BoneAnimationDefiner.bodyBoneDictionary[BoneAnimationDefiner.BodyBone.FEMALE_FOOTL]).FirstOrDefault();

                // Get the base of the female's right foot
                _femaleFootRight = femaleTransform.Where(x => x.name == BoneAnimationDefiner.bodyBoneDictionary[BoneAnimationDefiner.BodyBone.FEMALE_FOOTR]).FirstOrDefault();

                // Get the base of the female's left toes
                _femaleToesLeft = femaleTransform.Where(x => x.name == BoneAnimationDefiner.bodyBoneDictionary[BoneAnimationDefiner.BodyBone.FEMALE_TOESL]).FirstOrDefault();

                // Get the base of the female's right toes
                _femaleToesRight = femaleTransform.Where(x => x.name == BoneAnimationDefiner.bodyBoneDictionary[BoneAnimationDefiner.BodyBone.FEMALE_TOESR]).FirstOrDefault();
            }
            catch (Exception ex)
            {
                AC_SexRobotControllerPlugin.LogDebug("An error occurred when attempting to update the transforms for the female with Index: " + girlIndex + ". Error: " + ex.ToString());
                throw new Exception("Error occurred in UpdateFemaleTransforms(), cannot continue!");
            }
        }
        #endregion

        private void GetFemaleTargetPosition(out Vector3 femaleTargetXAxis, out Vector3 femaleTargetZAxis, out Vector3 targetPos)
        {
            // initialize vectors
            targetPos = Vector3.zero;
            femaleTargetZAxis = Vector3.up;
            femaleTargetXAxis = Vector3.right;
            // get current positions for body parts
            Vector3 femaleHip = _femaleHip.position;
            Vector3 femaleAnus = _femaleAnus.position;
            Vector3 femaleVagina = _femaleVagina.position;
            Vector3 femaleHandL = _femaleHandLeft.position;
            Vector3 femaleHandR = _femaleHandRight.position;
            Vector3 femaleFootL = _femaleFootLeft.position;
            Vector3 femaleFootR = _femaleFootRight.position;

            switch (_currentFemaleTargetType)
            {
                case BoneAnimationDefiner.FemaleTargetType.VAGINAL:
                case BoneAnimationDefiner.FemaleTargetType.VAGINALSWAP:
                    {
                        Vector3 side = (femaleAnus - femaleVagina).normalized;
                        // Vector from the selected female's vagina to hip
                        femaleTargetXAxis = (femaleHip - femaleVagina).normalized;
                        // Use the female's vagina and hip vector and the female's anus to establish the Z reference axis
                        femaleTargetZAxis = Vector3.Cross(femaleTargetXAxis, side);
                        // check that the vector length isn't zero
                        if (femaleTargetZAxis.sqrMagnitude < 1e-8f)
                            femaleTargetZAxis = Vector3.Cross(femaleTargetXAxis, Vector3.up);
                        femaleTargetZAxis.Normalize();
                        // set the targeted position
                        targetPos = femaleVagina;
                        break;
                    }
                case BoneAnimationDefiner.FemaleTargetType.ANAL:
                case BoneAnimationDefiner.FemaleTargetType.ANALSWAP:
                    {
                        Vector3 side = (femaleVagina - femaleAnus).normalized;
                        // Vector from the selected female's anus to hip
                        femaleTargetXAxis = (femaleHip - femaleAnus).normalized;
                        // Use the female's anus and hip vector and the female's vagina to establish the Z reference axis
                        femaleTargetZAxis = Vector3.Cross(femaleTargetXAxis, side);
                        // check that the vector length isn't zero
                        if (femaleTargetZAxis.sqrMagnitude < 1e-8f)
                            femaleTargetZAxis = Vector3.Cross(femaleTargetXAxis, Vector3.up);
                        femaleTargetZAxis.Normalize();
                        // set the targeted position
                        targetPos = femaleAnus;
                        break;
                    }
                case BoneAnimationDefiner.FemaleTargetType.ORAL:
                case BoneAnimationDefiner.FemaleTargetType.ORALSWAP:
                    {
                        // Calculate the center point between the left and right sides of the mouth
                        Vector3 mouthCenter = (_femaleMouthLeft.position + _femaleMouthRight.position) * 0.5f;
                        // Calculate the center point between the two lips of the mouth
                        Vector3 lipsCenter = (_femaleMouthLipsUpper.position + _femaleMouthLipsLower.position) * 0.5f;
                        Vector3 side = (_femaleMouthRight.position - mouthCenter).normalized;
                        // Vector from the selected female's mouth lips center point to mouth center point
                        femaleTargetXAxis = (mouthCenter - lipsCenter).normalized;
                        // Use the female's mouth and lips center points vector and the female's mouth to establish the Y reference axis
                        Vector3 femaleTargetYAxis = Vector3.Cross(femaleTargetXAxis, side);
                        // check that the vector length isn't zero
                        if (femaleTargetYAxis.sqrMagnitude < 1e-8f)
                            femaleTargetYAxis = Vector3.Cross(femaleTargetXAxis, Vector3.up);
                        femaleTargetYAxis.Normalize();
                        // Use the reference X and Y axes to establish the orthogonal Z axis
                        femaleTargetZAxis = Vector3.Cross(femaleTargetXAxis, femaleTargetYAxis).normalized;
                        // set the targeted position
                        targetPos = mouthCenter;
                        break;
                    }
                case BoneAnimationDefiner.FemaleTargetType.BREASTS:
                case BoneAnimationDefiner.FemaleTargetType.BREASTSWAP:
                    {
                        // Calculate the center point between the two middle breasts
                        Vector3 midBreasts = (_femaleMiddleBreastsLeft.position + _femaleMiddleBreastsRight.position) * 0.5f;
                        Vector3 side = (_femaleMiddleBreastsRight.position - midBreasts).normalized;
                        // Vector from the selected female's middle breasts to breasts on chest
                        Vector3 femaleTargetYAxis = (_femaleBreasts.position - midBreasts).normalized;
                        // Use the female's middle breasts and breasts on chest vector and the female's middle breasts right to establish the X reference axis
                        femaleTargetXAxis = Vector3.Cross(femaleTargetYAxis, side);
                        // check that the vector length isn't zero
                        if (femaleTargetXAxis.sqrMagnitude < 1e-8f)
                            femaleTargetXAxis = Vector3.Cross(femaleTargetYAxis, Vector3.up);
                        femaleTargetXAxis.Normalize();
                        // Use the reference X and Y axes to establish the orthogonal Z axis
                        femaleTargetZAxis = Vector3.Cross(femaleTargetYAxis, femaleTargetXAxis).normalized;
                        // set the targeted position
                        targetPos = midBreasts;
                        break;
                    }
                case BoneAnimationDefiner.FemaleTargetType.LEFTHAND:
                case BoneAnimationDefiner.FemaleTargetType.LEFTHANDSWAP:
                    {
                        // Vector from the selected female's hand and middle finger
                        Vector3 side = (femaleHandL - _femaleMiddleFingerLeft.position).normalized;
                        // Use the female's middle and ring fingers vector and the female's hand to establish the Y reference axis
                        femaleTargetXAxis = (_femaleMiddleFingerLeft.position - _femaleRingFingerLeft.position).normalized;
                        // Use the reference X and Y axes to establish the orthogonal Z axis
                        femaleTargetZAxis = Vector3.Cross(femaleTargetXAxis, side);
                        // check that the vector length isn't zero
                        if (femaleTargetZAxis.sqrMagnitude < 1e-8f)
                            femaleTargetZAxis = Vector3.Cross(femaleTargetXAxis, Vector3.up);
                        femaleTargetZAxis.Normalize();
                        // set the targeted position
                        targetPos = femaleHandL;
                        break;
                    }
                case BoneAnimationDefiner.FemaleTargetType.RIGHTHAND:
                case BoneAnimationDefiner.FemaleTargetType.RIGHTHANDSWAP:
                    {
                        // Vector from the selected female's hand and middle finger
                        Vector3 side = (femaleHandR - _femaleMiddleFingerRight.position).normalized;
                        // Use the female's middle and ring fingers vector and the female's hand to establish the Y reference axis
                        femaleTargetXAxis = (_femaleMiddleFingerRight.position - _femaleRingFingerRight.position).normalized;
                        // Use the reference X and Y axes to establish the orthogonal Z axis
                        femaleTargetZAxis = Vector3.Cross(femaleTargetXAxis, side);
                        // check that the vector length isn't zero
                        if (femaleTargetZAxis.sqrMagnitude < 1e-8f)
                            femaleTargetZAxis = Vector3.Cross(femaleTargetXAxis, Vector3.up);
                        femaleTargetZAxis.Normalize();
                        // set the targeted position
                        targetPos = femaleHandR;
                        break;
                    }
                case BoneAnimationDefiner.FemaleTargetType.INTERCRURAL_HIP:
                case BoneAnimationDefiner.FemaleTargetType.INTERCRURAL_HIP_SWAP:
                    {
                        Vector3 side = (femaleHip - femaleVagina).normalized;
                        // Vector from the selected female's vagina to anus
                        femaleTargetXAxis = (femaleVagina - femaleAnus).normalized;
                        // Use the female's vagina and hip vector and the female's anus to establish the Z reference axis
                        femaleTargetZAxis = Vector3.Cross(femaleTargetXAxis, side);
                        // check that the vector length isn't zero
                        if (femaleTargetZAxis.sqrMagnitude < 1e-8f)
                            femaleTargetZAxis = Vector3.Cross(femaleTargetXAxis, Vector3.up);
                        femaleTargetZAxis.Normalize();
                        // set the targeted position
                        targetPos = femaleHip;
                        break;
                    }
                case BoneAnimationDefiner.FemaleTargetType.LEFTFOOT:
                case BoneAnimationDefiner.FemaleTargetType.LEFTFOOTSWAP:
                    {
                        // There aren't singular toes, therefore only one "toe" can be tracked
                        Vector3 side = (femaleFootL - _femaleToesLeft.position).normalized;
                        femaleTargetXAxis = (_femaleToesLeft.position - femaleFootL).normalized;
                        // check that the vector length isn't zero
                        if (femaleTargetXAxis.sqrMagnitude < 1e-8f)
                            femaleTargetXAxis = Vector3.right;
                        femaleTargetXAxis.Normalize();

                        femaleTargetZAxis = Vector3.Cross(femaleTargetXAxis, side);
                        // check that the vector length isn't zero
                        if (femaleTargetZAxis.sqrMagnitude < 1e-8f)
                            femaleTargetZAxis = Vector3.Cross(femaleTargetXAxis, Vector3.up);
                        femaleTargetZAxis.Normalize();
                        // set the targeted position
                        targetPos = femaleFootL;
                        break;
                    }
                case BoneAnimationDefiner.FemaleTargetType.RIGHTFOOT:
                case BoneAnimationDefiner.FemaleTargetType.RIGHTFOOTSWAP:
                    {
                        // There aren't singular toes, therefore only one "toe" can be tracked
                        Vector3 side = (femaleFootR - _femaleToesRight.position).normalized;
                        femaleTargetXAxis = (_femaleToesRight.position - femaleFootR).normalized;
                        // check that the vector length isn't zero
                        if (femaleTargetXAxis.sqrMagnitude < 1e-8f)
                            femaleTargetXAxis = Vector3.forward;
                        femaleTargetXAxis.Normalize();

                        femaleTargetZAxis = Vector3.Cross(femaleTargetXAxis, side);
                        // check that the vector length isn't zero
                        if (femaleTargetZAxis.sqrMagnitude < 1e-8f)
                            femaleTargetZAxis = Vector3.Cross(femaleTargetXAxis, Vector3.up);
                        femaleTargetZAxis.Normalize();
                        // set the targeted position
                        targetPos = femaleFootR;
                        break;
                    }
                default:
                    {
                        femaleTargetXAxis = Vector3.right;
                        femaleTargetZAxis = Vector3.up;
                        targetPos = femaleHip;
                        break;
                    }
            }
        }

        private void SendTCodeHomeCommand()
        {
            // if an unknown/unsupported animation is playing, then
            // instead of "locking" the device in a weird position,
            // send it "home" Value: Mid-point (50%))
            string command = "L05000\n" +
            "L15000\n" +
            "L25000\n" +
            "R05000\n" +
            "R15000\n" +
            "R25000";
            SendTCodeCommand(command);
        }

        private void SendTCodeCommand(string command)
        {
            // If serial port is open then and it's not a repeated command, send the command to the robot
            if (_serialPortConnection?.AC_SerialPort != null &&
                _serialPortConnection.AC_SerialPort.IsOpen &&
                command != _lastCommand)
            {
                _serialPortConnection.AC_SerialPort.WriteLine(command);
                _lastCommand = command;
            }
        }

        #region AI_GENERATED
        private void UpdateRobotPosition()
        {
            // Setup T-code reference coordinate system
            // X(L0) is up/down in reference to the selected male's penis vector and is positive up
            // Y(L1) is toward/away orthogonal to the selected male's penis vector and is positive away
            // Z(L2) is left/right orthogonal to the selected male's penis vector and is positive left
            // RX(R0) is positive according to the right hand rule around X(L0)
            // RY(R1) is positive according to the right hand rule around Y(L1)
            // RZ(R2) is positive according to the right hand rule around Z(L2)
            try
            {
                CalculateL0Movement();
                Vector3 penisBasePos = _malePenisBase.position;
                // Calculate the center point between the two penis's balls
                Vector3 centerPointBalls = (_malePenisLeftBall.position + _malePenisRightBall.position) * 0.5f;
                // calculate direction based on the penis base and the center point of the balls (assuming vector length isn't zero)
                Vector3 penisBaseToBallsDir = (centerPointBalls - penisBasePos).sqrMagnitude < 1e-8f ? Vector3.up : (centerPointBalls - penisBasePos);
                penisBaseToBallsDir.Normalize();

                // Use the male's penis's base to tip and the male's penis's direction to establish the Z reference axis
                Vector3 malePenisZAxis = Vector3.Cross(_malePenisXAxis, penisBaseToBallsDir);
                if (malePenisZAxis.sqrMagnitude < 1e-8f)
                    malePenisZAxis = Vector3.Cross(_malePenisXAxis, Vector3.up);
                malePenisZAxis.Normalize();

                // Use the reference X and Z axes to establish the orthogonal Y axis
                Vector3 malePenisYAxis = Vector3.Cross(malePenisZAxis, _malePenisXAxis).normalized;

                GetFemaleTargetPosition(out Vector3 femaleTargetXAxis, out Vector3 femaleTargetZAxis, out Vector3 targetPos);
                // calculate distance from the current penis tip to the target position
                Vector3 targetToTip = _currPenisTip - targetPos;

                // Calculate Y(L1) for robot based on the male Y axis and the vector based on the distance between female target and penis tip 
                float robotL1 = Mathf.Clamp01(0.5f + Vector3.Dot(targetToTip, malePenisYAxis));
                // Calculate Z(L2) for robot based on the male Z axis and the vector based on the distance between female target and penis tip 
                float robotL2 = Mathf.Clamp01(0.5f + Vector3.Dot(targetToTip, malePenisZAxis));

                // Calculate RX(R0)  for robot based on the male Z-axis and the female Z-Axis
                float robotR0Angle = Vector3.Angle(malePenisZAxis, femaleTargetZAxis);
                // Calculate RY(R1) for robot based on the male Z-axis and the female X-Axis
                float robotR1Angle = 90f - Vector3.Angle(malePenisZAxis, femaleTargetXAxis);
                // Calculate RZ(R2) for robot based on the male Y-axis and the female X-Axis
                float robotR2Angle = 90f - Vector3.Angle(malePenisYAxis, femaleTargetXAxis);

                float robotR0 = Mathf.Clamp01(robotR0Angle / 180f);
                float robotR1 = Mathf.Clamp01(0.5f + robotR1Angle / 180f);
                float robotR2 = Mathf.Clamp01(0.5f + robotR2Angle / 180f);

                // Formulate T-Code command string
                string command = "L0" + GenerateTCode(_normalizedL0, AC_SexRobotControllerPlugin.RobotL0Min.Value, AC_SexRobotControllerPlugin.RobotL0Max.Value) + "\n"
                + "L1" + GenerateTCode(robotL1, AC_SexRobotControllerPlugin.RobotL1Min.Value, AC_SexRobotControllerPlugin.RobotL1Max.Value) + "\n"
                + "L2" + GenerateTCode(robotL2, AC_SexRobotControllerPlugin.RobotL2Min.Value, AC_SexRobotControllerPlugin.RobotL2Max.Value) + "\n"
                + "R0" + GenerateTCode(robotR0, AC_SexRobotControllerPlugin.RobotR0Min.Value, AC_SexRobotControllerPlugin.RobotR0Max.Value) + "\n"
                + "R1" + GenerateTCode(robotR1, AC_SexRobotControllerPlugin.RobotR1Min.Value, AC_SexRobotControllerPlugin.RobotR1Max.Value) + "\n"
                + "R2" + GenerateTCode(robotR2, AC_SexRobotControllerPlugin.RobotR2Min.Value, AC_SexRobotControllerPlugin.RobotR2Max.Value);

                // diagnostics/debugging
                if (AC_SexRobotControllerPlugin.DiagnosticsConfig.Value)
                    AC_SexRobotControllerPlugin.LogDebug(
                        $"AnimName: {AnimationName}, LoopType: {LoopType}, Speed: {AnimationSpeed}, IsNowOrgasm: {IsNowOrgasm}\n"
                        + $"_normalizedL0: {_normalizedL0} \nrobotL1: {robotL1} \nrobotL2: {robotL2} \n"
                        + $"robotR0: {robotR0} \nrobotR1: {robotR1} \nrobotR2: {robotR2} \n"
                        + $"T-Code: \n{command}"
                        );

                SendTCodeCommand(command);
            }
            catch (Exception ex)
            {
                AC_SexRobotControllerPlugin.LogDebug("Serial error: " + ex);
            }
        }
        private void CalculateL0Movement()
        {
            Vector3 penisBasePos = _malePenisBase.position;
            Vector3 penisMidPos = _malePenisMid.position;
            Vector3 penisTipPos = _malePenisTip.position;
            // Calculate the center point between the two penis's balls
            Vector3 centerPointBalls = (_malePenisLeftBall.position + _malePenisRightBall.position) * 0.5f;

            // check if Orgasm, since this uses the same LoopType as IDLE
            bool isIdle = (LoopType == (int)BoneAnimationDefiner.LoopType.IDLE && !IsNowOrgasm);

            // simplified Catmull-Rom for smooth motion 
            _currPenisTip = ((penisBasePos + penisMidPos + penisTipPos + centerPointBalls) / 4f);

            // since the penis "collapses" on itself in some animations...
            Vector3 spine = (penisTipPos - penisBasePos);
            //...check if the vector is zero length...
            if (spine.sqrMagnitude < 1e-8f)
                spine = _currPenisTip - penisBasePos;
            //...then calculate the X-Axis based on a value which has a valid length
            _malePenisXAxis = (penisTipPos - penisMidPos).sqrMagnitude < 1e-8f ? spine : (penisTipPos - penisMidPos);
            _malePenisXAxis.Normalize();

            // project current tip along the primary axis
            float penisTipProjection = Vector3.Dot(_currPenisTip - penisBasePos, _malePenisXAxis);

            // Initialize reference on first frame
            if (!_l0Initialized)
            {
                _l0Reference = penisTipProjection;
                _l0Initialized = true;
            }

            // Delta movement since last frame
            float delta = penisTipProjection - _l0Reference;

            // Smooth reference to avoid jitters
            _l0Reference = Mathf.Lerp(_l0Reference, penisTipProjection, 0.02f);

            // Ignore very tiny deltas
            if (Mathf.Abs(delta) < 0.0003f)
                delta = 0f;

            // Apply user-tunable speed multiplier
            float speedMultiplier = GetL0SpeedMultiplier() * L0SpeedMultiplier;

            // Apply intensity scaling (user-tunable)
            float targetActive = delta * L0MovementIntensity * speedMultiplier;

            // Target L0 position for active state
            targetActive = 0.5f + targetActive;

            // Idle motion (gentle sinusoidal)
            _idlePhase += Time.deltaTime * 1.2f;
            float idle = 0.5f + Mathf.Sin(_idlePhase) * 0.02f;

            // Select target based on state
            float target = isIdle ? idle : targetActive;

            // Smooth time (slower for idle)
            float smoothTime = isIdle ? 0.2f : 0.05f;

            // Smooth damp filtering
            _l0Filtered = Mathf.SmoothDamp(_l0Filtered, target, ref _l0SmoothVelocity, smoothTime);

            // Clamp for T-Code conversion
            _normalizedL0 = Mathf.Clamp01(_l0Filtered);
        }

        private float GetL0SpeedMultiplier()
        {
            // Normalize the speed of the animation 0->2 becomes 0->1
            float speed = Mathf.InverseLerp(0f, 2f, AnimationSpeed);
            float t = Mathf.SmoothStep(0f, 1f, speed);
            // Non-linear curve, mapped to amplification range
            // low speeds stay soft, high speeds ramp aggressively
            return Mathf.Lerp(1.0f, L0SpeedMultiplier, t);
        }

        private static string GenerateTCode(float normalized, float min, float max)
        {
            // Clamp normalized input to a value between 0 and 1
            normalized = Mathf.Clamp01(normalized);
            float value = Mathf.Lerp(min, max, normalized);
            int servo = Mathf.RoundToInt(value * 1000f);
            // Clamp to SR6 limits
            servo = Mathf.Clamp(servo, 0, 999);

            return servo.ToString("D3");
        }
        #endregion
    }
}
