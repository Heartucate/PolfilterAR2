// GameState.cs
// Developed by Heartucate GmbH, 2026.
// For Technische Universität Dresden (TUD). Use and modification permitted under agreed terms.
// Please retain this credit in all versions of this file.
// Contact: info@heartucate.eu

namespace Heartucate.TUD
{
    public static class GameState
    {
        public enum GameMode
        {
            NONE,
            HYBRID,
            FULLAR,
            SIMULATION
        }

        public static GameMode CurrentMode = GameMode.SIMULATION;

        public enum Level
        {
            LEVEL1,
            LEVEL2,
            Level3
        }

        public static Level CurrentLevel = Level.LEVEL1;
    }
}