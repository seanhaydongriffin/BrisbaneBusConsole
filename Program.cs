using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using ProtoBuf;
using TransitRealtime;
using System.IO;
using CsvHelper;
using System.Data.SQLite;
using System.Collections;

namespace BrisbaneBusConsole
{
    class Program
    {
        static void Main(string[] args)
        {

            // using System.Net;
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            // Use SecurityProtocolType.Ssl3 if needed for compatibility reasons


//            string connStr = @"URI=file:R:\BUS\BUS\BrisbaneBusConsole\SEQ_GTFS.db";
            string connStr = @"URI=file:R:\BUS\BUS\BrisbaneBusConsole\fred2.db";

            using (var cnn = new SQLiteConnection(connStr))
            {
                cnn.Open();
                cnn.EnableExtensions(true);
                cnn.LoadExtension("mod_spatialite");
                SQLiteCommand cmd = cnn.CreateCommand();
                cmd.CommandText = "SELECT InitSpatialMetaData(1);";
                cmd.ExecuteScalar();



                List<string[]> route_id = SQLite.ToList("SELECT route_id FROM routes where route_short_name = '130';", cnn);



                try
                {

                    WebRequest req = HttpWebRequest.Create("https://gtfsrt.api.translink.com.au/Feed/SEQ");
                    FeedMessage feed = Serializer.Deserialize<FeedMessage>(req.GetResponse().GetResponseStream());
                    foreach (FeedEntity entity in feed.Entities)
                    {

                        if (entity.Vehicle != null)
                        {
                            Console.WriteLine("Route ID = " + entity.Vehicle.Trip.RouteId);

                            for (int route_num = 0; route_num < route_id.Count; route_num++)
                            {
                                if (entity.Vehicle.Trip.RouteId.Contains("130"))
                                {
                                    int y = 0;
                                }

                                if (entity.Vehicle.Trip.RouteId.Equals(route_id[route_num][0]))
                                {
                                    //             String tt = entity.Vehicle.Trip.RouteId;

                                    Console.WriteLine("Trip ID = " + entity.Vehicle.Trip.TripId);
                                    Console.WriteLine("Route ID = " + entity.Vehicle.Trip.RouteId);
                                    Console.WriteLine("Vehicle ID = " + entity.Vehicle.Vehicle.Id);
                                    Console.WriteLine("Vehicle Label = " + entity.Vehicle.Vehicle.Label);
                                    Console.WriteLine("Vehicle License Plate = " + entity.Vehicle.Vehicle.LicensePlate);
                                    Console.WriteLine("Vehicle Stop ID = " + entity.Vehicle.StopId);
                                    Console.WriteLine("Vehicle Timestamp = " + entity.Vehicle.Timestamp);
                                    Console.WriteLine("Vehicle Latitude = " + entity.Vehicle.Position.Latitude);
                                    Console.WriteLine("Vehicle Longitude = " + entity.Vehicle.Position.Longitude);

                                }

                            }
                        }
                    }











                            //                            cmd.CommandText = "SELECT Distance(MakePoint(0, 0), MakePoint(3, 4), true);";  //set the passed query
            //                cmd.CommandText = "SELECT Distance(GeomFromText('POINT(153.0503168 -27.6316159)',4326),GeomFromText('POINT(" + entity.Vehicle.Position.Longitude + " " + entity.Vehicle.Position.Latitude + ")',4326), 0) FROM routes;";  //set the passed query
//                            cmd.CommandText = "SELECT GeodesicLength(Distance(GeomFromText('POINT(153.0503168 -27.6316159)',4326),GeomFromText('POINT(" + entity.Vehicle.Position.Longitude + " " + entity.Vehicle.Position.Latitude + ")',4326))) FROM routes;";  //set the passed query
//                            cmd.CommandText = "SELECT GeodesicLength(GeomFromText('LINESTRING(153.0503168 -27.6316159, " + entity.Vehicle.Position.Longitude + " " + entity.Vehicle.Position.Latitude + ")',4326)) FROM routes;";  //set the passed query
//                            cmd.CommandText = "SELECT GeodesicLength(GeomFromText('LINESTRING(8.328957 49.920900, 8.339665 49.918000)',4326));";  //set the passed query
     //                       cmd.CommandText = "select Distance(GeomFromText('POINT(0 0)',4326),GeomFromText('POINT(3 4)',4326));";
//                              cmd.CommandText = "SELECT Distance(GeomFromText('POINT(153.0503168 -27.6316159)',4326),GeomFromText('POINT(" + entity.Vehicle.Position.Longitude + " " + entity.Vehicle.Position.Latitude + ")',4326)) FROM routes;";  //set the passed query
                            var result = cmd.ExecuteScalar().ToString();

                            Console.WriteLine("Result = " + result + " metres");

                            //                string sql = "SELECT ST_MINX(geometry), ST_MINY(geometry), ST_MAXX(geometry), ST_MAXY(geometry) FROM somefeature ";
                            //                            string sql = "SELECT GeomFromText('POINT(153.0503168 -27.6316159)',4326) FROM routes;";
                            //                            string sql = "SELECT Distance(GeomFromText('POINT(153.0503168 -27.6316159)',4326),GeomFromText('POINT(" + entity.Vehicle.Position.Longitude + " " + entity.Vehicle.Position.Latitude + ")',4326), 1) FROM routes;";
                            //                            string sql = "SELECT Distance(MakePoint(0, 0), MakePoint(3, 4));";
                            //                            string sql = "SELECT GeodesicLength(GeomFromText('LINESTRING(8.328957 49.920900, 8.339665 49.918000)',4326));";
                            string sql = "select distance(transform(GeomFromText('POINT(8.328957 49.920900)',4326),3035),transform(GeomFromText('POINT(8.339665 49.918000)',4326),3035));";
                            //                string sql = "SELECT * FROM routes;";

                            using (SQLiteCommand command = new SQLiteCommand(sql, cnn))
                            {
                                using (SQLiteDataReader reader = command.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        var dtype = reader.GetFieldType(0);
                                        string gg = (string)reader["route_id"];
                                        double minX = reader.GetDouble(0);
                                        double minY = reader.GetDouble(1);
                                        double maxX = reader.GetDouble(2);
                                        double maxY = reader.GetDouble(3);
                                    }
                                }
                            }




                    int i = 0;
                }
                catch (Exception e)
                {

                }










                // Close and clean up the database connection
                cnn.Close();
                cnn.Dispose();



                //using (SQLiteCommand mycommand = new SQLiteCommand("SELECT load_extension(\"mod_spatialite\")", cnn))
                //{
                //    mycommand.ExecuteNonQuery();
                //}
            }
            


//            using (StreamReader streamReader = new StreamReader(@"R:\BUS\BUS\BrisbaneBusConsole\SEQ_GTFS\routes.txt"))
//            {
//                CsvReader reader = new CsvReader(streamReader);
//                reader.Configuration.Encoding = Encoding.UTF8;
//                reader.Configuration.Delimiter = ";";
//            //    List<DataRecord> records = reader.GetRecords<DataRecord>().ToList();
////                List records = reader.GetRecords().ToList();

//                //records has to be a distinct list

//                Dictionary<string, string> dict = new Dictionary<string, string>();

//                //foreach (DataRecord record in records)
//                //{
//                //    dict.Add(record.ORIGTITLE, record.REPLACETITLE);
//                //    //i get a error because the key is not distinctq
//                //}
//            }




        }
    }

}
