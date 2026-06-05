using AC_SexRobotController.Helpers;
using AC_SexRobotController.Plugin;
using AC_SexRobotController.RobotController;
using HarmonyLib;

namespace AC_SexRobotController.Hooks
{
    internal static partial class Hooks
    {
        private static SexRobotController _ac_SRC;

        public static void InstallHooks()
        {
            _ac_SRC = AC_SexRobotControllerPlugin.Get_SexRobotController();
            Harmony.CreateAndPatchAll(typeof(HSceneTriggers), StringConstants.PLUGIN_GUID);
        }
    }
}
