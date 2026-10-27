using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Utility
{
    /// <summary>
    /// Extends TMP_InputField with additional functionality:
    /// - Route drag events to its parent ScrollRect. If available, this disables the ability to select text in editor.
    /// </summary>
    public class TMP_InputFieldEx : TMP_InputField
    {
        [SerializeField] private ScrollRect m_parentScrollRect;

        protected override void OnEnable()
        {
            m_parentScrollRect ??= GetComponentInParent<ScrollRect>();
            base.OnEnable();
        }

        public override void OnBeginDrag(PointerEventData p_eventData)
        {
            if (m_parentScrollRect)
            {
                m_parentScrollRect.OnBeginDrag(p_eventData);
            }
            else
            {
                base.OnBeginDrag(p_eventData);
            }
        }

        public override void OnDrag(PointerEventData p_eventData)
        {
            if (m_parentScrollRect)
            {
                m_parentScrollRect.OnDrag(p_eventData);
            }
            else
            {
                base.OnDrag(p_eventData);
            }
        }

        public override void OnEndDrag(PointerEventData p_eventData)
        {
            if (m_parentScrollRect)
            {
                m_parentScrollRect.OnEndDrag(p_eventData);
            }
            else
            {
                base.OnEndDrag(p_eventData);
            }
        }
    }
}