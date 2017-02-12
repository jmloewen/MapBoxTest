namespace Mapbox.Examples.Directions {
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;
	using Mapbox;
	using Mapbox.Directions;
	using Mapbox.Scripts.Utilities;

	public class DirectionsComponent : MonoBehaviour
	{
		[SerializeField]
		Transform[] _waypoints;

		// FIXME: do not couple to line renderer.
		[SerializeField]
		LineRenderer _lineRenderer;

		[SerializeField]
		LayerMask _mask;

		DirectionResource _directionResource;

		float _lastDistance;

		void Update()
		{
			var distance = 0f;
			for (int i = 0; i < _waypoints.Length - 1; i++)
			{
				distance += (_waypoints[i + 1].position - _waypoints[i].position).magnitude;
			}

			var delta = System.Math.Abs(distance - _lastDistance);
			if (delta > .1f)
			{
				// TODO: cancel last request!
				Request();
				_lastDistance = distance;			}
		}

		void Request()
		{
			var waypointCoordinates = new List<GeoCoordinate>();
			foreach (var point in _waypoints)
			{
				var latLon = transform.GetLatitudeLongitudeFromPosition(point.localPosition);
				var coord = new GeoCoordinate();
				coord.Latitude = latLon.x;
				coord.Longitude = latLon.y;
				waypointCoordinates.Add(coord);
				point.GetComponentInChildren<Text>().text = string.Format("({0:0.000}, {1:0.000})", coord.Latitude, coord.Longitude);;
			}

			_directionResource = new DirectionResource(waypointCoordinates.ToArray(), RoutingProfile.Driving);
			_directionResource.Steps = true;
			MapboxConvenience.Instance.Directions.Query(_directionResource, HandleDirectionsResponse);
		}

		void HandleDirectionsResponse(DirectionsResponse response)
		{
			var positions = new List<Vector3>();
			foreach (var leg in response.Routes[0].Legs)
			{
				foreach (var step in leg.Steps)
				{
					var localOffset = transform.GetPositionFromGeoCoordinate(step.Maneuver.Location);
					localOffset.y = 1000f;

					// TODO: we can improve this by having a reference to an elevation component.
					// Elevation could "decorate" this.
					RaycastHit hit;
					if (Physics.Raycast(localOffset + transform.position, Vector3.down, out hit, Mathf.Infinity, _mask))
					{
						localOffset.y = hit.point.y;
						positions.Add(localOffset);
					}
				}
			}

#if UNITY_5_5_OR_NEWER
			_lineRenderer.numPositions = positions.Count;
#else
			_lineRenderer.SetVertexCount(positions.Count);
#endif
			_lineRenderer.SetPositions(positions.ToArray());
		}
	}
}