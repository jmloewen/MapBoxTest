
namespace Mapbox.Examples.Tileviewer
{
	using System.Collections.Generic;
	using System.Linq;
	using Mapbox.Scripts.Utilities;
	using Mapbox.VectorTile;
	using TriangleNet;
	using TriangleNet.Geometry;
	using UnityEngine;

    public class UvModifier : MeshModifier
    {
        public override ModifierType Type { get { return ModifierType.Preprocess; } }

        public override void Run(VectorTileFeature feature, MeshData md)
        {
            var uv = new List<Vector2>();
            foreach (var c in md.Vertices)
            {
                uv.Add(new Vector2(c.x, c.z));
            }
            md.UV[0].AddRange(uv);
        }
    }
}
