namespace Mapbox.Examples.Directions {
	using UnityEngine;
	using Mapbox;
	using Mapbox.Scripts.Utilities;
	using System.Linq;
	using System.Collections.Generic;

	// TODO: add error handling.
	public static class ExtensionMethods
	{
		public static Vector3 GetPositionFromLatitudeLongitude(this Transform transform, float latitude, float longitude)
		{
			var tile = transform.GetComponent<ITile>();
			GPSEncoder.SetLocalOrigin(tile.Center);
			var position = GPSEncoder.GPSToUCS(new Vector2(latitude, longitude)) / tile.TileScaleInMeters;

			// FIXME: this should not be coupled to Mapbox convenience!
			position.x = -position.x * MapboxConvenience.Instance.TileScale;
			position.z = -position.z * MapboxConvenience.Instance.TileScale;

			return position;
		}

		public static Vector2 GetLatitudeLongitudeFromPosition(this Transform transform, Vector3 point)
		{
			var tile = transform.GetComponent<ITile>();
			GPSEncoder.SetLocalOrigin(tile.Center);
			var x = -point.x / MapboxConvenience.Instance.TileScale * tile.TileScaleInMeters;
			var y = -point.z / MapboxConvenience.Instance.TileScale * tile.TileScaleInMeters;
			return GPSEncoder.USCToGPS(new Vector3(x, 0, y));
		}

		public static Vector3 GetPositionFromGeoCoordinate(this Transform transform, GeoCoordinate geoCoordinate)
		{
			var tile = transform.GetComponent<ITile>();
			GPSEncoder.SetLocalOrigin(tile.Center);
			var position = GPSEncoder.GPSToUCS(new Vector2((float)geoCoordinate.Latitude, (float)geoCoordinate.Longitude)) / tile.TileScaleInMeters;
			position.x = -position.x * MapboxConvenience.Instance.TileScale;
			position.z = -position.z * MapboxConvenience.Instance.TileScale;
			return position;
		}

		public static T GetInterface<T>(this GameObject gameObject) where T : class
		{
			if (!typeof(T).IsInterface)
			{
				Debug.LogError(typeof(T) + ": is not an interface!");
				return null;
			}

			return gameObject.GetComponents<Component>().OfType<T>().FirstOrDefault();
		}

		public static List<T> GetInterfaces<T>(this GameObject gameObject) where T : class
		{
			if (!typeof(T).IsInterface)
			{
				Debug.LogError(typeof(T) + ": is not an interface!");
				return new List<T>();
			}

			return gameObject.GetComponents<Component>().OfType<T>().ToList();
		}
	}
}