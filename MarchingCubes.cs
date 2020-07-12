using Godot;
using System;

public class MarchingCubes : Spatial
{
    public static float CubeWidth = 0.1f;
    public static int NumCubesPerAxis = 100;
    public OpenSimplexNoise Noise;
    public MeshBuilder meshBuilder;
    public MeshInstance meshInstance;

    public override void _Ready()
    {
        Setup();
    }

    public int GetIndex(int x, int y, int z)
    {
        return x * NumCubesPerAxis * NumCubesPerAxis + y * NumCubesPerAxis + z;
    }

    public void Setup()
    {
        meshInstance = new MeshInstance();
        meshBuilder = new MeshBuilder();
        Noise = new OpenSimplexNoise();
        Noise.Period = 6f;
        Noise.Seed = 1;

        for (int i = 0; i < NumCubesPerAxis; i++)
        {
            for (int j = 0; j < NumCubesPerAxis; j++)
            {
                for (int k = 0; k < NumCubesPerAxis; k++)
                {
                    int index = GetIndex(i, j, k);
                    Cube cube = new Cube(index, CubeWidth, Noise);
                    cube.meshBuilder = meshBuilder;
                    cube.Translation = new Vector3(i, j, k) * CubeWidth;

                    cube.Draw();
                }
            }
        }

        ArrayMesh mesh = new ArrayMesh();
        Godot.Collections.Array arr = new Godot.Collections.Array();
        arr.Resize((int) ArrayMesh.ArrayType.Max);
        arr[(int) ArrayMesh.ArrayType.Vertex] = meshBuilder.VertexBuffer.ToArray();
        arr[(int) ArrayMesh.ArrayType.Normal] = meshBuilder.NormalBuffer.ToArray();
        arr[(int) ArrayMesh.ArrayType.Index] = meshBuilder.IndexBuffer.ToArray();
        mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arr);

        meshInstance.Mesh = mesh;
        AddChild(meshInstance);

        Translation = -Vector3.One * NumCubesPerAxis * CubeWidth / 2;
    }

}
