using System;
using System.Numerics;

namespace CG2.Extensions;

public static class Vector3Extensions
{
    public static float Angle(this Vector3 vector1, Vector3 vector2)
    {
        var dotProduct = Vector3.Dot(vector1, vector2);
        var magnitude1 = vector1.Length();
        var magnitude2 = vector2.Length();

        var cosTheta = dotProduct / (magnitude1 * magnitude2);

        var thetaRadians = Math.Acos(cosTheta);

        var thetaDeg = thetaRadians.ToDegrees();
        var sign = Math.Sign(Vector3.Cross(vector1, vector2).Y);

        if (sign == 0) sign = 1;

        return (float)thetaDeg * sign;
    }
}