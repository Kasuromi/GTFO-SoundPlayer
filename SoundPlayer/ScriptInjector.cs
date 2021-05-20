using CellMenu;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SoundPlayer
{
    [HarmonyPatch(typeof(CM_PageRundown_New), "PlaceRundown")]
    public class ScriptInjector {
        private static bool _injected = false;
        [HarmonyPostfix]
        public static void PostFix() {
            if (_injected) return;
            GameObject gameObj = new GameObject();
            gameObj.AddComponent<SoundPlayerScript>();
            UnityEngine.Object.DontDestroyOnLoad(gameObj);
            _injected = true;
        }
    }
}
