using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using CharaCustom;

using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace HS2_MakerAdditions
{
    public static class Tools
    {
        public static List<Toggle> toggles = new List<Toggle>();

        public static IEnumerator ApplyScroller(Scrollbar scrollbar, float __state)
        {
            yield return null;
            
            if(scrollbar != null)
                scrollbar.value = __state;
        }

        public static float NewRepeat(float a, float b)
        {
            return (float)Math.Round(Mathf.Repeat(a, b), 2);
        }

        public static void CreateAccessoryAdjust(CustomAcsCorrectSet __instance, float[] ___movePosValue, float[] ___moveRotValue)
        {
            toggles = toggles.Where(c => c != null).ToList();

            foreach (var target in new [] {"grpPos", "grpRot"})
            {
                var orig = __instance.transform.Find(target + "/imgRbCol02");
                var text = __instance.transform.Find(target + "/textRate");
                text.GetComponent<Text>().alignment = TextAnchor.MiddleLeft;

                var copy = Object.Instantiate(orig, orig.transform.parent);
                copy.name = "imgRbCol00_001";

                var rect = copy.GetComponent<RectTransform>();
                var oldMin = rect.offsetMin;
                var oldMax = rect.offsetMax;

                rect.offsetMin = new Vector2(oldMin.x - 240, oldMin.y);
                rect.offsetMax = new Vector2(oldMax.x - 240, oldMax.y);

                var rectText = text.GetComponent<RectTransform>();
                var oldMinText = rectText.offsetMin;
                var oldMaxText = rectText.offsetMax;

                rectText.offsetMin = new Vector2(0, oldMinText.y);
                rectText.offsetMax = new Vector2(90, oldMaxText.y);

                var textrbObj = copy.transform.Find("textRbSelect");
                textrbObj.GetComponent<Text>().text = target == "grpPos" ? "0.01" : "0.1";

                var toggle = copy.GetComponent<Toggle>();
                toggle.onValueChanged.RemoveAllListeners();
                toggle.onValueChanged.AddListener(delegate(bool on)
                {
                    if (target == "grpPos")
                    {
                        if (on)
                        {
                            ___movePosValue[0] = 0.01f;
                            ___movePosValue[1] = 0.01f;
                            ___movePosValue[2] = 0.01f;
                        }
                        else
                        {
                            ___movePosValue[0] = 0.1f;
                            ___movePosValue[1] = 1f;
                            ___movePosValue[2] = 10f;
                        }
                    }
                    else
                    {
                        if (on)
                        {
                            ___moveRotValue[0] = 0.1f;
                            ___moveRotValue[1] = 0.1f;
                            ___moveRotValue[2] = 0.1f;
                        }
                        else
                        {
                            ___moveRotValue[0] = 1f;
                            ___moveRotValue[1] = 5f;
                            ___moveRotValue[2] = 10f;
                        }
                    }
                });
            
                toggles.Add(toggle);
            }
        }
    }
}