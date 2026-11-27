using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace GroveGames.DeveloperTools.Unity
{
    public sealed class DeveloperOptionRow
    {
        private const float ValueFieldWidth = 280f;
        private const float ToggleWidth = 200f;
        private const float TrackerHeight = 16f;
        private const float DraggerSize = 64f;
        private const string OnText = "On";
        private const string OffText = "Off";

        private readonly DeveloperOption _option;
        private readonly object _target;
        private readonly Action _changed;
        private readonly string _searchText;
        private Button? _toggle;
        private Slider? _slider;
        private SliderInt? _sliderInt;
        private IntegerField? _integerField;
        private FloatField? _floatField;
        private TextField? _textField;
        private Button[]? _enumButtons;
        private object[]? _enumValues;
        private object? _parsableValue;
        private bool _hasParsableValue;

        private readonly VisualElement _root;
        public VisualElement Root => _root;

        public DeveloperOptionRow(DeveloperOption option, object target, Action changed)
        {
            _option = option;
            _target = target;
            _changed = changed;
            _searchText = option.Tooltip == null ? option.Label : option.Label + " " + option.Tooltip;

            _root = new VisualElement();
            Root.style.paddingTop = DeveloperToolsStyle.Spacing;
            Root.style.paddingBottom = DeveloperToolsStyle.Spacing;

            if (option.Problem != null)
            {
                var problem = new Label($"{option.Label}: {option.Problem}");
                problem.style.fontSize = DeveloperToolsStyle.FontSize;
                problem.style.color = DeveloperToolsStyle.MutedTextColor;
                problem.style.whiteSpace = WhiteSpace.Normal;
                Root.Add(problem);
                return;
            }

            if (option.Kind == DeveloperOptionKind.Action)
            {
                var button = DeveloperToolsStyle.CreateButton(option.Label, HandleActionClicked);
                button.style.marginRight = 0f;
                Root.Add(button);

                if (option.Tooltip != null)
                {
                    var actionTooltip = new Label(option.Tooltip);
                    actionTooltip.style.fontSize = DeveloperToolsStyle.FontSize;
                    actionTooltip.style.color = DeveloperToolsStyle.MutedTextColor;
                    actionTooltip.style.whiteSpace = WhiteSpace.Normal;
                    Root.Add(actionTooltip);
                }

                return;
            }

            var header = DeveloperToolsStyle.CreateRow();
            header.style.marginBottom = 0f;
            header.style.minHeight = DeveloperToolsStyle.ControlHeight;
            var labels = new VisualElement();
            labels.style.flexGrow = 1f;
            labels.style.flexShrink = 1f;
            labels.style.marginRight = DeveloperToolsStyle.Spacing;
            var label = new Label(option.Label);
            label.style.fontSize = DeveloperToolsStyle.ControlFontSize;
            label.style.whiteSpace = WhiteSpace.Normal;
            labels.Add(label);

            if (option.Tooltip != null)
            {
                var tooltip = new Label(option.Tooltip);
                tooltip.style.fontSize = DeveloperToolsStyle.FontSize;
                tooltip.style.color = DeveloperToolsStyle.MutedTextColor;
                tooltip.style.whiteSpace = WhiteSpace.Normal;
                labels.Add(tooltip);
            }

            header.Add(labels);
            Root.Add(header);

            switch (option.Kind)
            {
                case DeveloperOptionKind.Toggle:
                    _toggle = DeveloperToolsStyle.CreateButton(OffText, HandleToggleClicked);
                    _toggle.style.width = ToggleWidth;
                    _toggle.style.marginRight = 0f;
                    header.Add(_toggle);
                    break;
                case DeveloperOptionKind.Slider:
                    CreateSlider(header);
                    break;
                case DeveloperOptionKind.Integer:
                    _integerField = CreateIntegerField();
                    header.Add(_integerField);
                    break;
                case DeveloperOptionKind.Float:
                    _floatField = CreateFloatField();
                    header.Add(_floatField);
                    break;
                case DeveloperOptionKind.Text:
                    _textField = new TextField { isDelayed = true };
                    _textField.RegisterValueChangedCallback(HandleTextChanged);
                    DeveloperToolsStyle.StyleInput(_textField);
                    Root.Add(_textField);
                    break;
                case DeveloperOptionKind.Enum:
                    CreateEnumButtons();
                    break;
                case DeveloperOptionKind.Parsable:
                    _textField = new TextField { isDelayed = true };
                    _textField.style.width = ValueFieldWidth;
                    _textField.RegisterValueChangedCallback(HandleParsableChanged);
                    DeveloperToolsStyle.StyleInput(_textField);
                    header.Add(_textField);
                    break;
            }
        }

        public bool Matches(string text)
        {
            return text.Length == 0 || _searchText.IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public void Read()
        {
            var value = _option.GetValue(_target);

            switch (_option.Kind)
            {
                case DeveloperOptionKind.Toggle:
                    var isOn = value is true;
                    _toggle!.text = isOn ? OnText : OffText;
                    _toggle.style.backgroundColor = isOn ? DeveloperToolsStyle.AccentColor : DeveloperToolsStyle.ControlColor;
                    break;
                case DeveloperOptionKind.Slider:
                    if (value is int integer)
                    {
                        _sliderInt!.SetValueWithoutNotify(integer);
                        _integerField!.SetValueWithoutNotify(integer);
                    }
                    else if (value is float number)
                    {
                        _slider!.SetValueWithoutNotify(number);
                        _floatField!.SetValueWithoutNotify(number);
                    }

                    break;
                case DeveloperOptionKind.Integer:
                    _integerField!.SetValueWithoutNotify(value is int integerValue ? integerValue : 0);
                    break;
                case DeveloperOptionKind.Float:
                    _floatField!.SetValueWithoutNotify(value is float floatValue ? floatValue : 0f);
                    break;
                case DeveloperOptionKind.Text:
                    _textField!.SetValueWithoutNotify(value as string ?? string.Empty);
                    break;
                case DeveloperOptionKind.Parsable:
                    if (_hasParsableValue && Equals(_parsableValue, value))
                    {
                        break;
                    }

                    _parsableValue = value;
                    _hasParsableValue = true;
                    _textField!.SetValueWithoutNotify(value?.ToString() ?? string.Empty);
                    break;
                case DeveloperOptionKind.Enum:
                    for (var i = 0; i < _enumButtons!.Length; i++)
                    {
                        var isSelected = Equals(_enumValues![i], value);
                        _enumButtons[i].style.backgroundColor = isSelected ? DeveloperToolsStyle.AccentColor : DeveloperToolsStyle.ControlColor;
                    }

                    break;
            }
        }

        private void CreateSlider(VisualElement header)
        {
            if (_option.ValueType == typeof(int))
            {
                _integerField = CreateIntegerField();
                header.Add(_integerField);
                _sliderInt = new SliderInt((int)Math.Ceiling(_option.Min), (int)Math.Floor(_option.Max));
                _sliderInt.RegisterValueChangedCallback(HandleIntegerChanged);
                StyleSlider(_sliderInt);
                Root.Add(_sliderInt);
                return;
            }

            _floatField = CreateFloatField();
            header.Add(_floatField);
            _slider = new Slider(_option.Min, _option.Max);
            _slider.RegisterValueChangedCallback(HandleFloatChanged);
            StyleSlider(_slider);
            Root.Add(_slider);
        }

        private IntegerField CreateIntegerField()
        {
            var field = new IntegerField { isDelayed = true };
            field.style.width = ValueFieldWidth;
            field.RegisterValueChangedCallback(HandleIntegerChanged);
            DeveloperToolsStyle.StyleInput(field);
            return field;
        }

        private FloatField CreateFloatField()
        {
            var field = new FloatField { isDelayed = true };
            field.style.width = ValueFieldWidth;
            field.RegisterValueChangedCallback(HandleFloatChanged);
            DeveloperToolsStyle.StyleInput(field);
            return field;
        }

        private void CreateEnumButtons()
        {
            var values = Enum.GetValues(_option.ValueType);
            var names = Enum.GetNames(_option.ValueType);
            var row = DeveloperToolsStyle.CreateRow();
            row.style.flexWrap = Wrap.Wrap;
            row.style.marginBottom = 0f;
            _enumValues = new object[values.Length];
            _enumButtons = new Button[values.Length];

            for (var i = 0; i < values.Length; i++)
            {
                var value = values.GetValue(i)!;
                _enumValues[i] = value;
                var button = DeveloperToolsStyle.CreateButton(DeveloperOptionsReader.Nicify(names[i]), () => SetValue(value));
                button.style.marginTop = DeveloperToolsStyle.Spacing;
                _enumButtons[i] = button;
                row.Add(button);
            }

            Root.Add(row);
        }

        private static void StyleSlider<TValue>(BaseSlider<TValue> slider) where TValue : IComparable<TValue>
        {
            slider.style.height = DeveloperToolsStyle.ControlHeight;
            slider.style.marginLeft = DraggerSize * 0.5f;
            slider.style.marginRight = DraggerSize * 0.5f;
            var dragContainer = slider.Q(className: BaseSlider<TValue>.dragContainerUssClassName);

            if (dragContainer != null)
            {
                dragContainer.style.height = DeveloperToolsStyle.ControlHeight;
            }

            var tracker = slider.Q(className: BaseSlider<TValue>.trackerUssClassName);

            if (tracker != null)
            {
                tracker.style.height = TrackerHeight;
                tracker.style.marginTop = -TrackerHeight * 0.5f;
                tracker.style.backgroundColor = DeveloperToolsStyle.ControlColor;
                DeveloperToolsStyle.SetBorderWidth(tracker.style, 0f);
                DeveloperToolsStyle.SetBorderRadius(tracker.style, TrackerHeight * 0.5f);
            }

            var dragger = slider.Q(className: BaseSlider<TValue>.draggerUssClassName);

            if (dragger != null)
            {
                dragger.style.width = DraggerSize;
                dragger.style.height = DraggerSize;
                dragger.style.marginTop = -DraggerSize * 0.5f;
                dragger.style.marginLeft = -DraggerSize * 0.5f;
                dragger.style.backgroundColor = DeveloperToolsStyle.AccentColor;
                DeveloperToolsStyle.SetBorderWidth(dragger.style, 0f);
                DeveloperToolsStyle.SetBorderRadius(dragger.style, DraggerSize * 0.5f);
            }
        }

        private void HandleActionClicked()
        {
            _option.Invoke(_target);
            _changed();
        }

        private void HandleToggleClicked()
        {
            SetValue(_option.GetValue(_target) is not true);
        }

        private void HandleIntegerChanged(ChangeEvent<int> changeEvent)
        {
            var value = changeEvent.newValue;

            if (value < (double)_option.Min)
            {
                value = (int)Math.Ceiling(_option.Min);
            }
            else if (value > (double)_option.Max)
            {
                value = (int)Math.Floor(_option.Max);
            }

            SetValue(value);
        }

        private void HandleFloatChanged(ChangeEvent<float> changeEvent)
        {
            SetValue(Mathf.Clamp(changeEvent.newValue, _option.Min, _option.Max));
        }

        private void HandleTextChanged(ChangeEvent<string> changeEvent)
        {
            SetValue(changeEvent.newValue);
        }

        private void HandleParsableChanged(ChangeEvent<string> changeEvent)
        {
            if (_option.TryParse(changeEvent.newValue ?? string.Empty, out var value))
            {
                _hasParsableValue = false;
                SetValue(value);
                return;
            }

            _hasParsableValue = false;
            Read();
        }

        private void SetValue(object? value)
        {
            _option.SetValue(_target, value);
            _changed();
        }
    }
}
