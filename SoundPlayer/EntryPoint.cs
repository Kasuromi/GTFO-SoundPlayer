using BepInEx;
using BepInEx.IL2CPP;
using HarmonyLib;
using System;
using UnhollowerRuntimeLib;

namespace SoundPlayer
{
    [BepInPlugin("com.kasuromi.soundplayer", "SoundPlayer", VERSION)]
    public class SoundPlayerPlugin : BasePlugin {
        public const string VERSION = "1.0.2";
        public override void Load() {
            ClassInjector.RegisterTypeInIl2Cpp<SoundPlayerScript>();

            var harmony = new Harmony("com.kasuromi.soundplayer");
            harmony.PatchAll();
        }
    }
}
