﻿namespace LocalShoppingCommon.Responses
{
    public class NearbyPlaceResponse
    {
        public object[] html_attributions { get; set; }
        public Result[] results { get; set; }
        public string status { get; set; }
        public string[] info_messages { get; set; }
        public string next_page_token { get; set; }


        public class Result
        {
            public Geometry geometry { get; set; }
            public string icon { get; set; }
            public string name { get; set; }
            public Opening_Hours opening_hours { get; set; }
            public Photo[] photos { get; set; }
            public string place_id { get; set; }
            public string reference { get; set; }
            public string[] types { get; set; }
            public string vicinity { get; set; }
        }

        public class Geometry
        {
            public Location location { get; set; }
        }

        public class Location
        {
            public float lat { get; set; }
            public float lng { get; set; }
        }

        public class Opening_Hours
        {
            public bool open_now { get; set; }
        }

        public class Photo
        {
            public int height { get; set; }
            public object[] html_attributions { get; set; }
            public string photo_reference { get; set; }
            public int width { get; set; }
        }
    }
}

