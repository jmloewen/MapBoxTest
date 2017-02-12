namespace Mapbox.Examples.Tileviewer
{
	using Mapbox.Scripts.Utilities;
	using Mapbox.VectorTile;
	using UnityEngine;

    public enum ModifierType
    {
        Preprocess,
        Postprocess
    }

    public class MeshModifier
    {
        public virtual ModifierType Type { get { return ModifierType.Preprocess; } }

        public virtual void Run(VectorTileFeature feature, MeshData md)
        {

        }
    }
}