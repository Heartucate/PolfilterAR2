// VisibilityButton.cs
// Developed by Heartucate GmbH, 2026.
// For Technische Universität Dresden (TUD). Use and modification permitted under agreed terms.
// Please retain this credit in all versions of this file.
// Contact: info@heartucate.eu

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

namespace Heartucate.TUD
{
    public class VisibilityButton : MonoBehaviour
    {
        [SerializeField] private List<GameObject> targets = new List<GameObject>();
        private Canvas canvas;
        private bool isVisible;

        private void Awake()
        {
            canvas = GetComponent<Canvas>();
        }
        private void Start()
        {
            if (GameState.CurrentMode != GameState.GameMode.SIMULATION)
            {
                canvas.worldCamera = FindObjectOfType<TrackedPoseDriver>().GetComponent<Camera>();
            }
        }

        public void Show()
        {
            SetVisibleInstant(true);
        }

        public void Hide()
        {
            SetVisibleInstant(false);
        }

        public void SetVisibleInstant(bool visible)
        {
            isVisible = visible;

            foreach (GameObject go in targets)
            {
                if (go != null)
                    go.SetActive(visible);
            }
        }
    }
}