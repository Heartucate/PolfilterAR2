// MicrocontrollerSetupManager.cs
// Developed by Heartucate GmbH, 2026.
// For Technische Universität Dresden (TUD). Use and modification permitted under agreed terms.
// Please retain this credit in all versions of this file.
// Contact: info@heartucate.eu

using TMPro;
using UnityEngine;

namespace Heartucate.TUD
{
    public class MicrocontrollerSetupManager : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _idInputField;
        [SerializeField] private TextMeshProUGUI _debugText;
        private string _defaultID;

        private const string PlayerPrefDeviceKey = "MicrocontrollerDeviceName";

        private void OnEnable()
        {
            _debugText.text = "Warte auf Signal...";
            MicrocontrollerManager.OnTextOrStatusChanged += UpdateDebugText;
        }

        private void OnDisable()
        {
            MicrocontrollerManager.OnTextOrStatusChanged -= UpdateDebugText;
        }

        private void Start()
        {
            // load saved device name if present, otherwise use MicrocontrollerManager default
            _defaultID = PlayerPrefs.GetString(PlayerPrefDeviceKey, MicrocontrollerManager.DeviceName);
            MicrocontrollerManager.DeviceName = _defaultID;

            _idInputField.SetTextWithoutNotify(_defaultID);

            _idInputField.onValueChanged.AddListener(OnIdChanged);
        }

        public void ResetID()
        {
            _idInputField.SetTextWithoutNotify(_defaultID);
            // also restore static and saved value
            MicrocontrollerManager.DeviceName = _defaultID;
            PlayerPrefs.SetString(PlayerPrefDeviceKey, _defaultID);
            PlayerPrefs.Save();
        }

        private void OnDestroy()
        {
            _idInputField.onValueChanged.RemoveListener(OnIdChanged);
        }

        private void OnIdChanged(string value)
        {
            Debug.Log("Neue ID: " + value);
            MicrocontrollerManager.DeviceName = value;
            PlayerPrefs.SetString(PlayerPrefDeviceKey, value);
            PlayerPrefs.Save();
        }

        void UpdateDebugText(string text)
        {
            _debugText.text = text;

            // if a device was found during scan, store the current DeviceName
            // the MicrocontrollerManager sets status messages like "Found <name>"
            if (!string.IsNullOrEmpty(text) && text.StartsWith("Found "))
            {
                // save current DeviceName as last found
                PlayerPrefs.SetString(PlayerPrefDeviceKey, MicrocontrollerManager.DeviceName);
                PlayerPrefs.Save();
                Debug.Log("Saved DeviceName to PlayerPrefs: " + MicrocontrollerManager.DeviceName);
            }
        }

        public void SendResetToController()
        {
            bool success = MicrocontrollerManager.SendResetCommand();

            _debugText.text = success ? "Reset Befehl gesendet." : "Reset Befehl nicht gesendet.";
        }

        public void StartScanForController()
        {
            bool success = MicrocontrollerManager.StartScan();

            //_debugText.text = success ? "Reset Befehl gesendet." : "Reset Befehl nicht gesendet.";
        }
    }
}