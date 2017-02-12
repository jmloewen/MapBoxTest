
namespace Mapbox.Examples.Tileviewer
{
	using System.Collections.Generic;
	using Mapbox.Scripts.Utilities;
	using Mapbox.Map;
	using Mapbox.VectorTile;
	using UnityEngine;

    public class Builder
    {
        public string Name;
        public List<MeshModifier> Modifiers;

        public string Key;

        public Builder()
        {
            //Modifiers = new List<MeshModifier>
            //{
            //    new PolygonMeshModifier(),
            //    new UvModifier(),
            //    new HeightModifier()
            //};
        }

        public void Init(List<MeshModifier> mods)
        {
            Modifiers = mods;
        }

        public virtual void Create(VectorTileLayer layer, Rect rect, Texture2D height, Transform parent, Tile.Parameters parameters)
        {

        }
    }
}
