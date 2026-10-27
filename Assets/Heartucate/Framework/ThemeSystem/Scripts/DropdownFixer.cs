using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace Heartucate
{
    public class DropdownFixer : MonoBehaviour
    {
        private void Start()
        {
            //StartCoroutine(FixDropDownCoroutine());
            FixDropDown();
        }

        private void OnDestroy()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent.GetComponent<RectTransform>());

        }

        IEnumerator FixDropDownCoroutine()
        {
            yield return new WaitForEndOfFrame();
            FixDropDown();
        }

        public void FixDropDown()
        {
            gameObject.SetActive(false);
            //Destroy(GetComponent<GraphicRaycaster>());
            //Destroy(GetComponent<Canvas>());
            gameObject.SetActive(true);
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent.GetComponent<RectTransform>());
        }
    }
}
