// AudioManager.cs
// Developed by Heartucate GmbH, 2026.
// For Technische Universität Dresden (TUD). Use and modification permitted under agreed terms.
// Please retain this credit in all versions of this file.
// Contact: info@heartucate.eu

using UnityEngine;
using System.Collections.Generic;

namespace Heartucate.TUD
{
    public class AudioManager : MonoBehaviour
    {
        private static AudioManager _instance;
        private List<AudioSource> _sources = new List<AudioSource>();

        [SerializeField] private int _initialSources = 5;

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            for (int i = 0; i < _initialSources; i++)
                CreateSource();
        }

        private AudioSource CreateSource()
        {
            AudioSource src = gameObject.AddComponent<AudioSource>();
            src.playOnAwake = false;
            _sources.Add(src);
            return src;
        }

        private AudioSource GetFreeSource()
        {
            foreach (var src in _sources)
                if (!src.isPlaying)
                    return src;

            return CreateSource(); // falls alle belegt
        }

        public static void PlaySound(AudioClip clip, float volume = 1f)
        {
            if (_instance == null || clip == null) return;

            AudioSource src = _instance.GetFreeSource();
            src.volume = Mathf.Clamp01(volume);
            src.clip = clip;
            src.Play();
        }
    }
}