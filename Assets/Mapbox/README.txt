Thanks again for signing up for the Mapbox Unity SDK! 

Please note: we currently only support Unity 5.4 and above. 

If you run into any issues or have any feedback, please reach out to us at unity-beta@mapbox.com.

API documentation is available here: https://mapbox.github.io/mapbox-sdk-unity/api/index.html

All uses of Mapbox’s custom maps and data must attribute both Mapbox and the appropriate data providers. Mapbox’s
custom design is copyrighted and our data sources require attribution. This requirement extends to all plan levels.
Read more on our website: https://www.mapbox.com/help/attribution/.

For your convenience, we have included a prefab called “Attribution.” This prefab utilizes UGUI for simple
integration and customization. To adjust the display size, simply change the font size of the text component on
“CopyrightText.” You can adjust the background and text colors to best match your design aesthetics. You can freely
adjust pivots and anchors of the rect transforms. 

If you are using a Mapbox map or data from Mapbox map tiles for a small part of your Unity project and the "Attribution"
prefab becomes too intrusive, then please include '© Mapbox, © OpenStreetMap' as text in an info or help screen.

Before testing the included examples, paste your api token in: Assets/Mapbox/Prefabs/MapboxConvenience.prefab Token field.
This will ensure that all the demos work properly.

Demos:

* Slippy Demo
	The purpose of this example is to demonstrate a slippy map built with the sdk
	using satellite imagery draped over geometry generated from terrain data.
	At runtime an area that corresponds the specified lat/lon Northeast and Southwest coordinates\
	and zoom level will be created.
	The area displayed will be determined by the zoom and bounding box.
	mouse click and drag to pan the map in play mode.

* Tile Demo
	The purpose of this example is to demonstrate loading and creating game objects
	from multiple tile types.
	At runtime an area that corresponds to a single tile at the specified lat/lon
    and zoom level will be created. The object(s) displayed are determined by the type of tile.
	Raster Tile = image from mapbox/satellite-v9
	Vector Tile = 3D buildings generated from mapbox.mapbox-streets-v7
	Terrain Tile = 3D terrain generated from mapbox.terrain-rgb
	All = All of the above combined (satellite imagery and buildings draped on terrain)

* DirectionsWithUnityComponents
	This example demonstrates how to decorate our core sdk with unity components.
	MapTileComponent.cs is responsible for aggreating a set of tiles and placing them in unity 
	coordinate space to form a grid. The directions prefab has three waypoints that can be 
	moved to initiate a new directions query. The line renderer is used to display the results of the
	directions query. Conversion from earth space (geocoordinates) to unity space is based on 
	local positions. This is why the Directions instance is a child of Map.

* Playground Demo

	Playground demos consist of the following examples:

	ForwardGeocoder:
		A forward geocoding request will fetch GeoJSON from a place name query.
		See: https://www.mapbox.com/api-documentation/#geocoding for more information.

	ReverseGeocoder:
		A reverse geocoding request will fetch GeoJSON from a latitude, longitude query.
		See: https://www.mapbox.com/api-documentation/#geocoding for more information.

	VectorTile:
		Uses a forward geocoder request to fetch GeoJSON from a Map object.
		In this example, the result is GeoJSON with a feature collection.
		See: https://www.mapbox.com/api-documentation/#retrieve-features-from-vector-tiles

	RasterTile:
		Uses a forward geocoder request to fetch a style's raster tile from a Map object.
		"Request image tiles from a style that can be arranged and displayed with the help of a mapping library."
		See: https://www.mapbox.com/help/define-style/
		See: https://www.mapbox.com/api-documentation/#retrieve-raster-tiles-from-styles

	Directions:
		Enter a start location query (eg. "San Francisco, CA"), this is a forward geocode request.
		Enter a destination query (eg. "Los Angeles, CA"), this is also a forward geocode request.
		When the requests have been completed, a directions request is executed.
		Direction results will be logged to the UI when they are available (in the form of JSON).


CHANGELOG

v0.2.0
  - Added core sdk support for mapbox styles
  - vector tile decoding optimizations for speed and lazy decoding
  - Added attribution prefab
  - new Directions example
  - All examples scripts updated streamlined to use MapboxConvenience object

v0.1.1
  - removed orphaned references from link.xml, this was causing build errors
  - moved JSON utility to Mapbox namespace to avoid conflicts with pre-exisiting frameworks
