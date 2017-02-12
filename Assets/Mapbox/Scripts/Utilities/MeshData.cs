//-----------------------------------------------------------------------
// <copyright file="MeshData.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Mapbox.Scripts.Utilities
{
	using System.Collections.Generic;
	using UnityEngine;

    public class MeshData
    {
        public List<Vector3> Vertices { get; set; }
        public List<List<int>> Triangles { get; set; }
        public List<List<Vector2>> UV { get; set; }

        public MeshData()
        {
            Vertices = new List<Vector3>();
            Triangles = new List<List<int>>();
            UV = new List<List<Vector2>>();
            UV.Add(new List<Vector2>());
        }
    }
}
