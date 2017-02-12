namespace Mapbox.Examples.Tileviewer
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Mapbox.VectorTile;
	using Mapbox.Scripts.Utilities;
	using TriangleNet;
	using TriangleNet.Geometry;
	using UnityEngine;

    public class HeightModifier : MeshModifier
    {
        public float Height;
        public bool CloseEnd = true;
        public bool FlatTop = true;

        public override ModifierType Type { get { return ModifierType.Preprocess; } }

        public override void Run(VectorTileFeature feature, MeshData md)
        {
            if (md.Vertices.Count == 0)
                return;

            var prop = feature.GetProperties();
            if (prop.ContainsKey("height"))
                Height = Convert.ToSingle(feature.GetValue("height"));

            var buildingLowest = float.MaxValue;
            var buildingHighest = float.MinValue;

            foreach (var vector in md.Vertices)
            {
                if (vector.y < buildingLowest)
                    buildingLowest = vector.y;
                if (vector.y > buildingHighest)
                    buildingHighest = vector.y;
            }
            var buildingHighestOnTerrain = buildingHighest + Height;

            if (FlatTop)
            {
                for (int i = 0; i < md.Vertices.Count; i++)
                {
                    md.Vertices[i] = new Vector3(md.Vertices[i].x, buildingHighestOnTerrain, md.Vertices[i].z);
                }
            }
            else
            {
                var pushUp = new Vector3(0, Height, 0);
                for (int i = 0; i < md.Vertices.Count; i++)
                {
                    md.Vertices[i] += pushUp;
                }
            }

            var vertsStartCount = 0;
            var count = md.Vertices.Count;
            float d = 0f;
            Vector3 v1;
            Vector3 v2;
            int ind = 0;

            var wallTri = new List<int>();
            var wallUv = new List<Vector2>();

            for (int i = 1; i < count; i++)
            {
                v1 = md.Vertices[vertsStartCount + i - 1];
                v2 = md.Vertices[vertsStartCount + i];
                ind = md.Vertices.Count;
                md.Vertices.Add(v1);
                md.Vertices.Add(v2);
                md.Vertices.Add(new Vector3(v1.x, buildingLowest, v1.z));
                md.Vertices.Add(new Vector3(v2.x, buildingLowest, v2.z));

                d = (v2 - v1).magnitude;

                wallUv.Add(new Vector2(0, 0));
                wallUv.Add(new Vector2(d, 0));
                wallUv.Add(new Vector2(0, Height));
                wallUv.Add(new Vector2(d, Height));

                wallTri.Add(ind);
                wallTri.Add(ind + 2);
                wallTri.Add(ind + 1);

                wallTri.Add(ind + 1);
                wallTri.Add(ind + 2);
                wallTri.Add(ind + 3);
            }

            if (CloseEnd)
            {
                v1 = md.Vertices[vertsStartCount];
                v2 = md.Vertices[vertsStartCount + count - 1];
                ind = md.Vertices.Count;
                md.Vertices.Add(v1);
                md.Vertices.Add(v2);
                md.Vertices.Add(new Vector3(v1.x, buildingLowest, v1.z));
                md.Vertices.Add(new Vector3(v2.x, buildingLowest, v2.z));

                d = (v2 - v1).magnitude;

                wallUv.Add(new Vector2(0, 0));
                wallUv.Add(new Vector2(d, 0));
                wallUv.Add(new Vector2(0, Height));
                wallUv.Add(new Vector2(d, Height));

                wallTri.Add(ind);
                wallTri.Add(ind + 1);
                wallTri.Add(ind + 2);

                wallTri.Add(ind + 1);
                wallTri.Add(ind + 3);
                wallTri.Add(ind + 2);
            }

            md.Triangles.Add(wallTri);

            md.UV[0].AddRange(wallUv);
        }
    }
}