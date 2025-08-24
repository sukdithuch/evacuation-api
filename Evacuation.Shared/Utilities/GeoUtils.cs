
namespace Evacuation.Shared.Utilities
{
    public static class GeoUtils
    {
        public static double HaversineDistanceKm(
            double lat1, double lng1,
            double lat2, double lng2
            )
        {
            double distanceLat = (Math.PI / 180) * (lat2 - lat1);
            double distanceLng = (Math.PI / 180) * (lng2 - lng1);

            lat1 = (Math.PI / 180) * lat1;
            lat2 = (Math.PI / 180) * lat2;

            double a = Math.Pow(Math.Sin(distanceLat / 2), 2) +
                       Math.Pow(Math.Sin(distanceLng / 2), 2) *
                       Math.Cos(lat1) * Math.Cos(lat2);

            double r = 6371; // radius of Earth in meters
            double c = 2 * Math.Asin(Math.Sqrt(a));
            return r * c;
        }

        public static double EstimatedTravelTimeMinutes(double distanceKm, double speedKmPerHour)
        {
            if (speedKmPerHour <= 0)
                throw new ArgumentException("Speed must be greater than 0");

            return (distanceKm / speedKmPerHour) * 60;
        }
    }
}
