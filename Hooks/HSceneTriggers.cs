using AC.Config;
using AC_SexRobotController.Helpers;
using AC_SexRobotController.Plugin;
using AC_SexRobotController.RobotController;
using H;
using HarmonyLib;
using ILLGAMES.Extensions;

namespace AC_SexRobotController.Hooks
{
    internal static partial class Hooks
    {

        private static class HSceneTriggers
        {
            // HScene start
            [HarmonyPostfix]
            [HarmonyPatch(typeof(ProcBase), "Initialize")]
            public static void ProcBasePostInitialize(ProcBase __instance)
            {
                try
                {
                    if (__instance == null || __instance._hScene == null)
                        return;

                    if (_ac_SRC == null)
                        _ac_SRC = AC_SexRobotControllerPlugin.Get_SexRobotController();

                    if (!_ac_SRC.IsHSceneSet())
                        _ac_SRC.SetHScene(__instance._hScene);

                    if (__instance._hScene.NowAnimationInfo.NameAnimation != "")
                        SexRobotController.CheckAnimationName();

                    _ac_SRC.ReadBodyPosition();
                    //SexRobotController.UpdateAnimationDictionary();
                }
                catch (System.Exception ex)
                {
                    AC_SexRobotControllerPlugin.LogDebug("An error occurred when attemptng to initialize HScene (ProcBasePostInitialize): " + ex.ToString());
                }
            }

            // HScene ended
            [HarmonyPrefix]
            [HarmonyPatch(typeof(HResult), "EvaluationResult")]
            [HarmonyPatch(typeof(HScene), "RestoreActors")]
            public static void HScenePreEnd()
            {
                try
                {
                    _ac_SRC.HsceneEnding();
                }
                catch (System.Exception ex)
                {
                    AC_SexRobotControllerPlugin.LogDebug("An error occurred when attempting to end the HScene (HScenePreEnd): " + ex.ToString());
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch(typeof(ConfigWindow), nameof(ConfigWindow.Start))]
            private static void OnOpen(ConfigWindow __instance)
            {
                var btnInit = __instance.transform.FindLoop(StringConstants.CONFIG_MENU_RESET_BUTTON).transform;
                var btnTitle = __instance.transform.FindLoop(StringConstants.CONFIG_MENU_TITLE_BUTTON).transform;

                AC_SexRobotControllerPlugin.CreateControllerButtons(btnInit, btnTitle);
            }
        }
    }
}