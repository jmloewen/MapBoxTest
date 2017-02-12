using Mapbox.Scripts.Utilities;
using Mapbox.Scripts.UI;
namespace Mapbox.Examples.Slippy 
{
	using System.Collections.Generic;
	using UnityEngine;
	using Mapbox;
	using Mapbox.Map;
	using Mapbox.Unity;

	public class Slippy : MonoBehaviour, IObserver<RasterTile>, IObserver<RawPngRasterTile>
	{
		[SerializeField]
		ForwardGeocodeUserInput _geocodeInput;

		[SerializeField]
		string _styleUrl;

		[HideInInspector]
		public double South = 0;
		[HideInInspector]
	    public double West = 0;
		[HideInInspector]
	    public double North = 0;
		[HideInInspector]
	    public double East = 0;
		[HideInInspector]
	    public int Zoom = 0;

		[HideInInspector]
	    public float Edge = 1;

	    private FileSource fs;

		private Map<RasterTile> raster;
	    private Map<RawPngRasterTile> rawpng;

	    private Dictionary<string, SlippyTile> tiles = new Dictionary<string, SlippyTile>();

	    private Vector3 lastPosition;
	    private Vector2 dragOffset = new Vector2(0, 0);

	    private CanonicalTileId nwAnchor;
	    private CanonicalTileId seAnchor;

		void Start()
		{
			fs = MapboxConvenience.Instance.FileSource;
			_geocodeInput.OnGeocoderResponse += GeocodeInput_OnGeocoderResponse;
		}

		void OnDestroy()
		{
			if (_geocodeInput != null)
			{
				_geocodeInput.OnGeocoderResponse -= GeocodeInput_OnGeocoderResponse;
			}
		}

	    void OnMouseDown()
	    {
	        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

	        var result = new RaycastHit();
	        GetComponent<BoxCollider>().Raycast(ray, out result, Mathf.Infinity);

	        lastPosition = transform.InverseTransformPoint(result.point);
	    }

	    void OnMouseDrag()
	    {
	        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

	        var result = new RaycastHit();
	        GetComponent<BoxCollider>().Raycast(ray, out result, Mathf.Infinity);

	        var offset = lastPosition - transform.InverseTransformPoint(result.point);
	        lastPosition = transform.InverseTransformPoint(result.point);

	        dragOffset.x = dragOffset.x + offset.x;
	        dragOffset.y = dragOffset.y + offset.y;


	        if (Mathf.Abs(dragOffset.x) > Edge)
	        {
	            var d = dragOffset.x > 0 ? 1 : -1;

	            West = new CanonicalTileId(Zoom, nwAnchor.X + d, nwAnchor.Y).ToGeoCoordinate().Longitude;
	            East = new CanonicalTileId(Zoom, seAnchor.X + d, seAnchor.Y).ToGeoCoordinate().Longitude;

	            dragOffset.x = dragOffset.x % Edge;
	        }

	        if (Mathf.Abs(dragOffset.y) > Edge)
	        {
	            var d = dragOffset.y < 0 ? 1 : -1;

	            North = new CanonicalTileId(Zoom, nwAnchor.X, nwAnchor.Y + d).ToGeoCoordinate().Latitude;
	            South = new CanonicalTileId(Zoom, seAnchor.X, seAnchor.Y + d).ToGeoCoordinate().Latitude;

	            dragOffset.y = dragOffset.y % Edge;
	        }
	    }

	    void Update()
	    {
	        UpdateMap();
	        UpdateTiles();
	    }

	    void UpdateMap()
	    {
	        if (raster == null)
	        {
				raster = new Map<RasterTile>(fs);
				raster.MapId = string.IsNullOrEmpty(_styleUrl) ? "mapbox://styles/mapbox/satellite-v9" : _styleUrl;
				//to use a custom map style:
				//raster.Source = "mapbox://styles/mapbox/outdoors-v10";
	            raster.Subscribe(this);

	            rawpng = new Map<RawPngRasterTile>(fs);
	            rawpng.Subscribe(this);
	        }

	        raster.SetGeoCoordinateBoundsZoom(new GeoCoordinateBounds(
	            new GeoCoordinate(South, West),
	            new GeoCoordinate(North, East)),
	            Zoom);

	        nwAnchor = TileCover.CoordinateToTileId(
	            new GeoCoordinate(raster.GeoCoordinateBounds.North, raster.GeoCoordinateBounds.West),
	            Zoom).Canonical;

	        seAnchor = TileCover.CoordinateToTileId(
	            new GeoCoordinate(raster.GeoCoordinateBounds.South, raster.GeoCoordinateBounds.East),
	            Zoom).Canonical;

	        rawpng.SetGeoCoordinateBoundsZoom(raster.GeoCoordinateBounds, raster.Zoom);
	    }

	    void UpdateTiles()
	    {
	        Vector2 tileOffset = new Vector2(
	            ((seAnchor.X - nwAnchor.X) * Edge) / 2 + dragOffset.x,
	            ((seAnchor.Y - nwAnchor.Y) * Edge) / 2 - dragOffset.y);

	        foreach (KeyValuePair<string, SlippyTile> entry in tiles)
	        {
	            entry.Value.SetEdge(Edge);
	            entry.Value.SetAnchor(nwAnchor, tileOffset);
	        }

	        var boxCollider = GetComponent<BoxCollider>();

	        boxCollider.size = new Vector3(
	            (seAnchor.X - nwAnchor.X + 3) * Edge,
	            (seAnchor.Y - nwAnchor.Y + 3) * Edge,
	            0.1f);
	    }

	    public void OnNext(RasterTile tile)
	    {
	        var id = tile.Id.ToString();
	        var contains = tiles.ContainsKey(id);

	        switch (tile.CurrentState)
	        {
	            case Tile.State.Loading:
	                if (!contains)
	                {
	                    var slippy = ScriptableObject.CreateInstance<SlippyTile>();

	                    slippy.SetTileId(tile.Id);
	                    slippy.SetParent(transform);

	                    tiles.Add(id, slippy);
	                }
	                break;
	            case Tile.State.Loaded:
	                if (contains && tile.Error == null)
	                {
	                    tiles[id].SetRaster(tile.Data);
	                }
	                break;
	            case Tile.State.Canceled:
	                if (contains)
	                {
	                    tiles[id].Clear();
	                    tiles.Remove(id);
	                }
	                break;
	        }
	    }

	    public void OnNext(RawPngRasterTile tile)
	    {
	        var id = tile.Id.ToString();
	        var contains = tiles.ContainsKey(id);

	        if (tile.CurrentState == Tile.State.Loaded && tile.Error == null && contains)
	        {
	            tiles[id].SetElevation(tile.Data);
	        }
	    }

	    public void SetCenter(GeoCoordinate center)
	    {
	        var bounds = raster.GeoCoordinateBounds;
	        bounds.Center = center;

	        North = bounds.North;
	        South = bounds.South;
	        East = bounds.East;
	        West = bounds.West;
	    }

		void GeocodeInput_OnGeocoderResponse(object sender, System.EventArgs e)
		{
			SetCenter(_geocodeInput.Coordinate);
		}
	}
}