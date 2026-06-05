using System.Collections.Generic;

namespace KKS_SexRobotController
{
    internal sealed class BoneAnimationDefiner
    {
        internal enum LoopSpeed
        {
            IDLE,
            INSERT_IDLE,
            SLOW,
            MEDIUM,
            FAST,
            ANAL_SLOW,
            ANAL_MEDIUM,
            ANAL_FAST,
            ORGASM,
            ABOUT_TO_CUM_INSIDE,
            CUMMING_INSIDE,
            AFTER_CUMMING_INSIDE,
            IDLE_AFTER_CUMMING_INSIDE
        }

        internal enum BodyBone
        {
            FEMALE,
            FEMALE_HEAD,
            FEMALE_THIGHTL,
            FEMALE_THIGHTR,
            FEMALE_HIPS,
            FEMALE_EARL,
            FEMALE_EARR,
            FEMALE_MOUTH,
            FEMALE_MOUTH_UPPER_LIP,
            FEMALE_MOUTH_LOWER_LIP,
            FEMALE_MOUTHL,
            FEMALE_MOUTHR,
            FEMALE_BREAST,
            FEMALE_BREASTL,
            FEMALE_BREASTR,
            FEMALE_HANDL,
            FEMALE_HAND_THUMBL,
            FEMALE_HAND_INDEXL,
            FEMALE_HAND_MIDDLEL,
            FEMALE_HAND_RINGL,
            FEMALE_HAND_LITTLEL,
            FEMALE_HANDR,
            FEMALE_HAND_THUMBR,
            FEMALE_HAND_INDEXR,
            FEMALE_HAND_MIDDLER,
            FEMALE_HAND_RINGR,
            FEMALE_HAND_LITTLER,
            FEMALE_FOOTL,
            FEMALE_FOOTR,
            FEMALE_TOESL,
            FEMALE_TOESR,
            VAGINA,
            VAGINA_ALT1,
            VAGINA_ALT2,
            ANUS,
            FEMALE_LBUTT,
            FEMALE_RBUTT,
            MALE,
            MALE_HIPS,
            MALE_THIGHTL,
            MALE_THIGHTR,
            MALE_HEAD,
            MALE_HANDL,
            MALE_HAND_THUMBL,
            MALE_HAND_INDEXL,
            MALE_HAND_MIDDLEL,
            MALE_HAND_RINGL,
            MALE_HAND_LITTLEL,
            MALE_HANDR,
            MALE_HAND_THUMBR,
            MALE_HAND_INDEXR,
            MALE_HAND_MIDDLER,
            MALE_HAND_RINGR,
            MALE_HAND_LITTLER,
            PENIS,
            PENIS_BASE,
            PENIS_TIP,
            BALLS_L,
            BALLS_R
        }

        internal enum FemaleTargetType
        {
            VAGINAL,
            ANAL,
            ORAL,
            BREASTS,
            LEFTHAND,
            RIGHTHAND,
            LEFTFOOT,
            RIGHTFOOT,
            BOTH_FEET,
            INTERCRURAL,
            VAGINALSWAP,
            ORALSWAP,
            BREASTSWAP,
            LEFTHANDSWAP,
            RIGHTHANDSWAP,
            INTERCRURALSWAP,
            LEFTFOOTSWAP,
            RIGHTFOOTSWAP
        }

        internal enum MaleTargetType
        {
            LEFTHAND,
            RIGHTHAND
        }

        // retrieved via HFlag.nowAnimStateName
        internal static readonly Dictionary<LoopSpeed, string> loopSpeedDict = new Dictionary<LoopSpeed, string> {
            {LoopSpeed.IDLE, "Idle"},
            {LoopSpeed.INSERT_IDLE, "InsertIdle"},
            {LoopSpeed.SLOW, "WLoop"},
            {LoopSpeed.MEDIUM, "SLoop"},
            {LoopSpeed.FAST, "OLoop"},
            {LoopSpeed.ANAL_SLOW, "A_WLoop"},
            {LoopSpeed.ANAL_MEDIUM, "A_SLoop"},
            {LoopSpeed.ANAL_FAST, "A_OLoop"},
            //{LoopSpeed.ORGASM, "xxx"},
            {LoopSpeed.ABOUT_TO_CUM_INSIDE, "WS_IN_Start"},
            {LoopSpeed.CUMMING_INSIDE, "WS_IN_A"},
            {LoopSpeed.AFTER_CUMMING_INSIDE, "SS_IN_A"},
            {LoopSpeed.IDLE_AFTER_CUMMING_INSIDE, "IN_A"},
        };

        internal static readonly Dictionary<BodyBone, string> bodyBoneDictionary = new Dictionary<BodyBone, string> {
            {BodyBone.FEMALE, "k_f_tamaL_00"},
            {BodyBone.FEMALE_HEAD, "cf_j_head"},
            {BodyBone.FEMALE_THIGHTL, "cf_j_thigh00_L"},
            {BodyBone.FEMALE_THIGHTR, "cf_j_thigh00_R"},
            {BodyBone.FEMALE_HIPS, "cf_j_hips"},
            {BodyBone.FEMALE_EARL, "a_n_earrings_L"},
            {BodyBone.FEMALE_EARR, "a_n_earrings_R"},
            {BodyBone.FEMALE_MOUTH, "cf_J_MouthCavity"},
            {BodyBone.FEMALE_MOUTH_UPPER_LIP, "cf_J_Mouthup"},
            {BodyBone.FEMALE_MOUTH_LOWER_LIP, "cf_J_MouthLow"},
            {BodyBone.FEMALE_MOUTHL, "cf_J_Mouth_L"},
            {BodyBone.FEMALE_MOUTHR, "cf_J_Mouth_R"},
            {BodyBone.FEMALE_BREAST, "cf_d_bust00"},
            {BodyBone.FEMALE_BREASTL, "k_f_munenipL_00"},
            {BodyBone.FEMALE_BREASTR, "k_f_munenipR_00"},
            {BodyBone.FEMALE_HANDL, "a_n_hand_L"},
            {BodyBone.FEMALE_HAND_THUMBL, "cf_j_thumb01_L"},
            {BodyBone.FEMALE_HAND_INDEXL, "cf_j_index01_L"},
            {BodyBone.FEMALE_HAND_MIDDLEL, "cf_j_middle01_L"},
            {BodyBone.FEMALE_HAND_RINGL, "cf_j_ring01_L"},
            {BodyBone.FEMALE_HAND_LITTLEL, "cf_j_little01_L"},
            {BodyBone.FEMALE_HANDR, "a_n_hand_R"},
            {BodyBone.FEMALE_HAND_THUMBR, "cf_j_thumb01_R"},
            {BodyBone.FEMALE_HAND_INDEXR, "cf_j_index01_R"},
            {BodyBone.FEMALE_HAND_MIDDLER, "cf_j_middle01_R"},
            {BodyBone.FEMALE_HAND_RINGR, "cf_j_ring01_R"},
            {BodyBone.FEMALE_HAND_LITTLER, "cf_j_little01_R"},
            {BodyBone.FEMALE_FOOTL, "cf_j_foot_L"},
            {BodyBone.FEMALE_FOOTR, "cf_j_foot_R"},
            {BodyBone.FEMALE_TOESL, "cf_j_toes_L"},
            {BodyBone.FEMALE_TOESR, "cf_j_toes_R"},
            {BodyBone.VAGINA, "cf_j_kokan"},
            {BodyBone.VAGINA_ALT1, "a_n_kokan"},
            {BodyBone.VAGINA_ALT2, "cf_n_pee"},
            {BodyBone.ANUS, "cf_j_ana"},
            {BodyBone.FEMALE_LBUTT, "k_f_siriL_00"},
            {BodyBone.FEMALE_RBUTT, "k_f_siriR_00"},
            {BodyBone.MALE, "k_f_tamaL_00"},
            {BodyBone.MALE_HEAD, "cf_j_head"},
            {BodyBone.MALE_HIPS, "cf_j_hips"},
            {BodyBone.MALE_THIGHTL, "cf_j_thigh00_L"},
            {BodyBone.MALE_THIGHTR, "cf_j_thigh00_R"},
            {BodyBone.MALE_HANDL, "a_n_hand_L"},
            {BodyBone.MALE_HAND_THUMBL, "cf_j_thumb01_L"},
            {BodyBone.MALE_HAND_INDEXL, "cf_j_index01_L"},
            {BodyBone.MALE_HAND_MIDDLEL, "cf_j_middle01_L"},
            {BodyBone.MALE_HAND_RINGL, "cf_j_ring01_L"},
            {BodyBone.MALE_HAND_LITTLEL, "cf_j_little01_L"},
            {BodyBone.MALE_HANDR, "a_n_hand_R"},
            {BodyBone.MALE_HAND_THUMBR, "cf_j_thumb01_R"},
            {BodyBone.MALE_HAND_INDEXR, "cf_j_index01_R"},
            {BodyBone.MALE_HAND_MIDDLER, "cf_j_middle01_R"},
            {BodyBone.MALE_HAND_RINGR, "cf_j_ring01_R"},
            {BodyBone.MALE_HAND_LITTLER, "cf_j_little01_R"},
            {BodyBone.PENIS, "a_n_dan"},
            {BodyBone.PENIS_BASE, "cm_J_dan100_00"},
            {BodyBone.PENIS_TIP, "cm_J_dan109_00"},
            {BodyBone.BALLS_L, "cm_J_dan_f_L"},
            {BodyBone.BALLS_R, "cm_J_dan_f_R"}
        };

        internal static Dictionary<string, FemaleTargetType> animationFemaleTargetDictionary = new Dictionary<string, FemaleTargetType>
        {
            // Schema: {ANIMATION NAME, FEMALE TARGET(S) TO USE IN REGARD TO MAPPING TO THE MALE'S PENIS TARGET}

            // Koikatsu Service HScene Category
            //handjob
            //{"HANDJOB", femaleTargetType.LEFTHAND},
            {"片手コキ", FemaleTargetType.RIGHTHAND},
            {"椅子片手コキ", FemaleTargetType.LEFTHAND},
            {"両手コキ", FemaleTargetType.LEFTHAND},
            {"椅子亀頭いじり", FemaleTargetType.RIGHTHAND},
            {"椅子乳首舐め手コキ", FemaleTargetType.LEFTHAND},
            {"立ち片手コキ", FemaleTargetType.RIGHTHAND},
            {"立ち亀頭いじり", FemaleTargetType.RIGHTHAND},
            {"立ち先キス+竿キス手コキ", FemaleTargetType.LEFTHAND},
            {"On Table Handjob", FemaleTargetType.LEFTHAND},
            //crowded handjob
            {"密着手コキ", FemaleTargetType.LEFTHAND},
            //blowjob
            //{"ORAL", femaleTargetType.ORAL},
            {"Forced Deepthroat 2", FemaleTargetType.ORAL},
            {"両手フェラ", FemaleTargetType.ORAL},
            {"Lying Mouth Fuck", FemaleTargetType.ORAL},
            {"Sucking Forced", FemaleTargetType.ORAL},
            {"先舐め＋竿舐め", FemaleTargetType.ORAL},
            {"Lying on Table Blowjob", FemaleTargetType.ORAL},
            {"[Horizon]", FemaleTargetType.ORAL},
            {"[Loop-Deformed-Halloween]", FemaleTargetType.ORAL},
            {"[Reverse lift blowjob]", FemaleTargetType.ORAL},
            {"片手フェラ", FemaleTargetType.ORAL},
            {"Lying, Foot & Blowjob", FemaleTargetType.ORAL},
            {"椅子両手フェラ", FemaleTargetType.ORAL},
            {"椅子先舐め＋竿舐め", FemaleTargetType.ORAL},
            {"立ち片手フェラ", FemaleTargetType.ORAL},
            {"立ち玉舐め手コキ", FemaleTargetType.ORAL},
            {"Sucking Sitting Girl", FemaleTargetType.ORAL},
            {"69", FemaleTargetType.ORAL},
            {"Deepthroat", FemaleTargetType.ORAL},
            {"Forced Deepthroat", FemaleTargetType.ORAL},
            {"Forced Deepthroat Sitting", FemaleTargetType.ORAL},
            {"Kneeling Deepthroat", FemaleTargetType.ORAL},
            {"ちんぐり返しフェラ", FemaleTargetType.ORAL},
            {"椅子ノーハンド先舐め", FemaleTargetType.ORAL},
            {"立ち両手フェラ", FemaleTargetType.ORAL},
            {"Stand 69", FemaleTargetType.ORAL},
            {"立ちノーハンドフェラ", FemaleTargetType.ORAL},
            {"Ying Yang", FemaleTargetType.ORAL},
            {"Desk Leaning Deepthroat", FemaleTargetType.ORAL},
            {"Forced Deepthroat Desk", FemaleTargetType.ORAL},
            {"Kneeling on Table Blowjob", FemaleTargetType.ORAL},
            //bench blowjob
            {"ベンチフェラ", FemaleTargetType.ORAL},
            //periscope bj
            {"潜望鏡フェラ", FemaleTargetType.ORAL},
            //straddle bench BJ
            {"椅子またがりフェラ", FemaleTargetType.ORAL},
            // volleyball net BJ
            {"ネットフェラ", FemaleTargetType.ORAL},

            //titjob
            //{"TITJOB", femaleTargetType.BREASTS},
            {"Pressed Titjob 2", FemaleTargetType.BREASTS},
            {"椅子パイズリ", FemaleTargetType.BREASTS},
            {"パイズリ", FemaleTargetType.BREASTS},
            {"立ちパイズリ", FemaleTargetType.BREASTS},
            {"Desk Grabbed Boobs Fuck", FemaleTargetType.BREASTS},
            {"Grabbed Boobs Fuck", FemaleTargetType.BREASTS},
            {"Lying Grabbed Boobs Fuck", FemaleTargetType.BREASTS},
            {"椅子パイズリ+舐め", FemaleTargetType.BREASTS},
            {"椅子乳首舐めパイズリ", FemaleTargetType.BREASTS},
            {"腕はさみパイズリ", FemaleTargetType.BREASTS},
            {"押し付けパイズリ", FemaleTargetType.BREASTS},
            {"椅子腕はさみパイズリ", FemaleTargetType.BREASTS},
            {"椅子パイズリ+咥え", FemaleTargetType.BREASTS},
            {"立ち腕はさみパイズリ", FemaleTargetType.BREASTS},
            {"立ちパイズリ+咥え", FemaleTargetType.BREASTS},
            {"パイズリ+咥え", FemaleTargetType.BREASTS},
            {"プールパイズリ", FemaleTargetType.BREASTS},
            
            // Koikatsu Insert HScene Category
            //vaginal
            //{"VAGINAL", femaleTargetType.VAGINAL},
            {"[Cowgirl leaning]", FemaleTargetType.VAGINAL},
            {"[Leg Hold Against Wall]", FemaleTargetType.VAGINAL},
            {"[Reverse Cowgirl]", FemaleTargetType.VAGINAL},
            {"[Standing behind-Ghost]", FemaleTargetType.VAGINAL},
            {"[Standing Forward Bending]", FemaleTargetType.VAGINAL},
            {"[V-shell]", FemaleTargetType.VAGINAL},
            {"Amazon", FemaleTargetType.VAGINAL},
            {"Arm-Grab Doggy", FemaleTargetType.VAGINAL},
            {"Arm-Grab Doggystyle Against Desk", FemaleTargetType.VAGINAL},
            {"Arm-Grab Doggystyle Against Seat", FemaleTargetType.VAGINAL},
            {"Arms Locked Doggystyle", FemaleTargetType.VAGINAL},
            {"B. Doggy (Arm Grab)", FemaleTargetType.VAGINAL},
            {"Bed-Grabbing Doggystyle 2", FemaleTargetType.VAGINAL},
            {"Bench Doggystyle", FemaleTargetType.VAGINAL},
            {"Bench Rev. Cowgirl", FemaleTargetType.VAGINAL},
            {"Bowing Doggystyle 2", FemaleTargetType.VAGINAL},
            {"Bridging Missionary 2", FemaleTargetType.VAGINAL},
            {"ButtChair facing", FemaleTargetType.VAGINAL},
            {"ButtChair Rev. Cowgirl", FemaleTargetType.VAGINAL},
            {"Chair Doggy", FemaleTargetType.VAGINAL},
            {"Chair Legs Spread Missionary", FemaleTargetType.VAGINAL},
            {"Cowgirl", FemaleTargetType.VAGINAL},
            {"Cowgirl Hands on Knees", FemaleTargetType.VAGINAL},
            {"Cowgirl Hug", FemaleTargetType.VAGINAL},
            {"Cowgirl Nipple Torture 2", FemaleTargetType.VAGINAL},
            {"Cowgirl Restrain", FemaleTargetType.VAGINAL},
            {"Desk Against the Table", FemaleTargetType.VAGINAL},
            {"Desk Doggy 2", FemaleTargetType.VAGINAL},
            {"Desk Facing the Girl", FemaleTargetType.VAGINAL},
            {"Desk Lotus", FemaleTargetType.VAGINAL},
            {"Desk Lying Legs Spread", FemaleTargetType.VAGINAL},
            {"Desk Lying Legs Up", FemaleTargetType.VAGINAL},
            {"Doggy", FemaleTargetType.VAGINAL},
            {"Doggy On Table", FemaleTargetType.VAGINAL},
            {"Doggy Standing Arm-Grab", FemaleTargetType.VAGINAL},
            {"Doggy Straddle", FemaleTargetType.VAGINAL},
            {"Doggystyle Against Chair", FemaleTargetType.VAGINAL},
            {"Doggystyle Against Desk", FemaleTargetType.VAGINAL},
            {"Doggystyle Against Fence", FemaleTargetType.VAGINAL},
            {"Doggystyle Against Wall", FemaleTargetType.VAGINAL},
            {"Doggystyle", FemaleTargetType.VAGINAL},
            {"Doggystyle Hair-Pull Forced", FemaleTargetType.VAGINAL},
            {"Doggystyle In Pool", FemaleTargetType.VAGINAL},
            {"Extreme Piledriver", FemaleTargetType.VAGINAL},
            {"Facing Away On Bed 2", FemaleTargetType.VAGINAL},
            {"Facing On Bed 2", FemaleTargetType.VAGINAL},
            {"Facing on Bed Stradle Legs On Shoulders", FemaleTargetType.VAGINAL},
            {"From Behind in Seiza 2", FemaleTargetType.VAGINAL},
            {"Hand-Holding Cowgirl 2", FemaleTargetType.VAGINAL},
            {"Hand-Holding Cowgirl 3", FemaleTargetType.VAGINAL},
            {"Hip-Holding Missionary 2", FemaleTargetType.VAGINAL},
            {"Holding legs behind doggy style", FemaleTargetType.VAGINAL},
            {"Holding Legs Missionary", FemaleTargetType.VAGINAL},
            {"Hold-Knees Spread Missionary", FemaleTargetType.VAGINAL},
            {"Knee-Holding Missionary 2", FemaleTargetType.VAGINAL},
            {"Leg Held Doggytyle 2", FemaleTargetType.VAGINAL},
            {"Leg Held Missionary 2", FemaleTargetType.VAGINAL},
            {"Leg Held Missionary 3", FemaleTargetType.VAGINAL},
            {"Leg Held Missionary Leg Lock", FemaleTargetType.VAGINAL},
            {"Leg Lifted Doggystyle Against Wall", FemaleTargetType.VAGINAL},
            {"Legs on Shoulders", FemaleTargetType.VAGINAL},
            {"Legs Spread Missionary 2", FemaleTargetType.VAGINAL},
            {"Lifted Missionary", FemaleTargetType.VAGINAL},
            {"Lifting Nelson", FemaleTargetType.VAGINAL},
            {"Lotus", FemaleTargetType.VAGINAL},
            {"Lying Doggystyle", FemaleTargetType.VAGINAL},
            {"Missionary 2", FemaleTargetType.VAGINAL},
            {"Missionary", FemaleTargetType.VAGINAL},
            {"Missionary Holding Hands", FemaleTargetType.VAGINAL},
            {"Missionary Interlock", FemaleTargetType.VAGINAL},
            {"NTR Forced missionary", FemaleTargetType.VAGINAL},
            {"Piledriver", FemaleTargetType.VAGINAL},
            {"Piledriver Missionary", FemaleTargetType.VAGINAL},
            {"Pool doggy", FemaleTargetType.VAGINAL},
            {"Princess Hug Side Position 2", FemaleTargetType.VAGINAL},
            {"Reverse Amazon", FemaleTargetType.VAGINAL},
            {"Reverse Cowgirl 3", FemaleTargetType.VAGINAL},
            {"Reverse Cowgirl 4", FemaleTargetType.VAGINAL},
            {"Reverse Cowgirl 5", FemaleTargetType.VAGINAL},
            {"Reverse Cowgirl Bridge", FemaleTargetType.VAGINAL},
            {"Reverse Cowgirl Nelson", FemaleTargetType.VAGINAL},
            {"Reverse Cowgirl Split", FemaleTargetType.VAGINAL},
            {"Reverse Piledriver Piston", FemaleTargetType.VAGINAL},
            {"Reverse Spooning 2", FemaleTargetType.VAGINAL},
            {"Reverse Spooning 3", FemaleTargetType.VAGINAL},
            {"Seated Squats", FemaleTargetType.VAGINAL},
            {"Sitting Behind Four Legs", FemaleTargetType.VAGINAL},
            {"Sitting Behind Squat", FemaleTargetType.VAGINAL},
            {"Sitting Cowgirl Straddling", FemaleTargetType.VAGINAL},
            {"Sofa Cowgirl", FemaleTargetType.VAGINAL},
            {"Spread Eagle", FemaleTargetType.VAGINAL},
            {"Standing Arm-Grab", FemaleTargetType.VAGINAL},
            {"Standing Back Carry", FemaleTargetType.VAGINAL},
            {"Standing Doggystyle", FemaleTargetType.VAGINAL},
            {"Standing From Behind 2", FemaleTargetType.VAGINAL},
            {"Trust Back Cuddle", FemaleTargetType.VAGINAL},
            {"Trust Back Grabbing Arms", FemaleTargetType.VAGINAL},
            {"Vault doggy", FemaleTargetType.VAGINAL},
            {"Vaulting Horse Doggystyle", FemaleTargetType.VAGINAL},
            {"Wall Facing Leg Lifted", FemaleTargetType.VAGINAL},
            {"Wall Kneeling Doggystyle", FemaleTargetType.VAGINAL},
            {"Wall Riding", FemaleTargetType.VAGINAL},
            {"Wall Standing Split", FemaleTargetType.VAGINAL},
            //Pool Back
            {"プールバック", FemaleTargetType.VAGINAL},
            //Wall Back
            {"壁バック", FemaleTargetType.VAGINAL},
            //Floor Hand-Supported Back
            {"床手付きバック", FemaleTargetType.VAGINAL},
            //Doggy Style
            {"後背位", FemaleTargetType.VAGINAL},
            //Desk Back
            {"机バック", FemaleTargetType.VAGINAL},
            //Chair Back
            {"椅子バック", FemaleTargetType.VAGINAL},
            //Missionary
            {"正常位", FemaleTargetType.VAGINAL},
            //Arm-Pull Doggy Style
            {"腕引っ張り後背位", FemaleTargetType.VAGINAL},
            //Cowgirl
            {"騎乗位", FemaleTargetType.VAGINAL},
            //Manguri Missionary 
            {"マングリ正常位", FemaleTargetType.VAGINAL},
            //Wall-Facing One-Leg-Up
            {"壁対面片足上げ", FemaleTargetType.VAGINAL},
            //Doggystyle with legs
            {"足抱え後背位", FemaleTargetType.VAGINAL},
            //Floor-Facing Sitting Position
            {"床対面座位", FemaleTargetType.VAGINAL},
            //Waist-Hugging Missionary 
            {"腰抱え正常位", FemaleTargetType.VAGINAL},
            //Nipple Torture Cowgirl
            {"乳首責め騎乗位", FemaleTargetType.VAGINAL},
            //Pounding Piston
            {"杭打ちピストン", FemaleTargetType.VAGINAL},
            //Reverse Cowgirl”
            {"背面騎乗位", FemaleTargetType.VAGINAL},
            //Princess Carry Side 
            {"お姫様抱っこ側位", FemaleTargetType.VAGINAL},
            //Standing
            {"立位", FemaleTargetType.VAGINAL},
            //Standing Doggy Style
            {"立ちバック", FemaleTargetType.VAGINAL},
            //Bridge Missionary
            {"ブリッジ正常位", FemaleTargetType.VAGINAL},
            //Desk Doggy Style with Arms Pulled Back
            {"腕引っ張り机バック", FemaleTargetType.VAGINAL},
            //Desk Side             
            {"机側位", FemaleTargetType.VAGINAL},
            //Seiza Reverse Sitting 
            {"正座背面座位", FemaleTargetType.VAGINAL},
            //Hand-Holding Cowgirl
            {"手つなぎ騎乗位", FemaleTargetType.VAGINAL},
            //Knee-Hugging Missionary
            {"膝抱え正常位", FemaleTargetType.VAGINAL},
            //Spread-Leg Missionary
            {"開脚正常位", FemaleTargetType.VAGINAL},
            //On Desk
            {"机寝位", FemaleTargetType.VAGINAL},
            //Back/Rear (Doggy) Pounding Piston
            {"背面杭打ちピストン", FemaleTargetType.VAGINAL},
            //Back/Rear (Doggy) Side 
            {"背面側位", FemaleTargetType.VAGINAL},
            //Seated Facing
            {"椅子対面", FemaleTargetType.VAGINAL},
            //Prone Doggy Style
            {"伏せ後背位", FemaleTargetType.VAGINAL},
            //Arm Pull Chair Doggy
            {"腕引っ張り椅子バック", FemaleTargetType.VAGINAL},
            //Floor Backward Sitting
            {"床背面座位", FemaleTargetType.VAGINAL},
            //Kneeling Backward Sitting
            {"膝立て背面座位", FemaleTargetType.VAGINAL},
            //One Leg Up Wall Doggy
            {"片足上げ壁バック", FemaleTargetType.VAGINAL},
            //Spread Legs Missionary
            {"大股開き正常位", FemaleTargetType.VAGINAL},
            //Eki-ben
            {"駅弁", FemaleTargetType.VAGINAL},
            //Lying/Sleeping Back (Doggy?)
            {"寝バック", FemaleTargetType.VAGINAL},
            //Mating Press
            {"種付けプレス", FemaleTargetType.VAGINAL},
            //Chair Backward
            {"椅子背面", FemaleTargetType.VAGINAL},
            //Leg-Hanging Face-to-Face Sitting Position
            {"足掛け対面座位", FemaleTargetType.VAGINAL},
            //Side Position
            {"側位", FemaleTargetType.VAGINAL},
            // banana boat cowgirl, doggy
            {"バナナボート騎乗位", FemaleTargetType.VAGINAL},
            {"バナナボート後背位", FemaleTargetType.VAGINAL},
            // tennis table
            {"卓球台バック", FemaleTargetType.VAGINAL},
            {"卓球台正常位", FemaleTargetType.VAGINAL},
            // sofa cowgirl
            {"ソファ騎乗位", FemaleTargetType.VAGINAL},
            // box doggy
            {"箱バック", FemaleTargetType.VAGINAL},
            //pressed from behind
            {"押し付けバック", FemaleTargetType.VAGINAL},
            //beach ball normal
            {"ビーチボール正常位", FemaleTargetType.VAGINAL},
            //floating doggy
            {"浮き輪後背位", FemaleTargetType.VAGINAL},
            // fence doggy
            {"フェンス後背位", FemaleTargetType.VAGINAL},
            // volleyball net doggystyle
            {"ネット後背位", FemaleTargetType.VAGINAL},
            
            //anal
            //{"ANAL", femaleTargetType.ANAL},
            {"Cowgirl Restrain Anal", FemaleTargetType.ANAL},

            //intercrucial
            //{"INTERCRURAL", femaleTargetType.INTERCRURAL},
            {"Back Thigh Job", FemaleTargetType.INTERCRURAL},
            {"Lying Cowgirl Ass Hump", FemaleTargetType.INTERCRURAL},
            {"Sit, Straddle Thigh Job", FemaleTargetType.INTERCRURAL},
            {"Desk, Back Ass Hump", FemaleTargetType.INTERCRURAL},
            //not implemented, placeholder
            {"[Armpitjob]", FemaleTargetType.INTERCRURAL},
            {"Lying, Massage with knee", FemaleTargetType.INTERCRURAL},

            //footjob
            //{"FOOTJOB", femaleTargetType.LEFTFOOT},
            {"[Footjob sitting]", FemaleTargetType.LEFTFOOT},
            {"Cinderella", FemaleTargetType.LEFTFOOT},
            {"Piledriver Massage", FemaleTargetType.RIGHTFOOT},
            {"Sitting Penis Massage", FemaleTargetType.LEFTFOOT},
            {"Under Pressure", FemaleTargetType.RIGHTFOOT},
            {"立ち足コキ", FemaleTargetType.LEFTFOOT},
            //poses using both feet, set to use left foot for now
            {"Back Cuddle", FemaleTargetType.LEFTFOOT},
            {"Desk Bending Over Footjob", FemaleTargetType.LEFTFOOT},
            {"Desk Penis Massage", FemaleTargetType.LEFTFOOT},
            {"Desk, Footjob", FemaleTargetType.LEFTFOOT},
            {"Laying Belly Footjob", FemaleTargetType.LEFTFOOT},
            {"床足コキ", FemaleTargetType.LEFTFOOT},
            {"Lying Massage with 2 feet", FemaleTargetType.LEFTFOOT},
            {"Lying Rim & Foot Jobs", FemaleTargetType.LEFTFOOT},
            {"Lying Rimjob 2", FemaleTargetType.LEFTFOOT},
            {"椅子足コキ", FemaleTargetType.LEFTFOOT},
            {"Sitting Footjob 2", FemaleTargetType.LEFTFOOT},

            // 3P - none of these currently implemented
            // 3P - 2 girls, 1 guy - HJ
            //{"GIRL1", femaleTargetType.LEFTHAND},
            //{"GIRL2", femaleTargetType.LEFTHANDSWAP},
            //footjob & HJ
            {"足コキ＆手コキ", FemaleTargetType.LEFTHAND},
            {"足コキ＆手コキ入れ替え", FemaleTargetType.LEFTHANDSWAP},
            //nipple lick HJ & BJ
            {"乳首舐め手コキ＆フェラ", FemaleTargetType.LEFTHAND},
            {"乳首舐め手コキ＆フェラ入れ替え", FemaleTargetType.LEFTHANDSWAP},

            // 3P - 2 girls, 1 guy - BJ
            //{"GIRL1", femaleTargetType.ORAL},
            //{"GIRL2", femaleTargetType.ORALSWAP},
            //double fellatio
            {"Wフェラ", FemaleTargetType.ORAL},
            {"Wフェラ入れ替え", FemaleTargetType.ORALSWAP},
            {"Fellatio & Nip Licking", FemaleTargetType.ORAL},
            {"Fellatio & Nip Licking (Switch Girl)", FemaleTargetType.ORALSWAP},
            //Piledriver ball licking & BJ
            {"ちんぐり玉舐め＆フェラ", FemaleTargetType.ORAL},
            {"ちんぐり玉舐め＆フェラ入れ替え", FemaleTargetType.ORALSWAP},
            //Sitting double fellatio
            {"座りWフェラ", FemaleTargetType.ORAL},
            {"座りWフェラ入れ替え", FemaleTargetType.ORALSWAP},

            // 3P - 2 girls, 1 guy - TJ
            //{"GIRL1", femaleTargetType.BREASTS},
            //{"GIRL2", femaleTargetType.BREASTSWAP},

            // 3P - 2 girls, 1 guy - FJ
            //{"GIRL1", femaleTargetType.LEFTFOOT},
            //{"GIRL2", femaleTargetType.LEFTFOOTSWAP},

            // 3P - 2 girls, 1 guy - intercrural
            //{"GIRL1", femaleTargetType.INTERCRURAL},
            //{"GIRL2", femaleTargetType.INTERCRURALSWAP},

            // 3P - 2 girls, 1 guy - Insert
            //{"GIRL1", femaleTargetType.VAGINAL},
            //{"GIRL2", femaleTargetType.VAGINALSWAP},
            //cowgirl & cunnilingus
            {"騎乗位クンニ", FemaleTargetType.VAGINAL},
            {"騎乗位クンニ入れ替え", FemaleTargetType.VAGINALSWAP},
            {"Doggy & Cunnilingus", FemaleTargetType.VAGINAL},
            {"Doggy & Cunnilingus (Switch Girl)", FemaleTargetType.VAGINALSWAP},
            //doggy & fingering
            {"後背位手マン", FemaleTargetType.VAGINAL},
            {"後背位手マン入れ替え", FemaleTargetType.VAGINALSWAP},
            //missionary & fingering
            {"正常位手マン", FemaleTargetType.VAGINAL},
            {"正常位手マン入れ替え", FemaleTargetType.VAGINALSWAP},
            {"Missionary & Fingering 2", FemaleTargetType.VAGINAL},
            {"Missionary & Fingering 2 (Switch Girl)", FemaleTargetType.VAGINALSWAP},
            //reverse cowgirl & fingering
            {"背面騎乗位手マン", FemaleTargetType.VAGINAL},
            {"背面騎乗位手マン入れ替え", FemaleTargetType.VAGINALSWAP},
            //reverse sitting & cunnilingus
            {"背面座位クンニ", FemaleTargetType.VAGINAL},
            {"背面座位クンニ入れ替え", FemaleTargetType.VAGINALSWAP}
        };

        internal static readonly Dictionary<string, MaleTargetType> animationMaleTargetDictionary = new Dictionary<string, MaleTargetType>
        {
            //male hand (masturbation) - not implemented
            {"Standing Cunni & Masturbation", MaleTargetType.RIGHTHAND}
        };

    }
}
