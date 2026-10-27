// TrackingSystemManager.cs
// Developed by Heartucate GmbH, 2026.
// For Technische Universität Dresden (TUD). Use and modification permitted under agreed terms.
// Please retain this credit in all versions of this file.
// Contact: info@heartucate.eu

using UnityEngine;
using UnityEngine.SceneManagement;

namespace Heartucate.TUD
{
    public class TrackingSystemManager : MonoBehaviour
    {
        [SerializeField] private Camera _arCamera;
        public static Camera ArCamera;

        private void Awake()
        {
            ArCamera = _arCamera.GetComponent<Camera>();
            DontDestroyOnLoad(this.gameObject);
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            _arCamera.enabled = GameState.CurrentMode != GameState.GameMode.SIMULATION;
        }
    }
}