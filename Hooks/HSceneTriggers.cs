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
            private static void ProcBasePostInitialize(ProcBase __instance)
            {
                // this could've been set in Hooks, but that would require a game restart
                // therefore, check here instead, so that the plugin can be enabled/disabled without game restart
                if (AC_SexRobotControllerPlugin.DisablePlugin.Value)
                {
                    // also check whether or not a scene has been loaded/started,
                    // to avoid unforeseen movement from the device itself
                    if (_ac_SRC != null || !_ac_SRC.IsHSceneSet())
                        _ac_SRC.HsceneEnding();
                    return;
                }
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
            private static void HScenePreEnd()
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
                // if the plugin is disabled, don't load the menu buttons
                if (AC_SexRobotControllerPlugin.DisablePlugin.Value)
                    return;
                var btnInit = __instance.transform.FindLoop(StringConstants.CONFIG_MENU_RESET_BUTTON).transform;
                var btnTitle = __instance.transform.FindLoop(StringConstants.CONFIG_MENU_TITLE_BUTTON).transform;

                AC_SexRobotControllerPlugin.CreateControllerButtons(btnInit, btnTitle);
            }
        }
    }
}