using System.Threading.Tasks;

namespace Heartucate.Core.Presentation
{
    /// <summary>
    /// Anything that can animate a popup in (intro) and out (outro).
    /// </summary>
    public interface IPopupAnimation
    {
        /// <summary>Animate the “in” (intro) sequence; completes when done.</summary>
        Task PlayIntroAsync();

        /// <summary>Animate the “out” (outro) sequence; completes when done.</summary>
        Task PlayOutroAsync();
    }
}
