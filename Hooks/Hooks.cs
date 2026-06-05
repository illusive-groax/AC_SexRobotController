using KKAPI.Studio;
using System;

namespace KKS_SexRobotController
{
    internal static partial class Hooks
    {
        public static void InstallHooks()
        {
            if (StudioAPI.InsideStudio)
            {
                return;
            }
            HarmonyLib.Harmony.CreateAndPatchAll(typeof(HSceneTriggers));
        }

        private static KKS_SexRobotController GetController()
        {
            // has apparently been tagged as obsolete in latest version
            // using the suggested method (FindAnyObjectByType) throws a "MissingMethodException"
            // therefore, suppress warning and use the functional one
#pragma warning disable CS0618 // Type or member is obsolete
            return UnityEngine.Object.FindObjectOfType<KKS_SexRobotController>();
#pragma warning restore CS0618 // Type or member is obsolete
            /*
            KKS_SexRobotController srcObj;
            try
            {
                // VS says to use this, because the other is deprecated
                // yet BepinEx / Unity throws MissingMethodException
                srcObj = UnityEngine.Object.FindAnyObjectByType<KKS_SexRobotController>();
            }
            catch (MissingMethodException)
            {
                // therefore, suppress the warning and use this method as fallback
#pragma warning disable CS0618 // Type or member is obsolete
                srcObj = UnityEngine.Object.FindObjectOfType<KKS_SexRobotController>();
#pragma warning restore CS0618 // Type or member is obsolete
            }
            return srcObj;
            */
        }
    }
}
