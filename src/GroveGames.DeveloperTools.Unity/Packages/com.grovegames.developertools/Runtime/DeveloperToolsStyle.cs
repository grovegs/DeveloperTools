using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace GroveGames.DeveloperTools.Unity
{
    public static class DeveloperToolsStyle
    {
        public const float Spacing = 16f;
        public const float LargeSpacing = 32f;
        public const float ControlHeight = 120f;
        public const float RowHeight = 88f;
        public const float FontSize = 32f;
        public const float ControlFontSize = 40f;
        public const float TitleFontSize = 48f;
        public const float CornerRadius = 24f;
        private const float PressedScale = 0.94f;
        private const int PressDurationMilliseconds = 80;

        public static readonly Color BackgroundColor = new(0.06f, 0.07f, 0.09f, 0.97f);
        public static readonly Color SurfaceColor = new(0.12f, 0.13f, 0.16f, 1f);
        public static readonly Color ControlColor = new(0.2f, 0.22f, 0.27f, 1f);
        public static readonly Color AccentColor = new(0f, 0.53f, 0.24f, 1f);
        public static readonly Color TextColor = new(0.93f, 0.94f, 0.96f, 1f);
        public static readonly Color MutedTextColor = new(0.62f, 0.66f, 0.72f, 1f);

        private static readonly EventCallback<PointerDownEvent> s_pointerDownHandler = HandlePointerDown;
        private static readonly EventCallback<PointerUpEvent> s_pointerUpHandler = HandlePointerUp;
        private static readonly EventCallback<PointerLeaveEvent> s_pointerLeaveHandler = HandlePointerLeave;
        private static readonly EventCallback<PointerCancelEvent> s_pointerCancelHandler = HandlePointerCancel;
        private static readonly EventCallback<PointerDownEvent> s_highlightDownHandler = HandleHighlightDown;
        private static readonly EventCallback<PointerUpEvent> s_highlightUpHandler = HandleHighlightUp;
        private static readonly EventCallback<PointerLeaveEvent> s_highlightLeaveHandler = HandleHighlightLeave;
        private static readonly EventCallback<PointerCancelEvent> s_highlightCancelHandler = HandleHighlightCancel;

        public static Button CreateButton(string text, Action clicked)
        {
            var button = new Button(clicked) { text = text };
            button.style.height = ControlHeight;
            button.style.minWidth = ControlHeight;
            button.style.fontSize = ControlFontSize;
            button.style.color = TextColor;
            button.style.backgroundColor = ControlColor;
            button.style.paddingLeft = LargeSpacing;
            button.style.paddingRight = LargeSpacing;
            button.style.marginLeft = 0f;
            button.style.marginRight = Spacing;
            SetBorderWidth(button.style, 0f);
            SetBorderRadius(button.style, CornerRadius);
            AddPressFeedback(button);
            return button;
        }

        public static void AddPressFeedback(VisualElement element)
        {
            SetTransition(element, "scale", PressDurationMilliseconds);
            element.RegisterCallback(s_pointerDownHandler, TrickleDown.TrickleDown);
            element.RegisterCallback(s_pointerUpHandler, TrickleDown.TrickleDown);
            element.RegisterCallback(s_pointerLeaveHandler);
            element.RegisterCallback(s_pointerCancelHandler, TrickleDown.TrickleDown);
        }

        public static void AddHighlightFeedback(VisualElement element)
        {
            SetTransition(element, "background-color", PressDurationMilliseconds);
            element.RegisterCallback(s_highlightDownHandler, TrickleDown.TrickleDown);
            element.RegisterCallback(s_highlightUpHandler, TrickleDown.TrickleDown);
            element.RegisterCallback(s_highlightLeaveHandler);
            element.RegisterCallback(s_highlightCancelHandler, TrickleDown.TrickleDown);
        }

        public static void SetTransition(VisualElement element, string property, int durationMilliseconds)
        {
            element.style.transitionProperty = new List<StylePropertyName> { new(property) };
            element.style.transitionDuration = new List<TimeValue> { new(durationMilliseconds, TimeUnit.Millisecond) };
            element.style.transitionTimingFunction = new List<EasingFunction> { new(EasingMode.EaseOut) };
        }

        public static void StyleInput<TValue>(TextInputBaseField<TValue> field)
        {
            field.style.marginLeft = 0f;
            field.style.marginRight = 0f;
            var input = field.Q(className: TextInputBaseField<TValue>.inputUssClassName);

            if (input == null)
            {
                return;
            }

            input.style.minHeight = ControlHeight;
            input.style.fontSize = ControlFontSize;
            input.style.color = TextColor;
            input.style.backgroundColor = ControlColor;
            input.style.paddingLeft = LargeSpacing;
            input.style.paddingRight = LargeSpacing;
            SetBorderWidth(input.style, 0f);
            SetBorderRadius(input.style, CornerRadius);
        }

        public static void ApplyBold(VisualElement element, Font? boldFont)
        {
            if (boldFont != null)
            {
                element.style.unityFontDefinition = new StyleFontDefinition(FontDefinition.FromFont(boldFont));
                return;
            }

            element.style.unityFontStyleAndWeight = FontStyle.Bold;
        }

        public static VisualElement CreateRow()
        {
            var row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.alignItems = Align.Center;
            row.style.flexShrink = 0f;
            row.style.marginBottom = Spacing;
            return row;
        }

        public static VisualElement CreateSpacer()
        {
            var spacer = new VisualElement();
            spacer.style.flexGrow = 1f;
            return spacer;
        }

        public static void SetBorderWidth(IStyle style, float width)
        {
            style.borderLeftWidth = width;
            style.borderRightWidth = width;
            style.borderTopWidth = width;
            style.borderBottomWidth = width;
        }

        public static void SetBorderRadius(IStyle style, float radius)
        {
            style.borderTopLeftRadius = radius;
            style.borderTopRightRadius = radius;
            style.borderBottomLeftRadius = radius;
            style.borderBottomRightRadius = radius;
        }

        public static void SetBorderColor(IStyle style, Color color)
        {
            style.borderLeftColor = color;
            style.borderRightColor = color;
            style.borderTopColor = color;
            style.borderBottomColor = color;
        }

        private static void HandlePointerDown(PointerDownEvent pointerEvent)
        {
            SetPressed(pointerEvent.currentTarget, true);
        }

        private static void HandlePointerUp(PointerUpEvent pointerEvent)
        {
            SetPressed(pointerEvent.currentTarget, false);
        }

        private static void HandlePointerLeave(PointerLeaveEvent pointerEvent)
        {
            SetPressed(pointerEvent.currentTarget, false);
        }

        private static void HandlePointerCancel(PointerCancelEvent pointerEvent)
        {
            SetPressed(pointerEvent.currentTarget, false);
        }

        private static void HandleHighlightDown(PointerDownEvent pointerEvent)
        {
            SetHighlighted(pointerEvent.currentTarget, true);
        }

        private static void HandleHighlightUp(PointerUpEvent pointerEvent)
        {
            SetHighlighted(pointerEvent.currentTarget, false);
        }

        private static void HandleHighlightLeave(PointerLeaveEvent pointerEvent)
        {
            SetHighlighted(pointerEvent.currentTarget, false);
        }

        private static void HandleHighlightCancel(PointerCancelEvent pointerEvent)
        {
            SetHighlighted(pointerEvent.currentTarget, false);
        }

        private static void SetHighlighted(IEventHandler target, bool isHighlighted)
        {
            if (target is VisualElement element)
            {
                element.style.backgroundColor = isHighlighted ? ControlColor : Color.clear;
            }
        }

        private static void SetPressed(IEventHandler target, bool isPressed)
        {
            if (target is not VisualElement element)
            {
                return;
            }

            var scale = isPressed ? PressedScale : 1f;
            element.style.scale = new Scale(new Vector3(scale, scale, 1f));
        }
    }
}
