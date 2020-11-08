using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class builder{
    public Transform cube;
    private int dimension;
    private float[,,] results;
    private int[,,] vertexBuffer;
    public Queue<builder> finishQueue;

    private Vector3[] positions;
    private Vector3[] normals;
    private int[] triangles;
    
    Block myBlock;

    private Vector3 fullSize;

    public void finishJob()
    {
        Mesh ourMesh = new Mesh();
        ourMesh.vertices = positions;
        ourMesh.triangles = triangles;
        ourMesh.normals = normals;
        Transform result = MonoBehaviour.Instantiate(cube, Vector3.zero, Quaternion.identity) as Transform;
        result.GetComponent<MeshFilter>().mesh = ourMesh;
        myBlock.finishBuild(result, Block.parMult(myBlock.start, fullSize), Block.parMult(myBlock.size, fullSize));
    }

    public void startJob(CallIteration iterator, Block block, Vector3 fullSize)
    {
        this.fullSize = fullSize;
        myBlock = block;
        iterator.run(block.start, block.size, results);
        new System.Threading.Thread(this.build).Start();
    }

    public builder (int dimension, Queue<builder> finishQueue, Transform start)
    {
        cube = start;
        vertexBuffer = new int[dimension + 1,dimension + 1,dimension + 1];
        results = new float[dimension , dimension, dimension];
        this.dimension = dimension;
        this.finishQueue = finishQueue;
    }

    public float[,,] getResultBuffer()
    {
        return results;
    }

    public Vector3[] makeVerts() 
    {
        List<Vector3> vertexPositions = new List<Vector3>();
        for (int z = 0; z < dimension + 1; z++)
        {
            for (int y = 0; y < dimension + 1; y++)
            {
                for (int x = 0; x < dimension + 1; x++)
                {
                    if (hasVertex(x, y, z))
                    {
                        vertexBuffer[x, y, z] = vertexPositions.Count;
                        vertexPositions.Add(new Vector3(x, y, z) / dimension);
                    }
                }
            }
        }
        return vertexPositions.ToArray();
    }

    private bool hasVertex(int x, int y, int z)
    {
        return (inSection(x, y, z) || inSection(x - 1, y, z) ||
                        inSection(x, y - 1, z) || inSection(x - 1, y - 1, z) ||
                        inSection(x, y, z - 1) || inSection(x - 1, y, z - 1) ||
                        inSection(x, y - 1, z - 1) || inSection(x - 1, y - 1, z - 1)) &&
                        (!inSection(x, y, z) || !inSection(x - 1, y, z) ||
                        !inSection(x, y - 1, z) || !inSection(x - 1, y - 1, z) ||
                        !inSection(x, y, z - 1) || !inSection(x - 1, y, z - 1) ||
                        !inSection(x, y - 1, z - 1) || !inSection(x - 1, y - 1, z - 1));
    }

    public int[] newTris()
    {
        List<int> toReturn = new List<int>(vertexBuffer.Length);
        for (int z = 0; z < dimension; z++)
        {
            for (int y = 0; y < dimension; y++)
            {
                for (int x = 0; x < dimension; x++)
                {
                    if (!inSection(x - 1, y, z) &&
                        inSection(x, y, z))
                    {
                        toReturn.Add(vertexBuffer[x,y,z]);
                        toReturn.Add(vertexBuffer[x, y, z + 1]);
                        toReturn.Add(vertexBuffer[x, y + 1, z]);

                        toReturn.Add(vertexBuffer[x, y + 1, z]);
                        toReturn.Add(vertexBuffer[x, y, z + 1]);
                        toReturn.Add(vertexBuffer[x, y + 1, z + 1]);
                    }
                    if (inSection(x, y, z) &&
                        !inSection(x + 1, y, z))
                    {
                        toReturn.Add(vertexBuffer[x + 1, y, z]);
                        toReturn.Add(vertexBuffer[x + 1, y + 1, z]);
                        toReturn.Add(vertexBuffer[x + 1, y, z + 1]);

                        toReturn.Add(vertexBuffer[x + 1, y, z + 1]);
                        toReturn.Add(vertexBuffer[x + 1, y + 1, z]);
                        toReturn.Add(vertexBuffer[x + 1, y + 1, z + 1]);
                    }

                    if (!inSection(x , y-1, z) &&
                        inSection(x, y, z))
                    {
                        toReturn.Add(vertexBuffer[x, y, z]);
                        toReturn.Add(vertexBuffer[x + 1, y, z]);
                        toReturn.Add(vertexBuffer[x, y, z + 1]);

                        toReturn.Add(vertexBuffer[x, y, z + 1]);
                        toReturn.Add(vertexBuffer[x + 1, y, z]);
                        toReturn.Add(vertexBuffer[x + 1, y, z + 1]);
                    }
                    if (inSection(x, y, z) &&
                        !inSection(x, y + 1, z))
                    {
                        toReturn.Add(vertexBuffer[x, y+1, z]);
                        toReturn.Add(vertexBuffer[x, y + 1, z + 1]);
                        toReturn.Add(vertexBuffer[x + 1, y + 1, z]);

                        toReturn.Add(vertexBuffer[x + 1, y + 1, z]);
                        toReturn.Add(vertexBuffer[x, y + 1, z + 1]);
                        toReturn.Add(vertexBuffer[x + 1, y + 1, z + 1]);
                    }

                    if (!inSection(x, y, z -1) &&
                        inSection(x, y, z))
                    {
                        toReturn.Add(vertexBuffer[x, y, z]);
                        toReturn.Add(vertexBuffer[x, y + 1, z]);
                        toReturn.Add(vertexBuffer[x + 1, y, z]);

                        toReturn.Add(vertexBuffer[x + 1, y, z]);
                        toReturn.Add(vertexBuffer[x, y + 1, z]);
                        toReturn.Add(vertexBuffer[x + 1, y + 1, z]);
                    }
                    if (inSection(x, y, z) &&
                       !inSection(x, y, z+1))
                    {
                        toReturn.Add(vertexBuffer[x, y, z + 1]);
                        toReturn.Add(vertexBuffer[x + 1, y, z + 1]);
                        toReturn.Add(vertexBuffer[x, y + 1, z + 1]);

                        toReturn.Add(vertexBuffer[x, y + 1, z + 1]);
                        toReturn.Add(vertexBuffer[x + 1, y, z + 1]);
                        toReturn.Add(vertexBuffer[x + 1, y + 1, z + 1]);
                    }
                }
            }
        }
        return toReturn.ToArray();
    }

    public Vector3[] getVertexNormals()
    {
        List<Vector3> normals = new List<Vector3>(positions.Length);
        foreach (Vector3 pos in positions)
        {
            Vector3 rp = pos * dimension;
            int x = (int)rp.x;
            int y = (int)rp.y;
            int z = (int)rp.z;

            Vector3 normal = Vector3.zero;
            if (countsForNorm(x-1, y-1, z-1))
                normal += new Vector3(-1, -1, -1);
            if (countsForNorm(x, y - 1, z - 1))
                normal += new Vector3(1, -1, -1);
            if (countsForNorm(x - 1, y, z - 1))
                normal += new Vector3(-1, 1, -1);
            if (countsForNorm(x, y, z - 1))
                normal += new Vector3(1, 1, -1);

            if (countsForNorm(x - 1, y - 1, z))
                normal += new Vector3(-1, -1, 1);
            if (countsForNorm(x, y - 1, z))
                normal += new Vector3(1, -1, 1);
            if (countsForNorm(x - 1, y, z))
                normal += new Vector3(-1, 1, 1);
            if (countsForNorm(x, y, z))
                normal += new Vector3(1, 1, 1);

            if (normal == Vector3.zero)
            {
                normal = Vector3.up;
            }

            normals.Add(normal.normalized);
        }
        return normals.ToArray();
    }

    public Vector3[] adjustPosition()
    {
        List<Vector3> newPositions = new List<Vector3>(positions.Length);
        foreach (Vector3 pos in positions)
        {
            Vector3 rp = pos * dimension;
            int x = (int)rp.x;
            int y = (int)rp.y;
            int z = (int)rp.z;
            if (x == 0 || y == 0 || z == 0 || !inBounds(x, y, x))
            {
                newPositions.Add(pos);
                continue;
            }

            Vector3 contractVect = Vector3.zero;
            if (hasVertex(x - 1, y,z))
                contractVect += new Vector3(-1, 0, 0);
            if (hasVertex(x + 1, y, z))
                contractVect += new Vector3(1, 0, 0);
            
            if (hasVertex(x, y-1, z))
                contractVect += new Vector3(0,-1, 0);
            if (hasVertex(x, y+1, z))
                contractVect += new Vector3(0, 1, 0);

            if (hasVertex(x, y, z-1))
                contractVect += new Vector3(0, 0, -1);
            if (hasVertex(x, y, z+1))
                contractVect += new Vector3(0, 0, 1);

            newPositions.Add(pos + contractVect*0.4f/dimension);
        }
        return newPositions.ToArray();
    }

    public void build()
    {
        positions = makeVerts();
        triangles = newTris();
        normals = getVertexNormals();

        positions = adjustPosition();
        lock (finishQueue)
        {
            finishQueue.Enqueue(this);
        }
    }

    private bool inBounds(int xInd, int yInd, int zInd)
    {
        return xInd >= 0 && yInd >= 0 && zInd >= 0 && xInd < dimension && yInd < dimension && zInd < dimension;
    }

    private bool countsForNorm(int xInd, int yInd, int zInd)
    {
        return inBounds(xInd, yInd, zInd) &&
            results[xInd, yInd, zInd] > 0;
    }

    private bool inSection(int xInd, int yInd, int zInd)
    {
        return inBounds(xInd, yInd, zInd) &&
            results[xInd, yInd, zInd] == 0;
    }
}
