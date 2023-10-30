using System.Numerics;
using CG2.Extensions;
using CG2.Models.Figure;
using SharpGL;
using SharpGL.SceneGraph;

namespace CG2.Geometry;

public class FigureBuilder
{
    private readonly ModelViewTransformations _modelViewTransformations;
    private Section[] _sections;

    public FigureBuilder(ModelViewTransformations modelViewTransformations)
    {
        _modelViewTransformations = modelViewTransformations;
    }

    public Figure Build(Vector2[] section, Vector3[] path)
    {
        _sections = new Section[path.Length];

        for (var i = 0; i < _sections.Length; i++)
        {
            _sections[i] = new Section(section);
        }

        _sections[0] = BuildBeginning(_sections[0], path[0] - path[1], path[1] - path[2], path[0]);

        for (var i = 1; i < _sections.Length - 1; i++)
        {
            _sections[i] = BuildNode(_sections[i], path[i - 1] - path[i], path[i] - path[i + 1], path[i]);
        }

        _sections[^1] = BuildEnd(_sections[^1], path[^3] - path[^2], path[^2] - path[^1], path[^1]);

        return new Figure(_sections);
    }

    private Section BuildBeginning(Section section, Vector3 direction, Vector3 nextDirection, Vector3 point)
    {
        var crossVector = Vector3.Cross(direction, nextDirection);

        var directionY0 = direction with { Y = 0 };
        var crossVectorY0 = crossVector with { Y = 0 };

        var xAngle = directionY0.Angle(direction);
        var yAngle = crossVectorY0.Angle(crossVector);
        var zAngle = Vector3.UnitZ.Angle(directionY0);

        for (var i = 0; i < section.Count; i++)
        {
            var vertex = section[i];
            vertex = _modelViewTransformations.RotateByX(vertex, xAngle);
            vertex = _modelViewTransformations.RotateByY(vertex, zAngle);
            vertex = _modelViewTransformations.RotateByZ(vertex, yAngle);
            section[i] = vertex;
        }

        section.Translate(point);

        return section;
    }

    private Section BuildNode(Section section, Vector3 direction, Vector3 nextDirection, Vector3 point)
    {
        var crossVector = Vector3.Cross(direction, nextDirection);

        var directionY0 = direction with { Y = 0 };
        var crossVectorY0 = crossVector with { Y = 0 };

        var xAngle = directionY0.Angle(direction);
        var yAngle = crossVectorY0.Angle(crossVector);
        var zAngle = Vector3.UnitZ.Angle(directionY0);
        var directionAngle = direction.Angle(nextDirection);

        for (var i = 0; i < section.Count; i++)
        {
            var vertex = section[i];
            vertex = _modelViewTransformations.RotateByX(vertex, xAngle);
            vertex = _modelViewTransformations.RotateByY(vertex, zAngle + directionAngle / 2);
            vertex = _modelViewTransformations.RotateByZ(vertex, yAngle);
            section[i] = vertex;
        }

        section.Translate(point);

        return section;
    }

    private Section BuildEnd(Section section, Vector3 direction, Vector3 nextDirection, Vector3 point)
    {
        var crossVector = Vector3.Cross(direction, nextDirection);

        var directionY0 = direction with { Y = 0 };
        var crossVectorY0 = crossVector with { Y = 0 };

        var xAngle = directionY0.Angle(direction);
        var yAngle = crossVectorY0.Angle(crossVector);
        var zAngle = Vector3.UnitZ.Angle(directionY0);

        for (var i = 0; i < section.Count; i++)
        {
            var vertex = section[i];
            vertex = _modelViewTransformations.RotateByX(vertex, xAngle);
            vertex = _modelViewTransformations.RotateByY(vertex, zAngle);
            vertex = _modelViewTransformations.RotateByZ(vertex, yAngle);
            section[i] = vertex;
        }

        section.Translate(point);

        return section;
    }
}