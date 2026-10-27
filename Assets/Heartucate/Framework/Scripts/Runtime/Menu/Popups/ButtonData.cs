using System;
using System.Threading.Tasks;

namespace Heartucate.Core.Presentation
{
    public class ButtonData
    {
        public enum ButtonStyle { Action, Warning, Discreet }
        public string Label { get; set; }
        public ButtonStyle Style { get; set; }
        public Func<Task> Action { get; set; }
        public bool IsExit { get; set; }

        public ButtonData(
            string label,
            ButtonStyle style,
            Func<Task> action,
            bool isExit = true
        )
        {
            Label = label;
            Style = style;
            Action = action;
            IsExit = isExit;
        }
    }
}
