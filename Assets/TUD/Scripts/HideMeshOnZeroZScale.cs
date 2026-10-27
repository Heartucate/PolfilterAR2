// HideMeshOnZeroZScale.cs
// Developed by Heartucate GmbH, 2026.
// For Technische Universität Dresden (TUD). Use and modification permitted under agreed terms.
// Please retain this credit in all versions of this file.
// Contact: info@heartucate.eu


using UnityEngine;

namespace Heartucate.TUD
{
    public class HideMeshOnZeroZScale : MonoBehaviour
    {
        MeshRenderer _meshRenderer;
        [SerializeField] private float zHideThreshold = 0.001f;
        void Start()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
        }

        void LateUpdate()
        {
            float z = transform.localScale.z;
            _meshRenderer.enabled = Mathf.Abs(z) > zHideThreshold;
        }
    }
}