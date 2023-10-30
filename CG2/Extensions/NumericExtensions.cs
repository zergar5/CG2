using System;

namespace CG2.Extensions;

public static class NumericsExtensions
{
    public static float ToRadians(this float degrees)
    {
        return (float)(Math.PI / 180d * degrees);
    }

    public static double ToRadians(this double degrees)
    {
        return Math.PI / 180f * degrees;
    }

    public static float ToDegrees(this float radians)
    {
        return (float)(radians * 180d / Math.PI);
    }

    public static double ToDegrees(this double radians)
    {
        return radians * 180f / Math.PI;
    }
}