using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using System.Collections.Generic;

namespace AvaloniaAnimationTest
{
    public class MyCustomControl : Control
    {
        private readonly Stack<DrawingContext.PushedState> _pushedStates = new Stack<DrawingContext.PushedState>();

        public override void Render(DrawingContext context)
        {
            context.DrawRectangle(new SolidColorBrush(Colors.Green), null, new Rect(Bounds.Size));

            using (context.PushClip(new Rect(Bounds.Size)))
            {
                _pushedStates.Push(context.PushTransform(Matrix.Identity));
                _pushedStates.Push(context.PushClip(new Rect(Bounds.Size)));
                _pushedStates.Push(context.PushOpacity(1, new Rect(Bounds.Size)));

                // If you uncommment below drawing code, everything works
                //SolidColorBrush brush = new SolidColorBrush(Colors.Red);
                //IPen pen = new Pen(brush, 2);
                //context.DrawLine(pen, new Point(0, 0), new Point(300, 300));

                _pushedStates.Pop().Dispose();
                _pushedStates.Pop().Dispose(); // < this here fails
                _pushedStates.Pop().Dispose();
            }
        }
    }
}
