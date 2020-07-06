using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace AI_MakerAdditions
{
    [BepInProcess("AI-Syoujyo")]
    [BepInPlugin(nameof(AI_MakerAdditions), nameof(AI_MakerAdditions), VERSION)]
    public class AI_MakerAdditions : BaseUnityPlugin
    {
        public const string VERSION = "1.0.0";
        
        public new static ManualLogSource Logger;
        
        public static AI_MakerAdditions instance;
        
        private void Awake()
        {
            Logger = base.Logger;
            
            instance = this;
            
            var harmony = new Harmony(nameof(AI_MakerAdditions));
            harmony.PatchAll(typeof(PreserveScrollHooks));
            harmony.PatchAll(typeof(Hooks));
        }
    }
}