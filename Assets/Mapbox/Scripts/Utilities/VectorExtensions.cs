//-----------------------------------------------------------------------
// <copyright file="VectorExtensions.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Mapbox.Scripts.Utilities
{
	using UnityEngine;

    public static class VectorExtensions
    {
        public static Vector3 ToVector3xz(this Vector2 v)
        {
            return new Vector3(v.x, 0, v.y);
        }
    }
}
