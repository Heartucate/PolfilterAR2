using UnityEngine;

namespace Heartucate.Tracking
{
    public class CalibrationTarget : MonoBehaviour
    {
        public static CalibrationTarget instance;

        private void Awake()
        {
            instance = this;
        }

        private void OnDestroy()
        {
            if (instance == this)
                instance = null;
        }
    }
}