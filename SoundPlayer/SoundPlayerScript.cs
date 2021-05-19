using AK;
using Player;
using SNetwork;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace SoundPlayer
{
    public class SoundPlayerScript : MonoBehaviour {
        public uint SoundId {
            get {
                return _soundId;
            }
            set {
                _soundId = value;
                if(_soundPlayerMarker != null) { 
                    _soundPlayerMarker.SetTitle($"Sound Source ({SoundId})");
                    _soundPlayerMarker.SetStyle(eNavMarkerStyle.LocationBeacon);
                    _soundPlayerMarker.SetVisible(true);
                }
            }
        } 

        private uint _soundId = 0;
        private CellSoundPlayer _soundPlayer;
        private GameObject _soundPlayerDummy;
        private NavMarker _soundPlayerMarker;
        private bool _guiEnabled = true;
        public SoundPlayerScript(IntPtr ptr) : base(ptr) {}

        private void OnGUI() {
            if (!_guiEnabled) return;
            if (!SNet.LocalPlayer.HasPlayerAgent || PlayerChatManager.InChatMode) return;
            GUIStyle style = new GUIStyle {
                fontSize = 18,
                normal = new GUIStyleState {
                    textColor = new Color(1, 1, 1)
                }
            };
            GUI.Label(new Rect(5, 200, 200, 20), new GUIContent($"Sound Player V{SoundPlayerPlugin.VERSION}"), style);
            GUI.Label(new Rect(5, 220, 200, 20), new GUIContent($"Current Sound Id: {SoundId}"), style);
            GUI.Label(new Rect(5, 240, 200, 20), new GUIContent($"Teleport Sound Source to player: K"), style);
            GUI.Label(new Rect(5, 260, 200, 20), new GUIContent($"Play Sound: P"), style);
            GUI.Label(new Rect(5, 280, 200, 20), new GUIContent($"Stop Sound: L"), style);
            GUI.Label(new Rect(5, 300, 200, 20), new GUIContent($"Hide Menu: Insert"), style);
        }

        private void Update() {
            if (!SNet.LocalPlayer.HasPlayerAgent || PlayerChatManager.InChatMode) return;
            PlayerAgent playerAgent = SNet.LocalPlayer.PlayerAgent.Cast<PlayerAgent>();
            if(_soundPlayer == null) {
                if (_soundPlayerDummy != null) Destroy(_soundPlayerDummy);
                _soundPlayerDummy = new GameObject();
                _soundPlayerDummy.transform.position = playerAgent.Position;
                _soundPlayer = new CellSoundPlayer(playerAgent.Position);
                Log.Info($"Created CSP at Position: [{_soundPlayerDummy.transform.position.x}, {_soundPlayerDummy.transform.position.y}, {_soundPlayerDummy.transform.position.z}]");
                _soundPlayerMarker = GuiManager.NavMarkerLayer.PrepareGenericMarker(_soundPlayerDummy);
                SoundId = 0;
            }
            if(Input.GetKeyDown(KeyCode.K)) {
                _soundPlayer.UpdatePosition(playerAgent.Position);
                _soundPlayerDummy.transform.position = playerAgent.Position;
                Log.Info($"Moved CSP to Position: [{_soundPlayerDummy.transform.position.x}, {_soundPlayerDummy.transform.position.y}, {_soundPlayerDummy.transform.position.z}]");
            }
            if(Input.GetKeyDown(KeyCode.L)) { 
                if(_soundPlayer != null) {
                    _soundPlayer.Stop();
                } else {
                    Log.Error($"Attempting to stop sound without a CSP object");
                }
            }
            if(Input.GetKeyDown(KeyCode.P)) { 
                if(_soundPlayer != null) {
                    _soundPlayer.Post(SoundId);
                } else {
                    Log.Error($"Attempting to play sound without a CSP object");
                }
            }
            if (Input.GetKeyDown(KeyCode.Insert)) { 
                _guiEnabled = !_guiEnabled;
                Log.Info($"Toggled GUI to {_guiEnabled}");
            }
        }
    }
}
