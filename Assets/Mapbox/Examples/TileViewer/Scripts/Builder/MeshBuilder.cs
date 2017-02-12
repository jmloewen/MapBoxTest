namespace Mapbox.Examples.Tileviewer
{
	using System.Linq;
	using Mapbox.Map;
	using Mapbox.VectorTile;
	using Mapbox.VectorTile.ExtensionMethods;
	using Mapbox.Scripts.Utilities;
	using UnityEngine;

    public class MeshBuilder : Builder
    {
        private GameObject _container;
        [SerializeField]
        private Material _roofMaterial;
        [SerializeField]
        private Material _sideMaterial;

        public override void Create(VectorTileLayer layer, Rect rect, Texture2D height, Transform parent, Tile.Parameters parameters)
        {
            _container = new GameObject("Buildings");
            _container.transform.SetParent(parent, true);

			if (layer == null)
			{
				return;			}

			for (int i = 0; i < layer.FeatureCount(); i++)
			{
				var feature = layer.GetFeature(i);
				Build(feature, rect, height, _container, parameters);
			}
		}

        private void Build(VectorTileFeature feature, Rect rect, Texture2D height, GameObject parent, Tile.Parameters parameters)
        {
            foreach (var geometry in feature.GeometryAsWgs84((ulong) parameters.Id.Z, (ulong) parameters.Id.X, (ulong) parameters.Id.Y))
            {
                var meshData = new MeshData();
                //we'll run all visualizers on MeshData here 
                var list = geometry.Select(wgs84 => GM.LatLonToMeters(wgs84.Lat, wgs84.Lng).ToVector3xz()).ToList();
                meshData.Vertices = list.Select(vertex =>
                {
                    var cord = vertex - rect.center.ToVector3xz();
                    var rx = (vertex.x - rect.min.x) / rect.width;
                    var ry = 1 - (vertex.z - rect.min.y) / rect.height;

                    var h = height == null ? 0 : GetHeightFromColor(height.GetPixel(
                        (int)Mathf.Clamp((rx * 256), 0, 255),
                        (int)Mathf.Clamp((ry * 256), 0, 255)));
                    cord.y += h;

                    return cord;
                }).ToList();

                foreach (var mod in Modifiers)
                {
                    mod.Run(feature, meshData);
                }

                CreateGameObject(meshData, parent);
            }
        }

        private void CreateGameObject(MeshData data, GameObject main)
        {
            var go = new GameObject("entity");
            var mesh = go.AddComponent<MeshFilter>().mesh;
            
            mesh.subMeshCount = data.Triangles.Count;
            
            mesh.SetVertices(data.Vertices);
            for (int i = 0; i < data.Triangles.Count; i++)
            {
                var triangle = data.Triangles[i];
                mesh.SetTriangles(triangle, i);
            }

            ////this is really not necessary for now but whatever
			/// commented out for now because it was causing an error with vector only.
            //for (int i = 0; i < data.UV.Count; i++)
            //{
            //    var uv = data.UV[i];
            //    mesh.SetUVs(i, uv);
            //}

            mesh.RecalculateNormals();
            go.transform.SetParent(main.transform, false);

            var rend = go.AddComponent<MeshRenderer>();
            var mat = Resources.Load<Material>("Materials/BuildingMaterial");
            rend.materials = new Material[2]
            {
                mat, //roof
                mat //sides
            };
        }

        private float GetHeightFromColor(Color c)
        {
            //additional *256 to switch from 0-1 to 0-256
            return (float)(-10000 + ((c.r * 256 * 256 * 256 + c.g * 256 * 256 + c.b * 256) * 0.1));
        }
    }
}
