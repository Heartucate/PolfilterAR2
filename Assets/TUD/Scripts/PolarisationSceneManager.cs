// PolarisationSceneManager.cs
// Developed by Heartucate GmbH, 2026.
// For Technische Universität Dresden (TUD). Use and modification permitted under agreed terms.
// Please retain this credit in all versions of this file.
// Contact: info@heartucate.eu

using UnityEngine;

namespace Heartucate.TUD
{
    public class PolarisationSceneManager : MonoBehaviour
    {
        //[Header("Start Game Mode")]
        //[SerializeField] private GameState.GameMode startMode;
        [Header("Start Level")]
        [SerializeField] private GameState.Level startLevel;

        void Awake()
        {
            //GameState.CurrentMode = startMode;
            GameState.CurrentLevel = startLevel;
        }
        void Start()
        {
            Debug.Log("Gamemode: " + GameState.CurrentMode + "    |   Level: " + GameState.CurrentLevel);
        }
    }
}