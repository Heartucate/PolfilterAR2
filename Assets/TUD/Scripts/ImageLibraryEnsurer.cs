// ImageLibraryEnsurer.cs
// Developed by Heartucate GmbH, 2026.
// For Technische Universität Dresden (TUD). Use and modification permitted under agreed terms.
// Please retain this credit in all versions of this file.
// Contact: info@heartucate.eu

using UnityEngine;
using UnityEngine.XR.ARSubsystems;

namespace Heartucate.TUD
{
    public class ImageLibraryEnsurer : MonoBehaviour
    {
        public XRReferenceImageLibrary imageLibrary;

        private void Start()
        {
            Debug.Log($"Ensuring image library is loaded... (Count: {imageLibrary.count})");
        }
    }
}
