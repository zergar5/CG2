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

        vector.Y = (float)(cos * vector.Y - sin * vector.Z);
        vector.Z = (float)(sin * vector.Y + cos * vector.Z);

        return vector;
    }

    public Vector3 RotateByY(Vector3 vector, float angle)
    {
        var radians = angle.ToRadians();

        var sin = Math.Sin(radians);
        var cos = Math.Cos(radians);

        vector.X = (float)(cos * vector.X + sin * vector.Z);
        vector.Z = (float)(-sin * vector.X + cos * vector.Z);

        return vector;
    }

    public Vector3 RotateByZ(Vector3 vector, float angle)
    {
        var radians = angle.ToRadians();

        var sin = Math.Sin(radians);
        var cos = Math.Cos(radians);

        vector.X = (float)(cos * vector.X - sin * vector.Y);
        vector.Y = (float)(sin * vector.X + cos * vector.Y);

        return vector;
    }
}