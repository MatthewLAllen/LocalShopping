using LocalShoppingCommon.DataAccess;
using LocalShoppingCommon.DataAccess.Models;
using LocalShoppingCommon.Responses;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Hosting;
using System.Web.Http;

namespace LocalShoppingAPI.Controllers
{
    public class ShoppingController : ApiController
    {
        private const string MyKey = "<YourGoogleApiKey>";
        // GET: http://localhost/LocalShoppingAPI/api/Shopping?address=601%20N%2034th%20St,%20Seattle,%20WA%2098103
        public List<Place> Get(string address)
        {
            List<Place> retVal = null;
            House findHouse = DataAccessor.GetHouseByAddress(HostingEnvironment.MapPath("~") + @"\LocalShopping.db", address);
            if (findHouse == null)
            {                
                string requestUri = string.Format("https://maps.googleapis.com/maps/api/geocode/json?key={1}&address={0}&sensor=false", Uri.EscapeDataString(address), MyKey);

                var result = new WebClient().DownloadString(requestUri);
                
                GeoCodeResponse geo = JsonConvert.DeserializeObject<GeoCodeResponse>(result);

                HouseLatLon findLatLon = DataAccessor.GetLatLonByLatLon(HostingEnvironment.MapPath("~") + @"\LocalShopping.db", geo.results[0].geometry.location.lat, geo.results[0].geometry.location.lng);

                if (findLatLon == null)
                {
                    findLatLon = new HouseLatLon() { Latitude = geo.results[0].geometry.location.lat, Longitude = geo.results[0].geometry.location.lng };
                    DataAccessor.AddLatLonData(HostingEnvironment.MapPath("~") + @"\LocalShopping.db", findLatLon);
                    findHouse = new House() { Address = address, GPlaceID = geo.results[0].place_id, LatLonId = findLatLon.LatLonId };
                    DataAccessor.AddHouseData(HostingEnvironment.MapPath("~") + @"\LocalShopping.db", findHouse);

                    retVal = new List<Place>();
                    
                    GetNearbyPlaces(retVal, findLatLon.Latitude, findLatLon.Longitude, "restaurant", findLatLon.LatLonId, MyKey);

                    GetNearbyPlaces(retVal, findLatLon.Latitude, findLatLon.Longitude, "store", findLatLon.LatLonId, MyKey);


                }
                else
                {
                    findHouse = new House() { Address = address, GPlaceID = geo.results[0].place_id, LatLonId = findLatLon.LatLonId };
                    DataAccessor.AddHouseData(HostingEnvironment.MapPath("~") + @"\LocalShopping.db", findHouse);

                    retVal = DataAccessor.GetPlacesByLatLonId(HostingEnvironment.MapPath("~") + @"\LocalShopping.db", findHouse.LatLonId);
                }
            }
            else
            {
                retVal = DataAccessor.GetPlacesByLatLonId(HostingEnvironment.MapPath("~") + @"\LocalShopping.db", findHouse.LatLonId);
            }

            return retVal;
        }

        private void GetNearbyPlaces(List<Place> retVal, float lat, float lng, string type, long houseLatLngId, string key )
        {            
            string requestUri = string.Format("https://maps.googleapis.com/maps/api/place/nearbysearch/json?location={0},{1}&type={2}&radius=3219&key={3}", lat, lng, type, MyKey);
            var resultNearby = new WebClient().DownloadString(requestUri);
                        
            NearbyPlaceResponse npResponse = JsonConvert.DeserializeObject<NearbyPlaceResponse>(resultNearby);

            List<NearbyPlaceResponse.Result> placeResults = new List<NearbyPlaceResponse.Result>();
            placeResults.AddRange(npResponse.results);

            while(npResponse.next_page_token != null)
            {
                requestUri = string.Format("https://maps.googleapis.com/maps/api/place/nearbysearch/json?pagetoken={0}&key={1}", npResponse.next_page_token, MyKey);
                resultNearby = new WebClient().DownloadString(requestUri);
                npResponse = JsonConvert.DeserializeObject<NearbyPlaceResponse>(resultNearby);
                placeResults.AddRange(npResponse.results);
            }
                        
            foreach (NearbyPlaceResponse.Result res in placeResults)
            {
                Place storePlace = new Place();
                storePlace.GPlaceID = res.place_id;
                storePlace.HouseLatLonId = houseLatLngId;
                storePlace.Latitude = res.geometry.location.lat;
                storePlace.Longitude = res.geometry.location.lng;
                storePlace.Name = res.name;
                storePlace.Types = string.Join(",", res.types);

                DataAccessor.AddPlaceData(HostingEnvironment.MapPath("~") + @"\LocalShopping.db", storePlace);

                retVal.Add(storePlace);
            }
        }
    }
}
