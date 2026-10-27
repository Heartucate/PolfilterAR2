// DegreeDiagram.cs
// Developed by Heartucate GmbH, 2026.
// For Technische Universität Dresden (TUD). Use and modification permitted under agreed terms.
// Please retain this credit in all versions of this file.
// Contact: info@heartucate.eu

using UnityEngine;
using UnityEngine.UI;

namespace Heartucate.TUD
{
    public class DegreeDiagram : MonoBehaviour
    {
        [SerializeField] private Image _preDegree;
        [SerializeField] private Image _postDegree;
        [SerializeField] private GameObject _connectorRotator;
        [SerializeField] private Image _degreeConnector1;
        [SerializeField] private Image _degreeConnector2;
        [SerializeField] private Image _lot;

        [SerializeField] private Image _degreeSymbol1;
        [SerializeField] private Image _degreeSymbol2;
        [SerializeField] private Color _postColor;
        [SerializeField] private Color _preColor;
        [SerializeField] private Color _supportColor;
        [SerializeField] private bool _isFirst;
        [SerializeField] private bool _setPreScale;
        [SerializeField] private bool _getPreScale;
        [SerializeField] private Image _preScaleTarget;
        [SerializeField] private bool _scaleArrow;
        [SerializeField] private Transform _arrowTrans;

        private float _preScale;

        private void Start()
        {
            _preDegree.color = _preColor;
            _postDegree.color = _postColor;
            _degreeConnector1.color = _supportColor;
            _degreeConnector2.color = _supportColor;
            _degreeSymbol1.color = _supportColor;
            _degreeSymbol2.color = _supportColor;
            _lot.color = _supportColor;
        }

        public void SetDegreeDiagram(float degree1, float degree2, float scale)
        {
            if (!_isFirst) _preDegree.transform.localRotation = Quaternion.Euler(0, 0, 360f - degree1);
            _postDegree.transform.localRotation = Quaternion.Euler(0, 0, 360f - degree2);

            float ak = 1 * Mathf.Cos((degree2 - degree1) * Mathf.Deg2Rad);

            if (_scaleArrow)
            {
                _arrowTrans.localScale = new Vector3(1, 1, ak);
            }

            if (!_isFirst)
            {
                RectTransform postRT = _postDegree.rectTransform;
                postRT.sizeDelta = new Vector2(postRT.sizeDelta.x, Mathf.Abs(ak * 2f));

                if (_getPreScale)
                {
                    RectTransform rt = _preDegree.rectTransform;
                    _preScale = (rt.sizeDelta.y * 0.5f);

                    RectTransform postR = _postDegree.rectTransform;
                    postR.sizeDelta = new Vector2(postR.sizeDelta.x, Mathf.Abs((ak * 2f) * _preScale));
                }
            }

            if (_setPreScale)
            {
                RectTransform postRT = _preScaleTarget.rectTransform;
                postRT.sizeDelta = new Vector2(postRT.sizeDelta.x, Mathf.Abs(ak * 2f));
            }

            float gk = 1 * Mathf.Sin((degree2 - degree1) * Mathf.Deg2Rad);

            _connectorRotator.transform.localRotation = Quaternion.Euler(0, 0, 360f - degree2);

            if (_getPreScale)
            {
                Debug.Log("PRE SCALE:  " + _preScale);
                _degreeConnector1.transform.localScale = new Vector3(1.0f, gk * _preScale, 1.0f);
                _degreeConnector2.transform.localScale = new Vector3(1.0f, gk * _preScale, 1.0f);
                _degreeConnector1.transform.localPosition = new Vector3(0f, ak * _preScale, 0f);
                _degreeConnector2.transform.localPosition = new Vector3(0f, -ak * _preScale, 0f);
                _degreeSymbol1.transform.localPosition = new Vector3(0f, ak * _preScale, 0f);
                _degreeSymbol2.transform.localPosition = new Vector3(0f, -ak * _preScale, 0f);

                if (_scaleArrow)
                {
                    _arrowTrans.localScale = new Vector3(1, 1, ak * _preScale);
                }

            }
            else if (!_getPreScale)
            {
                _degreeConnector1.transform.localScale = new Vector3(1.0f, gk, 1.0f);
                _degreeConnector2.transform.localScale = new Vector3(1.0f, gk, 1.0f);
                _degreeConnector1.transform.localPosition = new Vector3(0f, ak, 0f);
                _degreeConnector2.transform.localPosition = new Vector3(0f, -ak, 0f);
                _degreeSymbol1.transform.localPosition = new Vector3(0f, ak, 0f);
                _degreeSymbol2.transform.localPosition = new Vector3(0f, -ak, 0f);
                if (_scaleArrow)
                {
                    _arrowTrans.localScale = new Vector3(1, 1, ak);
                }
            }

            float deg = (degree1 - degree2);
            Debug.Log("Winkel =  " + degree1 + " / " + degree2 + "  (" + deg + ")");

            if (deg >= 0 && deg <= 90)
            {
                _degreeSymbol1.transform.localRotation = Quaternion.Euler(0, 0, 180);
                _degreeSymbol2.transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
            if (deg >= 90 && deg <= 180)
            {
                _degreeSymbol1.transform.localRotation = Quaternion.Euler(0, 0, 270);
                _degreeSymbol2.transform.localRotation = Quaternion.Euler(0, 0, 90);
            }
            if (deg >= 180 && deg <= 270)
            {
                _degreeSymbol1.transform.localRotation = Quaternion.Euler(0, 0, 0);
                _degreeSymbol2.transform.localRotation = Quaternion.Euler(0, 0, 180);
            }
            if (deg >= 270 && deg <= 360)
            {
                _degreeSymbol1.transform.localRotation = Quaternion.Euler(0, 0, 90);
                _degreeSymbol2.transform.localRotation = Quaternion.Euler(0, 0, 270);
            }
            if (deg <= 0 && deg >= -90)
            {
                _degreeSymbol1.transform.localRotation = Quaternion.Euler(0, 0, 90);
                _degreeSymbol2.transform.localRotation = Quaternion.Euler(0, 0, -90);
            }
            if (deg <= -90 && deg >= -180)
            {
                _degreeSymbol1.transform.localRotation = Quaternion.Euler(0, 0, 0);
                _degreeSymbol2.transform.localRotation = Quaternion.Euler(0, 0, 180);
            }
            if (deg <= -180 && deg >= -270)
            {
                _degreeSymbol1.transform.localRotation = Quaternion.Euler(0, 0, 270);
                _degreeSymbol2.transform.localRotation = Quaternion.Euler(0, 0, 90);
            }
            if (deg <= -270 && deg >= -360)
            {
                _degreeSymbol1.transform.localRotation = Quaternion.Euler(0, 0, 180);
                _degreeSymbol2.transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
        }
    }
}