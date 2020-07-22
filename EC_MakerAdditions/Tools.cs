using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using TMPro;
using ChaCustom;

using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace EC_MakerAdditions
{
    public static class Tools
    {
        public static List<Toggle> toggles = new List<Toggle>();

        private static readonly int[] adjVertical = {20, 16, 12};

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

        public static void CreateAccessoryAdjust(CustomAcsMoveWindow __instance, float[] ___movePosValue, float[] ___moveRotValue)
        {
            toggles = toggles.Where(c => c != null).ToList();

            foreach (var target in new [] {"grpMove", "grpRot"})
            {
                var btn0 = __instance.transform.Find("grpParent/" + target + "/imgRbCol00");
                var btn1 = __instance.transform.Find("grpParent/" + target + "/imgRbCol01");
                var orig = __instance.transform.Find("grpParent/" + target + "/imgRbCol02");
                
                var i = 0;
                foreach (var tr in new[] {btn0, btn1, orig})
                {
                    var rect0 = tr.GetComponent<RectTransform>();
                    
                    var oldMin0 = rect0.offsetMin;
                    var oldMax0 = rect0.offsetMax;
                    
                    rect0.offsetMin = new Vector2(oldMin0.x, oldMin0.y - adjVertical[i]);
                    rect0.offsetMax = new Vector2(oldMax0.x, oldMax0.y - adjVertical[i]);
                    
                    i++;
                }

                if (target == "grpRot")
                {
                    var par = __instance.transform.Find("grpParent/grpRot");
                    
                    foreach (var str in new[] {"X/Inp", "Y/Inp", "Z/Inp"})
                    {
                        var input = par.Find(str).GetComponent<TMP_InputField>();
                        input.contentType = TMP_InputField.ContentType.DecimalNumber;
                        input.keyboardType = TouchScreenKeyboardType.NumbersAndPunctuation;
                        input.characterLimit = 5;
                    }
                }
                
                var copy = Object.Instantiate(orig, orig.transform.parent);
                copy.name = "imgRbCol00_001";

                var rect = copy.GetComponent<RectTransform>();
                var oldMin = rect.offsetMin;
                var oldMax = rect.offsetMax;

                rect.offsetMin = new Vector2(oldMin.x, oldMin.y + 60);
                rect.offsetMax = new Vector2(oldMax.x, oldMax.y + 60);

                var textrbObj = copy.transform.Find("textRbSelect");
                textrbObj.GetComponent<TextMeshProUGUI>().text = target == "grpMove" ? "0.01" : "0.1";

                var toggle = copy.GetComponent<Toggle>();
                toggle.onValueChanged.RemoveAllListeners();
                toggle.onValueChanged.AddListener(delegate(bool on)
                {
                    if (target == "grpMove")
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