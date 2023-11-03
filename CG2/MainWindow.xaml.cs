using CG2.Extensions;
using CG2.Geometry;
using CG2.Models;
using CG2.Models.Figure;
using SharpGL;
using SharpGL.SceneGraph.Assets;
using SharpGL.WPF;
using System.Drawing;
using System.Globalization;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using CG2.IO;
using Point = System.Windows.Point;

namespace CG2;

public partial class MainWindow : Window
{
    private OpenGL _gl;
    private readonly Camera _camera;
    private readonly FigureBuilder _figureBuilder;
    private readonly FigureIO _figureI = new();
    private Figure? _figure;
    private Vector2[] _section;
    private Vector3[] _path;
    private Vector2[] _scales;
    private Point _previousPosition;
    private bool _carcassMode;
    private bool _showNormals;
    private bool _smooth;
    private Projection _projection;
    private int _materialNumber;
    private readonly Texture _texture = new();
    private bool _textureMode;
    private readonly float[] _direction = { -1f, 1f, 0f, 0f };
    private readonly float[] _position = { 1f, 1f, 500f, 1f };
    private readonly float[] _spotlightPosition = { 0.5f, 0.375f, 3f, 1f };
    private readonly float[] _spotlightDirection = { 0f, 0f, -3f, 0f };
    private readonly float[] _positionIntensity = { 1f, 1f, -500f, 1f };

    public MainWindow()
    {
        InitializeComponent();

        GlWindow.FrameRate = 60;
        _camera = new Camera();
        _figureBuilder = new FigureBuilder(new Mapper2D());
    }

    private void OpenGLDraw(object sender, OpenGLRoutedEventArgs args)
    {
        _gl.ClearColor(0f, 0f, 0f, 1f);
        _gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

        _camera.ChangeCamera(_gl);

        _gl.Disable(OpenGL.GL_LIGHTING);

        if (_gl.IsEnabled(OpenGL.GL_LIGHT0))
        {
            _gl.Enable(OpenGL.GL_LIGHTING);
        }

        if (_gl.IsEnabled(OpenGL.GL_LIGHT1))
        {
            _gl.Enable(OpenGL.GL_LIGHTING);

            _gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_POSITION, _direction);
        }

        if (_gl.IsEnabled(OpenGL.GL_LIGHT2))
        {
            _gl.Enable(OpenGL.GL_LIGHTING);

            _gl.Light(OpenGL.GL_LIGHT2, OpenGL.GL_POSITION, _position);
        }

        if (_gl.IsEnabled(OpenGL.GL_LIGHT3))
        {
            _gl.Enable(OpenGL.GL_LIGHTING);

            _gl.Light(OpenGL.GL_LIGHT3, OpenGL.GL_POSITION, _spotlightPosition);
            _gl.Light(OpenGL.GL_LIGHT3, OpenGL.GL_SPOT_DIRECTION, _spotlightDirection);
        }

        if (_gl.IsEnabled(OpenGL.GL_LIGHT4))
        {
            _gl.Enable(OpenGL.GL_LIGHTING);

            _gl.Light(OpenGL.GL_LIGHT4, OpenGL.GL_POSITION, _positionIntensity);
        }

        SetMaterial(_materialNumber);

        if (_figure != null)
        {
            _figure.DrawPath(_gl);

            if (_carcassMode)
            {
                _figure.DrawCarcass(_gl);
            }
            else
            {
                _figure.Draw(_gl, _textureMode, _smooth);
            }

            if (_showNormals)
            {
                _figure.DrawNormals(_gl, _smooth);
            }
        }

        _gl.Flush();
    }

    private void OpenGLInitialized(object sender, OpenGLRoutedEventArgs args)
    {
        _gl = args.OpenGL;

        _gl.Enable(OpenGL.GL_DEPTH_TEST);
        BufferStockCheckBox.IsChecked = true;

        _gl.Disable(OpenGL.GL_DOUBLEBUFFER);

        SetPerspectiveProjection(_gl);
        PerspectiveProjectionCheckBox.IsChecked = true;

        InitLights();
    }

    private void OpenGLControlResized(object sender, OpenGLRoutedEventArgs args)
    {
        _previousPosition = new Point(GlWindow.ActualWidth / 2, GlWindow.ActualHeight / 2);

        if (_projection == Projection.Orthographic)
        {
            SetOrthographicProjection(_gl);
        }
        else if (_projection == Projection.Perspective)
        {
            SetPerspectiveProjection(_gl);
        }

        _camera.Rotate(_previousPosition, _previousPosition);
    }

    private void GlWindowOnKeyDown(object sender, KeyEventArgs args)
    {
        if (args.Key == Key.W)
        {
            _camera.MoveForward();
        }
        else if (args.Key == Key.A)
        {
            _camera.MoveLeft();
        }
        else if (args.Key == Key.S)
        {
            _camera.MoveBackward();
        }
        else if (args.Key == Key.D)
        {
            _camera.MoveRight();
        }
        else if (args.Key == Key.Space)
        {
            _camera.MoveUp();
        }
        else if (args.Key == Key.LeftCtrl)
        {
            _camera.MoveDown();
        }
    }

    private void GlWindow_OnMouseMove(object sender, MouseEventArgs e)
    {
        if (!IsActive) return;

        var currentPosition = e.GetPosition(GlWindow);

        if (e.LeftButton == MouseButtonState.Pressed)
        {
            _camera.Rotate(currentPosition, _previousPosition);
        }

        _previousPosition = currentPosition;
    }

    private void SetOrthographicProjection(OpenGL gl)
    {
        gl.MatrixMode(OpenGL.GL_PROJECTION);
        gl.LoadIdentity();
        gl.Ortho(-2, 2, -2, 2, 0.1, 2);
        gl.MatrixMode(OpenGL.GL_MODELVIEW);
        gl.LoadIdentity();
    }

    private void SetPerspectiveProjection(OpenGL gl)
    {
        gl.MatrixMode(OpenGL.GL_PROJECTION);
        gl.LoadIdentity();
        gl.Perspective(60f, GlWindow.ActualWidth / GlWindow.ActualHeight, 0.1f, 100f);
        gl.MatrixMode(OpenGL.GL_MODELVIEW);
        gl.LoadIdentity();
    }

    private void InitLights()
    {
        _gl.LightModel(OpenGL.GL_LIGHT_MODEL_TWO_SIDE, OpenGL.GL_TRUE);

        _gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, new[] { 0.1f, 0.1f, 0.1f, 1f });
        _gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, new[] { 0f, 0f, 0f, 1f });
        _gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPECULAR, new[] { 0f, 0f, 0f, 1f });

        _gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_AMBIENT, new[] { 0f, 0f, 0f, 1f });
        _gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_DIFFUSE, new[] { 1f, 1f, 1f, 1f });
        _gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPECULAR, new[] { 1f, 1f, 1f, 1f });

        _gl.Light(OpenGL.GL_LIGHT2, OpenGL.GL_AMBIENT, new[] { 0f, 0f, 0f, 1f });
        _gl.Light(OpenGL.GL_LIGHT2, OpenGL.GL_DIFFUSE, new[] { 1f, 1f, 1f, 1f });
        _gl.Light(OpenGL.GL_LIGHT2, OpenGL.GL_SPECULAR, new[] { 1f, 1f, 1f, 1f });

        _gl.Light(OpenGL.GL_LIGHT3, OpenGL.GL_AMBIENT, new[] { 0f, 0f, 0f, 1f });
        _gl.Light(OpenGL.GL_LIGHT3, OpenGL.GL_DIFFUSE, new[] { 1f, 1f, 1f, 1f });
        _gl.Light(OpenGL.GL_LIGHT3, OpenGL.GL_SPECULAR, new[] { 1f, 1f, 1f, 1f });
        _gl.Light(OpenGL.GL_LIGHT3, OpenGL.GL_SPOT_EXPONENT, 15f);
        _gl.Light(OpenGL.GL_LIGHT3, OpenGL.GL_SPOT_CUTOFF, 30f);

        _gl.Light(OpenGL.GL_LIGHT4, OpenGL.GL_AMBIENT, new[] { 0f, 0f, 0f, 1f });
        _gl.Light(OpenGL.GL_LIGHT4, OpenGL.GL_DIFFUSE, new[] { 1f, 1f, 1f, 1f });
        _gl.Light(OpenGL.GL_LIGHT4, OpenGL.GL_SPECULAR, new[] { 1f, 1f, 1f, 1f });
        _gl.Light(OpenGL.GL_LIGHT4, OpenGL.GL_CONSTANT_ATTENUATION, 1f);
        _gl.Light(OpenGL.GL_LIGHT4, OpenGL.GL_LINEAR_ATTENUATION, 0.014f);
        _gl.Light(OpenGL.GL_LIGHT4, OpenGL.GL_QUADRATIC_ATTENUATION, 0.0007f);
    }

    private void SetMaterial(int number)
    {
        if (number == 0)
        {
            _gl.Material(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT, new[] { 0.2f, 0.2f, 0.2f });
            _gl.Material(OpenGL.GL_FRONT, OpenGL.GL_DIFFUSE, new[] { 0.8f, 0.8f, 0.8f });
            _gl.Material(OpenGL.GL_FRONT, OpenGL.GL_SPECULAR, new[] { 0f, 0f, 0f });
            _gl.Material(OpenGL.GL_FRONT, OpenGL.GL_SHININESS, 0f);
        }
        else if (number == 1)
        {
            _gl.Material(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT, new[] { 0.0215f, 0.1745f, 0.0215f });
            _gl.Material(OpenGL.GL_FRONT, OpenGL.GL_DIFFUSE, new[] { 0.0756f, 0.6142f, 0.0756f });
            _gl.Material(OpenGL.GL_FRONT, OpenGL.GL_SPECULAR, new[] { 0.6330f, 0.7278f, 0.6330f });
            _gl.Material(OpenGL.GL_FRONT, OpenGL.GL_SHININESS, 76.8f);
        }
        else if (number == 2)
        {
            _gl.Material(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT, new[] { 0.0537f, 0.05f, 0.0662f });
            _gl.Material(OpenGL.GL_FRONT, OpenGL.GL_DIFFUSE, new[] { 0.1827f, 0.17f, 0.2252f });
            _gl.Material(OpenGL.GL_FRONT, OpenGL.GL_SPECULAR, new[] { 0.3327f, 0.3286f, 0.3464f });
            _gl.Material(OpenGL.GL_FRONT, OpenGL.GL_SHININESS, 38.4f);
        }
        else if (number == 3)
        {
            _gl.Material(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT, new[] { 0.2472f, 0.1995f, 0.0745f });
            _gl.Material(OpenGL.GL_FRONT, OpenGL.GL_DIFFUSE, new[] { 0.7516f, 0.6064f, 0.2264f });
            _gl.Material(OpenGL.GL_FRONT, OpenGL.GL_SPECULAR, new[] { 0.6282f, 0.5558f, 0.3660f });
            _gl.Material(OpenGL.GL_FRONT, OpenGL.GL_SHININESS, 51.2f);
        }
        else if (number == 4)
        {
            _gl.Material(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT, new[] { 0f, 0f, 0f });
            _gl.Material(OpenGL.GL_FRONT, OpenGL.GL_DIFFUSE, new[] { 0.055f, 0.055f, 0.055f });
            _gl.Material(OpenGL.GL_FRONT, OpenGL.GL_SPECULAR, new[] { 0.7f, 0.7f, 0.7f });
            _gl.Material(OpenGL.GL_FRONT, OpenGL.GL_SHININESS, 32f);
        }
        else if (number == 5)
        {
            _gl.Material(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT, new[] { 0f, 0f, 0f });
            _gl.Material(OpenGL.GL_FRONT, OpenGL.GL_DIFFUSE, new[] { 0.5f, 0f, 0f });
            _gl.Material(OpenGL.GL_FRONT, OpenGL.GL_SPECULAR, new[] { 0.7f, 0.6f, 0.6f });
            _gl.Material(OpenGL.GL_FRONT, OpenGL.GL_SHININESS, 32f);
        }
    }

    private void BufferStockCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        _gl.Enable(OpenGL.GL_DEPTH_TEST);
    }

    private void DoubleBufferingCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        _gl.Enable(OpenGL.GL_DOUBLEBUFFER);
    }

    private void ObjectFrameCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        _carcassMode = true;
    }

    private void NormalsCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        _showNormals = true;
    }

    private void OrthographicProjectionCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        _projection = Projection.Orthographic;
        SetOrthographicProjection(_gl);
    }

    private void PerspectiveProjectionCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        _projection = Projection.Perspective;
        SetPerspectiveProjection(_gl);
    }

    private void NormalsSmoothingCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        _smooth = true;
    }

    private void BufferStockCheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
        _gl.Disable(OpenGL.GL_DEPTH_TEST);
    }

    private void DoubleBufferingCheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
        _gl.Disable(OpenGL.GL_DOUBLEBUFFER);
    }

    private void ObjectFrameCheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
        _carcassMode = false;
    }

    private void NormalsCheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
        _showNormals = false;
    }

    private void NormalsSmoothingCheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
        _smooth = false;
    }

    private void OpenFileDialogButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            FileName = "Document",
            DefaultExt = ".txt",
            Filter = "Text documents (.txt) | *.txt"
        };

        var result = dialog.ShowDialog();

        if (result == true)
        {
            var filePath = dialog.FileName;
            var filename = filePath.Split("\\")[^1];
            FileNameText.Text = filename;

            _figureI.Read(filePath, out _section, out _path, out _scales);

            _figure = _figureBuilder
                .CalculateSections(_section, _path, _scales)
                .CalculateNormals()
                .Build();
        }
    }

    private void BackLightCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        _gl.Enable(OpenGL.GL_LIGHT0);
    }

    private void BackLightCheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
        _gl.Disable(OpenGL.GL_LIGHT0);
    }

    private void PointLightCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        _gl.Enable(OpenGL.GL_LIGHT2);
    }

    private void PointLightCheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
        _gl.Disable(OpenGL.GL_LIGHT2);
    }

    private void DirectLightCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        _gl.Enable(OpenGL.GL_LIGHT1);
    }

    private void DirectLightCheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
        _gl.Disable(OpenGL.GL_LIGHT1);
    }

    private void SearchLightCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        _gl.Enable(OpenGL.GL_LIGHT3);
    }

    private void SearchLightCheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
        _gl.Disable(OpenGL.GL_LIGHT3);
    }

    private void PointLightIntensityCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        _gl.Enable(OpenGL.GL_LIGHT4);
    }

    private void PointLightIntensityCheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
        _gl.Disable(OpenGL.GL_LIGHT4);
    }

    private void EmeraldMaterialRadioButton_Checked(object sender, RoutedEventArgs e)
    {
        _materialNumber = 1;
    }

    private void ObsidianMaterialRadioButton_Checked(object sender, RoutedEventArgs e)
    {
        _materialNumber = 2;
    }

    private void GoldMaterialRadioButton_Checked(object sender, RoutedEventArgs e)
    {
        _materialNumber = 3;
    }

    private void WhitePlasticMaterialRadioButton_Checked(object sender, RoutedEventArgs e)
    {
        _materialNumber = 4;
    }

    private void RedPlasticMaterialRadioButton_Checked(object sender, RoutedEventArgs e)
    {
        _materialNumber = 5;
    }

    private void FirstTextureRadioButton_Checked(object sender, RoutedEventArgs e)
    {
        var path = @"..\CG2\Textures\Texture1.png";
        var textureImage = new Bitmap(path);

        _texture.Create(_gl, textureImage);

        _gl.Enable(OpenGL.GL_TEXTURE_2D);

        _texture.Bind(_gl);

        _textureMode = true;
    }

    private void SecondTextureRadioButton_Checked(object sender, RoutedEventArgs e)
    {
        var path = @"..\CG2\Textures\Texture2.png";
        var textureImage = new Bitmap(path);

        _texture.Create(_gl, textureImage);

        _gl.Enable(OpenGL.GL_TEXTURE_2D);

        _texture.Bind(_gl);

        _textureMode = true;
    }

    private void NoMaterialRadioButton_Checked(object sender, RoutedEventArgs e)
    {
        _materialNumber = 0;
    }

    private void NoTextureRadioButton_Checked(object sender, RoutedEventArgs e)
    {
        _gl.Disable(OpenGL.GL_TEXTURE_2D);
        _textureMode = false;
    }
}