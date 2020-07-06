using System;

using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace HS2_MakerAdditions
{
    [BepInProcess("HoneySelect2")]
    [BepInPlugin(nameof(HS2_MakerAdditions), nameof(HS2_MakerAdditions), VERSION)]
    public class HS2_MakerAdditions : BaseUnityPlugin
    {
        public const string VERSION = "1.0.0";
        
        public new static ManualLogSource Logger;
        
        public static HS2_MakerAdditions instance;
        
        private void Awake()
        {
            Logger = base.Logger;
            
            instance = this;
            
            var harmony = new Harmony(nameof(HS2_MakerAdditions));

            var moreAccs = Type.GetType("MoreAccessoriesAI.Patches.ChaControl_ChaControl_Patches, MoreAccessories, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
            if (moreAccs != null)
            {
                harmony.Patch(moreAccs.GetMethod("SetAccessoryPos_Prefix", AccessTools.all), null, null, new HarmonyMethod(typeof(Hooks), nameof(Hooks.ChaControl_SetAccessoryPos_ChangeLimit)));
                harmony.Patch(moreAccs.GetMethod("SetAccessoryRot_Prefix", AccessTools.all), null, null, new HarmonyMethod(typeof(Hooks), nameof(Hooks.ChaControl_SetAccessoryRot_ChangeLimit)));
            }
            
            harmony.PatchAll(typeof(PreserveScrollHooks));
            harmony.PatchAll(typeof(Hooks));
        }
    }
}