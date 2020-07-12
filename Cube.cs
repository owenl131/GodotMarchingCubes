using Godot;
using System.Collections.Generic;

public class Cube : Spatial
{
    OpenSimplexNoise noise;
    float width;
    public bool debug = false;
    public int cubeIndex;
    public MeshBuilder meshBuilder;

    public Cube()
    {

    }

    public Cube(int cubeIndex, float width, OpenSimplexNoise noise)
    {
        this.cubeIndex = cubeIndex;
        this.width = width;
        this.noise = noise;
    }

    public IEnumerable<CubeNode> Vertices()
    {
        for (int i = 0; i <= 1; i++)
        {
            for (int j = 0; j <= 1; j++)
            {
                for (int k = 0; k <= 1; k++)
                {
                    yield return new CubeNode(i, j, k, Transform.origin, width, noise);
                }
            }
        }
    }


    public void Draw()
    {
        bool[] processed = new bool[8];

        foreach (CubeNode nd in Vertices())
        {
            if (debug)
                GD.Print(nd.x, " ", nd.y, " ", nd.z, " ", nd.IsIn());

            if (processed[nd.Index()]) 
            {
                continue;
            }

            HashSet<CubeNode> connected = nd.AllConnected();
            if (debug)
                GD.Print(connected.Count);

            if (connected.Count == 1)
            {
                if (debug)
                    GD.Print("Single");

                meshBuilder.AddTriangle(cubeIndex, nd, 0, nd, 1, nd, 2);
                processed[nd.Index()] = true;
                continue;
            }

            if (connected.Count == 2)
            {
                IEnumerator<CubeNode> e = connected.GetEnumerator();
                e.MoveNext();
                CubeNode n1 = e.Current;
                e.MoveNext();
                CubeNode n2 = e.Current;

                int nbAxis = n1.WhichAxis(n2);
                
                meshBuilder.AddTriangle(cubeIndex, n1, (nbAxis + 1) % 3, n2, (nbAxis + 1) % 3, n2, (nbAxis + 2) % 3);
                meshBuilder.AddTriangle(cubeIndex, n2, (nbAxis + 2) % 3, n1, (nbAxis + 2) % 3, n1, (nbAxis + 1) % 3);

                if (debug)
                    GD.Print("Double");

                processed[n1.Index()] = true;
                processed[n2.Index()] = true;
                continue;
            }

            if (connected.Count == 3)
            {
                // find middle elem
                CubeNode md = nd;
                foreach (CubeNode n in connected)
                {
                    if (n.CountNbs() == 2)
                    {
                        md = n;
                    }
                }
                int axis = 0;
                for (int i = 0; i < 3; i++)
                {
                    if (md.Nb(i).IsIn() != md.IsIn())
                    {
                        axis = i;
                    }
                }
                CubeNode nb1 = md.Nb((axis + 1) % 3);
                CubeNode nb2 = md.Nb((axis + 2) % 3);
                Vector3 intermediate1 = nb1.Intermediate(axis);
                Vector3 intermediate2 = nb2.Intermediate(axis);

                // parallel
                meshBuilder.AddTriangle(cubeIndex, md, axis, nb1, axis, nb2, axis);
                // part 1
                meshBuilder.AddTriangle(cubeIndex, nb1, axis, nb2, axis, nb1, (axis + 2) % 3);
                // part 2
                meshBuilder.AddTriangle(cubeIndex, nb1, (axis + 2) % 3, nb2, (axis + 1) % 3, nb2, axis);

                if (debug)
                    GD.Print("Trio");

                processed[md.Index()] = true;
                processed[nb1.Index()] = true;
                processed[nb2.Index()] = true;
                continue;
            }

            if (connected.Count > 4)
            {
                foreach (CubeNode c in connected)
                {
                    processed[c.Index()] = true;
                }
                continue;
            }

            if (!nd.IsIn())
            {
                foreach (CubeNode c in connected)
                {
                    processed[c.Index()] = true;
                }
                continue;
            }

            // connected.Count is 4
            // square, tetra or line

            CubeNode mid = nd;
            bool tetra = false;

            foreach (CubeNode c in connected)
            {
                if (c.CountNbs() == 3)
                {
                    // tetra
                    mid = c;
                    tetra = true;
                }
            }

            if (tetra)
            {
                CubeNode n0 = mid.Nb(0);
                CubeNode n1 = mid.Nb(1);
                CubeNode n2 = mid.Nb(2);

                meshBuilder.AddTriangle(cubeIndex, n0, 1, n1, 0, n1, 2);
                meshBuilder.AddTriangle(cubeIndex, n0, 1, n1, 2, n0, 2);
                meshBuilder.AddTriangle(cubeIndex, n1, 2, n0, 2, n2, 1);
                meshBuilder.AddTriangle(cubeIndex, n0, 2, n2, 1, n2, 0);

                if (debug)
                    GD.Print("Tetra");

                processed[mid.Index()] = true;
                processed[n0.Index()] = true;
                processed[n1.Index()] = true;
                processed[n2.Index()] = true;
                continue;
            }

            bool sameX = true, sameY = true, sameZ = true;
            foreach (CubeNode n in connected)
            {
                if (n.x != nd.x) sameX = false;
                if (n.y != nd.y) sameY = false;
                if (n.z != nd.z) sameZ = false;
            }

            if (sameX || sameY || sameZ)
            {
                int axis = 0;
                if (sameY) axis = 1;
                if (sameZ) axis = 2;

                // plane
                meshBuilder.AddTriangle(cubeIndex, 
                    nd, axis, 
                    nd.Nb((axis + 1) % 3), axis, 
                    nd.Nb((axis + 2) % 3), axis);
                meshBuilder.AddTriangle(cubeIndex, 
                    nd.Nb((axis + 1) % 3), axis, 
                    nd.Nb((axis + 2) % 3), axis, 
                    nd.Nb((axis + 1) % 3).Nb((axis + 2) % 3), axis);

                if (debug)
                    GD.Print("Plane");
                foreach (CubeNode c in connected)
                {
                    processed[c.Index()] = true;
                }
                continue;
            }

            // line
            CubeNode l1 = nd, l2 = nd, l3 = nd, l4 = nd;
            foreach (CubeNode n in connected)
            {
                if (n.CountNbs() == 1)
                {
                    l1 = n;
                }
            }
            foreach (CubeNode n in connected)
            {
                if (l1.WhichAxis(n) != -1)
                {
                    l2 = n;
                }
            }
            foreach (CubeNode n in connected)
            {
                if (n.Index() != l1.Index() && l2.WhichAxis(n) != -1)
                {
                    l3 = n;
                }
            }
            foreach (CubeNode n in connected)
            {
                if (n.Index() != l2.Index() && l3.WhichAxis(n) != -1)
                {
                    l4 = n;
                }
            }

            int axis1 = l1.WhichAxis(l2);
            int axis2 = l2.WhichAxis(l3);
            int axis3 = l3.WhichAxis(l4);

            meshBuilder.AddTriangle(cubeIndex, l1, axis3, l2, axis3, l1, axis2);
            meshBuilder.AddTriangle(cubeIndex, l1, axis2, l3, axis1, l4, axis1);
            meshBuilder.AddTriangle(cubeIndex, l2, axis3, l1, axis2, l4, axis1);
            meshBuilder.AddTriangle(cubeIndex, l2, axis3, l4, axis1, l4, axis2);

            if (debug)
                GD.Print("Line");

            foreach (CubeNode c in connected)
            {
                processed[c.Index()] = true;
            }
            continue;
        }

        // geometry.End();
    }




}
