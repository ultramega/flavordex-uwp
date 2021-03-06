﻿/*
  The MIT License (MIT)
  Copyright © 2016 Steve Guidetti

  Permission is hereby granted, free of charge, to any person obtaining a copy
  of this software and associated documentation files (the “Software”), to deal
  in the Software without restriction, including without limitation the rights
  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
  copies of the Software, and to permit persons to whom the Software is
  furnished to do so, subject to the following conditions:

  The above copyright notice and this permission notice shall be included in
  all copies or substantial portions of the Software.

  THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
  THE SOFTWARE.
*/
using Flavordex.Models;
using Flavordex.Models.Data;
using System;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace Flavordex.Utilities
{
    /// <summary>
    /// Methods for handling location functionality.
    /// </summary>
    public class LocationUtilities
    {
        /// <summary>
        /// Gets or sets the name of the current location.
        /// </summary>
        public static string LocationName { get; private set; }

        /// <summary>
        /// Gets or sets the current location.
        /// </summary>
        public static Geoposition Location { get; private set; }

        /// <summary>
        /// Adds a new location to the database.
        /// </summary>
        /// <param name="name">The name of the location.</param>
        public static void AddLocation(string name)
        {
            LocationName = name;
            var location = new Location()
            {
                Name = name,
                Latitude = Location.Coordinate.Point.Position.Latitude,
                Longitude = Location.Coordinate.Point.Position.Longitude
            };
            DatabaseHelper.InsertLocation(location);
        }

        /// <summary>
        /// Updates the current location and sets the location name to the nearest known location.
        /// </summary>
        public static async Task UpdateLocationAsync()
        {
            if (await Geolocator.RequestAccessAsync() == GeolocationAccessStatus.Allowed)
            {
                var geolocator = new Geolocator();
                Location = await geolocator.GetGeopositionAsync();
                LocationName = await GetNearestLocationNameAsync(Location);
            }
            else
            {
                Settings.DetectLocation = false;
                Location = null;
                LocationName = null;
            }
        }

        /// <summary>
        /// Gets the name of the nearest known location to the provided Geoposition.
        /// </summary>
        /// <param name="position">The position to locate.</param>
        /// <returns>The name of the nearest known location.</returns>
        public static async Task<string> GetNearestLocationNameAsync(Geoposition position)
        {
            string name = null;

            var locationA = new BasicGeoposition()
            {
                Latitude = position.Coordinate.Point.Position.Latitude,
                Longitude = position.Coordinate.Point.Position.Longitude
            };
            var locationB = new BasicGeoposition();
            var minimum = double.MaxValue;
            foreach (var location in await DatabaseHelper.GetLocationListAsync())
            {
                locationB.Latitude = location.Latitude;
                locationB.Longitude = location.Longitude;
                var distance = GetDistanceBetween(locationA, locationB);
                if (distance < minimum)
                {
                    name = location.Name;
                    minimum = distance;
                }
            }

            return name;
        }

        /// <summary>
        /// Calculates the distance between two geographic locations in kilometers.
        /// </summary>
        /// <param name="locationA">The first location.</param>
        /// <param name="locationB">The second location.</param>
        /// <returns>The distance in kilometers.</returns>
        private static double GetDistanceBetween(BasicGeoposition locationA,
            BasicGeoposition locationB)
        {
            var distanceLatitude = DegreesToRadians(locationB.Latitude - locationA.Latitude);
            var distanceLongitude = DegreesToRadians(locationB.Longitude - locationA.Longitude);
            var a = Math.Sin(distanceLatitude / 2) * Math.Sin(distanceLatitude / 2)
                + Math.Cos(DegreesToRadians(locationA.Latitude))
                * Math.Cos(DegreesToRadians(locationB.Latitude)) * Math.Sin(distanceLongitude / 2)
                * Math.Sin(distanceLongitude / 2);
            return 2 * Math.Asin(Math.Min(1, Math.Sqrt(a))) * 6371;
        }

        /// <summary>
        /// Converts a value from degrees to radians.
        /// </summary>
        /// <param name="degrees">The value in degrees.</param>
        /// <returns>The value in radians.</returns>
        private static double DegreesToRadians(double degrees)
        {
            return (Math.PI / 180) * degrees;
        }
    }
}
