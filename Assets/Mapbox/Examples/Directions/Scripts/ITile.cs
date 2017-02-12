using Mapbox.Map;
using UnityEngine;

namespace Mapbox.Examples.Directions
{
    public interface ITile
    {
        float TileScaleInMeters { get; }
        Vector2 Center { get; }
        void Initialize(Tile.Parameters parameters);
    }
}