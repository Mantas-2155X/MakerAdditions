using System.Collections.Generic;
using System.Reflection.Emit;
using System.Linq;
using System.Reflection;
using HarmonyLib;

using AIChara;
using CharaCustom;

using UnityEngine.UI;

namespace HS2_MakerAdditions
{
    public static class Hooks
    {
        [HarmonyPostfix, HarmonyPatch(typeof(CustomAcsCorrectSet), "Start")]
        public static void CustomAcsCorrectSet_Start_CreateAccessoryAdjust(CustomAcsCorrectSet __instance, ref float[] ___movePosValue, ref float[] ___moveRotValue)
        {
            Tools.CreateAccessoryAdjust(__instance, ___movePosValue, ___moveRotValue);
        }
        
        [HarmonyPostfix, HarmonyPatch(typeof(CustomAcsCorrectSet), "UpdateCustomUI")]
        public static void CustomAcsCorrectSet_UpdateCustomUI_ResetAdjToggles()
        {
            foreach (var toggle in Tools.toggles.Where(toggle => toggle != null))
                toggle.isOn = false;
        }
        
        // Increase maximum digits to two
        [HarmonyPrefix, HarmonyPatch(typeof(CustomBase), "ConvertValueFromTextLimit")]
        public static void CustomBase_ConvertValueFromTextLimit_OverrideDigit(ref int digit)
        {
            digit = 2;
        }
        
        // Change filter to allow two digits
        [HarmonyTranspiler, HarmonyPatch(typeof(ChaControl), "SetAccessoryPos")]
        public static IEnumerable<CodeInstruction> ChaControl_SetAccessoryPos_ChangeLimit(IEnumerable<CodeInstruction> instructions)
        {
            var il = instructions.ToList();

            {
                var items = il.FindAll(instruction => instruction.opcode == OpCodes.Ldstr && (string)instruction.operand == "f1");
                foreach (var index in items.Select(item => il.IndexOf(item)).ToList())
                {
                    if (index <= 0)
                    {
                        HS2_MakerAdditions.Logger.LogMessage("Failed transpiling 'ChaControl_SetAccessoryPos_ChangeLimit' f1 index not found!");
                        HS2_MakerAdditions.Logger.LogWarning("Failed transpiling 'ChaControl_SetAccessoryPos_ChangeLimit' f1 index not found!");
                        return il;
                    }

                    il[index].operand = "f2";
                }
            }

            {
                var items = il.FindAll(instruction => instruction.opcode == OpCodes.Call && (instruction.operand as MethodInfo)?.Name == "Clamp");
                foreach (var index in items.Select(item => il.IndexOf(item)).ToList())
                {
                    if (index <= 0)
                    {
                        HS2_MakerAdditions.Logger.LogMessage("Failed transpiling 'ChaControl_SetAccessoryPos_ChangeLimit' Clamp index not found!");
                        HS2_MakerAdditions.Logger.LogWarning("Failed transpiling 'ChaControl_SetAccessoryPos_ChangeLimit' Clamp index not found!");
                        return il;
                    }

                    il[index - 2].opcode = OpCodes.Nop;
                    il[index - 1].opcode = OpCodes.Nop;
                    il[index].opcode = OpCodes.Nop;
                }
            }

            return il;
        }
        
        [HarmonyTranspiler, HarmonyPatch(typeof(ChaControl), "SetAccessoryRot")]
        public static IEnumerable<CodeInstruction> ChaControl_SetAccessoryRot_ChangeLimit(IEnumerable<CodeInstruction> instructions)
        {
            var il = instructions.ToList();
            
            // Remove int cast to allow point values
            var items = il.FindAll(instruction => instruction.opcode == OpCodes.Conv_I4);
            foreach (var index in items.Select(item => il.IndexOf(item)).ToList())
            {
                if (index <= 0)
                {
                    HS2_MakerAdditions.Logger.LogMessage("Failed transpiling 'ChaControl_SetAccessoryRot_ChangeLimit' Conv_I4 index not found!");
                    HS2_MakerAdditions.Logger.LogWarning("Failed transpiling 'ChaControl_SetAccessoryRot_ChangeLimit' Conv_I4 index not found!");
                    return il;
                }

                il[index].opcode = OpCodes.Nop;
                il[index + 1].opcode = OpCodes.Nop;
            }
            
            // Round to two digits
            var items1 = il.FindAll(instruction => instruction.opcode == OpCodes.Call && (instruction.operand as MethodInfo)?.Name == "Repeat");
            foreach (var index in items1.Select(item => il.IndexOf(item)).ToList())
            {
                if (index <= 0)
                {
                    HS2_MakerAdditions.Logger.LogMessage("Failed transpiling 'ChaControl_SetAccessoryRot_ChangeLimit' Repeat index not found!");
                    HS2_MakerAdditions.Logger.LogWarning("Failed transpiling 'ChaControl_SetAccessoryRot_ChangeLimit' Repeat index not found!");
                    return il;
                }

                il[index] = new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Tools), nameof(Tools.NewRepeat)));
            }
            
            return il;
        }
    }
    
    public static class PreserveScrollHooks
    {
        [HarmonyPrefix, HarmonyPatch(typeof(CustomCharaWindow), "Sort")]
        public static void CustomCharaWindow_Sort_SaveScroll(CustomCharaWindow __instance, out float __state)
        {
            var scrollObj = __instance.transform.Find("Scroll View/Scrollbar Vertical");
            __state = scrollObj.GetComponent<Scrollbar>().value;
        }
        
        [HarmonyPostfix, HarmonyPatch(typeof(CustomCharaWindow), "Sort")]
        public static void CustomCharaWindow_Sort_ApplyScroll(CustomCharaWindow __instance, float __state)
        {
            if (__state == 1f || !KKAPI.Maker.MakerAPI.InsideAndLoaded)
                return;
            
            var scrollObj = __instance.transform.Find("Scroll View/Scrollbar Vertical");
            var scrollbar = scrollObj.GetComponent<Scrollbar>();

            HS2_MakerAdditions.instance.StartCoroutine(Tools.ApplyScroller(scrollbar, __state));
        }
        
        [HarmonyPrefix, HarmonyPatch(typeof(CustomClothesWindow), "Sort")]
        public static void CustomClothesWindow_Sort_SaveScroll(CustomClothesWindow __instance, out float __state)
        {
            var scrollObj = __instance.transform.Find("Scroll View/Scrollbar Vertical");
            __state = scrollObj.GetComponent<Scrollbar>().value;
        }
        
        [HarmonyPostfix, HarmonyPatch(typeof(CustomClothesWindow), "Sort")]
        public static void CustomClothesWindow_Sort_ApplyScroll(CustomClothesWindow __instance, float __state)
        {
            if (__state == 1f || !KKAPI.Maker.MakerAPI.InsideAndLoaded)
                return;
            
            var scrollObj = __instance.transform.Find("Scroll View/Scrollbar Vertical");
            var scrollbar = scrollObj.GetComponent<Scrollbar>();

            HS2_MakerAdditions.instance.StartCoroutine(Tools.ApplyScroller(scrollbar, __state));
        }
    }
}