using System;
using System.Numerics;
using CG2.Extensions;
using SharpGL;
using SharpGL.SceneGraph;

namespace CG2.Models.Figure;

public class Figure
{
    private readonly Section[] _sections;
    private Vector3[][] _normals;

    public Figure(Section[] sections)
    {
        _sections = sections;
        _normals = new Vector3[_sections.Length + 1][];
    }

    public void CalculateNormals()
    {
        _normals[0] = new Vector3[1];
        var vector01 = _sections[0][1] - _sections[0][0];
        var vector02 = _sections[0][2] - _sections[0][0];
        var vector = Vector3.Normalize(Vector3.Cross(vector01, vector02));

        _normals[0][0] = vector;

        for (var i = 1; i < _sections.Length; i++)
        {
            _normals[i] = new Vector3[_sections[i].Count];

            for (var j = 0; j < _sections[i].Count - 1; j++)
            {
                vector01 = _sections[i][j] - _sections[i - 1][j];
                vector02 = _sections[i - 1][j + 1] - _sections[i - 1][j];
                vector = Vector3.Normalize(Vector3.Cross(vector01, vector02));

                _normals[i][j] = vector;
            }

            vector01 = _sections[i][^1] - _sections[i - 1][^1];
            vector02 = _sections[i - 1][0] - _sections[i - 1][^1];
            vector = Vector3.Normalize(Vector3.Cross(vector01, vector02));

            _normals[i][^1] = vector;
        }

        _normals[^1] = new Vector3[1];
        vector01 = _sections[^1][1] - _sections[^1][0];
        vector02 = _sections[^1][2] - _sections[^1][0];
        vector = -Vector3.Normalize(Vector3.Cross(vector01, vector02));

        _normals[^1][0] = vector;
    }

    public void Draw(OpenGL gl)
    {
        DrawBeginning(gl);

        for (var i = 1; i < _sections.Length; i++)
        {
            DrawNode(gl, _sections[i - 1], _sections[i]);
        }

        DrawEnd(gl);
    }

    public void DrawCarcass(OpenGL gl)
    {
        DrawBeginningCarcass(gl);

        for (var i = 1; i < _sections.Length; i++)
        {
            DrawNodeCarcass(gl, _sections[i - 1], _sections[i]);
        }

        DrawEndCarcass(gl);
    }

    public void DrawNormals(OpenGL gl)
    {
        gl.Begin(OpenGL.GL_POLYGO);

        foreach (var vertex in _sections[0])
        {
            gl.Color(1f, 0f, 0f);
            gl.Vertex(vertex);

            gl.Color(1f, 0f, 0f);
            gl.Vertex(vertex + _normals[0][0]);
        }

        gl.End();

        gl.Begin(OpenGL.GL_LINES);

        for (var i = 1; i < _sections.Length; i++)
        {
            var centerX = (_sections[i - 1][^1].X + _sections[i][^1].X +
                           _sections[i - 1][0].X + _sections[i][0].X) / 4;
            var centerY = (_sections[i - 1][^1].Y + _sections[i][^1].Y +
                           _sections[i - 1][0].Y + _sections[i][0].Y) / 4;
            var centerZ = (_sections[i - 1][^1].Z + _sections[i][^1].Z +
                           _sections[i - 1][0].Z + _sections[i][0].Z) / 4;

            var vertex = new Vector3(centerX, centerY, centerZ);

            gl.Color(1f, 0f, 0f);
            gl.Vertex(vertex);

            gl.Color(1f, 0f, 0f);
            gl.Vertex(vertex + _normals[i][^1]);

            for (var j = 0; j < _sections[i].Count - 1; j++)
            {
                centerX = (_sections[i - 1][j].X + _sections[i][j].X + 
                               _sections[i - 1][j + 1].X + _sections[i][j + 1].X) / 4;
                centerY = (_sections[i - 1][j].Y + _sections[i][j].Y + 
                               _sections[i - 1][j + 1].Y + _sections[i][j + 1].Y) / 4;
                centerZ = (_sections[i - 1][j].Z + _sections[i][j].Z +
                               _sections[i - 1][j + 1].Z + _sections[i][j + 1].Z) / 4;

                vertex = new Vector3(centerX, centerY, centerZ);

                gl.Color(1f, 0f, 0f);
                gl.Vertex(vertex);

                gl.Color(1f, 0f, 0f);
                gl.Vertex(vertex + _normals[i][j]);
            }

            gl.End();
        }

        gl.Begin(OpenGL.GL_LINES);

        foreach (var vertex in _sections[^1])
        {
            gl.Color(1f, 0f, 0f);
            gl.Vertex(vertex);

            gl.Color(1f, 0f, 0f);
            gl.Vertex(vertex + _normals[^1][0]);
        }

        gl.End();
    }

    private void DrawBeginning(OpenGL gl)
    {
        gl.Begin(OpenGL.GL_POLYGON);

        foreach (var vertex in _sections[0])
        {
            gl.Color(1f, 0f, 0f);
            gl.Vertex(vertex);
        }

        gl.End();
    }

    private void DrawNode(OpenGL gl, Section previousSection, Section section)
    {
        gl.Begin(OpenGL.GL_TRIANGLE_STRIP);

        for (var i = 0; i < section.Count; i++)
        {
            gl.Color(1f, 0f, 0f);
            gl.Vertex(previousSection[i]);

            gl.Color(1f, 0f, 0f);
            gl.Vertex(section[i]);
        }

        gl.Color(1f, 0f, 0f);
        gl.Vertex(previousSection[0]);

        gl.Color(1f, 0f, 0f);
        gl.Vertex(section[0]);

        gl.End();
    }

    private void DrawEnd(OpenGL gl)
    {
        gl.Begin(OpenGL.GL_POLYGON);

        foreach (var vertex in _sections[^1])
        {
            gl.Color(1f, 0f, 0f);
            gl.Vertex(vertex);
        }

        gl.End();
    }

    private void DrawBeginningCarcass(OpenGL gl)
    {
        gl.Begin(OpenGL.GL_LINE_LOOP);

        foreach (var vertex in _sections[0])
        {
            gl.Color(1f, 0f, 0f);
            gl.Vertex(vertex);
        }

        gl.End();
    }

    private void DrawNodeCarcass(OpenGL gl, Section previousSection, Section section)
    {
        gl.Begin(OpenGL.GL_LINES);

        for (var i = 0; i < section.Count; i++)
        {
            gl.Color(1f, 0f, 0f);
            gl.Vertex(previousSection[i]);

            gl.Color(1f, 0f, 0f);
            gl.Vertex(section[i]);
        }

        gl.End();

        gl.Begin(OpenGL.GL_LINE_LOOP);

        foreach (var vertex in section)
        {
            gl.Color(1f, 0f, 0f);
            gl.Vertex(vertex);
        }

        gl.End();
    }

    private void DrawEndCarcass(OpenGL gl)
    {
        gl.Begin(OpenGL.GL_LINE_LOOP);

        foreach (var vertex in _sections[^1])
        {
            gl.Color(1f, 0f, 0f);
            gl.Vertex(vertex);
        }

        gl.End();
    }
}