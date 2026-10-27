// DegreeAdjusterManager.cs
// Developed by Heartucate GmbH, 2026.
// For Technische Universität Dresden (TUD). Use and modification permitted under agreed terms.
// Please retain this credit in all versions of this file.
// Contact: info@heartucate.eu

using System.Collections.Generic;
using UnityEngine;

namespace Heartucate.TUD
{
    public class DegreeAdjusterManager : MonoBehaviour
    {
        public static DegreeAdjusterManager Instance;
        public List<DegreeAdjuster> degreeAdjusters;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void SetAngle(int id, int angle)
        {
            degreeAdjusters[id].SetDegree(angle, true);
        }
    }
}