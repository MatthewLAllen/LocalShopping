using Dapper;
using LocalShoppingCommon.DataAccess.Models;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;

namespace LocalShoppingCommon.DataAccess
{
    public static class DataAccessor
    {
        public static void InitializeDatabase(string dbPath)
        {            
            if(!File.Exists(dbPath))
            {
                SQLiteConnection.CreateFile(dbPath);
            }
    
            using (SQLiteConnection db =
               new SQLiteConnection($"Data Source={dbPath}"))
            {
                db.Open();

                string tableCommand = "CREATE TABLE IF NOT " +
                    "EXISTS HOUSE_LAT_LON (LAT_LON_ID INTEGER PRIMARY KEY, " +
                    "LATITUDE NUMERIC, LONGITUDE NUMERIC)";

                SQLiteCommand com = new SQLiteCommand(db);
                com.CommandText = tableCommand;
                com.ExecuteNonQuery();
                                
                tableCommand = "CREATE TABLE IF NOT " +
                    "EXISTS HOUSES (HOUSE_ID INTEGER PRIMARY KEY, " +
                    "ADDRESS TEXT, GOOGLE_PLACE_ID TEXT, LAT_LON_ID INTEGER NOT NULL, FOREIGN KEY(LAT_LON_ID) REFERENCES HOUSE_LAT_LON(LAT_LON_ID))";
                com.CommandText = tableCommand;
                com.ExecuteNonQuery();


                tableCommand = "CREATE TABLE IF NOT " +
                    "EXISTS PLACES (PLACE_ID INTEGER PRIMARY KEY, " +
                    "LAT_LON_ID INTEGER NOT NULL, GOOGLE_PLACE_ID TEXT, " +
                    "NAME TEXT, LATITUDE NUMERIC, LONGITUDE NUMERIC, TYPES TEXT, FOREIGN KEY(LAT_LON_ID) REFERENCES HOUSE_LAT_LON(LAT_LON_ID))";
                com.CommandText = tableCommand;
                com.ExecuteNonQuery();


                db.Close();
            }
        }

        public static void AddHouseData(string dbPath, House inputHouse)
        {
            using (SQLiteConnection db =
              new SQLiteConnection($"Data Source={dbPath}"))
            {
                db.Open();

                inputHouse.HouseId = db.Query<long>(
                    @"INSERT INTO HOUSES (ADDRESS, GOOGLE_PLACE_ID, LAT_LON_ID) VALUES " +
                    "(@Address, @GPlaceId, @LatLonId); " +
                    "SELECT LAST_INSERT_ROWID()", inputHouse).First();

                db.Close();
            }

        }

        public static void AddPlaceData(string dbPath, Place inputPlace)
        {
            using (SQLiteConnection db =
              new SQLiteConnection($"Data Source={dbPath}"))
            {
                db.Open();

                inputPlace.PlaceId = db.Query<long>(
                    @"INSERT INTO PLACES (LAT_LON_ID, GOOGLE_PLACE_ID, NAME, LATITUDE, LONGITUDE, TYPES) VALUES " +
                    @"(@HouseLatLonId, @GPlaceID, @Name, @Latitude, @Longitude, @Types); " +
                    "SELECT LAST_INSERT_ROWID()", inputPlace).First();

                db.Close();
            }

        }

        public static void AddLatLonData(string dbPath, HouseLatLon inputLatLon)
        {
            using (SQLiteConnection db =
              new SQLiteConnection($"Data Source={dbPath}"))
            {
                db.Open();

                inputLatLon.LatLonId = db.Query<long>(
                    @"INSERT INTO HOUSE_LAT_LON (LATITUDE, LONGITUDE) VALUES " +
                    @"( @Latitude, @Longitude); " +
                    "SELECT LAST_INSERT_ROWID()", inputLatLon).First();

                db.Close();
            }

        }

        public static House GetHouseById(string dbPath, long HouseId)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@HOUSEID", HouseId);

            House retVal = null;
            using (SQLiteConnection db =
              new SQLiteConnection($"Data Source={dbPath}"))
            {
                db.Open();

                retVal = db.Query<House>(
                    @"SELECT HOUSE_ID AS HouseId, ADDRESS AS Address, GOOGLE_PLACE_ID AS GPlaceId, LAT_LON_ID AS LatLonId " +
                    "FROM HOUSES WHERE HouseId = @HOUSEID", p).FirstOrDefault();

                db.Close();
            }

            return retVal;
        }

        public static Place GetPlaceById(string dbPath, long PlaceId)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@PLACEID", PlaceId);

            Place retVal = null;
            using (SQLiteConnection db =
              new SQLiteConnection($"Data Source={dbPath}"))
            {
                db.Open();

                retVal = db.Query<Place>(
                    @"SELECT PLACE_ID AS PlaceId, LAT_LON_ID AS HouseLatLonId, GOOGLE_PLACE_ID AS GPlaceId, " +
                    "NAME AS Name, LATITUDE AS Latitude, LONGITUDE AS Longitude, TYPES as Types " +
                    "FROM PLACES WHERE PlaceId = @PLACEID", p).FirstOrDefault();

                db.Close();
            }

            return retVal;
        }

        public static HouseLatLon GetLatLonById(string dbPath, long LatLonId)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@LATLONID", LatLonId);

            HouseLatLon retVal = null;
            using (SQLiteConnection db =
              new SQLiteConnection($"Data Source={dbPath}"))
            {
                db.Open();

                retVal = db.Query<HouseLatLon>(
                    @"SELECT LAT_LON_ID AS LatLonId, LATITUDE AS Latitude, LONGITUDE AS Longitude " +
                    "FROM HOUSE_LAT_LON WHERE LatLonId = @LATLONID", p).FirstOrDefault();

                db.Close();
            }

            return retVal;
        }

        public static House GetHouseByAddress(string dbPath, string Address)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@ADDRESS", Address);

            House retVal = null;
            using (SQLiteConnection db =
              new SQLiteConnection($"Data Source={dbPath}"))
            {
                db.Open();

                retVal = db.Query<House>(
                    @"SELECT HOUSE_ID AS HouseId, ADDRESS AS Address, GOOGLE_PLACE_ID AS GPlaceId, LAT_LON_ID AS LatLonId " +
                    "FROM HOUSES WHERE Address = @ADDRESS", p).FirstOrDefault();

                db.Close();
            }

            return retVal;
        }

        public static List<Place> GetPlacesByLatLonId(string dbPath, long LatLonId)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@LATLONID", LatLonId);

            List<Place> retVal = null;
            using (SQLiteConnection db =
              new SQLiteConnection($"Data Source={dbPath}"))
            {
                db.Open();

                retVal = db.Query<Place>(
                    @"SELECT PLACE_ID AS PlaceId, LAT_LON_ID AS HouseLatLonId, GOOGLE_PLACE_ID AS GPlaceId, " +
                    "NAME AS Name, LATITUDE AS Latitude, LONGITUDE AS Longitude, TYPES as Types " +
                    "FROM PLACES WHERE HouseLatLonID = @LATLONID", p).ToList();

                db.Close();
            }

            return retVal;
        }

        public static HouseLatLon GetLatLonByLatLon(string dbPath, float Latitude, float Longitude)
        {
            DynamicParameters p = new DynamicParameters();
            p.Add("@LATITUDE", Latitude);
            p.Add("@LONGITUDE", Longitude);

            HouseLatLon retVal = null;
            using (SQLiteConnection db =
              new SQLiteConnection($"Data Source={dbPath}"))
            {
                db.Open();

                retVal = db.Query<HouseLatLon>(
                    @"SELECT LAT_LON_ID AS LatLonId, LATITUDE AS Latitude, LONGITUDE AS Longitude " +
                    "FROM HOUSE_LAT_LON WHERE Latitude = @LATITUDE AND Longitude = @LONGITUDE", p).FirstOrDefault();

                db.Close();
            }

            return retVal;
        }

    }
}
