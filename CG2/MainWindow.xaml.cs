using CG2.Extensions;
using CG2.Geometry;
using CG2.Models;
using CG2.Models.Figure;
using SharpGL;
using SharpGL.SceneGraph.Assets;
using SharpGL.WPF;
using System.Drawing;
using System.Numerics;
using System.Windows;
using System.Windows.Input;
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
    private Vector2[] _scales;
    private Point _previousPosition;
    private bool _carcassMode;
    private bool _showNormals;
    private bool _smooth;
    private Projection _projection;
    private Texture _texture = new Texture();

    public MainWindow()
    {
        InitializeComponent();

        _section = new Vector2[]
        {
            new(-0.2f, -0.2f), new(0.2f, -0.2f), new(0, 1)
            //new(-0.1f, -0.3f), new(0.2f, -0.3f), new(0.2f, -0.15f), new(0.1f, -0.15f), new(0.1f, 0.15f), new(0.2f, 0.15f), new(0.2f, 0.3f), new(-0.1f, 0.3f)
        };
        _path = new Vector3[]
        {
            new(0, 0, 0), new(1, 0, 0), new(2, 1, 0)
        };
        _scales = new Vector2[]
        {
            new(1f, 1f), new(1f, 1f), new(1f, 1f)
        };
        GlWindow.FrameRate = 60;
        _camera = new Camera();
        _figureBuilder = new FigureBuilder(new Mapper2D());
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

        if (_carcassMode)
        {
            _figure.DrawCarcass(_gl);
        }
        else
        {
            _figure.Draw(_gl, true, _smooth);
        }

        if (_showNormals)
        {
            _figure.DrawNormals(_gl, _smooth);
        }

        _gl.Flush();
    }

    private void OpenGLInitialized(object sender, OpenGLRoutedEventArgs args)
    {
        _gl = args.OpenGL;

        _gl.Enable(OpenGL.GL_DEPTH_TEST);
        BufferStockCheckBox.IsChecked = true;
        _gl.Disable(OpenGL.GL_DOUBLEBUFFER);

        InitLights();

        SetPerspectiveProjection(_gl);
        PerspectiveProjectionCheckBox.IsChecked = true;

        _figure = _figureBuilder
            .CalculateSections(_section, _path, _scales)
            .CalculateNormals()
            .Build();
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
        gl.Ortho(-2, 2, -2, 2, 0.1, 2);
        gl.MatrixMode(OpenGL.GL_MODELVIEW);
        gl.LoadIdentity();

        _camera.Rotate(_previousPosition, _previousPosition);
        _camera.ChangeCamera(_gl);
    }

    private void SetPerspectiveProjection(OpenGL gl)
    {
        gl.MatrixMode(OpenGL.GL_PROJECTION);
        gl.LoadIdentity();
        gl.Perspective(60f, GlWindow.ActualWidth / GlWindow.ActualHeight, 0.1f, 100f);
        gl.MatrixMode(OpenGL.GL_MODELVIEW);
        gl.LoadIdentity();

        _camera.Rotate(_previousPosition, _previousPosition);
        _camera.ChangeCamera(_gl);
    }

    private void InitLights()
    {
        _gl.LightModel(OpenGL.GL_LIGHT_MODEL_TWO_SIDE, OpenGL.GL_FALSE);

        _gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, new[] { 0.1f, 0.1f, 0.1f, 1f });
        _gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, new[] { 0f, 0f, 0f, 1f });
        _gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPECULAR, new[] { 0f, 0f, 0f, 1f });

        _gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_AMBIENT, new[] { 0f, 0f, 0f, 1f });
        _gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_DIFFUSE, new[] { 1f, 1f, 1f, 1f });
        _gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPECULAR, new[] { 1f, 1f, 1f, 1f });
        _gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_POSITION, new[] { -0.2f, -1f, -0.3f, 0f });

        _gl.Light(OpenGL.GL_LIGHT2, OpenGL.GL_AMBIENT, new[] { 0f, 0f, 0f, 1f });
        _gl.Light(OpenGL.GL_LIGHT2, OpenGL.GL_DIFFUSE, new[] { 1f, 1f, 1f, 1f });
        _gl.Light(OpenGL.GL_LIGHT2, OpenGL.GL_SPECULAR, new[] { 1f, 1f, 1f, 1f });
        _gl.Light(OpenGL.GL_LIGHT2, OpenGL.GL_POSITION, new[] { 1.2f, 1f, 1f });

        _gl.Light(OpenGL.GL_LIGHT3, OpenGL.GL_AMBIENT, new[] { 0f, 0f, 0f, 1f });
        _gl.Light(OpenGL.GL_LIGHT3, OpenGL.GL_DIFFUSE, new[] { 1f, 1f, 1f, 1f });
        _gl.Light(OpenGL.GL_LIGHT3, OpenGL.GL_SPECULAR, new[] { 1f, 1f, 1f, 1f });
        _gl.Light(OpenGL.GL_LIGHT3, OpenGL.GL_POSITION, new[] { 1f, 1f, 1f, 1f });
        _gl.Light(OpenGL.GL_LIGHT3, OpenGL.GL_SPOT_EXPONENT, 30f);
        _gl.Light(OpenGL.GL_LIGHT3, OpenGL.GL_SPOT_CUTOFF, 20f);
        _gl.Light(OpenGL.GL_LIGHT3, OpenGL.GL_SPOT_DIRECTION, new[] { -1f, -1f, -1f, 0f });

        _gl.Light(OpenGL.GL_LIGHT4, OpenGL.GL_AMBIENT, new[] { 0f, 0f, 0f, 1f });
        _gl.Light(OpenGL.GL_LIGHT4, OpenGL.GL_DIFFUSE, new[] { 1f, 1f, 1f, 1f });
        _gl.Light(OpenGL.GL_LIGHT4, OpenGL.GL_SPECULAR, new[] { 1f, 1f, 1f, 1f });
        _gl.Light(OpenGL.GL_LIGHT4, OpenGL.GL_POSITION, new[] { 1f, 1f, 1f, 1f });
        _gl.Light(OpenGL.GL_LIGHT4, OpenGL.GL_CONSTANT_ATTENUATION, 0f);
        _gl.Light(OpenGL.GL_LIGHT4, OpenGL.GL_LINEAR_ATTENUATION, 5e-3f);
        _gl.Light(OpenGL.GL_LIGHT4, OpenGL.GL_QUADRATIC_ATTENUATION, 0f);
    }

    private void SetTexture()
    {

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
        var dialog = new Microsoft.Win32.OpenFileDialog();
        dialog.FileName = "Document";
        dialog.DefaultExt = ".txt";
        dialog.Filter = "Text documents (.txt)|*.txt";

        bool? result = dialog.ShowDialog();

        if (result == true)
        {
            string filename = dialog.FileName;
            FileNameText.Text = filename;
        }

    }

    private void BackLightCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        _gl.Enable(OpenGL.GL_LIGHTING);

        _gl.Enable(OpenGL.GL_LIGHT0);
    }

    private void BackLightCheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
        _gl.Disable(OpenGL.GL_LIGHT0);
    }

    private void PointLightCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        _gl.Enable(OpenGL.GL_LIGHTING);

        _gl.Enable(OpenGL.GL_LIGHT1);
    }

    private void PointLightCheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
        _gl.Disable(OpenGL.GL_LIGHT1);
    }

    private void DirectLightCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        _gl.Enable(OpenGL.GL_LIGHTING);

        _gl.Enable(OpenGL.GL_LIGHT2);
    }

    private void DirectLightCheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
        _gl.Disable(OpenGL.GL_LIGHT2);
    }

    private void SearchLightCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        _gl.Enable(OpenGL.GL_LIGHTING);

        _gl.Enable(OpenGL.GL_LIGHT3);
    }

    private void SearchLightCheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
        _gl.Disable(OpenGL.GL_LIGHT3);
    }

    private void SearchLight2CheckBox_Checked(object sender, RoutedEventArgs e)
    {
        _gl.Enable(OpenGL.GL_LIGHTING);

        _gl.Enable(OpenGL.GL_LIGHT4);
    }

    private void SearchLight2CheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
        _gl.Disable(OpenGL.GL_LIGHT4);
    }

    private void EmeraldMaterialRadioButton_Checked(object sender, RoutedEventArgs e)
    {
        //_gl.Disable(OpenGL.GL_COLOR_MATERIAL);

        _gl.Material(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT, new[] { 0.0215f, 0.1745f, 0.0215f });
        _gl.Material(OpenGL.GL_FRONT, OpenGL.GL_DIFFUSE, new[] { 0.0756f, 0.6142f, 0.0756f });
        _gl.Material(OpenGL.GL_FRONT, OpenGL.GL_SPECULAR, new[] { 0.6330f, 0.7278f, 0.6330f });
        _gl.Material(OpenGL.GL_FRONT, OpenGL.GL_SHININESS, 0.6f);

        //_gl.Enable(OpenGL.GL_COLOR_MATERIAL);
    }

    private void ObsidianMaterialRadioButton_Checked(object sender, RoutedEventArgs e)
    {
        //_gl.Disable(OpenGL.GL_COLOR_MATERIAL);

        _gl.Material(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT, new[] { 0.0537f, 0.05f, 0.0662f });
        _gl.Material(OpenGL.GL_FRONT, OpenGL.GL_DIFFUSE, new[] { 0.1827f, 0.17f, 0.2252f });
        _gl.Material(OpenGL.GL_FRONT, OpenGL.GL_SPECULAR, new[] { 0.3327f, 0.3286f, 0.3464f });
        _gl.Material(OpenGL.GL_FRONT, OpenGL.GL_SHININESS, 0.3f);

        //_gl.Enable(OpenGL.GL_Ma);
    }

    private void GoldMaterialRadioButton_Checked(object sender, RoutedEventArgs e)
    {
        //_gl.Disable(OpenGL.GL_COLOR_MATERIAL);

        _gl.Material(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT, new[] { 0.2472f, 0.1995f, 0.0745f });
        _gl.Material(OpenGL.GL_FRONT, OpenGL.GL_DIFFUSE, new[] { 0.7516f, 0.6064f, 0.2264f });
        _gl.Material(OpenGL.GL_FRONT, OpenGL.GL_SPECULAR, new[] { 0.6282f, 0.5558f, 0.3660f });
        _gl.Material(OpenGL.GL_FRONT, OpenGL.GL_SHININESS, 0.4f);

        //_gl.Enable(OpenGL.GL_COLOR_MATERIAL);
    }

    private void WhitePlasticMaterialRadioButton_Checked(object sender, RoutedEventArgs e)
    {
        //_gl.Disable(OpenGL.GL_COLOR_MATERIAL);

        _gl.Material(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT, new[] { 0f, 0f, 0f });
        _gl.Material(OpenGL.GL_FRONT, OpenGL.GL_DIFFUSE, new[] { 0.055f, 0.055f, 0.055f });
        _gl.Material(OpenGL.GL_FRONT, OpenGL.GL_SPECULAR, new[] { 0.7f, 0.7f, 0.7f });
        _gl.Material(OpenGL.GL_FRONT, OpenGL.GL_SHININESS, 0.25f);

        //_gl.Enable(OpenGL.GL_COLOR_MATERIAL);
    }

    private void RedPlasticMaterialRadioButton_Checked(object sender, RoutedEventArgs e)
    {
        //_gl.Disable(OpenGL.GL_COLOR_MATERIAL);

        _gl.Material(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT, new[] { 0f, 0f, 0f });
        _gl.Material(OpenGL.GL_FRONT, OpenGL.GL_DIFFUSE, new[] { 0.5f, 0f, 0f });
        _gl.Material(OpenGL.GL_FRONT, OpenGL.GL_SPECULAR, new[] { 0.7f, 0.6f, 0.6f });
        _gl.Material(OpenGL.GL_FRONT, OpenGL.GL_SHININESS, 0.25f);

        //_gl.Enable(OpenGL.GL_COLOR_MATERIAL);
    }

    private void FirstTextureRadioButton_Checked(object sender, RoutedEventArgs e)
    {
        var path = @"..\CG2\Textures\Texture1.png";
        var textureImage = new Bitmap(path);

        _gl.Disable(OpenGL.GL_TEXTURE_2D);

        _texture.Create(_gl, textureImage);

        _gl.Enable(OpenGL.GL_TEXTURE_2D);

        _texture.Bind(_gl);
    }

    private void SecondTextureRadioButton_Checked(object sender, RoutedEventArgs e)
    {
        var path = @"..\CG2\Textures\Texture2.png";
        var textureImage = new Bitmap(path);

        _gl.Disable(OpenGL.GL_TEXTURE_2D);

        _texture.Create(_gl, textureImage);

        _gl.Enable(OpenGL.GL_TEXTURE_2D);

        _texture.Bind(_gl);
    }
}