// RealtimeDataProcessor.cs
// Developed by Heartucate GmbH, 2026.
// For Technische Universität Dresden (TUD). Use and modification permitted under agreed terms.
// Please retain this credit in all versions of this file.
// Contact: info@heartucate.eu

using TMPro;
using UnityEngine;

namespace Heartucate.TUD
{
    public class RealtimeDataProcessor : MonoBehaviour
    {
        // Delegate type for received text
        public delegate void TextReceivedDelegate(string text);

        // Static delegate field that other scripts can assign to receive incoming text
        public static TextReceivedDelegate OnTextReceived;

        public TextMeshProUGUI TextReceived; // UI text field to display received strings

        // Enable debug controls in inspector
        public bool DebugControls = true;

        // Debug angle for X (0..360)
        private Vector3 _debugAngle;


        // Optional helper to add/remove handlers
        public static void Subscribe(TextReceivedDelegate handler)
        {
            OnTextReceived += handler;
        }

        public static void Unsubscribe(TextReceivedDelegate handler)
        {
            OnTextReceived -= handler;
        }

        // Start is called before the first frame update
        void Start()
        {
            Subscribe(HandleText);
        }

        void OnDestroy()
        {
            Unsubscribe(HandleText);
        }

        // Update used for debug input handling
        void Update()
        {
            if (!DebugControls)
                return;

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                _debugAngle.x = (_debugAngle.x + 1) % 360; // wrap 0..360
                string payload = $"{_debugAngle.x},{_debugAngle.y},{_debugAngle.z}";
                HandleText(payload);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                _debugAngle.x = _debugAngle.x - 1;
                if (_debugAngle.x < 0) _debugAngle.x = 359;
                string payload = $"{_debugAngle.x},{_debugAngle.y},{_debugAngle.z}";
                HandleText(payload);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                _debugAngle.y = (_debugAngle.y + 1) % 360; // wrap 0..360
                string payload = $"{_debugAngle.x},{_debugAngle.y},{_debugAngle.z}";
                HandleText(payload);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                _debugAngle.y = _debugAngle.y - 1;
                if (_debugAngle.y < 0) _debugAngle.y = 359;
                string payload = $"{_debugAngle.x},{_debugAngle.y},{_debugAngle.z}";
                HandleText(payload);
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                _debugAngle.z = (_debugAngle.z + 1) % 360; // wrap 0..360
                string payload = $"{_debugAngle.x},{_debugAngle.y},{_debugAngle.z}";
                HandleText(payload);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                _debugAngle.z = _debugAngle.z - 1;
                if (_debugAngle.z < 0) _debugAngle.z = 359;
                string payload = $"{_debugAngle.x},{_debugAngle.y},{_debugAngle.z}";
                HandleText(payload);
            }
        }

        // Example local handler processes "X, Y, Z" into three angles and forwards to DegreeAdjusterManager
        void HandleText(string text)
        {
            Debug.Log("RealtimeDataProcessor received text: " + text);

            if (TextReceived != null)
                TextReceived.text = text;

            if (string.IsNullOrEmpty(text))
                return;

            // split by comma and trim spaces
            var parts = text.Split(',');
            if (parts.Length < 3)
                return;

            for (int i = 0; i < 3; i++)
            {
                var part = parts[i].Trim();
                if (int.TryParse(part, out int angle))
                {
                    if (DegreeAdjusterManager.Instance != null)
                    {
                        DegreeAdjusterManager.Instance.SetAngle(i, angle);
                    }
                    else
                    {
                        Debug.LogWarning("DegreeAdjusterManager.Instance is null");
                    }
                }
                else
                {
                    Debug.LogWarning($"Failed to parse angle for index {i}: '{part}'");
                }
            }
        }
    }
}