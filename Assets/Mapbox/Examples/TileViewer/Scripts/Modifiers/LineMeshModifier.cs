
namespace Mapbox.Examples.Tileviewer
{
	using System.Collections.Generic;
	using System.Linq;
	using Mapbox.VectorTile;
	using Mapbox.Scripts.Utilities;
	using TriangleNet;
	using TriangleNet.Geometry;
	using UnityEngine;

    public class LineMeshModifier : MeshModifier
    {
        public float Width = 4;
        public override ModifierType Type { get { return ModifierType.Preprocess; } }

        public override void Run(VectorTileFeature feature, MeshData md)
        {
            var count = md.Vertices.Count;
            var tempList = new Vector3[count * 2];
            var uvList = new Vector2[count * 2];

            Vector3 lastPos = Vector3.zero;
            Vector3 norm;
            var lastUv = 0f;
            for (int i = 1; i < count; i++)
            {
                var p1 = md.Vertices[i - 1];
                var p2 = md.Vertices[i];
                var p3 = p2;
                if (i + 1 < md.Vertices.Count)
                    p3 = md.Vertices[i + 1];

                if (lastPos == Vector3.zero)
                {
                    lastPos = Vector3.Lerp(p1, p2, 0f);
                    norm = GetNormal(p1, lastPos, p2) * Width; //road width
                    tempList[0] = (lastPos + norm);
                    tempList[count * 2 - 1] = (lastPos - norm);
                    uvList[0] = new Vector2(0, 0);
                    uvList[count * 2 - 1] = new Vector2(1, 0);
                }
                var dist = Vector3.Distance(lastPos, p2);
                lastUv += dist;
                lastPos = p2;
                //lastPos = Vector3.Lerp(p1, p2, 1f);
                norm = GetNormal(p1, lastPos, p3) * Width;
                tempList[i] = (lastPos + norm);
                tempList[2 * count - 1 - i] = (lastPos - norm);

                uvList[i] = new Vector2(0, lastUv);
                uvList[2 * count - 1 - i] = new Vector2(1, lastUv);
            }
            md.Vertices = tempList.ToList();
            md.UV[0].AddRange(uvList);
            var lineTri = new List<int>();
            var n = md.Vertices.Count / 2;

            for (int i = 0; i < n - 1; i++)
            {
                lineTri.Add(i);
                lineTri.Add(i + 1);
                lineTri.Add(2 * n - 1 - i);

                lineTri.Add(i + 1);
                lineTri.Add(2 * n - i - 2);
                lineTri.Add(2 * n - i - 1);
            }
            md.Triangles.Add(lineTri);
        }

        private Vector3 GetNormal(Vector3 p1, Vector3 newPos, Vector3 p2)
        {
            if (newPos == p1 || newPos == p2)
            {
                var n = (p2 - p1).normalized;
                return new Vector3(-n.z, 0, n.x);
            }

            var b = (p2 - newPos).normalized + newPos;
            var a = (p1 - newPos).normalized + newPos;
            var t = (b - a).normalized;

            if (t == Vector3.zero)
            {
                var n = (p2 - p1).normalized;
                return new Vector3(-n.z, 0, n.x);
            }

            return new Vector3(-t.z, 0, t.x);
        }
    }
}
