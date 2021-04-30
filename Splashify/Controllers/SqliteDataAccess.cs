﻿using System;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Data.SqlClient;
using Dapper;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Splashify.Models;

namespace Splashify.Controllers

{
    public class SqliteDataAccess
    {


        private static string LoadConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["Default"].ConnectionString;
        }

        //Person outdated - doesnt exist in db
        public static List<UserModel> LoadPeople()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<UserModel>("SELECT * FROM user", new DynamicParameters());
                return output.ToList();
            }
        }

        //outdated - doesnt exist in db
        public static List<ScoreModel> LoadFinalScore()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<Models.ScoreModel>("SELECT * FROM user", new DynamicParameters());
                return output.ToList();
            }


        }
        //outdated - doesnt exist in db
        public static void SavePerson(UserModel person)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("insert into Person(fname,lname) values(@fname, @lname)", person);
            }
        }

        //Event
        public static void SaveEvent(EventModel eventObj)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("insert into Event(eventID,startdate,gender) values(@name, @startdate, @gender)", eventObj);
            }
        }


        public static List<EventModel> LoadEvent()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<EventModel>("SELECT * FROM event", new DynamicParameters());
                return output.ToList();
            }
        }

        //Login
        public static UserModel AuthorizeUser(UserModel user)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {

                UserModel output = cnn.QuerySingleOrDefault<UserModel>("SELECT * FROM user WHERE email = @email", user);
                if(output == null)
                {
                    Console.WriteLine("NULL!");
                }
                else
                {
                    Console.WriteLine(output.birthdate);

                }
                return output;
            }
        }
    }
}