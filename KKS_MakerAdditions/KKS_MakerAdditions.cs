using System;

using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

using KKAPI.Maker;
using KKAPI.Maker.UI.Sidebar;

using UniRx;
using UnityEngine;

namespace KKS_MakerAdditions
{
    [BepInProcess("KoikatsuSunshine")]
    [BepInProcess("KoikatsuSunshineTrial")]
    [BepInPlugin(nameof(KKS_MakerAdditions), nameof(KKS_MakerAdditions), VERSION)]
    public class KKS_MakerAdditions : BaseUnityPlugin
    {
        public const string VERSION = "1.0.1";
        
        public new static ManualLogSource Logger;
        
        private static GameObject oldParent;
        private static GameObject newParent;
        
        private SidebarToggle lockCamlightToggle;
        
        private void Awake()
        {
            Logger = base.Logger;

            var harmony = new Harmony(nameof(KKS_MakerAdditions));

            var moreAccsPos = Type.GetType("MoreAccessoriesKOI.ChaControl_SetAccessoryPos_Patches, MoreAccessories, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
            var moreAccsRot = Type.GetType("MoreAccessoriesKOI.ChaControl_SetAccessoryRot_Patches, MoreAccessories, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
            
            if (moreAccsPos != null && moreAccsRot != null)
            {
                harmony.Patch(moreAccsPos.GetMethod("Prefix", AccessTools.all), null, null, new HarmonyMethod(typeof(Hooks), nameof(Hooks.ChaControl_SetAccessoryPos_ChangeLimit)));
                harmony.Patch(moreAccsRot.GetMethod("Prefix", AccessTools.all), null, null, new HarmonyMethod(typeof(Hooks), nameof(Hooks.ChaControl_SetAccessoryRot_ChangeLimit)));
            }
            
            harmony.PatchAll(typeof(Hooks));
            
            MakerAPI.MakerBaseLoaded += MakerAPI_Enter;
            MakerAPI.MakerExiting += (_, __) => OnDestroy();
        }
        
        private void MakerAPI_Enter(object sender, RegisterCustomControlsEvent e)
        {
            var camLight = GameObject.Find("CustomScene/CamBase/Camera/Directional Light").transform;
            
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

                    camLight.transform.localEulerAngles = new Vector3(0, 3, 0);

                    Destroy(newParent);
                    newParent = null;
                }
            });
        }

        private void OnDestroy()
        {
            lockCamlightToggle = null;
        }
    }
}