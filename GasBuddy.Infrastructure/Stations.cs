using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelperFunctions;
using GasBuddy.Infrastructure.Base;

namespace GasBuddy.Infrastructure
{
    public static class Stations
    {
        public static bool AddStations(List<Model.ZipcodeStations> stations)
        {
            if (!stations.IsNullOrEmpty())
            {
                using (var db = new Db())
                {
                    db.ZipcodeStations.AddRange(stations);
                    db.SaveChanges();
                }
                return true;
            }

            return false;
        }

        public static bool AddStations(Model.ZipcodeStations station)
        {
            if (station != null)
            {
                return AddStations(new List<Model.ZipcodeStations> { station });
            }
            return false;
        }

        public static List<Model.ZipcodeStations> GetStations(int zipcode)
        {
            if (zipcode > 0)
            {
                List<Model.ZipcodeStations> stations;
                using (Db db = new Db())
                {
                    stations = db.ZipcodeStations.Where(z => z.ZipCode == zipcode).ToList();
                    return stations;
                }
            }
            return null;
        }

        public static List<int> GetZipcodes()
        {
            using (var db = new Db())
            {
                List<int> zipcodes = db.ZipcodeStations.Select(o => o.ZipCode).Distinct().ToList();
                return zipcodes;
            }

            return null;
        }

        public static bool UpdateStations(int zipcode, List<string> stationsURL)
        {
            if (zipcode != 0 && !stationsURL.IsNullOrEmpty())
                using (Db db = new Db())
                {
                    var result = db.ZipcodeStations.Where(o => o.ZipCode == zipcode).ToList();
                    //result.ForEach(r => { r.ZipCode = zipcode, stationsURL});
                    db.ZipcodeStations.AddRange(result);

                    result.ForEach(act => db.Entry(act).State = System.Data.Entity.EntityState.Modified);
                    db.SaveChanges();
                }

            return false;
        }
    }
}
