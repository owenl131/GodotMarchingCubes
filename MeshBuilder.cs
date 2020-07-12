using Godot;
using System.Collections.Generic;

public class MeshBuilder
{
    public Dictionary<int, int> BufferIndex;
    public List<Vector3> VertexBuffer;
    public List<Vector3> NormalBuffer;
    public List<int> IndexBuffer;

    public MeshBuilder()
    {
        BufferIndex = new Dictionary<int, int>();
        VertexBuffer = new List<Vector3>();
        NormalBuffer = new List<Vector3>();
        IndexBuffer = new List<int>();
    }

    private int GetIndex(int cubeIndex, CubeNode node, int axis)
    {
        int key = cubeIndex * 8 + node.Index();
        key = key * 4 + axis;
        if (BufferIndex.ContainsKey(key))
        {
            return BufferIndex[key];
        }
        int index = VertexBuffer.Count;
        Vector3 position = node.Intermediate(axis);
        VertexBuffer.Add(position);
        NormalBuffer.Add(node.Normal(position));
        BufferIndex.Add(key, index);
        return index;
    }

    public void AddTriangle(
        int cubeIndex, 
        CubeNode n1, int axis1, 
        CubeNode n2, int axis2,
        CubeNode n3, int axis3)
    {
        if (n1.Nb(axis1).Index() < n1.Index()) n1 = n1.Nb(axis1);
        if (n2.Nb(axis2).Index() < n2.Index()) n2 = n2.Nb(axis2);
        if (n3.Nb(axis3).Index() < n3.Index()) n3 = n3.Nb(axis3);

        IndexBuffer.Add(GetIndex(cubeIndex, n1, axis1));
        IndexBuffer.Add(GetIndex(cubeIndex, n2, axis2));
        IndexBuffer.Add(GetIndex(cubeIndex, n3, axis3));
        
        IndexBuffer.Add(GetIndex(cubeIndex, n3, axis3));
        IndexBuffer.Add(GetIndex(cubeIndex, n2, axis2));
        IndexBuffer.Add(GetIndex(cubeIndex, n1, axis1));
    }
}
