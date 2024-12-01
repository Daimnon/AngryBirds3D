using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/* could use unity's line renderer but this code is probably way more optimized. */

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class DrawStripFromPoints : MonoBehaviour
{
    [SerializeField] private float width = 0.2f;
    [SerializeField] private Vector3[] _vectorEnumerable;
    [SerializeField] private List<Vector3> _vectorList;
    [SerializeField] private float _factor = 1;

    private Mesh _m;

    private void Start()
    {
        _m = new Mesh(); // create new mesh
        GetComponent<MeshFilter>().mesh = _m; // assign gameObject's mesh filter to the new mesh

        /*_vectorList = new List<Vector3> // for testing
        {
            _vectorEnumerable[0],
            _vectorEnumerable[1],
            _vectorEnumerable[2]
        };*/
    }

    /*private void Update() // for testing
    {
        IEnumerable<Vector3> vector3Enumerable = _vectorList;
        DrawStrip(vector3Enumerable);

        for (int i = 0; i < _vectorList.Count; i++)
        {
            _vectorList[i] *= _factor;
        }
    }*/

    public void DrawStrip(IEnumerable<Vector3> p)
    {
        Vector3[] pArray = p as Vector3[] ?? p.ToArray(); // check if p is null and if not convert it to array and set points array accordingly

        _m.Clear(); // resets mesh, vertices, colors, and uvs
        Vector3[] verts = new Vector3[pArray.Length * 2]; // create new vetices
        Vector2[] uvs = new Vector2[pArray.Length * 2]; // create new uvs
        int[] tris = new int[(pArray.Length - 1) * 2 * 3 * 2]; // create new polygons

        for (int i = 0; i < pArray.Length; i++)
        {
            Vector3 curP = pArray[i];

            Vector3 prevP = i > 0 ? pArray[i - 1] : curP; // set last point
            Vector3 nextP = i < pArray.Length - 1 ? pArray[i + 1] : curP; // set next point

            Vector3 fd = (nextP - prevP).normalized; // set forward direction
            Vector3 rd = (Vector3.Cross(Vector3.up, fd)).normalized; // set right direction

            Vector3 vr = curP + rd * (width * 0.5f); // calculate right vertex
            Vector3 vl = curP - rd * (width * 0.5f); // calculate left vertex

            verts[i * 2] = vr; // set item at verts[i * 2] to be the right vertex
            verts[i * 2 + 1] = vl; // set item at  verts[i * 2 + 1] to be the left vertex

            uvs[i * 2] = new Vector2(1f, (float)i / pArray.Length);
            uvs[i * 2 + 1] = new Vector2(0f, (float)i / pArray.Length);

            if (i >= pArray.Length - 1) break;

            // set polygons
            tris[i * 12] = i * 2;
            tris[i * 12 + 1] = i * 2 + 1;
            tris[i * 12 + 2] = i * 2 + 2;
            tris[i * 12 + 3] = i * 2 + 2;
            tris[i * 12 + 4] = i * 2 + 1;
            tris[i * 12 + 5] = i * 2 + 3;

            tris[i * 12 + 6] = i * 2;
            tris[i * 12 + 7] = i * 2 + 2;
            tris[i * 12 + 8] = i * 2 + 1;
            tris[i * 12 + 9] = i * 2 + 1;
            tris[i * 12 + 10] = i * 2 + 2;
            tris[i * 12 + 11] = i * 2 + 3;

        }

        // apply to mesh filter on gameObject
        _m.vertices = verts;
        _m.uv = uvs;
        _m.triangles = tris;
    }
}