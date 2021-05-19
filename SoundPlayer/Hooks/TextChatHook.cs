using AK;
using HarmonyLib;
using Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SoundPlayer.Hooks
{
    [HarmonyPatch(typeof(PlayerChatManager), "PostMessage")]
    public class TextChatHook {
        [HarmonyPrefix]
        public static bool PreFix() {
            if (!(PlayerChatManager.Current.m_currentValue.Length > 2)) return true;
            if (!PlayerChatManager.Current.m_currentValue.StartsWith("/")) return true;
            PlayerChatManager.Current.m_currentValue = PlayerChatManager.Current.m_currentValue.Remove(0, 1);
            string[] args = PlayerChatManager.Current.m_currentValue.Split(" ");
            PUI_GameEventLog chat = UnityEngine.Object.FindObjectOfType<PUI_GameEventLog>();
            if (chat == null) {
                Log.Error($"TextChatHook invoked, but PUI_GameEventLog cannot be found");
                return true;
            }
            SoundPlayerScript soundPlayer = UnityEngine.Object.FindObjectOfType<SoundPlayerScript>();
            if(soundPlayer == null) {
                Log.Error($"TextChatHook invoked, but SoundPlayerScript cannot be found");
                return true;
            }
            switch (args[0].ToLower()) {
                case "clear": case "cls":
                    for (int i = 0; i < chat.m_logItemsMax; i++)
                        chat.AddLogItem("", eGameEventChatLogType.IncomingChat);
                    PlayerChatManager.Current.m_currentValue = "";
                    PlayerChatManager.Current.ExitChatMode();
                    return false;
                case "changesound": case "editsound": case "sound": case "s":
                    if(!uint.TryParse(args[1], out uint eventId)) {
                        PropertyInfo[] properties = typeof(EVENTS).GetProperties();
                        PropertyInfo[] matches = properties.Where((x) => x.Name.ToLower().Contains(args[1].ToLower())).ToArray();
                        if(matches.Length == 0) { 
                            chat.AddLogItem($"<color=#1ac725>Couldn't find a sound with name {args[1]}</color>", eGameEventChatLogType.IncomingChat);
                            PlayerChatManager.Current.m_currentValue = "";
                            PlayerChatManager.Current.ExitChatMode();
                            return false;
                        }
                        if(matches.Length > 1) {
                            string log = $"<color=#1ac725>Found more than one sounds that match {args[1]}:\n";
                            for (int i = 0; i < matches.Length; i++) {
                                log += $"\t{matches[i].Name}: {matches[i].GetValue(null)}";
                                if (i < matches.Length - 1)
                                    log += "\n";
                            }
                            log += $"</color>";
                            chat.AddLogItem(log, eGameEventChatLogType.IncomingChat);
                            PlayerChatManager.Current.m_currentValue = "";
                            PlayerChatManager.Current.ExitChatMode();
                            return false;
                        }
                        eventId = (uint)matches[0].GetValue(null);
                    }

                    chat.AddLogItem($"<color=#1ac725>Changing sound to {eventId}</color>", eGameEventChatLogType.IncomingChat);
                    soundPlayer.SoundId = eventId;
                    PlayerChatManager.Current.m_currentValue = "";
                    PlayerChatManager.Current.ExitChatMode();
                    return false;
            }
            
            return true;
        }
    }
}
