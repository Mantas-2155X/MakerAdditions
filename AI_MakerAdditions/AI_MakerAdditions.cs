using System;

using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

using AIChara;
using CharaCustom;
using KKAPI.Maker;
using KKAPI.Maker.UI.Sidebar;

using UniRx;
using UnityEngine;

namespace AI_MakerAdditions
{
    [BepInProcess("AI-Syoujyo")]
    [BepInProcess("AI-Shoujo")]
    [BepInPlugin(nameof(AI_MakerAdditions), nameof(AI_MakerAdditions), VERSION)]
    public class AI_MakerAdditions : BaseUnityPlugin
    {
        public const string VERSION = "1.0.0";
        
        public new static ManualLogSource Logger;
        
        public static AI_MakerAdditions instance;
        
        private static GameObject oldParent;
        private static GameObject newParent;
        
        private SidebarToggle lockCamlightToggle;
        private SidebarToggle backlightToggle;
        private SidebarToggle blinkingToggle;
        
        private void Awake()
        {
            Logger = base.Logger;
            
            instance = this;
            
            var harmony = new Harmony(nameof(AI_MakerAdditions));

            var moreAccs = Type.GetType("MoreAccessoriesAI.Patches.ChaControl_ChaControl_Patches, MoreAccessories, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
            if (moreAccs != null)
            {
                harmony.Patch(moreAccs.GetMethod("SetAccessoryPos_Prefix", AccessTools.all), null, null, new HarmonyMethod(typeof(Hooks), nameof(Hooks.ChaControl_SetAccessoryPos_ChangeLimit)));
                harmony.Patch(moreAccs.GetMethod("SetAccessoryRot_Prefix", AccessTools.all), null, null, new HarmonyMethod(typeof(Hooks), nameof(Hooks.ChaControl_SetAccessoryRot_ChangeLimit)));
            }
            
            harmony.PatchAll(typeof(PreserveScrollHooks));
            harmony.PatchAll(typeof(Hooks));
            
            MakerAPI.MakerBaseLoaded += MakerAPI_Enter;
            MakerAPI.MakerExiting += (_, __) => OnDestroy();
        }
        
        private void MakerAPI_Enter(object sender, RegisterCustomControlsEvent e)
        {
            var cha = Singleton<ChaControl>.Instance;
            var cbase = Singleton<CustomBase>.Instance;

            var camLight = GameObject.Find("CharaCustom/CustomControl/CharaCamera/Main Camera/Lights Custom/Directional Light Key").transform;
            var backLight = GameObject.Find("CharaCustom/CustomControl/CharaCamera/Main Camera/Lights Custom/Directional Light Back").transform;
            
            lockCamlightToggle = e.AddSidebarControl(new SidebarToggle("Lock Cameralight", false, this));
            lockCamlightToggle.Value = false;
            lockCamlightToggle.ValueChanged.Subscribe(x =>
            {
                if (camLight == null)
                    return;

                if (x)
                {
                    oldParent = camLight.parent.gameObject;

                    newParent = new GameObject("CamLightLock");
                    newParent.transform.position = oldParent.transform.position;
                    newParent.transform.eulerAngles = oldParent.transform.eulerAngles;

                    camLight.parent = newParent.transform;
                }
                else if(oldParent != null)
                {
                    camLight.parent = oldParent.transform;

                    cbase.ResetLightSetting();

                    Destroy(newParent);
                    newParent = null;
                }
            });
        
            backlightToggle = e.AddSidebarControl(new SidebarToggle("Toggle Backlight", true, this));
            backlightToggle.Value = true;
            backlightToggle.ValueChanged.Subscribe(b =>
            {
                backLight.gameObject.SetActive(b);
            });
            
            blinkingToggle = e.AddSidebarControl(new SidebarToggle("Toggle Blinking", true, this));
            blinkingToggle.Value = true;
            blinkingToggle.ValueChanged.Subscribe(b =>
            {
                cha.ChangeEyesBlinkFlag(b);
            });
        }

        private void OnDestroy()
        {
            lockCamlightToggle = null;
            backlightToggle = null;
            blinkingToggle = null;
        }
    }
}