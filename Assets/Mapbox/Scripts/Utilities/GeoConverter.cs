//-----------------------------------------------------------------------
// <copyright file="GeoConverter.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------


namespace Mapbox.Scripts.Utilities
{
	using System;
	using UnityEngine;

    //SOURCE: http://stackoverflow.com/questions/12896139/geographic-coordinates-converter
    public static class GM
    {
        private const int TileSize = 256;
        private const int EarthRadius = 6378137;
        private const double InitialResolution = 2 * Math.PI * EarthRadius / TileSize;
        private const double OriginShift = 2 * Math.PI * EarthRadius / 2;

        public static Vector2 LatLonToMeters(GeoCoordinate v)
        {
            return LatLonToMeters(v.Longitude, v.Latitude);
        }

        public static Vector2 LatLonToMeters(double Latitude, double lon)
        {
            var p = new GeoCoordinate();
            p.Longitude = (lon * OriginShift / 180);
            p.Latitude = (Math.Log(Math.Tan((90 + Latitude) * Math.PI / 360)) / (Math.PI / 180));
            p.Latitude = (p.Latitude * OriginShift / 180);
            return new Vector2((float)p.Longitude, (float)p.Latitude);
        }

        public static double Resolution(int zoom)
        {
            return InitialResolution / (Math.Pow(2, zoom));

        }
        public static Vector2 PixelsToMeters(Vector2 p, int zoom)
        {
            var res = Resolution(zoom);
            var met = new Vector2();
            met.x = (float)(p.x * res - OriginShift);
            met.y = (float)-(p.y * res - OriginShift);
            return met;
        }

        public static Vector2 MetersToTile(Vector2 m, int zoom)
        {
            var p = MetersToPixels(m, zoom);
            return PixelsToTile(p);
        }

        public static Vector2 MetersToPixels(Vector2 m, int zoom)
        {
            var res = Resolution(zoom);
            var pix = new Vector2((float) ((m.x + OriginShift) / res), (float) ((-m.y + OriginShift) / res));
            return pix;
        }

        public static Vector2 PixelsToTile(Vector2 p)
        {
            var t = new Vector2((int)Math.Ceiling(p.x / (double)TileSize) - 1, (int)Math.Ceiling(p.y / (double)TileSize) - 1);
            return t;
        }

        public static Rect TileBounds(Vector2 t, int zoom)
        {
            var min = PixelsToMeters(new Vector2(t.x * TileSize, t.y * TileSize), zoom);
            var max = PixelsToMeters(new Vector2((t.x + 1) * TileSize, (t.y + 1) * TileSize), zoom);
            return new Rect(min, max - min);
        }

        public static Rect TileBounds(float x, float y, int zoom)
        {
            var min = PixelsToMeters(new Vector2(x * TileSize, y * TileSize), zoom);
            var max = PixelsToMeters(new Vector2((x + 1) * TileSize, (y + 1) * TileSize), zoom);
            return new Rect(min, max - min);
        }
    }
}
