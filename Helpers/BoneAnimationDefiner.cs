using System.Collections.Generic;

namespace AC_SexRobotController.Helpers
{
    internal sealed class BoneAnimationDefiner
    {
        //Based on the CtrlFlag (FlagControl): LoopType
        internal enum LoopType
        {
            IDLE = -1, // before insertion, during/after orgasm
            SLOW = 0, // speed after insertion, available until finish (Speed: 0 -> 1)
            MEDIUM = 1, // speed after speed increased from SLOW, available until finish (Speed: 1 -> 2)
            FAST = 2 // finish (orgasm possible; Speed: 0 -> 1)
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
            PENIS_MID,
            PENIS_TIP,
            BALLS_L,
            BALLS_R
        }

        internal enum FemaleTargetType
        {
            ORAL,
            BREASTS,
            LEFTHAND,
            RIGHTHAND,
            BOTH_HANDS,
            INTERCRURAL_HIP,
            VAGINAL,
            ANAL,
            LEFTFOOT,
            RIGHTFOOT,
            BOTH_FEET,
            ORALSWAP,
            BREASTSWAP,
            LEFTHANDSWAP,
            RIGHTHANDSWAP,
            INTERCRURAL_HIP_SWAP,
            VAGINALSWAP,
            ANALSWAP,
            LEFTFOOTSWAP,
            RIGHTFOOTSWAP
        }

        internal enum MaleTargetType
        {
            LEFTHAND,
            RIGHTHAND
        }

        internal static readonly Dictionary<BodyBone, string> bodyBoneDictionary = new()
        {
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
            {BodyBone.PENIS_BASE, "cf_j_dan100_00"},
            {BodyBone.PENIS_MID, "cf_j_dan101_00"},
            {BodyBone.PENIS_TIP, "cf_j_dan109_00"},
            {BodyBone.BALLS_L, "cf_j_dan_f_L"},
            {BodyBone.BALLS_R, "cf_j_dan_f_R"}
        };
        /**
         * TODO: Some of these doesn't properly animate, therefore commented out for now
         * Need to check later on whether or not I can match the palm, tongue, etc. or just ignore them
         */
        internal static readonly Dictionary<string, FemaleTargetType> animationFemaleTargetDictionary = new()
        {
            // Schema: {ANIMATION NAME, FEMALE TARGET(S) TO USE IN REGARD TO MAPPING TO THE MALE'S PENIS TARGET}

            // Aicomi Service HScene Category
            //handjob
            //{"HANDJOB", FemaleTargetType.LEFTHAND}, 
            {"嫌がり手コキ", FemaleTargetType.LEFTHAND}, // Handjob
            {"手コキ", FemaleTargetType.LEFTHAND}, //Handjob
            {"無理矢理手コキ", FemaleTargetType.RIGHTHAND}, //Forced Handjob
            //{"亀頭弄り", FemaleTargetType.RIGHTHAND}, //Glans play
            //{"立ち手コキ", FemaleTargetType.RIGHTHAND}, //Standing handjob
            {"横向き手コキ", FemaleTargetType.RIGHTHAND}, //Side-facing handjob
            {"椅子手コキ", FemaleTargetType.RIGHTHAND}, //Chair handjob
            {"背面手コキ", FemaleTargetType.RIGHTHAND}, //Reverse handjob
            {"開脚手コキ", FemaleTargetType.LEFTHAND}, //Spread-leg handjob
            {"逆さ手コキ", FemaleTargetType.RIGHTHAND}, //Reverse handjob
            {"脱力手コキ", FemaleTargetType.RIGHTHAND}, //weak handjob

            //blowjob
            //{"ORAL", FemaleTargetType.ORAL},
            {"フェラ", FemaleTargetType.ORAL}, //Fellatio/Blowjob
            //{"ノーハンド先舐め", FemaleTargetType.ORAL}, //Licking Penis Tip / No-hands tip lick
            {"立ちノーハンドフェラ", FemaleTargetType.ORAL}, //Standing Blowjob
            {"椅子ノーハンドフェラ", FemaleTargetType.ORAL}, //Sitting No-Hand Blowjob / No-hands blowjob on chair
            {"ディープスロート", FemaleTargetType.ORAL}, //Deepthroat
            {"乳首舐め手コキ", FemaleTargetType.ORAL}, //Nipple Licking Fellatio
            {"嫌がりフェラ", FemaleTargetType.ORAL}, //Forced Fellatio
            {"立ちのノーハンドフェラ", FemaleTargetType.ORAL}, //Standing No-Hand Blowjob
            {"無理矢理フェラ", FemaleTargetType.ORAL}, //Forced Blowjob
            {"しゃがみフェラ", FemaleTargetType.ORAL}, //Crouching blowjob
            {"ソファーフェラ", FemaleTargetType.ORAL}, //Sofa blowjob
            {"机下フェラ", FemaleTargetType.ORAL}, //Under-desk blowjob
            {"フェラオナニー", FemaleTargetType.ORAL}, //Blowjob jerk-off
            {"壁穴フェラ", FemaleTargetType.ORAL}, //Ground-hole blowjob
            {"男拘束フェラ", FemaleTargetType.ORAL}, //Man restrained blowjob
            {"脱力フェラ", FemaleTargetType.ORAL}, //Weak blowjob
            {"触手フェラ", FemaleTargetType.ORAL}, //Tentacle blowjob
            {"路地裏フェラ", FemaleTargetType.ORAL}, //Back Alley Blowjob
            {"ゃがみフェラ", FemaleTargetType.ORAL}, //Crouching blowjob
            {"ギロチンフェラ", FemaleTargetType.ORAL}, //Guillotine blowjob
            {"ダブルピースフェラ", FemaleTargetType.ORAL}, //Double peace blowjob
            {"ノーハンドフェラ", FemaleTargetType.ORAL}, //No-hands blowjob

            //titjob
            //{"TITJOB", FemaleTargetType.BREASTS},
            {"パイズリ", FemaleTargetType.BREASTS}, //Boobjob / Titty fuck
            {"パイズリ舐め", FemaleTargetType.BREASTS}, //Licking Boobjob / Titty fuck + Licking
            {"パイズリ咥え", FemaleTargetType.BREASTS}, //Sucking Boobjob / Titty fuck + Mouth
            {"立ちパイズリ", FemaleTargetType.BREASTS}, //Standing Boobjob / Standing titjob
            {"立ちパイズリ舐め", FemaleTargetType.BREASTS}, //Standing Licking Boobjob / Standing titjob + lick
            {"椅子パイズリ", FemaleTargetType.BREASTS}, // Sitting Boobjob / Chair titfuck
            {"椅子パイズリ舐め", FemaleTargetType.BREASTS}, //Sitting Licking Boobjob / Chair titfuck + lick
            {"寝パイズリ", FemaleTargetType.BREASTS}, //Sleeping titfuck

            // Aicomi Insert HScene Category
            //vaginal
            //{"VAGINAL", FemaleTargetType.VAGINAL},
            {"正常位", FemaleTargetType.VAGINAL}, //missionary
            {"壁立ちバック", FemaleTargetType.VAGINAL}, //Against Wall Behind / Wall-standing doggy
            {"後背位", FemaleTargetType.VAGINAL}, //Doggy Style
            {"中腰バック", FemaleTargetType.VAGINAL}, //Kneeling Behind / Squatting doggy
            {"側位", FemaleTargetType.VAGINAL}, //Spooning
            {"騎乗位", FemaleTargetType.VAGINAL}, //Cowgirl
            {"胸もみ騎乗位", FemaleTargetType.VAGINAL}, //Breast massage cowgirl
            {"屈脚位", FemaleTargetType.VAGINAL}, //Bent Missionary
            {"手つなぎ正常位", FemaleTargetType.VAGINAL},//Hand holding missionary
            {"立位", FemaleTargetType.VAGINAL}, //Standing
            {"胸モミ正常位", FemaleTargetType.VAGINAL}, //Missionary Groping
            {"胸モミ騎乗", FemaleTargetType.VAGINAL}, //Cowgirl Groping
            {"床拘束正常位", FemaleTargetType.VAGINAL}, //Floor Bondage Missionary / Restrained missionary
            {"床拘束正常位２", FemaleTargetType.VAGINAL}, //Floor Bondage Missionary 2
            {"嫌がり正常位", FemaleTargetType.VAGINAL}, //Forced Missionary
            {"立ちバック", FemaleTargetType.VAGINAL}, // Standing Behind / Standing doggy
            {"突き上げバック", FemaleTargetType.VAGINAL}, //Thrusting doggy
            {"駅弁", FemaleTargetType.VAGINAL}, // Lifting
            {"椅子背面座位", FemaleTargetType.VAGINAL}, //Chair rear-facing sitting
            {"しゃがみバック", FemaleTargetType.VAGINAL}, //Crouching doggy
            {"ソファー側位", FemaleTargetType.VAGINAL}, //Sofa side
            {"ソファー騎乗位", FemaleTargetType.VAGINAL}, //Sofa cowgirl
            {"寝バック", FemaleTargetType.VAGINAL}, //Sleeping doggy
            {"対面座位", FemaleTargetType.VAGINAL}, //Face-to-face position
            {"椅子胸もみ背面座位", FemaleTargetType.VAGINAL}, //Chair breast massage rear-facing sitting
            {"壁押し付けバック", FemaleTargetType.VAGINAL}, //Wall-pressed doggy
            {"立ち松葉", FemaleTargetType.VAGINAL}, //Standing pine-needle
            {"しゃちほこ素股", FemaleTargetType.VAGINAL}, //Shachihoko sumata
            {"キャノンボール", FemaleTargetType.VAGINAL}, //Cannonball
            {"スパイダー騎乗位", FemaleTargetType.VAGINAL}, //Spider cowgirl
            {"足掛けバック", FemaleTargetType.VAGINAL}, //Legs-hooked doggy
            {"胸もみ正常位", FemaleTargetType.VAGINAL}, //Breast massage missionary
            {"背面騎乗位", FemaleTargetType.VAGINAL}, //Reverse cowgirl
            {"カウンターバック", FemaleTargetType.VAGINAL}, //Counter doggy
            {"ギロチンバック", FemaleTargetType.VAGINAL}, //Guillotine doggy
            {"バスタブ後背位", FemaleTargetType.VAGINAL}, //Bathtub doggy
            {"便座かがみバック", FemaleTargetType.VAGINAL}, //Toilet seat doggy
            {"壁穴後背位", FemaleTargetType.VAGINAL}, //Ground-hole doggy
            {"机後背位", FemaleTargetType.VAGINAL}, //Doggy style at desk
            {"水中バック", FemaleTargetType.VAGINAL}, //Underwater doggy
            {"縄拘束バック", FemaleTargetType.VAGINAL}, //Rope restraint doggy
            {"触手バック", FemaleTargetType.VAGINAL}, //Tentacle doggy
            {"階段後背位", FemaleTargetType.VAGINAL}, //Doggy on stairs
            {"チングリ背面騎乗位", FemaleTargetType.VAGINAL}, //Thigh reverse cowgirl
            {"トイレ背面座位", FemaleTargetType.VAGINAL}, //Toilet reverse cowgirl
            {"バスタブ背面座位", FemaleTargetType.VAGINAL}, //Bathtub reverse cowgirl
            {"ベンチ騎乗位", FemaleTargetType.VAGINAL}, //Bench cowgirl
            {"本棚騎乗位", FemaleTargetType.VAGINAL}, //Bookshelf cowgirl
            {"背面座位", FemaleTargetType.VAGINAL}, //Reverse cowgirl
            {"ベンチ正常位", FemaleTargetType.VAGINAL}, //Bench missionary
            {"マングリ正常位", FemaleTargetType.VAGINAL}, //Spread-leg missionary
            {"卓上正常位", FemaleTargetType.VAGINAL}, //Table missionary
            {"吊るし正常位", FemaleTargetType.VAGINAL}, //Hanging missionary
            {"浮き輪正常位", FemaleTargetType.VAGINAL}, //Swim ring missionary
            {"触手正常位", FemaleTargetType.VAGINAL}, //Tentacle missionary
            {"壁Y字バランス挿入", FemaleTargetType.VAGINAL}, //Wall Y-balanced insertion
            {"またがり対面座位", FemaleTargetType.VAGINAL}, //Straddle Face-to-Face

            //anal
            //{"ANAL", FemaleTargetType.ANAL},
            {"アナル正常位", FemaleTargetType.ANAL}, //Anal Missionary
            {"アナル後背位", FemaleTargetType.ANAL}, //Anal Doggystyle
            {"アナル椅子後背位", FemaleTargetType.ANAL}, //Anal Doggy on Chair
            {"アナル壁立ち後背位", FemaleTargetType.ANAL}, //Against Wall Anal
            {"アナル壁立ちバック", FemaleTargetType.ANAL}, //Anal wall doggy
            {"アナル開脚バック", FemaleTargetType.ANAL}, //Anal spread-legged doggy
            {"アナルマングリ正常位", FemaleTargetType.ANAL}, //Anal spread missionary

            // Aicomi Female-led HScene Category
            //vaginal
            //{"VAGINAL", FemaleTargetType.VAGINAL},
            {"女主導手コキ素股", FemaleTargetType.LEFTHAND}, //Handjob Humping (Her Lead)
            {"女主導椅子素股", FemaleTargetType.VAGINAL}, //Sitting Humping (Her Lead)
            {"女主導背面騎乗", FemaleTargetType.VAGINAL}, //Reverse Cowgirl (Her Lead)
            {"女主導騎乗", FemaleTargetType.VAGINAL}, //Cowgirl (Her Lead)
            {"女主導騎乗素股", FemaleTargetType.VAGINAL}, //Cowgirl Humping (Her Lead)
            {"アナル背面騎乗位", FemaleTargetType.ANAL}, //Anal reverse cowgirl
            {"騎乗素股", FemaleTargetType.INTERCRURAL_HIP}, //Cowgirl intercrural
            {"手コキ素股", FemaleTargetType.INTERCRURAL_HIP}, //Handjob intercrural
            {"椅子素股", FemaleTargetType.INTERCRURAL_HIP}, //Chair intercrural
            {"椅子対面座位", FemaleTargetType.VAGINAL}, //Chair Face-to-Face Sitting

            //intercrucial
            //{"INTERCRURAL_HIP", FemaleTargetType.INTERCRURAL_HIP},
            {"立ち素股", FemaleTargetType.INTERCRURAL_HIP}, //Standing intercrural


            //footjob
            //{"FOOTJOB", FemaleTargetType.LEFTFOOT},
            {"立ち足コキ", FemaleTargetType.LEFTFOOT}, //Standing Footjob
            {"床足コキ", FemaleTargetType.LEFTFOOT}, //Floor footjob
            {"椅子足コキ", FemaleTargetType.LEFTFOOT}, //Chair footjob (starts with left, speedup: both)
            {"机下足コキ", FemaleTargetType.LEFTFOOT}, //Under-desk footjob
            {"机下足愛撫", FemaleTargetType.LEFTFOOT}, //Under-desk foot caress
            {"男椅子拘束足コキ", FemaleTargetType.LEFTFOOT}, //Man chair restrained footjob
            {"バスタブ足コキ", FemaleTargetType.LEFTFOOT}, //Bathtub footjob


            // 3P - none of these currently implemented
            // 3P - 2 girls, 1 guy - HJ
            //{"GIRL1", FemaleTargetType.LEFTHAND},
            //{"GIRL2", FemaleTargetType.LEFTHANDSWAP},
            //footjob & HJ
            {"手コキ舐めA", FemaleTargetType.LEFTHAND}, //Double handjob + Licking A
            {"手コキ舐めB", FemaleTargetType.LEFTHANDSWAP}, //Double handjob + Licking B


            // 3P - 2 girls, 1 guy - BJ
            //{"GIRL1", FemaleTargetType.ORAL},
            //{"GIRL2", FemaleTargetType.ORALSWAP},
            //double fellatio
            {"フェラA", FemaleTargetType.ORAL}, //Double blowjob A
            {"フェラB", FemaleTargetType.ORALSWAP}, // Double blowjob A

            // 3P - 2 girls, 1 guy - TJ
            //{"GIRL1", FemaleTargetType.BREASTS},
            //{"GIRL2", FemaleTargetType.BREASTSWAP},

            // 3P - 2 girls, 1 guy - FJ
            //{"GIRL1", FemaleTargetType.LEFTFOOT},
            //{"GIRL2", FemaleTargetType.LEFTFOOTSWAP},

            // 3P - 2 girls, 1 guy - intercrural
            //{"GIRL1", FemaleTargetType.INTERCRURAL_HIP},
            //{"GIRL2", FemaleTargetType.INTERCRURAL_HIP_SWAP},
            {"素股サンドA", FemaleTargetType.INTERCRURAL_HIP}, //Intercrural sandwich A
            {"素股サンドB", FemaleTargetType.INTERCRURAL_HIP_SWAP}, //Intercrural sandwich B

            // 3P - 2 girls, 1 guy - Insert
            //{"GIRL1", FemaleTargetType.VAGINAL},
            //{"GIRL2", FemaleTargetType.VAGINALSWAP},
            //cowgirl & cunnilingus
            {"騎乗位A", FemaleTargetType.VAGINAL}, //Double cowgirl A
            {"騎乗位B", FemaleTargetType.VAGINALSWAP}, //Double cowgirl B
            {"机バッククンニA", FemaleTargetType.VAGINAL}, //Desk doggy cunnilingus A
            {"机バッククンニB", FemaleTargetType.VAGINALSWAP}, //Desk doggy cunnilingus B

            //doggy & fingering
            {"後背位＋手マンA", FemaleTargetType.VAGINAL}, //Doggy + fingering A
            {"後背位＋手マンB", FemaleTargetType.VAGINALSWAP}, //Doggy + fingering B

            //missionary & fingering

            //reverse cowgirl & fingering

            //reverse sitting & cunnilingus

            //3P - 1 girl, 2 guys
            {"W駅弁", FemaleTargetType.VAGINAL}, // W sandwich
            {"二竿フェラ", FemaleTargetType.ORAL}, //Two way blowjob
            {"背面騎乗＋正常位", FemaleTargetType.VAGINAL}, //Reverse cowgirl + Missionary
            {"対面騎乗＋後背位", FemaleTargetType.ANAL}, //Face-to-face cowgirl + Doggy style
            {"後背位＋フェラ", FemaleTargetType.VAGINAL}, //Doggystyle + Blowjob
            {"側位＋フェラ", FemaleTargetType.VAGINAL}, // Sideways Blowjob
            {"壁穴後背位＋フェラ", FemaleTargetType.VAGINAL}, // Wall hole back + Blowjob
            {"空中バック＋フェラ", FemaleTargetType.VAGINAL}, //Carry doggy + Blowjob
            {"胸揉みアナル挿入＋クンニ", FemaleTargetType.ANAL}, // Breast fondling, anal insertion + cunnilingus
            


            //5P - 4 girls, 1 guy - ???
            /*
           // 5P - Boobjob
           {"5PパイズリA", FemaleTargetType.BREASTS}, //5P titty fuck A
           {"5PパイズリB", FemaleTargetType.BREASTS}, //5P titty fuck B
           {"5PパイズリC", FemaleTargetType.BREASTS}, //5P titty fuck C
           {"5PパイズリD", FemaleTargetType.BREASTS}, //5P titty fuck D
           // 5P - missionary
           {"5P正常位A", FemaleTargetType.VAGINAL}, //5P missionary A
           {"5P正常位B", FemaleTargetType.VAGINAL}, //5P missionary B
           {"5P正常位C", FemaleTargetType.VAGINAL}, //5P missionary C
           {"5P正常位D", FemaleTargetType.VAGINAL}, //5P missionary D
           // 5P - licking
           {"5P舐めA", FemaleTargetType.ORAL}, //5P licking A
           {"5P舐めB", FemaleTargetType.ORAL}, //5P licking B
           {"5P舐めC", FemaleTargetType.ORAL}, //5P licking C
           {"5P舐めD", FemaleTargetType.ORAL}, //5P licking D
           //5P cowgirl
           {"5P騎乗位A", FemaleTargetType.VAGINAL}, //5P cowgirl A
           {"5P騎乗位B", FemaleTargetType.VAGINAL}, //5P cowgirl B
           {"5P騎乗位C", FemaleTargetType.VAGINAL}, //5P cowgirl C
           {"5P騎乗位D", FemaleTargetType.VAGINAL}, //5P cowgirl D
           */

        };
    }
}
