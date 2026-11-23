using UnityEngine;
using UnityEngine.UIElements;

namespace GroveGames.DeveloperTools.Unity
{
    public sealed class ChevronIcon : VisualElement
    {
        private const float LineWidth = 6f;
        private const float CollapsedAngle = -90f;
        private const int RotateDurationMilliseconds = 150;

        public ChevronIcon(float size)
        {
            pickingMode = PickingMode.Ignore;
            style.width = size;
            style.height = size;
            DeveloperToolsStyle.SetTransition(this, "rotate", RotateDurationMilliseconds);
            generateVisualContent += HandleGenerateVisualContent;
        }

        public void SetExpanded(bool isExpanded)
        {
            style.rotate = new Rotate(isExpanded ? 0f : CollapsedAngle);
        }

        private void HandleGenerateVisualContent(MeshGenerationContext context)
        {
            var rect = contentRect;
            var painter = context.painter2D;
            painter.lineWidth = LineWidth;
            painter.lineCap = LineCap.Round;
            painter.lineJoin = LineJoin.Round;
            painter.strokeColor = resolvedStyle.color;
            painter.BeginPath();
            painter.MoveTo(new Vector2(rect.xMin, rect.yMin + rect.height * 0.25f));
            painter.LineTo(new Vector2(rect.center.x, rect.yMax - rect.height * 0.25f));
            painter.LineTo(new Vector2(rect.xMax, rect.yMin + rect.height * 0.25f));
            painter.Stroke();
        }
    }
}
