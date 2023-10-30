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
}