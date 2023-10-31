using System.Numerics;
using System;
using SharpGL;

namespace CG2.Extensions;

public static class OpenGLExtensions
{
    public static void Vertex(this OpenGL gl, Vector3 vertex)
    {
        gl.Vertex(vertex.X, vertex.Y, vertex.Z);
    }

    public static void Normal(this OpenGL gl, Vector3 normal)
    {
        gl.Normal(normal.X, normal.Y, normal.Z);
    }
}