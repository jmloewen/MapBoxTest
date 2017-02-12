namespace Mapbox.Examples.Directions {
	using Mapbox.Map;
	using Mapbox.Scripts.Utilities;
	using UnityEngine;

	public abstract class AbstractTileComponent : MonoBehaviour, ITile
	{
		float _tileScaleInMeters;
		public float TileScaleInMeters
		{
			get
			{
				return _tileScaleInMeters;
			}
		}

		Vector2 _center;
		public Vector2 Center
		{
			get
			{
				return _center;
			}
		}

		public void Initialize(Tile.Parameters parameters)
		{
			_center = MapboxConvenience.TileIdToLatitudeLongitude(parameters.Id.X, parameters.Id.Y, parameters.Id.Z);
			_tileScaleInMeters = MapboxConvenience.GetTileScaleInMeters(_center.x, parameters.Id.Z);
			OnInitialized(parameters);
		}

		protected abstract void OnInitialized(Tile.Parameters parameters);
	}
}
