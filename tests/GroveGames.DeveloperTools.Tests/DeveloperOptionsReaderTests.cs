
namespace GroveGames.DeveloperTools.Tests;

public sealed class DeveloperOptionsReaderTests
{
    [Fact]
    public void Read_SampleOptions_ReturnsMarkedFieldsThenMethods()
    {
        var options = DeveloperOptionsReader.Read(typeof(FakeDeveloperOptions));

        Assert.Equal(10, options.Count);
        Assert.Equal("Readonly Value", options[0].Label);
        Assert.Equal("God Mode", options[1].Label);
        Assert.Equal("Address", options[6].Label);
        Assert.Equal("Version", options[7].Label);
        Assert.Equal("Run Action", options[8].Label);
        Assert.Equal("Add", options[9].Label);
    }

    [Fact]
    public void Read_LabelArgument_OverridesMemberName()
    {
        var options = DeveloperOptionsReader.Read(typeof(FakeDeveloperOptions));

        Assert.Equal("Coins", options[3].Label);
    }

    [Fact]
    public void Read_SampleOptions_MapsMembersToKinds()
    {
        var options = DeveloperOptionsReader.Read(typeof(FakeDeveloperOptions));

        Assert.Equal(DeveloperOptionKind.Toggle, options[1].Kind);
        Assert.Equal(DeveloperOptionKind.Slider, options[2].Kind);
        Assert.Equal(DeveloperOptionKind.Integer, options[3].Kind);
        Assert.Equal(DeveloperOptionKind.Text, options[4].Kind);
        Assert.Equal(DeveloperOptionKind.Enum, options[5].Kind);
        Assert.Equal(DeveloperOptionKind.Parsable, options[7].Kind);
        Assert.Equal(DeveloperOptionKind.Action, options[8].Kind);
    }

    [Fact]
    public void Read_MembersThatCannotBeShown_ReportProblems()
    {
        var options = DeveloperOptionsReader.Read(typeof(FakeDeveloperOptions));

        Assert.Equal(DeveloperOptionKind.Unsupported, options[0].Kind);
        Assert.Equal("Read-only fields are not supported", options[0].Problem);
        Assert.Equal("Uri is not supported", options[6].Problem);
        Assert.Equal(DeveloperOptionKind.Unsupported, options[9].Kind);
        Assert.Equal("Methods with parameters are not supported", options[9].Problem);
        Assert.Null(options[1].Problem);
    }

    [Fact]
    public void Read_FieldsAfterHeader_CarryHeaderAndTooltip()
    {
        var options = DeveloperOptionsReader.Read(typeof(FakeDeveloperOptions));

        Assert.Null(options[0].Header);
        Assert.Equal("Gameplay", options[1].Header);
        Assert.Equal("Turns god mode on.", options[1].Tooltip);
        Assert.Equal("Gameplay", options[3].Header);
        Assert.Null(options[3].Tooltip);
    }

    [Fact]
    public void Read_RangeAndMin_SetLimits()
    {
        var options = DeveloperOptionsReader.Read(typeof(FakeDeveloperOptions));

        Assert.Equal(0f, options[2].Min);
        Assert.Equal(10f, options[2].Max);
        Assert.Equal(1f, options[3].Min);
        Assert.Equal(float.PositiveInfinity, options[3].Max);
    }

    [Fact]
    public void SetValue_Field_WritesValueAndCallsOnValidate()
    {
        var target = new FakeDeveloperOptions();
        var timeScale = DeveloperOptionsReader.Read(typeof(FakeDeveloperOptions))[2];

        timeScale.SetValue(target, 4f);

        Assert.Equal(4f, target.TimeScale);
        Assert.Equal(4f, timeScale.GetValue(target));
        Assert.Equal(1, target.ValidateCount);
    }

    [Fact]
    public void Invoke_Action_CallsMethod()
    {
        var target = new FakeDeveloperOptions();
        var action = DeveloperOptionsReader.Read(typeof(FakeDeveloperOptions))[8];

        action.Invoke(target);

        Assert.Equal(1, target.ActionCount);
        Assert.Equal(0, target.ValidateCount);
    }

    [Fact]
    public void TryParse_ParsableField_UsesTypeTryParse()
    {
        var target = new FakeDeveloperOptions();
        var version = DeveloperOptionsReader.Read(typeof(FakeDeveloperOptions))[7];

        var isParsed = version.TryParse("2.5", out var value);
        version.SetValue(target, value);

        Assert.True(isParsed);
        Assert.Equal(new Version(2, 5), target.Version);
    }

    [Fact]
    public void TryParse_InvalidText_ReturnsFalse()
    {
        var version = DeveloperOptionsReader.Read(typeof(FakeDeveloperOptions))[7];

        var isParsed = version.TryParse("not a version", out var value);

        Assert.False(isParsed);
        Assert.Null(value);
    }

    [Theory]
    [InlineData("_rewardMultiplier", "Reward Multiplier")]
    [InlineData("m_timeScale", "Time Scale")]
    [InlineData("s_count", "Count")]
    [InlineData("AddMoney", "Add Money")]
    [InlineData("Level2Boss", "Level2 Boss")]
    [InlineData("HPBar", "HPBar")]
    public void Nicify_Name_ReturnsReadableLabel(string name, string expected)
    {
        var label = DeveloperOptionsReader.Nicify(name);

        Assert.Equal(expected, label);
    }

    [Fact]
    public void Read_StaticMembers_ReportProblems()
    {
        var options = DeveloperOptionsReader.Read(typeof(FakeStaticDeveloperOptions));

        Assert.Equal(2, options.Count);
        Assert.Equal(DeveloperOptionKind.Unsupported, options[0].Kind);
        Assert.Equal("Static fields are not supported", options[0].Problem);
        Assert.Equal(DeveloperOptionKind.Unsupported, options[1].Kind);
        Assert.Equal("Static methods are not supported", options[1].Problem);
    }

#pragma warning disable IDE0044, IDE0051, IDE0060, CS0649
    private sealed class FakeStaticDeveloperOptions : IDeveloperOptions
    {
        [DeveloperOption] private static int s_count;

        public string Category => "Static";

        public static int Count => s_count;

        [DeveloperOption]
        private static void ResetCount()
        {
            s_count = 0;
        }
    }

    private sealed class FakeDeveloperOptions : IDeveloperOptions
    {
        [DeveloperOption] private readonly int _readonlyValue;

        [DeveloperOption(Header = "Gameplay", Tooltip = "Turns god mode on.")] private bool _godMode;
        [DeveloperOption(Min = 0f, Max = 10f)] private float _timeScale = 1f;
        [DeveloperOption("Coins", Min = 1f)] private int _amount = 1;
        [DeveloperOption] private string _playerName = string.Empty;
        [DeveloperOption] private DayOfWeek _day;
        [DeveloperOption] private Uri? _address;
        [DeveloperOption] private Version _version = new(1, 0);
        private int _plainValue;

        private int _validateCount;
        private int _actionCount;
        public string Category => "Sample";
        public int ValidateCount => _validateCount;
        public int ActionCount => _actionCount;
        public float TimeScale => _timeScale;
        public Version Version => _version;

        public string Describe()
        {
            return $"{_readonlyValue} {_godMode} {_amount} {_playerName} {_day} {_address} {_plainValue}";
        }

        public void SetPlainValue(int value)
        {
            _plainValue = value;
        }

        [DeveloperOption]
        private void RunAction()
        {
            _actionCount++;
        }

        [DeveloperOption("Add")]
        private void AddAction(int value)
        {
            _actionCount += value;
        }

        private void OnValidate()
        {
            _validateCount++;
        }
    }
#pragma warning restore IDE0044, IDE0051, IDE0060, CS0649
}
