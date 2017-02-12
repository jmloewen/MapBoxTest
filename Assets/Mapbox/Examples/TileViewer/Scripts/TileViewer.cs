namespace Mapbox.Examples.Tileviewer
{
	using System.Collections;
	using Mapbox.Scripts.UI;
	using System.Collections.Generic;
	using UnityEngine;
	using Mapbox;
	using Mapbox.Map;
	using Mapbox.Scripts.Utilities;

	public class TileViewer : MonoBehaviour
	{
		[SerializeField]
		ForwardGeocodeUserInput _geocodeInput;

		[SerializeField]
		string _styleUrl;

		[SerializeField]
		GameObject _tilePrefab;

		private IFileSource _fileSource;

		[SerializeField]
		private float Latitude = (float)37.753032;

		[SerializeField]
		private float Longitude = (float)-122.447364;

		private Vector2 tileCoordinate;

		[SerializeField]
		private int zoomLevel = 16;

		[SerializeField]
		private TileConstructionType tileType;
		//[TextArea(20, 20)]

		private GameObject tileObject;
		private Texture2D heightData;
		private int tileResolution = 7;

		private Rect tileRect;
		private float tileSize;
		private float tileHalfEdge;

		Vector3 _originalCameraPosition;

		void Awake()
		{
			_geocodeInput.OnGeocoderResponse += GeocodeInput_OnGeocoderResponse;
		}

		void OnDestroy()
		{
			if (_geocodeInput != null)
			{
				_geocodeInput.OnGeocoderResponse -= GeocodeInput_OnGeocoderResponse;
			}
		}

		void GeocodeInput_OnGeocoderResponse(object sender, System.EventArgs e)
		{
			Latitude = (float)_geocodeInput.Coordinate.Latitude;
			Longitude = (float)_geocodeInput.Coordinate.Longitude;
			Initialize();
		}

		private void Start()
		{
			_originalCameraPosition = Camera.main.transform.localPosition;
			_fileSource = MapboxConvenience.Instance.FileSource;
			Initialize();
		}

		void Initialize()
		{
			heightData = null;
			Camera.main.transform.localPosition = _originalCameraPosition;
			if (tileObject != null)
			{
				Destroy(tileObject);
			}

			// Convert lat/lon to tile coordinates
			var v2 = GM.LatLonToMeters(Latitude, Longitude);
			tileCoordinate = GM.MetersToTile(v2, zoomLevel);
			tileRect = GM.TileBounds(tileCoordinate, zoomLevel);
			tileSize = tileRect.width;
			tileHalfEdge = tileSize / 2;
			CreateRootGameObject();
			Build(tileType);
		}

		// Create root object container. Tile objects will be placed inside as children
		private void CreateRootGameObject()
		{
			tileObject = Instantiate(_tilePrefab);
			tileObject.name = "TileRoot";
			Camera.main.transform.position += tileObject.transform.position;
			var mesh = new Mesh();
			var verts = new List<Vector3>();

			for (float x = 0; x < tileResolution; x++)
			{
				for (float y = 0; y < tileResolution; y++)
				{
					var stepx = Mathf.Lerp(-tileHalfEdge, tileHalfEdge, x / (tileResolution - 1));
					var stepy = 1 - Mathf.Lerp(-tileHalfEdge, tileHalfEdge, y / (tileResolution - 1));

					verts.Add(new Vector3(stepx, 0, stepy));
				}
			}

			mesh.SetVertices(verts);

			var trilist = new List<int>();
			for (int y = 0; y < tileResolution - 1; y++)
			{
				for (int x = 0; x < tileResolution - 1; x++)
				{
					trilist.Add((y * tileResolution) + x);
					trilist.Add((y * tileResolution) + x + tileResolution);
					trilist.Add((y * tileResolution) + x + tileResolution + 1);

					trilist.Add((y * tileResolution) + x);
					trilist.Add((y * tileResolution) + x + tileResolution + 1);
					trilist.Add((y * tileResolution) + x + 1);
				}
			}
			mesh.SetTriangles(trilist, 0);

			var uvlist = new List<Vector2>();
			var step = 1f / (tileResolution - 1);
			for (int i = 0; i < tileResolution; i++)
			{
				for (int j = 0; j < tileResolution; j++)
				{
					uvlist.Add(new Vector2(i * step, 1 - (j * step)));
				}
			}
			mesh.SetUVs(0, uvlist);
			mesh.RecalculateNormals();

			var mf = tileObject.GetComponent<MeshFilter>();
			mf.sharedMesh = mesh;

			var rend = tileObject.GetComponent<MeshRenderer>();
		}

		private void Build(TileConstructionType tileType)
		{
			switch (tileType)
			{
				case TileConstructionType.TerrainTile:
					{
						BuildTerrainTile();
						break;
					}
				case TileConstructionType.RasterTile:
					{
						BuildRasterTile();
						break;
					}
				case TileConstructionType.VectorTile:
					{
						BuildVectorTile();
						break;
					}
				case TileConstructionType.All:
					{
						StartCoroutine(BuildAll());
						break;
					}
			}
		}

		private IEnumerator BuildAll()
		{
			BuildRasterTile();
			BuildTerrainTile();

			while (heightData == null)
			{
				yield return null;
			}
			BuildVectorTile();
		}

		private void SnapCamera()
		{
			var h = GetHeightFromColor(heightData.GetPixel(128, 128));
			Camera.main.transform.position += new Vector3(0, h, 0);
		}

		private void BuildVectorTile()
		{
			// Define which features from the vector tile will be made into game objects
			var builderDictionary = new Dictionary<string, MeshBuilder>()
			{
				{
					"building", new MeshBuilder()
					{
						Modifiers = new List<MeshModifier>()
						{
							new PolygonMeshModifier(),
							new UvModifier(),
							new HeightModifier()
						}
					}
				},
				// Not using these for now.
	            //{
	            //    "road", new MeshBuilder()
	            //    {
	            //        Modifiers = new List<MeshModifier>()
	            //        {
	            //            new LineMeshModifier(),
	            //            new UvModifier(),
	            //            new HeightModifier()
	            //        }
	            //    }
	            //}
	        };

			var parameters = new Tile.Parameters
			{
				Fs = _fileSource,
				Id = new CanonicalTileId(zoomLevel, (int)tileCoordinate.x, (int)tileCoordinate.y)
			};

			// Use VectorTile class for vector tiles. Requests mapbox.mapbox-streets-v7 mapid by default.
			var vectorTile = new VectorTile();
			vectorTile.Initialize(parameters, () =>
			{
				if (vectorTile.Error != null)
				{
					Debug.Log(vectorTile.Error);
					return;
				}

				foreach (var builder in builderDictionary)
				{
					var layer = vectorTile.GetLayer(builder.Key);
					builder.Value.Create(layer, tileRect, heightData, tileObject.transform, parameters);
				}
			});
		}

		private void BuildRasterTile()
		{
			var parameters = new Tile.Parameters
			{
				Fs = _fileSource,
				Id = new CanonicalTileId(zoomLevel, (int)tileCoordinate.x, (int)tileCoordinate.y),
				MapId = string.IsNullOrEmpty(_styleUrl) ? "mapbox://styles/mapbox/satellite-v9" : _styleUrl
			};

			// Use RasterTile class for raster tiles. Requests mapbox.satellite mapid by default.
			var rasterTile = new RasterTile();
			rasterTile.Initialize(parameters, () =>
			{
				if (rasterTile.Error != null)
				{
					Debug.Log(rasterTile.Error);
					return;
				}

				var satt = new Texture2D(512, 512);
				satt.LoadImage(rasterTile.Data);

				var rend = tileObject.GetComponent<MeshRenderer>();
				rend.material.mainTexture = satt;
			});

		}

		private void BuildTerrainTile()
		{
			var parameters = new Tile.Parameters
			{
				Fs = _fileSource,
				Id = new CanonicalTileId(zoomLevel, (int)tileCoordinate.x, (int)tileCoordinate.y)
			};

			// Use RawPngRasterTile class for terrain. requests mapbox.terrain-rgb mapid by default.
			var heightTile = new RawPngRasterTile();
			heightTile.Initialize(parameters, () =>
			{
				if (heightTile.Error != null)
				{
					Debug.Log(heightTile.Error);
					return;
				}

				heightData = new Texture2D(256, 256);
				heightData.LoadImage(heightTile.Data);

				//var heightOffset = GetHeightFromColor(heightData.GetPixel(0, 0));

				var verts = new List<Vector3>();
				for (float x = 0; x < tileResolution; x++)
				{
					for (float y = 0; y < tileResolution; y++)
					{
						var stepx = Mathf.Lerp(-tileHalfEdge, tileHalfEdge, x / (tileResolution - 1));
						var stepy = 1 - Mathf.Lerp(-tileHalfEdge, tileHalfEdge, y / (tileResolution - 1));
						var height = GetHeightFromColor(heightData.GetPixel((int)Mathf.Clamp((x / (tileResolution - 1) * 256), 0, 255), (int)Mathf.Clamp((256 - (y / (tileResolution - 1) * 256)), 0, 255)));
						verts.Add(new Vector3(stepx,
							height,
							stepy));
					}
				}

				//tileObject.transform.position = new Vector3(0, heightOffset, 0);

				var mf = tileObject.GetComponent<MeshFilter>();
				mf.mesh.SetVertices(verts);
				mf.mesh.RecalculateNormals();
				mf.mesh.RecalculateBounds();
				SnapCamera();
			});
		}

		// Convert pixel values in mapbox.terrain-rgb to height
		private float GetHeightFromColor(Color c)
		{
			//additional *256 to switch from 0-1 to 0-256
			return (float)(-10000 + ((c.r * 256 * 256 * 256 + c.g * 256 * 256 + c.b * 256) * 0.1));
		}
	}
}