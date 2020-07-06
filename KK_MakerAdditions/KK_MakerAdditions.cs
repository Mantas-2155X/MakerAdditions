using System;

using BepInEx;
using BepInEx.Logging;

using HarmonyLib;

namespace KK_MakerAdditions
{
    [BepInProcess("Koikatu")]
    [BepInPlugin(nameof(KK_MakerAdditions), nameof(KK_MakerAdditions), VERSION)]
    public class KK_MakerAdditions : BaseUnityPlugin
    {
        public const string VERSION = "1.0.0";
        
        public new static ManualLogSource Logger;
        
        private void Awake()
        {
            Logger = base.Logger;

            var harmony = new Harmony(nameof(KK_MakerAdditions));

            var moreAccsPos = Type.GetType("MoreAccessoriesKOI.ChaControl_SetAccessoryPos_Patches, MoreAccessories, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
            var moreAccsRot = Type.GetType("MoreAccessoriesKOI.ChaControl_SetAccessoryRot_Patches, MoreAccessories, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
            
            if (moreAccsPos != null && moreAccsRot != null)
            {
                harmony.Patch(moreAccsPos.GetMethod("Prefix", AccessTools.all), null, null, new HarmonyMethod(typeof(Hooks), nameof(Hooks.ChaControl_SetAccessoryPos_ChangeLimit)));
                harmony.Patch(moreAccsRot.GetMethod("Prefix", AccessTools.all), null, null, new HarmonyMethod(typeof(Hooks), nameof(Hooks.ChaControl_SetAccessoryRot_ChangeLimit)));
            }
            
            harmony.PatchAll(typeof(Hooks));
        }
    }
}