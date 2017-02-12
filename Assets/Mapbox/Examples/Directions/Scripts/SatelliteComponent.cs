namespace Mapbox.Examples.Directions
{
	using UnityEngine;
	using Mapbox.Map;
	[RequireComponent(typeof(Renderer))]
	public class SatelliteComponent : AbstractTileComponent
	{
		RasterTile _tile;

		Renderer _renderer;

		Material _material;

		[SerializeField]
		string _styleUrl;

		void Awake()
		{
			_tile = new RasterTile();
			_renderer = GetComponent<Renderer>();
			_material = _renderer.material;
		}

		protected override void OnInitialized(Tile.Parameters parameters)
		{
			if (!string.IsNullOrEmpty(_styleUrl))
			{
				parameters.MapId = _styleUrl;
			}
			_tile.Initialize(parameters, HandleTileLoaded);
		}

		void HandleTileLoaded()
		{
			if (_tile.CurrentState == Tile.State.Loaded && _tile.Error == null)
			{
				var satelliteTexture = new Texture2D(0, 0);
				satelliteTexture.LoadImage(_tile.Data);
				_material.mainTexture = satelliteTexture;
			}
		}
	}
}