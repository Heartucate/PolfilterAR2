// ShaderColorChanger.cs
// Developed by Heartucate GmbH, 2026.
// For Technische Universität Dresden (TUD). Use and modification permitted under agreed terms.
// Please retain this credit in all versions of this file.
// Contact: info@heartucate.eu

using UnityEngine;

namespace Heartucate.TUD
{
    public class ShaderColorChanger : MonoBehaviour
    {
        [SerializeField] private bool _singleArrow;
        [ColorUsage(true, true)]
        [SerializeField] private Color _arrowColor;
        private Renderer rend;
        private MaterialPropertyBlock mpb;

        private void Start()
        {
            if (_singleArrow) SetShaderColor(_arrowColor);
        }

        public void SetShaderColor(Color col)
        {
            if (rend == null)
                rend = GetComponent<Renderer>();

            if (mpb == null)
                mpb = new MaterialPropertyBlock();

            rend.GetPropertyBlock(mpb);

            mpb.SetColor("_BaseColor", col);

            rend.SetPropertyBlock(mpb);
        }
    }
}