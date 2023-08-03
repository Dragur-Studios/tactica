using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct Face
{
    public List<Vector3> vertices;
    public List<int> triangles;
    public List<Vector2> uvs;

    //TODO setup normals

    public Face(List<Vector3> verts, List<int> tris, List<Vector2> uv)
    {
        vertices = verts;
        triangles = tris;
        uvs = uv;
    }
}

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class HexRenderer : MonoBehaviour
{
    Mesh _mesh;
    MeshFilter _filter;
    MeshRenderer _renderer;

    List<Face> _faces;

    public float innerSize = 0;
    public float outerSize = 0;
    public float height = 0.0f;
    public bool isFlatTopped = false;

    private void Init()
    {
        _filter = GetComponent<MeshFilter>();
        
        _mesh = new Mesh();
        _mesh.name = "Hex";
        _filter.sharedMesh = _mesh;

        _renderer = GetComponent<MeshRenderer>();
        _renderer.material = Resources.Load<Material>("Materials/Tiles/Hexagon");

        _faces = new List<Face>();
    }

    public void GenerateMesh()
    {
        Init();

        if (_mesh == null)
            return;

        CreateFaces();
        CombineFaces();
    }

    private void CombineFaces()
    {
        var verts = new List<Vector3>();
        var tris = new List<int>();
        var uv = new List<Vector2>();

        for (int i = 0; i < _faces.Count; i++)
        {
            verts.AddRange(_faces[i].vertices);
            uv.AddRange(_faces[i].uvs);

            int offset = (4 * i);
            foreach (int tri in _faces[i].triangles)
            {
                tris.Add(tri + offset);
            }

        }

        if (_mesh == null)
            _mesh = _filter.sharedMesh;

        _mesh.vertices = verts.ToArray();
        _mesh.uv = uv.ToArray();
        _mesh.triangles = tris.ToArray();

        _mesh.RecalculateNormals();
        _mesh.RecalculateBounds();
        _mesh.RecalculateTangents();

    }
    Vector3 GetPoint(float size, float height, int idx)
    {
        float angle_deg = isFlatTopped ? 60 * idx : 60 * idx-30 ;
        float angle_rad = Mathf.PI / 180.0f * angle_deg;
        return new Vector3((size* Mathf.Cos(angle_rad)), height, (size * Mathf.Sin(angle_rad)));

    }

    internal Mesh GetMesh()
    {
        return GetComponent<MeshFilter>().sharedMesh;
    }

    Face CreateFace(float innerRad, float outerRad, float heightA, float heightB, int point, bool reverse = false)
    {
        var pointA = GetPoint(innerRad, heightB, point);
        var pointB = GetPoint(innerRad, heightB, (point < 5) ?  point + 1 : 0);
        var pointC = GetPoint(outerRad, heightA, (point < 5) ? point + 1 : 0);
        var pointD = GetPoint(outerRad, heightA, point);

        var verts = new List<Vector3>() { pointA, pointB, pointC, pointD };
        var tris = new List<int>() { 0, 1, 2, 2, 3, 0 };
        var uv = new List<Vector2>() { new Vector2(0,0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1)};

        if(reverse)
        {
            verts.Reverse();
        }

        return new Face(verts, tris, uv);
    }

    private void CreateFaces()
    {
        _faces = new List<Face>();

        // TOP FACE
        for (int point = 0; point < 6; point++)
        {
            _faces.Add(CreateFace(innerSize, outerSize, height, height, point));
        }

        // BOTTOM FACE
        for (int point = 0; point < 6; point++)
        {
            _faces.Add(CreateFace(innerSize, outerSize, 0, 0, point, true));
        }

        // draw outer faces
        for (int point = 0; point < 6; point++)
        {
            _faces.Add(CreateFace(outerSize, outerSize, height , 0, point, true));
        }

        // draw inner faces
        for (int point = 0; point < 6; point++)
        {
            _faces.Add(CreateFace(innerSize, innerSize, height, 0, point));
        }

    }
}
