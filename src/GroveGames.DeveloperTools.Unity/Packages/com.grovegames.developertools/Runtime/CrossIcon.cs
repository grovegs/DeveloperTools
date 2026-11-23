using UnityEngine;
using UnityEngine.UIElements;

namespace GroveGames.DeveloperTools.Unity
{
    public sealed class CrossIcon : VisualElement
    {
        private const float LineWidth = 6f;

        public CrossIcon(float size)
        {
            pickingMode = PickingMode.Ignore;
            style.width = size;
            style.height = size;
            generateVisualContent += HandleGenerateVisualContent;
        }

        private void HandleGenerateVisualContent(MeshGenerationContext context)
        {
            var rect = contentRect;
            var painter = context.painter2D;
            painter.lineWidth = LineWidth;
            painter.lineCap = LineCap.Round;
            painter.strokeColor = resolvedStyle.color;
            painter.BeginPath();
            painter.MoveTo(new Vector2(rect.xMin, rect.yMin));
            painter.LineTo(new Vector2(rect.xMax, rect.yMax));
            painter.MoveTo(new Vector2(rect.xMax, rect.yMin));
            painter.LineTo(new Vector2(rect.xMin, rect.yMax));
            painter.Stroke();
        }
    }
}
