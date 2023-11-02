using System.Numerics;

namespace CG2.Geometry;

public class Mapper2D
{
    private Vector2[]? _section;

    private float _leftBorder;
    private float _rightBorder;
    private float _topBorder;
    private float _bottomBorder;

    public void Start(Vector2[] section)
    {
        if (section.Length < 3)
        {
            throw new System.Exception("");
        }

        _section = section;

        CreateSectionBox(_section);
    }

    private void CreateSectionBox(Vector2[] section)
    {
        _leftBorder = section[0].X;
        _rightBorder = section[0].X;

        _topBorder = section[0].Y;
        _bottomBorder = section[0].Y;

        for (var i = 1; i < section.Length; i++)
        {
            if (section[i].X > _rightBorder)
            {
                _rightBorder = section[i].X;
            }

            if (section[i].X < _leftBorder)
            {
                _leftBorder = section[i].X;
            }

            if (section[i].Y > _topBorder)
            {
                _topBorder = section[i].Y;
            }

            if (section[i].Y < _bottomBorder)
            {
                _bottomBorder = section[i].Y;
            }
        }
    }

    public Vector2 Lerp(Vector2 points)
    {
        return new Vector2((points.X - _leftBorder) / (_rightBorder - _leftBorder),
            (points.Y - _bottomBorder) / (_topBorder - _bottomBorder));
    }

    public void Finish()
    {
        _section = null;
    }
}