using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static UnityEngine.Mathf;


[RequireComponent(typeof(MeshFilter))]
public class MeshAnalyzer : MonoBehaviour
{
    Mesh mesh;
    public int seed;
    public int density;
    List<Vertex> scatteredVertices;
    public List<Vertex> GetScatteredVerts() {
        return scatteredVertices;
    }

    float TriangleArea(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        return (v1 - v2).magnitude // tri base
            * (v3 - v1 - Vector3.Dot((v3 - v1), (v2 - v1)) * (v2 - v1)).magnitude // tri height 
            / 2;
    }

    //Goes in order of triangle, generating a list of each triangle mapped to the total cumulative area of traversing that mesh up to that triangle
    // ex: [2.1, 14, 14.2, 21, 200, 201] where 201 is the total area
    List<float> MapTrianglesToAreas(int [] tris, Vector3 [] verts)
    {
        var areas = new List<float>();
        var areaAccumulator = 0.0f;
        for (int i = 0; i < tris.Length; i += 3)
        {
            var v1 = verts[tris[i]];
            var v2 = verts[tris[i+1]];
            var v3 = verts[tris[i+2]];
            var area = TriangleArea(v1, v2, v3);
            areas.Add(area + areaAccumulator);
            areaAccumulator += area;
        }

        return areas;
    }



    /// <summary>
    /// Places a vertex randomly inside a triangle, adjusting normals
    /// </summary>
    /// <returns></returns>
    private Vertex FillTriangleWithVertex(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 n1, Vector3 n2, Vector3 n3, System.Random rand)
    {
        Vertex v;
        float x1 = (float) rand.NextDouble();
        float x2 = (float) rand.NextDouble();
        float x3 = (float) rand.NextDouble();

        v.position = (v1 * x1 + v2 * x2 + v3 * x3) / (x1 + x2 + x3);
        v.normal = (n1 * x1 + n2 * x2 + n3 * x3) / (x1 + x2 + x3);

        return v;
    }
    private int ChooseTriangle(List<float> areas, int[] tris, System.Random rand)
    {
        var areaSum = areas.Last();
        int triangleIndex = 9999999;
        var area = rand.NextDouble() * areaSum;

        // Binary search for triangle with the random area
        if (area < areas[0])
        {
            triangleIndex = 0;
        }
        else if (area > areaSum)
        {
            Debug.LogError("Area should not be grater than total area");
        }
        else
        {
            int lo = 0, hi = areas.Count - 1, mid = (areas.Count - 1) / 2;

            while (lo <= hi)
            {
                mid = (lo + hi) / 2;
                if (areas[mid] < area)
                {
                    lo = mid + 1;
                }
                else if (areas[mid] > area)
                {
                    hi = mid - 1;
                }
                else
                {
                    lo = mid;
                    break;
                }
            }
            // lo = hi + 1
            triangleIndex = lo;
        }

        return triangleIndex;
    }

    // The reason for doing all this nonsense with areas is to make sure the triangle size doesnt influence point distribution
    //
    // areas: triangles to areas map. Should be sorted ascending with last triangle entry representing the total area of the mesh
    // tris: array of vertices. Every 3 makes a triangle. Object space
    // density: point per m^2
    // Returns an array of points scattered around evenly across the area
    private List<Vertex> DistributedSurfaceVertices(List<float> areas, int[] tris, Vector3[] verts, Vector3[] normals, float density, int seed = 0)
    {
        var areaSum = areas.Last();
        int vertexCount = (int)(density * areaSum);
        Debug.Log("Num Verts:" + vertexCount);
        List<Vertex> vertices = new List<Vertex>();
        System.Random rand = new System.Random(seed);

        for (int i = 0; i < vertexCount; i++)
        {
            int tri = 3 * ChooseTriangle(areas, tris, rand); // Index of the chosen triangle in tris array
            int i1 = tris[tri], i2 = tris[tri+1], i3 = tris[tri + 2]; // Vert and normal indexes of the triangle
            Vertex vert = FillTriangleWithVertex(
                            verts[i1],
                            verts[i2],
                            verts[i3],
                            normals[i1],
                            normals[i2],
                            normals[i3],
                            rand);
            vertices.Add(vert);
        }

        return vertices;
    }

    public void Solve(float density,int seed = 0)
    {
        var tris = mesh.triangles;
        var verts = mesh.vertices;
        var normals = mesh.normals;

        var areas = MapTrianglesToAreas(tris, verts);

        scatteredVertices = DistributedSurfaceVertices(
                                areas,
                                tris,
                                verts,
                                normals,
                                density*transform.localScale.magnitude,
                                seed);
    }


    private void OnEnable()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        if (mesh == null)
        {
            Debug.LogError("Should have mesh");
        }
        Solve((float)density, seed);
    }
    private void OnDisable()
    {
        scatteredVertices = null;
    }

    private void OnValidate()
    {
        if (scatteredVertices != null && enabled)
        {
            OnDisable();
            OnEnable();
        }
    }

    private void OnDrawGizmos()
    {
        if (scatteredVertices?.Count > 0 && enabled)
        {
            for (int i = 0; i < scatteredVertices.Count; i++)
            {
                Debug.DrawLine(
                    transform.localToWorldMatrix.MultiplyPoint(scatteredVertices[i].position),
                    transform.localToWorldMatrix.MultiplyPoint(scatteredVertices[i].position) + scatteredVertices[i].normal * .2f);
            }
        }
    }
}

public struct Vertex
{
    public Vector3 position;
    public Vector3 normal;
}