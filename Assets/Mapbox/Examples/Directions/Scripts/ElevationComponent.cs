
namespace Mapbox.Examples.Directions {
	using UnityEngine;
	using Mapbox.Map;
	using Mapbox.Scripts.Utilities;
	using System.Collections.Generic;

	[RequireComponent(typeof(MeshFilter))]
	public class ElevationComponent : AbstractTileComponent
	{
		RawPngRasterTile _elevation;

		MeshFilter _meshFilter;

		Vector3[] _vertices;

		void Awake()
		{
			_elevation = new RawPngRasterTile();
			_meshFilter = GetComponent<MeshFilter>();
			_vertices = _meshFilter.mesh.vertices;
		}

		protected override void OnInitialized(Tile.Parameters parameters)
		{
			_elevation.Initialize(parameters, HandleTileLoaded);
		}

		void HandleTileLoaded()
		{
			if (_elevation.CurrentState == Tile.State.Loaded && _elevation.Error == null)
			{
				ExtrudeHeight();
			}
		}

		void ExtrudeHeight()
		{
			// FIXME: what does this number represent?
			var relativeScale = TileScaleInMeters / 256 * MapboxConvenience.Instance.TileScale / transform.localScale.x;

			var elevationTexture = new Texture2D(0, 0);
			elevationTexture.LoadImage(_elevation.Data);

			// HACK: UV's are "flipped" on Unity plane. Undo that flip.
			var flip = Quaternion.Euler(0, 180, 0);

			var pixelsPerMeter = 256f / MapboxConvenience.Instance.TileScale;
			var newVertices = new List<Vector3>();

			// TODO: optimize with GetPixels()
			foreach (var vert in _vertices)
			{
				var localVert = flip * vert;

				// Account for local scale.
				localVert *= transform.localScale.x;
				var x = (int)Mathf.Clamp((localVert.x * pixelsPerMeter + 128), 0, 255);
				var y = (int)Mathf.Clamp((localVert.z * pixelsPerMeter + 128), 0, 255);
				var color = elevationTexture.GetPixel(x, y);
				var newVert = vert;

				newVert.y = MapboxConvenience.GetRelativeHeightFromColor(color, relativeScale);
				newVertices.Add(newVert);
			}

			_meshFilter.mesh.SetVertices(newVertices);

			var collider = GetComponent<MeshCollider>();
			if (collider)
			{
				collider.sharedMesh = _meshFilter.mesh;
			}

			// This fixes rendering bugs if vertices are too far offset from their original locations.
			_meshFilter.mesh.RecalculateBounds();
		}
	}
}