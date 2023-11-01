using System;
using System.Numerics;
using System.Reflection;
using CG2.Extensions;
using SharpGL;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Assets;

namespace CG2.Models.Figure;

public class Figure
{
    private readonly Section[] _sections;
    private readonly Vector3[][] _normals;
    private readonly Vector3[] _path;

    public Figure(Section[] sections, Vector3[][] normals, Vector3[] path)
    {
        _sections = sections;
        _normals = normals;
        _path = path;
    }

    public void Draw(OpenGL gl, bool texture, bool smooth)
    {
        if (smooth)
        {
            DrawBeginningSmooth(gl, texture);

            for (var i = 1; i < _sections.Length; i++)
            {
                DrawNodeSmooth(gl, _sections[i - 1], _sections[i], _normals[i-1], _normals[i], texture);
            }

            DrawEndSmooth(gl, texture);
        }
        else
        {
            DrawBeginning(gl, texture);

            for (var i = 1; i < _sections.Length; i++)
            {
                DrawNode(gl, _sections[i - 1], _sections[i], _normals[i], texture);
            }

            DrawEnd(gl, texture);
        }
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

    public void DrawNormals(OpenGL gl, bool smooth)
    {
        if (smooth)
        {
            gl.Begin(OpenGL.GL_LINES);

            for (var i = 0; i < _sections.Length; i++)
            {
                for (var j = 0; j < _sections[i].Count; j++)
                {
                    gl.Color(1f, 1f, 1f);
                    gl.Vertex(_sections[i][j]);

                    gl.Color(1f, 1f, 1f);
                    gl.Vertex(_sections[i][j] + _normals[i][j]);
                }
            }

            gl.End();
        }
        else
        {
            gl.Begin(OpenGL.GL_LINES);

            gl.Color(1f, 1f, 1f);
            gl.Vertex(_path[0]);

            gl.Color(1f, 1f, 1f);
            gl.Vertex(_path[0] + _normals[0][0]);

            for (var i = 1; i < _sections.Length; i++)
            {
                var centerX = (_path[i - 1].X + _path[i].X) / 2;
                var centerY = (_path[i - 1].Y + _path[i].Y) / 2;
                var centerZ = (_path[i - 1].Z + _path[i].Z) / 2;

                var vertex = new Vector3(centerX, centerY, centerZ);

                for (var j = 0; j < _sections[i].Count; j++)
                {
                    gl.Color(1f, 1f, 1f);
                    gl.Vertex(vertex);

                    gl.Color(1f, 1f, 1f);
                    gl.Vertex(vertex + _normals[i][j]);
                }
            }

            gl.Color(1f, 1f, 1f);
            gl.Vertex(_path[^1]);

            gl.Color(1f, 1f, 1f);
            gl.Vertex(_path[^1] + _normals[^1][0]);

            gl.End();
        }

    }

    private void DrawBeginning(OpenGL gl, bool texture)
    {
        gl.Normal(_normals[0][0]);
        gl.Begin(OpenGL.GL_POLYGON);

        foreach (var vertex in _sections[0])
        {
            if (texture)
            {
                gl.TexCoord(1f, 1f);
            }
            else
            {
                gl.Color(1f, 0f, 0f);
            }

            gl.Vertex(vertex);
        }

        gl.End();
    }

    //TODO поиграться с координатами
    private void DrawNode(OpenGL gl, Section previousSection, Section section, Vector3[] normals, bool texture)
    {
        gl.Begin(OpenGL.GL_TRIANGLES);

        for (var i = 0; i < section.Count - 1; i++)
        {
            if (texture)
            {
                gl.Normal(normals[i]);
                gl.TexCoord(1f, 1f);
                gl.Vertex(previousSection[i]);

                gl.Normal(normals[i]);
                gl.TexCoord(1f, 0f);
                gl.Vertex(section[i]);

                gl.Normal(normals[i]);
                gl.TexCoord(0f, 1f);
                gl.Vertex(previousSection[i + 1]);
            }
            else
            {
                gl.Normal(normals[i]);
                gl.Color(1f, 0f, 0f);
                gl.Vertex(previousSection[i]);

                gl.Normal(normals[i]);
                gl.Color(1f, 0f, 0f);
                gl.Vertex(section[i]);

                gl.Normal(normals[i]);
                gl.Color(1f, 0f, 0f);
                gl.Vertex(previousSection[i + 1]);
            }
        }

        if (texture)
        {
            gl.Normal(normals[^1]);
            gl.TexCoord(1f, 1f);
            gl.Vertex(previousSection[^1]);

            gl.Normal(normals[^1]);
            gl.TexCoord(1f, 0f);
            gl.Vertex(section[^1]);

            gl.Normal(normals[^1]);
            gl.TexCoord(0f, 1f);
            gl.Vertex(previousSection[0]);
        }
        else
        {
            gl.Normal(normals[^1]);
            gl.Color(1f, 0f, 0f);
            gl.Vertex(previousSection[^1]);

            gl.Normal(normals[^1]);
            gl.Color(1f, 0f, 0f);
            gl.Vertex(section[^1]);

            gl.Normal(normals[^1]);
            gl.Color(1f, 0f, 0f);
            gl.Vertex(previousSection[0]);
        }

        for (var i = 0; i < section.Count - 1; i++)
        {
            if (texture)
            {
                gl.Normal(normals[i]);
                gl.TexCoord(1f, 0f);
                gl.Vertex(section[i]);

                gl.Normal(normals[i]);
                gl.TexCoord(0f, 1f);
                gl.Vertex(previousSection[i + 1]);

                gl.Normal(normals[i]);
                gl.TexCoord(0f, 0f);
                gl.Vertex(section[i + 1]);
            }
            else
            {
                gl.Normal(normals[i]);
                gl.Color(1f, 0f, 0f);
                gl.Vertex(section[i]);

                gl.Normal(normals[i]);
                gl.Color(1f, 0f, 0f);
                gl.Vertex(previousSection[i + 1]);

                gl.Normal(normals[i]);
                gl.Color(1f, 0f, 0f);
                gl.Vertex(section[i + 1]);
            }
        }

        if (texture)
        {
            gl.Normal(normals[^1]);
            gl.TexCoord(1f, 0f);
            gl.Vertex(section[^1]);

            gl.Normal(normals[^1]);
            gl.TexCoord(0f, 1f);
            gl.Vertex(previousSection[0]);

            gl.Normal(normals[^1]);
            gl.TexCoord(0f, 0f);
            gl.Vertex(section[0]);
        }
        else
        {
            gl.Normal(normals[^1]);
            gl.Color(1f, 0f, 0f);
            gl.Vertex(section[^1]);

            gl.Normal(normals[^1]);
            gl.Color(1f, 0f, 0f);
            gl.Vertex(previousSection[0]);

            gl.Normal(normals[^1]);
            gl.Color(1f, 0f, 0f);
            gl.Vertex(section[0]);
        }

        gl.End();
    }

    private void DrawEnd(OpenGL gl, bool texture)
    {
        gl.Normal(_normals[^1][0]);
        gl.Begin(OpenGL.GL_POLYGON);

        foreach (var vertex in _sections[^1])
        {
            if (texture)
            {
                gl.TexCoord(1f, 1f);
            }
            else
            {
                gl.Color(1f, 0f, 0f);
            }

            gl.Vertex(vertex);
        }

        gl.End();
    }

    private void DrawBeginningSmooth(OpenGL gl, bool texture)
    {
        gl.Begin(OpenGL.GL_POLYGON);

        for (var i = 0; i < _sections[0].Count; i++)
        {
            gl.Normal(_normals[0][i]);

            var vertex = _sections[0][i];
            if (texture)
            {
                gl.TexCoord(1f, 1f);
            }
            else
            {
                gl.Color(1f, 0f, 0f);
            }

            gl.Vertex(vertex);
        }

        gl.End();
    }

    private void DrawNodeSmooth(OpenGL gl, Section previousSection, Section section, Vector3[] previousNormals, Vector3[] normals, bool texture)
    {
        gl.Begin(OpenGL.GL_TRIANGLE_STRIP);

        for (var i = 0; i < section.Count; i++)
        {
            gl.Normal(previousNormals[i]);
            gl.Color(1f, 0f, 0f);
            gl.Vertex(previousSection[i]);

            gl.Normal(normals[i]);
            gl.Color(1f, 0f, 0f);
            gl.Vertex(section[i]);
        }

        gl.Normal(previousNormals[0]);
        gl.Color(1f, 0f, 0f);
        gl.Vertex(previousSection[0]);

        gl.Normal(normals[0]);
        gl.Color(1f, 0f, 0f);
        gl.Vertex(section[0]);

        gl.End();
    }

    private void DrawEndSmooth(OpenGL gl, bool texture)
    {
        gl.Begin(OpenGL.GL_POLYGON);

        for (var i = 0; i < _sections[^1].Count; i++)
        {
            gl.Normal(_normals[^1][i]);

            var vertex = _sections[^1][i];

            if (texture)
            {
                gl.TexCoord(1f, 1f);
            }
            else
            {
                gl.Color(1f, 0f, 0f);
            }

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