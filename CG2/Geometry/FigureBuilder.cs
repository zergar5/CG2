using System.Numerics;
using System.Windows.Media.Media3D;
using CG2.Extensions;
using CG2.Models.Figure;
using SharpGL;
using SharpGL.SceneGraph;

namespace CG2.Geometry;

public class FigureBuilder
{
    private Section[] _sections;
    private Vector3[][] _normals;
    private Vector3[][] _smoothNormals;
    private Vector3[] _path;
    private Vector2[] _scales;


    public Figure Build()
    {
        return new Figure(_sections, _normals, _path);
    }

    public Figure BuildWithSmooth()
    {
        return new Figure(_sections, _smoothNormals, _path);
    }

    public FigureBuilder CalculateSections(Vector2[] section, Vector3[] path, Vector2[] scales)
    {
        _sections = new Section[path.Length];
        _path = path;
        _scales = scales;

        for (var i = 0; i < _sections.Length; i++)
        {
            _sections[i] = new Section(section);
        }

        var direction = path[0] - path[1];
        var nextDirection = path[1] - path[2];

        _sections[0] = CalculateBeginning(_sections[0], direction, nextDirection, path[0], scales[0]);

        for (var i = 1; i < _sections.Length - 1; i++)
        {
            direction = Vector3.Normalize(path[i - 1] - path[i]);
            nextDirection = Vector3.Normalize(path[i] - path[i + 1]);

            _sections[i] = CalculateNode(_sections[i], direction, nextDirection, path[i], scales[i]);
        }

        direction = path[^3] - path[^2];
        nextDirection = path[^2] - path[^1];

        _sections[^1] = CalculateEnd(_sections[^1], nextDirection, direction, path[^1], scales[^1]);

        return this;
    }

    public FigureBuilder CalculateNormals(bool smoothMode)
    {
        _normals = new Vector3[_sections.Length + 1][];

        _normals[0] = new Vector3[1];
        var vector01 = _sections[0][1] - _sections[0][0];
        var vector02 = _sections[0][2] - _sections[0][0];
        _normals[0][0] = Vector3.Normalize(Vector3.Cross(vector01, vector02));

        for (var i = 1; i < _sections.Length; i++)
        {
            _normals[i] = new Vector3[_sections[i].Count];

            for (var j = 0; j < _sections[i].Count - 1; j++)
            {
                vector01 = _sections[i][j] - _sections[i - 1][j];
                vector02 = _sections[i - 1][j + 1] - _sections[i - 1][j];
                _normals[i][j] = Vector3.Normalize(Vector3.Cross(vector01, vector02));
            }

            vector01 = _sections[i][^1] - _sections[i - 1][^1];
            vector02 = _sections[i - 1][0] - _sections[i - 1][^1];
            _normals[i][^1] = Vector3.Normalize(Vector3.Cross(vector01, vector02));
        }

        _normals[^1] = new Vector3[1];
        vector01 = _sections[^1][1] - _sections[^1][0];
        vector02 = _sections[^1][2] - _sections[^1][0];
        _normals[^1][0] = -Vector3.Normalize(Vector3.Cross(vector01, vector02));

        if (smoothMode)
        {
            _smoothNormals = new Vector3[_sections.Length][];
            _smoothNormals[0] = new Vector3[_sections[0].Count];

            _smoothNormals[0][0] = Vector3.Normalize(_normals[0][0] + _normals[1][0] + _normals[1][^1]);

            for (var i = 1; i < _sections[0].Count; i++)
            {
                _smoothNormals[0][i] = Vector3.Normalize(_normals[0][0] + _normals[1][i] + _normals[1][i - 1]);
            }

            for (var i = 1; i < _sections.Length - 1; i++)
            {
                _smoothNormals[i] = new Vector3[_sections[i].Count];

                _smoothNormals[i][0] = Vector3.Normalize(_normals[i][0] + _normals[i][^1] + _normals[i + 1][0] + _normals[i + 1][^1]);

                for (var j = 1; j < _sections[i].Count; j++)
                {
                    _smoothNormals[i][j] = Vector3.Normalize(_normals[i][j] + _normals[i][j - 1] + _normals[i + 1][j] + _normals[i + 1][j - 1]);
                }
            }

            _smoothNormals[^1] = new Vector3[_sections[^1].Count];

            _smoothNormals[^1][0] = Vector3.Normalize(_normals[^1][0] + _normals[^2][0] + _normals[^2][^1]);

            for (var i = 1; i < _sections[^1].Count; i++)
            {
                _smoothNormals[^1][i] = Vector3.Normalize(_normals[^1][0] + _normals[^2][i] + _normals[^2][i - 1]);
            }
        }

        return this;
    }

    private Section CalculateBeginning(Section section, Vector3 direction, Vector3 nextDirection, Vector3 point, Vector2 scales)
    {
        var sign = nextDirection.Y < direction.Y ? 1 : -1;
        var crossVector = Vector3.Cross(direction, nextDirection) * sign;

        var newX = Vector3.Normalize(crossVector);
        var newY = Vector3.Normalize(Vector3.Cross(direction, crossVector));
        var newZ = Vector3.Normalize(direction);

        section.Scale(scales);

        for (var i = 0; i < section.Count; i++)
        {
            var vertex = section[i];

            var x = newX.X * vertex.X + newY.X * vertex.Y + newZ.X * vertex.Z;
            var y = newX.Y * vertex.X + newY.Y * vertex.Y + newZ.Y * vertex.Z;
            var z = newX.Z * vertex.X + newY.Z * vertex.Y + newZ.Z * vertex.Z;

            vertex.X = x;
            vertex.Y = y;
            vertex.Z = z;

            section[i] = vertex;
        }

        section.Translate(point);

        return section;
    }

    private Section CalculateNode(Section section, Vector3 direction, Vector3 nextDirection, Vector3 point, Vector2 scales)
    {
        var sign = nextDirection.Y < direction.Y ? 1 : -1;
        var crossVector = Vector3.Cross(direction, nextDirection) * sign;

        var newX = Vector3.Normalize(crossVector);
        var newZ = Vector3.Normalize((direction + nextDirection) / 2);
        var newY = Vector3.Normalize(Vector3.Cross(newZ, crossVector));

        section.Scale(scales);

        for (var i = 0; i < section.Count; i++)
        {
            var vertex = section[i];

            var x = newX.X * vertex.X + newY.X * vertex.Y + newZ.X * vertex.Z;
            var y = newX.Y * vertex.X + newY.Y * vertex.Y + newZ.Y * vertex.Z;
            var z = newX.Z * vertex.X + newY.Z * vertex.Y + newZ.Z * vertex.Z;

            vertex.X = x;
            vertex.Y = y;
            vertex.Z = z;

            section[i] = vertex;
        }

        section.Translate(point);
        
        return section;
    }

    private Section CalculateEnd(Section section, Vector3 direction, Vector3 nextDirection, Vector3 point, Vector2 scales)
    {
        var sign = nextDirection.Y < direction.Y ? 1 : -1;
        var crossVector = Vector3.Cross(direction, nextDirection) * sign;

        var newX = Vector3.Normalize(crossVector);
        var newY = Vector3.Normalize(Vector3.Cross(direction, crossVector));
        var newZ = Vector3.Normalize(direction);

        section.Scale(scales);

        for (var i = 0; i < section.Count; i++)
        {
            var vertex = section[i];

            var x = newX.X * vertex.X + newY.X * vertex.Y + newZ.X * vertex.Z;
            var y = newX.Y * vertex.X + newY.Y * vertex.Y + newZ.Y * vertex.Z;
            var z = newX.Z * vertex.X + newY.Z * vertex.Y + newZ.Z * vertex.Z;

            vertex.X = x;
            vertex.Y = y;
            vertex.Z = z;

            section[i] = vertex;
        }

        section.Translate(point);

        return section;
    }
}