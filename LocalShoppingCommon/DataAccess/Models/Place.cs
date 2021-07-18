namespace LocalShoppingCommon.DataAccess.Models
{
    public class Place
    {
        public long PlaceId { get; set; }

        public long HouseLatLonId { get; set; }

        public string GPlaceID { get; set; }

        public string Name { get; set; }

        public float Latitude { get; set; }

        public float Longitude { get; set; }

        public string Types { get; set; }
    }
}
