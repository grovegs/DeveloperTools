using System.Numerics;

namespace GroveGames.DeveloperTools.Tests;

public sealed class CornerTapGestureTests
{
    private const float CornerSize = 162f;

    private static readonly Vector2 s_corner = CornerTapGesture.GetCorner(0f, 1920f, CornerSize);
    private const float NotchedSafeAreaTop = 1800f;
    private static readonly Vector2 s_notchedCorner = CornerTapGesture.GetCorner(0f, NotchedSafeAreaTop, CornerSize);
    private static readonly Vector2 s_topLeft = new(40f, 1880f);

    [Fact]
    public void Tap_ThreeQuickTapsInTopLeft_ReturnsTrueOnThird()
    {
        var gesture = new CornerTapGesture(3, 0.5f);

        var first = gesture.Tap(s_topLeft, s_corner, 0f);
        var second = gesture.Tap(s_topLeft, s_corner, 0.2f);
        var third = gesture.Tap(s_topLeft, s_corner, 0.4f);

        Assert.False(first);
        Assert.False(second);
        Assert.True(third);
    }

    [Fact]
    public void Tap_SlowTaps_DoesNotOpen()
    {
        var gesture = new CornerTapGesture(3, 0.5f);

        gesture.Tap(s_topLeft, s_corner, 0f);
        gesture.Tap(s_topLeft, s_corner, 1f);
        var third = gesture.Tap(s_topLeft, s_corner, 2f);

        Assert.False(third);
    }

    [Fact]
    public void Tap_OutsideCorner_ResetsCount()
    {
        var gesture = new CornerTapGesture(3, 0.5f);

        gesture.Tap(s_topLeft, s_corner, 0f);
        gesture.Tap(s_topLeft, s_corner, 0.1f);
        gesture.Tap(new Vector2(540f, 960f), s_corner, 0.2f);
        var afterReset = gesture.Tap(s_topLeft, s_corner, 0.3f);

        Assert.False(afterReset);
    }

    [Fact]
    public void Tap_InCorner_CountsTapsUntilOpened()
    {
        var gesture = new CornerTapGesture(3, 0.5f);

        gesture.Tap(s_topLeft, s_corner, 0f);
        var afterFirst = gesture.Taps;
        gesture.Tap(s_topLeft, s_corner, 0.1f);
        var afterSecond = gesture.Taps;
        gesture.Tap(s_topLeft, s_corner, 0.2f);

        Assert.Equal(1, afterFirst);
        Assert.Equal(2, afterSecond);
        Assert.Equal(0, gesture.Taps);
        Assert.Equal(3, gesture.RequiredTaps);
    }

    [Fact]
    public void Tap_BottomLeft_IsNotTheCorner()
    {
        var gesture = new CornerTapGesture(1, 0.5f);

        var opened = gesture.Tap(new Vector2(40f, 40f), s_corner, 0f);

        Assert.False(opened);
    }

    [Fact]
    public void GetCorner_NotchedSafeArea_StartsAtSafeAreaTopLeft()
    {
        var corner = CornerTapGesture.GetCorner(0f, NotchedSafeAreaTop, CornerSize);

        Assert.Equal(162f, corner.X, 0.001f);
        Assert.Equal(1638f, corner.Y, 0.001f);
    }

    [Fact]
    public void Tap_NotchedSafeArea_CountsTapsBelowAndAboveSafeAreaTop()
    {
        var gesture = new CornerTapGesture(3, 0.5f);

        gesture.Tap(new Vector2(40f, 1700f), s_notchedCorner, 0f);
        gesture.Tap(new Vector2(40f, 1900f), s_notchedCorner, 0.1f);

        Assert.Equal(2, gesture.Taps);
    }

    [Fact]
    public void Tap_NotchedSafeArea_BelowCorner_ResetsCount()
    {
        var gesture = new CornerTapGesture(3, 0.5f);

        gesture.Tap(new Vector2(40f, 1700f), s_notchedCorner, 0f);
        gesture.Tap(new Vector2(40f, 1600f), s_notchedCorner, 0.1f);

        Assert.Equal(0, gesture.Taps);
    }
}
