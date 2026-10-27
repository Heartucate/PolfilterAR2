// GameStateSelectionManager.cs
// Developed by Heartucate GmbH, 2026.
// For Technische Universität Dresden (TUD). Use and modification permitted under agreed terms.
// Please retain this credit in all versions of this file.
// Contact: info@heartucate.eu

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Heartucate.TUD
{
    public class GameStateSelectionManager : MonoBehaviour
    {
        [SerializeField] private Button _startButton;
        [SerializeField] private Canvas _menuUI;
        [SerializeField] private GameObject _trackingSystemPrefab;
        public static TrackingSystemManager TrackingSystemManager;

        private bool _started = false;
        private bool _gameStateLocked = false;
        private bool _scanDetected = false;

        void Awake()
        {
            if (TrackingSystemManager == null)
            {
                TrackingSystemManager = Instantiate(_trackingSystemPrefab, null).GetComponent<TrackingSystemManager>();
            }
            GameState.CurrentMode = GameState.GameMode.NONE;
        }

        private void OnEnable()
        {
            // Subscribe to calibration image changes
            Heartucate.Tracking.CalibrationManager.OnTrackedImageActiveChanged += OnTrackedImageActiveChanged;
            _startButton.onClick.AddListener(OnStartClicked);
        }

        private void OnDisable()
        {
            Heartucate.Tracking.CalibrationManager.OnTrackedImageActiveChanged -= OnTrackedImageActiveChanged;
            _startButton.onClick.RemoveAllListeners();
        }

        private void OnStartClicked()
        {
            // Hide menu and begin waiting for a scan (or immediately load if already locked)
            _menuUI.enabled = false;
            _started = true;
            StartCoroutine(WaitForScanAndLoad());
            print("Start button clicked. Waiting for scan to load the scene...");
        }

        private System.Collections.IEnumerator WaitForScanAndLoad()
        {
            // If a game state was already locked before pressing start, load immediately
            if (_gameStateLocked)
            {
                var opImmediate = SceneManager.LoadSceneAsync(1);
                while (!opImmediate.isDone)
                    yield return null;

                if (GameState.CurrentMode == GameState.GameMode.SIMULATION)
                    gameObject.SetActive(false);

                yield break;
            }

            // Otherwise wait until a scan is detected (OnTrackedImageActiveChanged will set _scanDetected)
            float timeout = 60f; // optional safety timeout
            float elapsed = 0f;
            while (!_scanDetected && elapsed < timeout)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }

            if (!_scanDetected)
            {
                // Timeout reached: re-enable menu so user can try again
                _menuUI.enabled = true;
                _started = false;
                yield break;
            }

            // Load scene asynchronously
            var op = SceneManager.LoadSceneAsync(1);
            while (!op.isDone)
                yield return null;

            if (GameState.CurrentMode == GameState.GameMode.SIMULATION)
                gameObject.SetActive(false);
        }

        private void OnTrackedImageActiveChanged(string imageName)
        {
            if (!_started)
                return;

            // If no name or already locked, ignore
            if (string.IsNullOrEmpty(imageName) || _gameStateLocked)
                return;

            // Use first character if longer strings are passed
            char key = imageName.Length > 0 ? imageName[0] : '\0';

            switch (key)
            {
                case 'H':
                    GameState.CurrentMode = GameState.GameMode.HYBRID;
                    break;
                case 'A':
                    GameState.CurrentMode = GameState.GameMode.FULLAR;
                    break;
                case 'S':
                    GameState.CurrentMode = GameState.GameMode.SIMULATION;
                    break;
                default:
                    // Unknown image, do not change
                    return;
            }

            _gameStateLocked = true;
            _scanDetected = true;
        }

        void Update()
        {
#if UNITY_EDITOR
            // Allow simulating scans with keyboard in the editor once Start was pressed
            if (_started && !_gameStateLocked)
            {
                if (Input.GetKeyDown(KeyCode.H))
                    OnTrackedImageActiveChanged("H");
                else if (Input.GetKeyDown(KeyCode.A))
                    OnTrackedImageActiveChanged("A");
                else if (Input.GetKeyDown(KeyCode.S))
                    OnTrackedImageActiveChanged("S");
            }
#endif
        }
    }
}