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
        private static SerialPortConnection _serialPortConnection;
        private static readonly Lazy<RobotMovement> _instance = new(() => new RobotMovement());

        internal bool SpeedChanged { get; set; }
        internal bool UpdatePosition { get; set; }
        internal string AnimationName { get; set; }
        internal bool AnimationChanged { get; set; }
        internal string PrevAnimationName { get; set; }
        /**
         * The HActor Objects are currently not in use.
         * However, they are being kept for now, in case they'll be needed in later development.
         * Once a "stable" version is reached for the Main, Free-H and VR, these can be removed.
         */
        internal HActor Player { get; set; }
        internal HActor[] Females { get; set; }
        internal HActor[] Attackers { get; set; }

        private Transform PlayerTransform { get; set; }
        private Transform[] FemaleTransform { get; set; }
        private Transform[] AttackerTransform { get; set; }

        private float _autoRangeMin;
        private float _autoRangeMid;
        private float _autoRangeMax;

        private Transform _malePenisBase;
        private Transform _malePenisTip;
        private Transform _malePenisLeftBall;
        private Transform _malePenisRightBall;
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

        private BoneAnimationDefiner.FemaleTargetType _currentFemaleTargetType;

        private RobotMovement()
        {
            Player = null;
            Females = null;
            AnimationName = "";
            SpeedChanged = false;
            UpdatePosition = false;
            PrevAnimationName = "";
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
                    {
                        GetBonePositionData();
                        _autoRangeMin = 1.0f;
                        _autoRangeMax = 0.0f;
                        // speed change is only relevant when increasing/decreasing the speed
                        SpeedChanged = false;
                    }
                    else if (SpeedChanged)
                    {
                        _autoRangeMin = 1.0f;
                        _autoRangeMax = 0.0f;
                        SpeedChanged = false;
                    }
                    UpdateRobotPosition();
                }
                else
                {
                    if (PrevAnimationName != AnimationName)
                    {
                        AC_SexRobotControllerPlugin.LogInfo("The animation name was not found in the plugin dictonary: '" + AnimationName + "'.'");
                        WriteUnknownAnimationsToFile();
                        // set previous to the current to avoid multiple rewrites on current animation refresh
                        PrevAnimationName = AnimationName;
                    }
                }
            }
            catch (Exception ex)
            {
                AC_SexRobotControllerPlugin.LogDebug("An error occurred upon attempting to update the position: " + ex.ToString());
            }
        }

        private void WriteUnknownAnimationsToFile()
        {
            // check current animation name (for finding unregistered sex-animations)
            // verify that position doesn't exist and isn't already printed
            if (AC_SexRobotControllerPlugin.WriteAnimationsToFile.Value)
            {
                FileHandler.WriteToFile(AnimationName);
                AC_SexRobotControllerPlugin.LogInfo("The animation name '" + AnimationName + "' was written to file!");
            }
        }

        internal void SetActorObjects(HActor _player, HActor[] females, HActor[] _attackers)
        {
            if (_player == null || _attackers[0] == null)
                AC_SexRobotControllerPlugin.LogDebug("--> Player is NULL");

            // assumption: Player object is always set, and if not, Player would be the first male
            Player = _player ?? _attackers[0];

            Females = females;
            Attackers = _attackers;
            // get the transforms for calculating the movement
            FemaleTransform = GetTransforms(Females);
            AttackerTransform = GetTransforms(Attackers);
            PlayerTransform = Player.Human.gameObject.GetComponentInChildren<Transform>();
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
            {
                AC_SexRobotControllerPlugin.LogInfo("Animation: " + AnimationName);
                AC_SexRobotControllerPlugin.LogInfo("females found: " + Females.Length.ToString());
            }

            if (Player != null && Females != null && Females.Length > 0)
            {
                UpdateMaleTransforms();

                // Lookup in the animation dictionary the female target type for this current animation
                BoneAnimationDefiner.animationFemaleTargetDictionary.TryGetValue(AnimationName, out _currentFemaleTargetType);

                if (_currentFemaleTargetType == BoneAnimationDefiner.FemaleTargetType.VAGINAL || _currentFemaleTargetType == BoneAnimationDefiner.FemaleTargetType.ANAL
                    || _currentFemaleTargetType == BoneAnimationDefiner.FemaleTargetType.ORAL || _currentFemaleTargetType == BoneAnimationDefiner.FemaleTargetType.BREASTS
                    || _currentFemaleTargetType == BoneAnimationDefiner.FemaleTargetType.LEFTHAND || _currentFemaleTargetType == BoneAnimationDefiner.FemaleTargetType.RIGHTHAND
                    || _currentFemaleTargetType == BoneAnimationDefiner.FemaleTargetType.INTERCRURAL || _currentFemaleTargetType == BoneAnimationDefiner.FemaleTargetType.LEFTFOOT
                    || _currentFemaleTargetType == BoneAnimationDefiner.FemaleTargetType.RIGHTFOOT || _currentFemaleTargetType == BoneAnimationDefiner.FemaleTargetType.BOTH_HANDS
                    || _currentFemaleTargetType == BoneAnimationDefiner.FemaleTargetType.BOTH_FEET)
                {
                    UpdateFemaleTransforms(girlIndex);
                }
                else if (_currentFemaleTargetType == BoneAnimationDefiner.FemaleTargetType.VAGINALSWAP || _currentFemaleTargetType == BoneAnimationDefiner.FemaleTargetType.ORALSWAP
                    || _currentFemaleTargetType == BoneAnimationDefiner.FemaleTargetType.BREASTSWAP || _currentFemaleTargetType == BoneAnimationDefiner.FemaleTargetType.LEFTHANDSWAP
                    || _currentFemaleTargetType == BoneAnimationDefiner.FemaleTargetType.RIGHTHANDSWAP || _currentFemaleTargetType == BoneAnimationDefiner.FemaleTargetType.INTERCRURALSWAP
                    || _currentFemaleTargetType == BoneAnimationDefiner.FemaleTargetType.LEFTFOOTSWAP || _currentFemaleTargetType == BoneAnimationDefiner.FemaleTargetType.RIGHTFOOTSWAP)
                {
                    if (Females.Length == 2)
                    {
                        girlIndex = 1;
                        UpdateFemaleTransforms(girlIndex);
                    }
                    else
                    {
                        AC_SexRobotControllerPlugin.LogInfo("Error: The current HScene (swap) doesn't have 2 females.");
                    }
                    //TODO: Implement check for 5P => 4F1M?
                }
                if (AC_SexRobotControllerPlugin.DiagnosticsConfig.Value)
                {
                    AC_SexRobotControllerPlugin.LogInfo("Current animation: " + AnimationName);
                }

                AnimationChanged = false;
                UpdatePosition = true;
            }
            else
            {
                UpdatePosition = false;
                AC_SexRobotControllerPlugin.LogInfo("Error: The current HScene doesn't have 1 male and at least 1 female.");
            }
        }

        private void UpdateMaleTransforms()
        {
            try
            {
                // required to get the current, the first attempt calling this directly when setting the HScene resulted in a NullException.
                // From inspecting the object(s) in the Inspector, the following is observed:
                // (X,Y,Z) -> Z ist mostly static and mostly has the same value as the female Vagina
                // In most cases, either X or Y stands out, for some both have a noticeable difference
                //
                // The e.g. the Cowgirl(s) positions seems to be the fact that the diff is only 0.04-0.05 for X/Y,
                // whereas on average for the rest, the diff is 11-19 for X/Y (for either or both).
                // 
                // Tt seems to me (visually) that the tip gets "pushed down", meaning the animation itself results in "failed" penetration animation.
                // The Cowgirl isn't the only place where this happens, but it's unclear to me, if this is caused by the uncensoring mechanism or the game itself.
                // A potential workaround could be to either create a "BadAnimations" or "PositionMultiplier", something like:
                // if (xDiff <= 5 && yDiff <= 5) x|y *= 2
                var maleTransform = PlayerTransform.gameObject.GetComponentsInChildren<Transform>();

                // Get the base of the male's penis
                _malePenisBase = maleTransform.Where(x => x.name == BoneAnimationDefiner.bodyBoneDictionary[BoneAnimationDefiner.BodyBone.PENIS_BASE]).FirstOrDefault();

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

                // Find/set all the female Transforms needed for the VAGINAL / ANAL / INTERCRURAL calculations
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

        private void UpdateRobotPosition()
        {

            if (UpdatePosition)
            {
                // Setup T-code reference coordinate system
                // X(L0) is up/down in reference to the selected male's penis vector and is positive up
                // Y(L1) is toward/away orthogonal to the selected male's penis vector and is positive away
                // Z(L2) is left/right orthogonal to the selected male's penis vector and is positive left
                // RX(R0) is positive according to the right hand rule around X(L0)
                // RY(R1) is positive according to the right hand rule around Y(L1)
                // RZ(R2) is positive according to the right hand rule around Z(L2)

                // Calculate the center point between the two penis's balls
                Vector3 malePenisBallsCenterPoint = (_malePenisLeftBall.position + _malePenisRightBall.position) / 2.0f;

                // Calculate male's penis length
                float malePenisLength = Vector3.Distance(_malePenisBase.position, _malePenisTip.position);

                // Vector from the selected male's penis's base to tip
                Vector3 malePenisXAxis = _malePenisTip.position - _malePenisBase.position;

                // Use the male's penis's base and the male's penis's balls center point to establish the Z reference axis
                Vector3 malePenisZAxis = Vector3.Cross(malePenisXAxis, malePenisBallsCenterPoint - _malePenisBase.position);
                malePenisZAxis = (malePenisXAxis.magnitude / malePenisZAxis.magnitude) * malePenisZAxis;

                // Use the reference X and Z axes to establish the orthogonal Y axis
                Vector3 malePenisYAxis = Vector3.Cross(malePenisXAxis, malePenisZAxis);
                malePenisYAxis = (malePenisXAxis.magnitude / malePenisYAxis.magnitude) * malePenisYAxis;

                Vector3 femaleTargetYAxis;
                Vector3 femaleTargetXAxis = new(0.0f, 0.0f, 0.0f);
                Vector3 femaleTargetZAxis = new(0.0f, 0.0f, 0.0f);
                Vector3 femaleTargetToMalePenisBase = new(0.0f, 0.0f, 0.0f);

                if (_currentFemaleTargetType == BoneAnimationDefiner.FemaleTargetType.VAGINAL || _currentFemaleTargetType == BoneAnimationDefiner.FemaleTargetType.VAGINALSWAP)
                {
                    // Vector from the selected female's vagina to hip
                    femaleTargetXAxis = _femaleHip.position - _femaleVagina.position;

                    // Use the female's vagina and hip vector and the female's anus to establish the Z reference axis
                    femaleTargetZAxis = Vector3.Cross(femaleTargetXAxis, _femaleAnus.position - _femaleVagina.position);
                    femaleTargetZAxis = (femaleTargetXAxis.magnitude / femaleTargetZAxis.magnitude) * femaleTargetZAxis;

                    // Use the reference X and Z axes to establish the orthogonal Y axis
                    femaleTargetYAxis = Vector3.Cross(femaleTargetXAxis, femaleTargetZAxis);
                    femaleTargetYAxis = (femaleTargetXAxis.magnitude / femaleTargetYAxis.magnitude) * femaleTargetYAxis;

                    // Vector from the female's vagina to the male's penis's base
                    femaleTargetToMalePenisBase = _femaleVagina.position - _malePenisBase.position;
                }
                else if (_currentFemaleTargetType == BoneAnimationDefiner.FemaleTargetType.ANAL)
                {
                    // Vector from the selected female's anus to hip
                    femaleTargetXAxis = _femaleHip.position - _femaleAnus.position;

                    // Use the female's vagina and hip vector and the female's anus to establish the Z reference axis
                    femaleTargetZAxis = Vector3.Cross(femaleTargetXAxis, _femaleAnus.position - _femaleVagina.position);
                    femaleTargetZAxis = (femaleTargetXAxis.magnitude / femaleTargetZAxis.magnitude) * femaleTargetZAxis;

                    // Use the reference X and Z axes to establish the orthogonal Y axis
                    femaleTargetYAxis = Vector3.Cross(femaleTargetXAxis, femaleTargetZAxis);
                    femaleTargetYAxis = (femaleTargetXAxis.magnitude / femaleTargetYAxis.magnitude) * femaleTargetYAxis;

                    // Vector from the female's vagina to the male's penis's base
                    femaleTargetToMalePenisBase = _femaleAnus.position - _malePenisBase.position;
                }
                else if (_currentFemaleTargetType == BoneAnimationDefiner.FemaleTargetType.ORAL || _currentFemaleTargetType == BoneAnimationDefiner.FemaleTargetType.ORALSWAP)
                {
                    // Calculate the center point between the two lips of the mouth
                    Vector3 femaleMouthLipsCenterPoint = (_femaleMouthLipsUpper.position + _femaleMouthLipsLower.position) / 2.0f;

                    // Calculate the center point between the left and right sides of the mouth
                    Vector3 femaleMouthCenterPoint = (_femaleMouthLeft.position + _femaleMouthRight.position) / 2.0f;

                    // Vector from the selected female's mouth lips center point to mouth center point
                    femaleTargetXAxis = femaleMouthCenterPoint - femaleMouthLipsCenterPoint;

                    // Use the female's mouth and lips center points vector and the female's mouth to establish the Y reference axis
                    femaleTargetYAxis = Vector3.Cross(femaleTargetXAxis, _femaleMouthRight.position - femaleMouthCenterPoint);
                    femaleTargetYAxis = (femaleTargetXAxis.magnitude / femaleTargetYAxis.magnitude) * femaleTargetYAxis;

                    // Use the reference X and Y axes to establish the orthogonal Z axis
                    femaleTargetZAxis = Vector3.Cross(femaleTargetXAxis, femaleTargetYAxis);
                    femaleTargetZAxis = (femaleTargetXAxis.magnitude / femaleTargetZAxis.magnitude) * femaleTargetZAxis;

                    // Vector from the female's mouth center point to the male's penis's base
                    femaleTargetToMalePenisBase = femaleMouthCenterPoint - _malePenisBase.position;
                }
                else if (_currentFemaleTargetType == BoneAnimationDefiner.FemaleTargetType.BREASTS)
                {
                    // Calculate the center point between the two middle breasts
                    Vector3 femaleMiddleBreastsCenterPoint = (_femaleMiddleBreastsLeft.position + _femaleMiddleBreastsRight.position) / 2.0f;

                    // Vector from the selected female's middle breasts to breasts on chest
                    femaleTargetYAxis = _femaleBreasts.position - femaleMiddleBreastsCenterPoint;

                    // Use the female's middle breasts and breasts on chest vector and the female's middle breasts right to establish the X reference axis
                    femaleTargetXAxis = Vector3.Cross(femaleTargetYAxis, _femaleMiddleBreastsRight.position - femaleMiddleBreastsCenterPoint);
                    femaleTargetXAxis = (femaleTargetYAxis.magnitude / femaleTargetXAxis.magnitude) * femaleTargetXAxis;

                    // Use the reference X and Y axes to establish the orthogonal Z axis
                    femaleTargetZAxis = Vector3.Cross(femaleTargetYAxis, femaleTargetXAxis);
                    femaleTargetZAxis = (femaleTargetYAxis.magnitude / femaleTargetZAxis.magnitude) * femaleTargetZAxis;

                    // Vector from the female's breasts center point to the male's penis's base
                    femaleTargetToMalePenisBase = femaleMiddleBreastsCenterPoint - _malePenisBase.position;
                }
                else if (_currentFemaleTargetType == BoneAnimationDefiner.FemaleTargetType.LEFTHAND
                    || _currentFemaleTargetType == BoneAnimationDefiner.FemaleTargetType.LEFTHANDSWAP
                    || _currentFemaleTargetType == BoneAnimationDefiner.FemaleTargetType.BOTH_HANDS)
                {
                    // Vector from the selected female's middle and ring fingers
                    femaleTargetXAxis = _femaleMiddleFingerLeft.position - _femaleRingFingerLeft.position;

                    // Use the female's middle and ring fingers vector and the female's hand to establish the Y reference axis
                    femaleTargetZAxis = Vector3.Cross(femaleTargetXAxis, _femaleHandLeft.position - _femaleMiddleFingerLeft.position);
                    femaleTargetZAxis = (femaleTargetXAxis.magnitude / femaleTargetZAxis.magnitude) * femaleTargetZAxis;

                    // Use the reference X and Y axes to establish the orthogonal Z axis
                    femaleTargetYAxis = Vector3.Cross(femaleTargetXAxis, femaleTargetZAxis);
                    femaleTargetYAxis = (femaleTargetXAxis.magnitude / femaleTargetYAxis.magnitude) * femaleTargetYAxis;

                    // Vector from the female's hand to the male's penis's base
                    femaleTargetToMalePenisBase = _femaleHandLeft.position - _malePenisBase.position;
                }
                else if (_currentFemaleTargetType == BoneAnimationDefiner.FemaleTargetType.RIGHTHAND
                    || _currentFemaleTargetType == BoneAnimationDefiner.FemaleTargetType.RIGHTHANDSWAP)
                {
                    // Vector from the selected female's middle and ring fingers
                    femaleTargetXAxis = _femaleMiddleFingerRight.position - _femaleRingFingerRight.position;

                    // Use the female's middle and ring fingers vector and the female's hand to establish the Y reference axis
                    femaleTargetZAxis = Vector3.Cross(femaleTargetXAxis, _femaleHandRight.position - _femaleMiddleFingerRight.position);
                    femaleTargetZAxis = (femaleTargetXAxis.magnitude / femaleTargetZAxis.magnitude) * femaleTargetZAxis;

                    // Use the reference X and Y axes to establish the orthogonal Z axis
                    femaleTargetYAxis = Vector3.Cross(femaleTargetXAxis, femaleTargetZAxis);
                    femaleTargetYAxis = (femaleTargetXAxis.magnitude / femaleTargetYAxis.magnitude) * femaleTargetYAxis;

                    // Vector from the female's hand to the male's penis's base
                    femaleTargetToMalePenisBase = _femaleHandRight.position - _malePenisBase.position;
                }
                else if (_currentFemaleTargetType == BoneAnimationDefiner.FemaleTargetType.LEFTFOOT
                    || _currentFemaleTargetType == BoneAnimationDefiner.FemaleTargetType.LEFTFOOTSWAP
                    // DUMMY: using BOTH_FEET not implemented, but at least ensure some movement is made
                    || _currentFemaleTargetType == BoneAnimationDefiner.FemaleTargetType.BOTH_FEET)
                {
                    //DUMMY Code, missing key/value for toes to properly map this
                    femaleTargetXAxis = _femaleToesLeft.position; // _femaleMiddleFingerLeft.position - _femaleRingFingerLeft.position;

                    // Use the female's middle and ring fingers vector and the female's hand to establish the Y reference axis
                    femaleTargetZAxis = Vector3.Cross(femaleTargetXAxis, _femaleFootLeft.position - _femaleToesLeft.position);
                    femaleTargetZAxis = (femaleTargetXAxis.magnitude / femaleTargetZAxis.magnitude) * femaleTargetZAxis;

                    // Use the reference X and Y axes to establish the orthogonal Z axis
                    femaleTargetYAxis = Vector3.Cross(femaleTargetXAxis, femaleTargetZAxis);
                    femaleTargetYAxis = (femaleTargetXAxis.magnitude / femaleTargetYAxis.magnitude) * femaleTargetYAxis;

                    // Vector from the female's hand to the male's penis's base
                    femaleTargetToMalePenisBase = _femaleFootLeft.position - _malePenisBase.position;
                }
                else if (_currentFemaleTargetType == BoneAnimationDefiner.FemaleTargetType.RIGHTFOOT
                    || _currentFemaleTargetType == BoneAnimationDefiner.FemaleTargetType.RIGHTFOOTSWAP)
                {
                    //DUMMY Code, missing key/value for toes to properly map this
                    femaleTargetXAxis = _femaleToesRight.position; // _femaleMiddleFingerLeft.position - _femaleRingFingerLeft.position;

                    // Use the female's middle and ring fingers vector and the female's hand to establish the Y reference axis
                    femaleTargetZAxis = Vector3.Cross(femaleTargetXAxis, _femaleFootRight.position - _femaleToesRight.position);
                    femaleTargetZAxis = (femaleTargetXAxis.magnitude / femaleTargetZAxis.magnitude) * femaleTargetZAxis;

                    // Use the reference X and Y axes to establish the orthogonal Z axis
                    femaleTargetYAxis = Vector3.Cross(femaleTargetXAxis, femaleTargetZAxis);
                    femaleTargetYAxis = (femaleTargetXAxis.magnitude / femaleTargetYAxis.magnitude) * femaleTargetYAxis;

                    // Vector from the female's hand to the male's penis's base
                    femaleTargetToMalePenisBase = _femaleFootRight.position - _malePenisBase.position;
                }
                else if (_currentFemaleTargetType == BoneAnimationDefiner.FemaleTargetType.INTERCRURAL
                    || _currentFemaleTargetType == BoneAnimationDefiner.FemaleTargetType.INTERCRURALSWAP)
                {
                    // Vector from the selected female's vagina to anus
                    femaleTargetXAxis = _femaleVagina.position - _femaleAnus.position;

                    // Use the female's vagina and hip vector and the female's anus to establish the Z reference axis
                    femaleTargetZAxis = Vector3.Cross(femaleTargetXAxis, _femaleHip.position - _femaleVagina.position);
                    femaleTargetZAxis = (femaleTargetXAxis.magnitude / femaleTargetZAxis.magnitude) * femaleTargetZAxis;

                    // Use the reference X and Z axes to establish the orthogonal Y axis
                    femaleTargetYAxis = Vector3.Cross(femaleTargetXAxis, femaleTargetZAxis);
                    femaleTargetYAxis = (femaleTargetXAxis.magnitude / femaleTargetYAxis.magnitude) * femaleTargetYAxis;

                    // Vector from the female's vagina to the male's penis's base
                    femaleTargetToMalePenisBase = _femaleVagina.position - _malePenisBase.position;
                }

                // Calculate X(L0) for robot based on the reference X axis and the vector from the female's vagina's labia trigger to the male's penis's base collider
                float robotL0 = Vector3.Dot(malePenisXAxis, femaleTargetToMalePenisBase) / (malePenisXAxis.magnitude * malePenisXAxis.magnitude);

                // Calculate Y(L1) for robot based on the reference Y axis and the vector from the female's vagina's labia trigger to the male's penis's base collider
                float robotL1 = 0.5f + Vector3.Dot(malePenisYAxis, femaleTargetToMalePenisBase) / (malePenisYAxis.magnitude * malePenisYAxis.magnitude);

                // Calculate Z(L2) for robot based on the reference Z axis and the vector from the female's vagina's labia trigger to the male's penis's base collider
                float robotL2 = 0.5f + Vector3.Dot(malePenisZAxis, femaleTargetToMalePenisBase) / (malePenisZAxis.magnitude * malePenisZAxis.magnitude);

                // Determine the coordinate system orientation between the male and female, used for calculating the R0 rotation
                bool coordinateAxesMatch = true;

                if (Vector3.Dot(malePenisZAxis, femaleTargetZAxis) < 0)
                {
                    coordinateAxesMatch = false;
                }

                // Calculate RX(R0) for robot based on the angle between reference Z axis and the female's vagina to anus vector
                float robotR0Angle = Vector3.Angle(malePenisZAxis, femaleTargetZAxis);

                // Calculate RY(R1) for robot based on the reference Z axis and the vector from the female's vagina's labia to vagina triggers
                float robotR1Angle = -(90.0f - Vector3.Angle(malePenisZAxis, femaleTargetXAxis));

                if (!coordinateAxesMatch)
                {
                    robotR0Angle = 180.0f - robotR0Angle;
                    robotR1Angle *= -1.0f;
                }

                float robotR0 = 0.5f + robotR0Angle / 180.0f;

                float robotR1 = 0.5f + robotR1Angle / 180.0f;

                // Calculate RZ(R2) for robot based on the reference Y axis and the vector from the female's vagina's labia to vagina triggers
                float robotR2Angle = -(90.0f - Vector3.Angle(malePenisYAxis, femaleTargetXAxis));

                float robotR2 = 0.5f + robotR2Angle / 180.0f;

                // Calculate automatic range values
                if (robotL0 >= 0.0f && robotL0 <= 1.0f)
                {
                    if (robotL0 < _autoRangeMin)
                    {
                        _autoRangeMin = robotL0;
                    }

                    if (robotL0 > _autoRangeMax)
                    {
                        _autoRangeMax = robotL0;
                    }
                }

                // Get the automatic range midpoint
                _autoRangeMid = (_autoRangeMin + _autoRangeMax) / 2.0f;

                float multiplier = (AC_SexRobotControllerPlugin.LimitRobotL0Length.Value) ? AC_SexRobotControllerPlugin.LimitRobotL0Multiplier.Value : AC_SexRobotControllerPlugin.RobotL0Multiplier.Value;

                // Caclulate modified robotL0
                robotL0 = 0.5f + (robotL0 - _autoRangeMid) * multiplier;

                // Formulate T-Code command string
                string command = "L0" + GenerateTCode(robotL0, AC_SexRobotControllerPlugin.RobotL0Min.Value, AC_SexRobotControllerPlugin.RobotL0Max.Value) + "\n";
                command += "L1" + GenerateTCode(robotL1, AC_SexRobotControllerPlugin.RobotL1Min.Value, AC_SexRobotControllerPlugin.RobotL1Max.Value) + "\n";
                command += "L2" + GenerateTCode(robotL2, AC_SexRobotControllerPlugin.RobotL2Min.Value, AC_SexRobotControllerPlugin.RobotL2Max.Value) + "\n";
                command += "R0" + GenerateTCode(robotR0, AC_SexRobotControllerPlugin.RobotR0Min.Value, AC_SexRobotControllerPlugin.RobotR0Max.Value) + "\n";
                command += "R1" + GenerateTCode(robotR1, AC_SexRobotControllerPlugin.RobotR1Min.Value, AC_SexRobotControllerPlugin.RobotR1Max.Value) + "\n";
                command += "R2" + GenerateTCode(robotR2, AC_SexRobotControllerPlugin.RobotR2Min.Value, AC_SexRobotControllerPlugin.RobotR2Max.Value);

                if (AC_SexRobotControllerPlugin.DiagnosticsConfig.Value)
                {
                    AC_SexRobotControllerPlugin.LogInfo("_malePenisBase: " + _malePenisBase.position.x.ToString() + ", " + _malePenisBase.position.y.ToString() + ", " + _malePenisBase.position.z.ToString());
                    AC_SexRobotControllerPlugin.LogInfo("_malePenisTip: " + _malePenisTip.position.x.ToString() + ", " + _malePenisTip.position.y.ToString() + ", " + _malePenisTip.position.z.ToString());
                    AC_SexRobotControllerPlugin.LogInfo("_malePenisLeftBall: " + _malePenisLeftBall.position.x.ToString() + ", " + _malePenisLeftBall.position.y.ToString() + ", " + _malePenisLeftBall.position.z.ToString());
                    AC_SexRobotControllerPlugin.LogInfo("_malePenisRightBall: " + _malePenisRightBall.position.x.ToString() + ", " + _malePenisRightBall.position.y.ToString() + ", " + _malePenisRightBall.position.z.ToString());
                    AC_SexRobotControllerPlugin.LogInfo("malePenisBallsCenterPoint: " + malePenisBallsCenterPoint.x.ToString() + ", " + malePenisBallsCenterPoint.y.ToString() + ", " + malePenisBallsCenterPoint.z.ToString());
                    AC_SexRobotControllerPlugin.LogInfo("malePenisLength: " + malePenisLength.ToString());
                    AC_SexRobotControllerPlugin.LogInfo("malePenisXAxis: " + malePenisXAxis.x.ToString() + ", " + malePenisXAxis.y.ToString() + ", " + malePenisXAxis.z.ToString());
                    AC_SexRobotControllerPlugin.LogInfo("malePenisZAxis: " + malePenisZAxis.x.ToString() + ", " + malePenisZAxis.y.ToString() + ", " + malePenisZAxis.z.ToString());
                    AC_SexRobotControllerPlugin.LogInfo("malePenisYAxis: " + malePenisYAxis.x.ToString() + ", " + malePenisYAxis.y.ToString() + ", " + malePenisYAxis.z.ToString());
                    AC_SexRobotControllerPlugin.LogInfo("Robot L0 Multiplier: " + AC_SexRobotControllerPlugin.RobotL0Multiplier.Value);
                    AC_SexRobotControllerPlugin.LogInfo("Robot L0 Multiplier (actual): " + multiplier);
                    AC_SexRobotControllerPlugin.LogInfo("animationName: " + AnimationName);
                    AC_SexRobotControllerPlugin.LogInfo("Robot L0: " + robotL0);
                    AC_SexRobotControllerPlugin.LogInfo("Robot L1: " + robotL1);
                    AC_SexRobotControllerPlugin.LogInfo("Robot L2: " + robotL2);
                    AC_SexRobotControllerPlugin.LogInfo("Robot R0: " + robotR0);
                    AC_SexRobotControllerPlugin.LogInfo("Robot R1: " + robotR1);
                    AC_SexRobotControllerPlugin.LogInfo("Robot R2: " + robotR2);
                    AC_SexRobotControllerPlugin.LogInfo("Robot R0 Angle: " + robotR0Angle);
                    AC_SexRobotControllerPlugin.LogInfo("Robot R1 Angle: " + robotR1Angle);
                    AC_SexRobotControllerPlugin.LogInfo("Robot R2 Angle: " + robotR2Angle);
                    AC_SexRobotControllerPlugin.LogInfo("T-Code Command: \n" + command);
                    AC_SexRobotControllerPlugin.LogInfo("_autoRangeMin: " + _autoRangeMin);
                    AC_SexRobotControllerPlugin.LogInfo("_autoRangeMid: " + _autoRangeMid);
                    AC_SexRobotControllerPlugin.LogInfo("_autoRangeMax: " + _autoRangeMax);
                    AC_SexRobotControllerPlugin.LogInfo("robotL0 percent: " + (((robotL0 - _autoRangeMin) / (_autoRangeMax - _autoRangeMin)) * 100.0f));
                }

                // Only update the sex robot's position/servos
                if (robotL0 >= 0.0f && robotL0 <= 1.0f)
                {
                    try
                    {
                        if (_serialPortConnection.AC_SerialPort != null)
                        {
                            // If serial port is open then send the command to the robot
                            if (_serialPortConnection.AC_SerialPort.IsOpen)
                            {
                                _serialPortConnection.AC_SerialPort.WriteLine(command);

                                if (AC_SexRobotControllerPlugin.DiagnosticsConfig.Value)
                                {
                                    AC_SexRobotControllerPlugin.LogInfo("Command value sent: " + command);
                                }
                            }
                        }
                    }
                    catch (System.Exception ex)
                    {
                        AC_SexRobotControllerPlugin.LogDebug("Error: " + ex.ToString());
                    }
                }
            }
        }

        private static string GenerateTCode(float input, float min, float max)
        {
            if (input > max) input = max;
            if (input < min) input = min;

            input *= 1000;

            string output;

            if (input >= 999f)
            {
                output = "999";
            }
            else if (input >= 1f)
            {
                output = input.ToString("000");
            }
            else
            {
                output = "000";
            }

            return output;
        }
    }
}
