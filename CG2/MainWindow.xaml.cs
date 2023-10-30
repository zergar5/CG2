using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CG2.Extensions;
using CG2.Geometry;
using CG2.Models;
using CG2.Models.Figure;
using SharpGL;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Assets;
using SharpGL.SceneGraph.Lighting;
using SharpGL.WPF;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Point = System.Windows.Point;

namespace CG2;

public partial class MainWindow : Window
{
    private OpenGL _gl;
    private readonly Camera _camera;
    private readonly FigureBuilder _figureBuilder;
    private Figure _figure;
    private Vector2[] _section;
    private Vector3[] _path;
    private Point _previousPosition;

    public MainWindow()
    {
        InitializeComponent();

        _section = new Vector2[]
        {
            new(-0.2f, -0.2f), new(0.2f, -0.2f), new(0, 1)
        };
        _path = new Vector3[]
        {
            new(-1, 0, 1), new(0, 1, 0), new(0, 2, -1)
        };
        GlWindow.FrameRate = 60;
        _camera = new Camera();
        _figureBuilder = new FigureBuilder(new ModelViewTransformations());
    }

    private void OpenGLDraw(object sender, OpenGLRoutedEventArgs args)
    {
        _gl.ClearColor(0f, 0f, 0f, 1f);
        _gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

        _gl.Begin(OpenGL.GL_LINE_STRIP);

        foreach (var point in _path)
        {
            _gl.Color(1f, 1f, 1f, 1f);
            _gl.Vertex(point);
        }

        _gl.End();

        _figure.Draw(_gl);
        _figure.DrawNormals(_gl);

        _camera.ChangeCamera(_gl);

        _gl.Flush();
    }

    private void OpenGLInitialized(object sender, OpenGLRoutedEventArgs args)
    {
        _gl = args.OpenGL;

        _camera.Rotate(_previousPosition, _previousPosition);
        _camera.ChangeCamera(_gl);

        SetDepthBuffer();
        SetDoubleBuffer();

        //_gl.PolygonMode(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_LINE);

        _figure = _figureBuilder.Build(_section, _path);
        _figure.CalculateNormals();
    }

    private void OpenGLControlResized(object sender, OpenGLRoutedEventArgs args)
    {
        //SetOrthographicProjection(_gl);
        SetPerspectiveProjection(_gl);

        SetLight();
        SetMaterial();
        SetTexture();

        OpenGLDraw(sender, args);
    }

    private void GlWindowOnKeyDown(object sender, KeyEventArgs args)
    {
        if (args.Key == Key.W)
        {
            _camera.MoveForward();
            _camera.ChangeCamera(_gl);
        }
        else if (args.Key == Key.A)
        {
            _camera.MoveLeft();
            _camera.ChangeCamera(_gl);
        }
        else if (args.Key == Key.S)
        {
            _camera.MoveBackward();
            _camera.ChangeCamera(_gl);
        }
        else if (args.Key == Key.D)
        {
            _camera.MoveRight();
            _camera.ChangeCamera(_gl);
        }
        else if (args.Key == Key.Space)
        {
            _camera.MoveUp();
            _camera.ChangeCamera(_gl);
        }
        else if (args.Key == Key.LeftCtrl)
        {
            _camera.MoveDown();
            _camera.ChangeCamera(_gl);
        }
    }

    private void GlWindow_OnMouseMove(object sender, MouseEventArgs e)
    {
        if (!IsActive) return;

        var currentPosition = e.GetPosition(GlWindow);

        if (e.LeftButton == MouseButtonState.Pressed)
        {
            _camera.Rotate(currentPosition, _previousPosition);
            _camera.ChangeCamera(_gl);
        }

        _previousPosition = currentPosition;
    }

    private void SetOrthographicProjection(OpenGL gl)
    {
        gl.MatrixMode(OpenGL.GL_PROJECTION);
        gl.LoadIdentity();
        gl.Ortho(-GlWindow.ActualWidth / 2, GlWindow.ActualWidth/2, -GlWindow.ActualHeight / 2, GlWindow.ActualHeight / 2, 0.1, 100);
        gl.MatrixMode(OpenGL.GL_MODELVIEW);
        gl.LoadIdentity();
        _previousPosition = new Point(GlWindow.ActualWidth / 2, GlWindow.ActualHeight / 2);
    }

    private void SetPerspectiveProjection(OpenGL gl)
    {
        gl.MatrixMode(OpenGL.GL_PROJECTION);
        gl.LoadIdentity();
        gl.Perspective(60f, GlWindow.ActualWidth / GlWindow.ActualHeight, 0.1f, 100f);
        gl.MatrixMode(OpenGL.GL_MODELVIEW);
        gl.LoadIdentity();
        _previousPosition = new Point(GlWindow.ActualWidth / 2, GlWindow.ActualHeight / 2);
    }

    private void SetDepthBuffer()
    {
        //_gl.Disable(OpenGL.GL_DEPTH_TEST);
        _gl.Enable(OpenGL.GL_DEPTH_TEST);
    }

    private void SetDoubleBuffer()
    {
        //_gl.Disable(OpenGL.GL_DOUBLEBUFFER);
        _gl.Enable(OpenGL.GL_DOUBLEBUFFER);
    }

    private void SetLight()
    {
        _gl.Enable(OpenGL.GL_LIGHTING);
        _gl.Enable(OpenGL.GL_LIGHT0);
        _gl.LightModel(OpenGL.GL_LIGHT_MODEL_TWO_SIDE, OpenGL.GL_TRUE);
        _gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, new[] { -6f, 2f, -3f, 1f });
    }

    private void SetMaterial()
    {
        _gl.Material(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT_AND_DIFFUSE, new[] { 0.2f, 0.2f, 0.2f, 1f });
        _gl.Enable(OpenGL.GL_COLOR_MATERIAL);
    }

    private void SetTexture()
    {
        var texture = new Texture();

        var path = @"..\CG2\Textures\Texture1.bmp";
        var textureImage = new Bitmap(path);

        texture.Create(_gl, textureImage);

        _gl.Enable(OpenGL.GL_TEXTURE_2D);

        texture.Bind(_gl);
    }
}