using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using AccountingSuite.Models.Master;
using Microsoft.Data.SqlClient;

namespace AccountingSuite.Data
{
    public class StateRepository
    {
        private readonly DbHelper _db;
        public StateRepository(DbHelper db) => _db = db;

        public IEnumerable<State> GetByRegion(int regionId)
        {
            using var conn = _db.GetConnection();
            using var cmd = _db.CreateCommand(conn, "spState_GetByRegion");
            cmd.Parameters.AddWithValue("@RegionId", regionId);

            using var reader = cmd.ExecuteReader();
            var list = new List<State>();
            while (reader.Read())
            {
                list.Add(new State
                {
                    StateId = Convert.ToInt32(reader["StateId"]),
                    StateName = reader["StateName"].ToString()!,
                    RegionId = Convert.ToInt32(reader["RegionId"])
                });
            }
            return list;
        }

        public IEnumerable<State> GetAll()
        {
            using var conn = _db.GetConnection();
            using var cmd = _db.CreateCommand(conn, "spState_GetAll"); // ✅ new stored procedure

            using var reader = cmd.ExecuteReader();
            var list = new List<State>();
            while (reader.Read())
            {
                list.Add(new State
                {
                    StateId = Convert.ToInt32(reader["StateId"]),
                    StateName = reader["StateName"].ToString()!,
                    RegionId = Convert.ToInt32(reader["RegionId"])
                });
            }
            return list;
        }


        // public int Insert(State state)
        // {
        //     using var conn = _db.GetConnection();
        //     using var cmd = _db.CreateCommand(conn, "spState_Insert");
        //     cmd.Parameters.AddWithValue("@StateName", state.StateName);
        //     cmd.Parameters.AddWithValue("@RegionId", state.RegionId);

        //     var result = cmd.ExecuteScalar();
        //     return Convert.ToInt32(result); // new StateId
        // }
        public int Insert(State state)
        {
            using var conn = _db.GetConnection();
            using var cmd = _db.CreateCommand(conn, "spState_Insert");
            cmd.Parameters.AddWithValue("@StateName", state.StateName);
            cmd.Parameters.AddWithValue("@RegionId", state.RegionId);

            var result = cmd.ExecuteScalar();

            if (result == null || result == DBNull.Value)
            {
                throw new Exception("Insert failed: spState_Insert did not return a new StateId.");
            }

            return Convert.ToInt32(result); // ✅ new StateId returned from SQL
        }



        public State? GetById(int id)
        {
            using var conn = _db.GetConnection();
            using var cmd = _db.CreateCommand(conn, "spState_GetById");
            cmd.Parameters.AddWithValue("@StateId", id);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new State
                {
                    StateId = Convert.ToInt32(reader["StateId"]),
                    StateName = reader["StateName"].ToString()!,
                    RegionId = Convert.ToInt32(reader["RegionId"])
                };
            }
            return null;
        }


        public bool Exists(string stateName, int regionId)
        {
            return GetByRegion(regionId)
                   .Any(s => string.Equals(s.StateName, stateName, StringComparison.OrdinalIgnoreCase));
        }




    }
}
