using System;
using System.Numerics;
using CG2.Extensions;

namespace CG2.Geometry;

public class ModelViewTransformations
{
    public Vector3 Translate(Vector3 vertex, Vector3 translation)
    {
        vertex.X += translation.X;
        vertex.Y += translation.Y;
        vertex.Z += translation.Z;
        
        return vertex;
    }

    public Vector3 RotateByX(Vector3 vector, float angle)
    {
        var radians = angle.ToRadians();

        var sin = Math.Sin(radians);
        var cos = Math.Cos(radians);

        var y = (float)(cos * vector.Y - sin * vector.Z);
        var z = (float)(sin * vector.Y + cos * vector.Z);

        vector.Y = y;
        vector.Z = z;

        return vector;
    }

    public Vector3 RotateByY(Vector3 vector, float angle)
    {
        var radians = angle.ToRadians();

        var sin = Math.Sin(radians);
        var cos = Math.Cos(radians);

        var x = (float)(cos * vector.X + sin * vector.Z);
        var z = (float)(-sin * vector.X + cos * vector.Z);

        vector.X = x;
        vector.Z = z;

        return vector;
    }

    public Vector3 RotateByZ(Vector3 vector, float angle)
    {
        var radians = angle.ToRadians();

        var sin = Math.Sin(radians);
        var cos = Math.Cos(radians);

        var x = (float)(cos * vector.X - sin * vector.Y);
        var y = (float)(sin * vector.X + cos * vector.Y);

        vector.X = x;
        vector.Y = y;

        return vector;
    }
}