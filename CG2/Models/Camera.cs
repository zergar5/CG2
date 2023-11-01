using CG2.Extensions;
using SharpGL;
using System;
using System.Numerics;
using System.Windows;

namespace CG2.Models;

public class Camera
{
    private Vector3 _cameraPosition = new(0f, 0f, 3f);
    private Vector3 _cameraFront = Vector3.UnitZ;
    private Vector3 _cameraUp = Vector3.UnitY;

    private float _yaw = -90.0f;
    private float _pitch = 0f;
    private float _speed = 2.5f * 1f / 60f;

    private const float Sensitivity = 0.15f;

    public void ChangeCamera(OpenGL openGl)
    {
        openGl.MatrixMode(OpenGL.GL_MODELVIEW);
        openGl.LoadIdentity();

        var cameraDir = Vector3.Add(_cameraPosition, _cameraFront);

        openGl.LookAt(
            _cameraPosition.X, _cameraPosition.Y, _cameraPosition.Z,
            cameraDir.X, cameraDir.Y, cameraDir.Z,
            _cameraUp.X, _cameraUp.Y, _cameraUp.Z);

        
    }

    public void MoveRight()
    {
        _cameraPosition += Vector3.Normalize(Vector3.Cross(_cameraFront, _cameraUp)) * _speed;
    }

    public void MoveLeft()
    {
        _cameraPosition -= Vector3.Normalize(Vector3.Cross(_cameraFront, _cameraUp)) * _speed;
    }

    public void MoveForward()
    {
        _cameraPosition += _speed * _cameraFront;
    }

    public void MoveBackward()
    {
        _cameraPosition -= _speed * _cameraFront;
    }

    public void MoveUp()
    {
        _cameraPosition += _speed * Vector3.UnitY;
    }

    public void MoveDown()
    {
        _cameraPosition -= _speed * Vector3.UnitY;
    }

    public void Rotate(Point currentMousePos, Point prevMousePos)
    {
        var xOffset = (currentMousePos.X - prevMousePos.X) * Sensitivity;
        var yOffset = (prevMousePos.Y - currentMousePos.Y) * Sensitivity;

        _yaw += (float)xOffset;
        _pitch += (float)yOffset;

        if (_pitch > 89.0f)
            _pitch = 89.0f;
        if (_pitch < -89.0f)
            _pitch = -89.0f;

        var yawRadians = _yaw.ToRadians();
        var pitchRadians = _pitch.ToRadians();

        Vector3 direction;

        direction.X = (float)(Math.Cos(yawRadians) * Math.Cos(pitchRadians));
        direction.Y = (float)Math.Sin(pitchRadians);
        direction.Z = (float)(Math.Sin(yawRadians) * Math.Cos(pitchRadians));

        _cameraFront = Vector3.Normalize(direction);
    }
}