// OneShotPlayer.cs
// Developed by Heartucate GmbH, 2026.
// For Technische Universität Dresden (TUD). Use and modification permitted under agreed terms.
// Please retain this credit in all versions of this file.
// Contact: info@heartucate.eu

using UnityEngine;

namespace Heartucate.TUD
{
    public class OneShotPlayer : MonoBehaviour
    {
        [SerializeField] private AudioClip _sound;
        [Range(0f, 1f)]
        [SerializeField] private float _volume = 1f;
        [SerializeField] private float _minInterval = 0.05f; // 20x pro Sekunde

        private float _lastPlayTime;

        public void PlayOS()
        {
            if (Time.time - _lastPlayTime < _minInterval)
                return;

            _lastPlayTime = Time.time;
            AudioManager.PlaySound(_sound, _volume);
        }
    }
}