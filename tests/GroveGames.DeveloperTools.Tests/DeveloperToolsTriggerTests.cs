using System.Numerics;

namespace GroveGames.DeveloperTools.Tests;

public sealed class DeveloperToolsTriggerTests
{
    private const float SafeAreaTop = 1920f;
    private const float CornerSize = 162f;

    private static readonly Vector2 s_topLeft = new(40f, 1880f);
    private static readonly Vector2 s_center = new(540f, 960f);

    [Fact]
    public void Constructor_NullWindow_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new DeveloperToolsTrigger(null!, 3, 0.5f));
    }

    [Fact]
    public void PressToggleKey_ClosedWindow_OpensIt()
    {
        var window = new TestWindow();
        var trigger = new DeveloperToolsTrigger(window, 3, 0.5f);

        trigger.PressToggleKey();

        Assert.True(window.IsOpen);
    }

    [Fact]
    public void PressToggleKey_OpenWindow_ClosesIt()
    {
        var window = new TestWindow();
        var trigger = new DeveloperToolsTrigger(window, 3, 0.5f);
        window.Open();

        trigger.PressToggleKey();

        Assert.False(window.IsOpen);
    }

    [Fact]
    public void PressPointer_EnoughQuickTapsInCorner_OpensTheWindow()
    {
        var window = new TestWindow();
        var trigger = new DeveloperToolsTrigger(window, 3, 0.5f);

        trigger.PressPointer(s_topLeft, 0f, SafeAreaTop, CornerSize, 0f);
        trigger.PressPointer(s_topLeft, 0f, SafeAreaTop, CornerSize, 0.2f);
        var openAfterSecond = window.IsOpen;
        trigger.PressPointer(s_topLeft, 0f, SafeAreaTop, CornerSize, 0.4f);

        Assert.False(openAfterSecond);
        Assert.True(window.IsOpen);
    }

    [Fact]
    public void PressPointer_TapBeforeTheLast_ShowsTheHintWithProgress()
    {
        var window = new TestWindow();
        var trigger = new DeveloperToolsTrigger(window, 3, 0.5f);

        trigger.PressPointer(s_topLeft, 0f, SafeAreaTop, CornerSize, 0f);
        trigger.PressPointer(s_topLeft, 0f, SafeAreaTop, CornerSize, 0.2f);

        Assert.Equal(2, window.HintCount);
        Assert.Equal(2, window.HintTaps);
        Assert.Equal(3, window.HintRequiredTaps);
        Assert.Equal(new Vector2(162f, 1758f), window.HintCorner);
        Assert.Equal(CornerSize, window.HintSize);
    }

    [Fact]
    public void PressPointer_LastTap_OpensWithoutAHint()
    {
        var window = new TestWindow();
        var trigger = new DeveloperToolsTrigger(window, 1, 0.5f);

        trigger.PressPointer(s_topLeft, 0f, SafeAreaTop, CornerSize, 0f);

        Assert.True(window.IsOpen);
        Assert.Equal(0, window.HintCount);
    }

    [Fact]
    public void PressPointer_OutsideCorner_ResetsTheTaps()
    {
        var window = new TestWindow();
        var trigger = new DeveloperToolsTrigger(window, 3, 0.5f);

        trigger.PressPointer(s_topLeft, 0f, SafeAreaTop, CornerSize, 0f);
        trigger.PressPointer(s_topLeft, 0f, SafeAreaTop, CornerSize, 0.1f);
        trigger.PressPointer(s_center, 0f, SafeAreaTop, CornerSize, 0.2f);
        trigger.PressPointer(s_topLeft, 0f, SafeAreaTop, CornerSize, 0.3f);

        Assert.False(window.IsOpen);
        Assert.Equal(1, window.HintTaps);
    }

    [Fact]
    public void PressPointer_OpenWindow_DoesNothing()
    {
        var window = new TestWindow();
        var trigger = new DeveloperToolsTrigger(window, 1, 0.5f);
        window.Open();

        trigger.PressPointer(s_topLeft, 0f, SafeAreaTop, CornerSize, 0f);

        Assert.Equal(1, window.OpenCount);
        Assert.Equal(0, window.HintCount);
    }

    private sealed class TestWindow : IDeveloperToolsWindow
    {
        private bool _isOpen;
        private int _openCount;
        private int _hintCount;
        private int _hintTaps;
        private int _hintRequiredTaps;
        private Vector2 _hintCorner;
        private float _hintSize;

        public bool IsOpen => _isOpen;
        public int OpenCount => _openCount;
        public int HintCount => _hintCount;
        public int HintTaps => _hintTaps;
        public int HintRequiredTaps => _hintRequiredTaps;
        public Vector2 HintCorner => _hintCorner;
        public float HintSize => _hintSize;

        public void Open()
        {
            _isOpen = true;
            _openCount++;
        }

        public void Close()
        {
            _isOpen = false;
        }

        public void Toggle()
        {
            if (_isOpen)
            {
                Close();
            }
            else
            {
                Open();
            }
        }

        public void ShowTapHint(Vector2 corner, float size, int taps, int requiredTaps)
        {
            _hintCount++;
            _hintCorner = corner;
            _hintSize = size;
            _hintTaps = taps;
            _hintRequiredTaps = requiredTaps;
        }
    }
}
