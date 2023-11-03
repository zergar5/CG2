using System.IO;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Globalization;
using System.Threading;

namespace CG2.IO;

public class FigureIO
{
    public void Read(string filePath, out Vector2[] section, out Vector3[] path, out Vector2[] scales)
    {
        using var reader = new StreamReader(filePath);

        var sectionList = new List<Vector2>();
        var pathList = new List<Vector3>();
        var scalesList = new List<Vector2>();

        while (reader.ReadLine() is { } line)
        {
            var data = line.Split(' ');

            if (data.Length == 2)
            {
                sectionList.Add(
                    new Vector2(
                        float.Parse(data[0], CultureInfo.InvariantCulture), 
                        float.Parse(data[1], CultureInfo.InvariantCulture)
                        )
                    );
            }
            else if(data.Length == 5)
            {
                pathList.Add(
                    new Vector3(
                        float.Parse(data[0], CultureInfo.InvariantCulture), 
                        float.Parse(data[1], CultureInfo.InvariantCulture), 
                        float.Parse(data[2], CultureInfo.InvariantCulture)
                        )
                    );

                scalesList.Add(
                    new Vector2(
                        float.Parse(data[3], CultureInfo.InvariantCulture), 
                        float.Parse(data[4], CultureInfo.InvariantCulture)
                        )
                    );
            }
        }

        section = sectionList.ToArray();
        path = pathList.ToArray();
        scales = scalesList.ToArray();
    }
}