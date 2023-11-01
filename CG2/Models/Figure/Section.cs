using System.Collections.Generic;
using System.Numerics;

namespace CG2.Models.Figure;

public class Section
{
    private readonly Vector3[] _vertices3D;

    public Section(Vector2[] vertices2D)
    {
        _vertices3D = new Vector3[vertices2D.Length];

        for (var i = 0; i < vertices2D.Length; i++)
        {
            var vertex = vertices2D[i];
            _vertices3D[i] = new Vector3(vertex.X, vertex.Y, 0);
        }
    }

    public Vector3 this[int index]
    {
        get => _vertices3D[index];
        set => _vertices3D[index] = value;
    }

    public int Count => _vertices3D.Length;

    public void Translate(Vector3 translation)
    {
        for (var i = 0; i < _vertices3D.Length; i++)
        {
            _vertices3D[i].X += translation.X;
            _vertices3D[i].Y += translation.Y;
            _vertices3D[i].Z += translation.Z;
        }
    }

    public void Scale(Vector2 scales)
    {
        for (var i = 0; i < _vertices3D.Length; i++)
        {
            _vertices3D[i].X *= scales.X;
            _vertices3D[i].Y *= scales.Y;
        }
    }

    public IEnumerator<Vector3> GetEnumerator() => ((IEnumerable<Vector3>)_vertices3D).GetEnumerator();
}