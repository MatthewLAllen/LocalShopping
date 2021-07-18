namespace LocalShoppingCommon.DataAccess.Models
{ 
    public class House
    {
        public long HouseId { get; set; }

        public string Address { get; set; }

        public long LatLonId { get; set; }

        public string GPlaceID { get; set; }
    }
}
