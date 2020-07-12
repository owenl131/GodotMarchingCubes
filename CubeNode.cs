using Godot;
using System.Collections.Generic;

public class CubeNode
{
    public int x, y, z;
    Vector3 centre;
    float width;
    OpenSimplexNoise noise;

    public CubeNode(int x, int y, int z, Vector3 centre, float width, OpenSimplexNoise noise) 
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.centre = centre;
        this.width = width;
        this.noise = noise;
    }

    public int WhichAxis(CubeNode nb)
    {
        for (int i = 0; i < 3; i++)
        {
            if (Nb(i).Index() == nb.Index())
            {
                return i;
            }
        }
        return -1;
    }

    public CubeNode Nb(int axis)
    {
        if (axis == 0)
        {
            return new CubeNode(1 - x, y, z, centre, width, noise);
        }
        else if (axis == 1)
        {
            return new CubeNode(x, 1 - y, z, centre, width, noise);
        }
        else // if (axis == 2)
        {
            return new CubeNode(x, y, 1 - z, centre, width, noise);
        }
    }

    public Vector3 Intermediate(int axis)
    {
        CubeNode other = Nb(axis);
        return Intermediate(other);
    }

    public Vector3 Intermediate(CubeNode other)
    {
        float val = Mathf.Abs(Eval());
        float nbVal = Mathf.Abs(other.Eval());
        Vector3 p = Coords();
        Vector3 nbP = other.Coords();
        return p + (nbP - p) * (val / (val + nbVal));
    }

    public Vector3 Normal(Vector3 point)
    {
        float delta = width / 2;
        float val = Eval(point);
        Vector3 normal = new Vector3(
            (Eval(new Vector3(delta, 0, 0) + point) - val) / delta,
            (Eval(new Vector3(0, delta, 0) + point) - val) / delta,
            (Eval(new Vector3(0, 0, delta) + point) - val) / delta
        );
        return normal.Normalized();
    }

    public int Index()
    {
        return x * 4 + y * 2 + z;
    }

    public Vector3 Coords()
    {
        return new Vector3(x, y, z) * width + centre;
    }

    public float Eval()
    {
        Vector3 worldCoords = Coords();
        return Eval(worldCoords);
    }

    public float Eval(Vector3 worldCoords)
    {
        // Single
        // if (x * y * z == 1) return 1;

        // Double
        // if (x * z == 1) return 1;
        
        // Triple
        // if (y > 0 && (x * z) == 0) return 1;

        // Tetra
        // if (y > 0 && (x * z == 0)) return 1;
        // if (x + y + z == 0) return 1;
        
        // Plane
        // if (y > 0) return 1;

        // Line
        // if (x + y + z == 0) return 1;
        // if (x == 1 && z == 0) return 1;
        // if (x + y + z == 3) return 1;
        
        // return -1;
        float bounds = MarchingCubes.CubeWidth * MarchingCubes.NumCubesPerAxis - 0.5f;
        if (worldCoords.x > bounds || worldCoords.y > bounds || worldCoords.z > bounds)
        {
            return 1;
        }
        if (worldCoords.x < 0.5f || worldCoords.y < 0.5f || worldCoords.z < 0.5f)
        {
            return 1;
        }
        return noise.GetNoise3dv(worldCoords);
    }

    public bool IsIn()
    {
        return Eval() < 0;
    }

    public IEnumerable<CubeNode> Nbs()
    {
        for (int i = 0; i < 3; i++)
        {
            CubeNode nb = Nb(i);
            if (nb.IsIn() == IsIn())
                yield return nb;
        }
    }

    public int CountNbs()
    {
        int count = 0;
        foreach (CubeNode n in Nbs())
        {
            count += 1;
        }
        return count;
    }

    public int CountNotNbs()
    {
        return 3 - CountNbs();
    }

    public override int GetHashCode()
    {
        return Index();
    }

    public override bool Equals(object obj) 
    { 
        if (obj is CubeNode)
        {
            return Index() == (obj as CubeNode).Index();
        }
        return false;
    }

    public HashSet<CubeNode> AllConnected()
    {
        HashSet<CubeNode> connected = new HashSet<CubeNode>();
        HashSet<CubeNode> toAdd = new HashSet<CubeNode>();
        connected.Add(this);
        for (int i = 0; i < 5; i++)
        {
            foreach (CubeNode nd in connected)
            {
                foreach (CubeNode n in nd.Nbs())
                {
                    toAdd.Add(n);
                }
            }
            foreach (CubeNode n in toAdd)
            {
                connected.Add(n);
            }
            toAdd.Clear();
        }
        return connected;
    }
}
