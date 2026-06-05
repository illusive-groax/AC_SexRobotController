using AC_SexRobotController.Helpers;
using AC_SexRobotController.RobotController;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;

namespace AC_SexRobotController.Plugin
{
    [BepInProcess(StringConstants.GAME_NAME)]
    [BepInProcess(StringConstants.GAME_VR_NAME)]
    [BepInPlugin(StringConstants.PLUGIN_GUID, StringConstants.PLUGIN_NAME, StringConstants.PLUGIN_VERSION)]

    internal partial class AC_SexRobotControllerPlugin : BasePlugin
    {
        public static AC_SexRobotControllerPlugin Instance { get; private set; }
        private static ManualLogSource _Log;

        public override void Load()
        {
            _Log = base.Log;
            Instance = this;
            SetupPluginConfigurations();
            Harmony.CreateAndPatchAll(typeof(AC_SexRobotControllerPlugin), StringConstants.PLUGIN_GUID);
            Hooks.Hooks.InstallHooks();
            _Log.LogInfo($"Plugin {StringConstants.PLUGIN_GUID} is loaded!");
        }

        internal static SexRobotController Get_SexRobotController()
        {
            if (SexRobotController.Instance != null)
            {
                return SexRobotController.Instance;
            }
            else
            {
                return Instance.AddComponent<SexRobotController>();
            }
        }

        internal static void LogInfo(string log)
        {
            _Log.LogInfo(log);
        }

        internal static void LogDebug(string log)
        {
            _Log.LogDebug(log);
        }
    }
}
