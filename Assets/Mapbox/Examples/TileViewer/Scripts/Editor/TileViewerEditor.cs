namespace Mapbox.Examples.Tileviewer
{
	using UnityEditor;

	[CustomEditor(typeof(TileViewer))]
	public class TileViewerEditor : Editor
	{
		string _notes = "The purpose of this example is to demonstrate loading and creating game objects"
		+ " from multiple tile types.\n\n"
		+ "At runtime an area that corresponds to a single tile at the specified lat/lon"
		+ " and zoom level will be created. The object(s) displayed are determined by the type of tile.\n\n"
		+ "Raster Tile = image from mapbox.satellite\n\n"
		+ "Vector Tile = 3D buildings generated from mapbox.mapbox-streets-v7\n\n"
		+ "Terrain Tile = 3D terrain generated from mapbox.terrain-rgb\n\n"
		+ "All = All of the above combined (satellite imagery and buildings draped on terrain)\n\n";

		public override void OnInspectorGUI()
		{
			EditorGUILayout.HelpBox(_notes, MessageType.Info);
			DrawDefaultInspector();
		}
	}
}