namespace Mapbox.Examples.Directions 
{
	using Mapbox.Scripts.UI;
	using System.Collections.Generic;
	using Mapbox.Map;
	using Mapbox.Scripts.Utilities;
	using UnityEngine;

	public class MapTileComponent : AbstractTileComponent
	{
		[SerializeField]
		ForwardGeocodeUserInput _geocodeInput;

		[SerializeField]
		GameObject _tilePrefab;

		[SerializeField]
		int _mapWidth;

		[SerializeField]
		int _mapHeight;

		[SerializeField]
		float _tileSize;

		[SerializeField]
		[Range(1, 19)]
		int _zoom;

		[SerializeField]
		float _latitude;

		[SerializeField]
		float _longitude;

		Tile.Parameters _parameters;

		UnwrappedTileId _unwrappedTileId;

		List<GameObject> _instantiatedTiles = new List<GameObject>();

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

		void Start()
		{
			MapboxConvenience.Instance.TileScale = _tileSize;
			Build();
		}

		void Build()
		{
			foreach (var tile in _instantiatedTiles)
			{
				Destroy(tile);
			}

			_instantiatedTiles.Clear();

			_unwrappedTileId = MapboxConvenience.LatitudeLongitudeToTileId(_latitude, _longitude, _zoom);

			_parameters = new Tile.Parameters
			{
				Fs = MapboxConvenience.Instance.FileSource,
				Id = new CanonicalTileId(_zoom, _unwrappedTileId.X, _unwrappedTileId.Y)
			};

			Initialize(_parameters);
		}

		protected override void OnInitialized(Tile.Parameters parameters)
		{
			for (int i = 0; i < _mapWidth; i++)
			{
				for (int j = 0; j < _mapHeight; j++)
				{
					var instance = Instantiate<GameObject>(_tilePrefab);
					_instantiatedTiles.Add(instance);
					instance.transform.SetParent(transform, false);
					instance.transform.localPosition = new Vector3(_tileSize * i, 0, _tileSize * j);

					// Unity plane UVs are "backwards."
					var tiles = instance.GetInterfaces<ITile>();
					foreach (var tile in tiles)
					{
						parameters.Id = new CanonicalTileId(_zoom, _unwrappedTileId.X - i, _unwrappedTileId.Y + j);
						tile.Initialize(parameters);
					}
				}
			}
		}

		void GeocodeInput_OnGeocoderResponse(object sender, System.EventArgs e)
		{
			_latitude = (float)_geocodeInput.Coordinate.Latitude;
			_longitude = (float)_geocodeInput.Coordinate.Longitude;
			Build();
		}
	}
}