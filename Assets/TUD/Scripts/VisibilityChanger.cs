// VisibilityChanger.cs
// Developed by Heartucate GmbH, 2026.
// For Technische Universität Dresden (TUD). Use and modification permitted under agreed terms.
// Please retain this credit in all versions of this file.
// Contact: info@heartucate.eu

using UnityEngine;

namespace Heartucate.TUD
{
    public class VisibilityChanger : MonoBehaviour
    {
        private static VisibilityChanger _instance;

        [SerializeField] private GameObject[] hideInHybrid;
        [SerializeField] private GameObject[] hideInFullAr;
        [SerializeField] private GameObject[] hideInLevel1;
        [SerializeField] private GameObject[] hideInLevel2;
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
        }
        private void Start()
        {
            ChangeVisibility();
        }
        public static void ChangeVisibility()
        {
            if (_instance == null)
            {
                Debug.LogWarning("VisibilityChanger not present in scene.");
                return;
            }

            _instance.ApplyVisibility();
        }
        private void ApplyVisibility()
        {
            Debug.Log($"Applying Visiblity for {GameState.CurrentMode} (Current Level: {GameState.CurrentLevel})");

            if (GameState.CurrentMode == GameState.GameMode.FULLAR)
            {
                SetActive(hideInHybrid, true);
                SetActive(hideInFullAr, false);
            }

            if (GameState.CurrentLevel == GameState.Level.LEVEL1)
            {
                SetActive(hideInLevel1, false);
                SetActive(hideInLevel2, true);
            }

            if (GameState.CurrentLevel == GameState.Level.LEVEL2)
            {
                SetActive(hideInLevel2, false);
                SetActive(hideInLevel1, true);
            }

            if (GameState.CurrentMode == GameState.GameMode.HYBRID)
            {
                SetActive(hideInHybrid, false);
            }
        }

        private void SetActive(GameObject[] objects, bool state)
        {
            foreach (var obj in objects)
            {
                if (obj != null)
                    obj.SetActive(state);
            }
        }
    }
}