using Godot;
using System;

public class MarchingCubes : Spatial
{
    public float CubeWidth = 0.5f;
    public int NumCubesPerAxis = 30;
    public OpenSimplexNoise Noise;
    public ImmediateGeometry Geometry;

    public override void _Ready()
    {
        Setup();
    }

    public void Setup()
    {
        Noise = new OpenSimplexNoise();
        Noise.Period = 10f;
        Geometry = new ImmediateGeometry();
        AddChild(Geometry);
        for (int i = 0; i < NumCubesPerAxis; i++)
        {
            for (int j = 0; j < NumCubesPerAxis; j++)
            {
                for (int k = 0; k < NumCubesPerAxis; k++)
                {
                    Cube cube = new Cube(CubeWidth, Noise, Geometry);
                    cube.Translation = new Vector3(i, j, k) * CubeWidth;
                    cube.Draw();
                }
            }
        }

        Translation = -Vector3.One * NumCubesPerAxis * CubeWidth / 2;
    }

}
