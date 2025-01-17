﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Splashify.Models;


namespace Splashify.Controllers
{ 
    public class UserController : Controller
    {

        List<UserModel> user = new List<UserModel>();


        public UserController()
        {

        }
    

        [HttpPost]
        public ActionResult Register(RegisterModel user)
        {


            UserModel newUser = new UserModel();
            newUser.fname = user.FnameField;
            newUser.lname = user.LnameField;
            newUser.email = user.EmailField;
            newUser.password = user.PasswordField;
            newUser.birthdate = user.BirthField;
            newUser.gender = user.GenderField;
            newUser.role = user.RoleField;


            UserModel replica =SqliteDataAccess.UserExist(newUser);
            if (replica == null)
            {
                Console.WriteLine("replica: " + replica);
                if (ModelState.IsValid)
                {
                    //Creates encryption object and adds hash + salt to users password and salt
                    passwordEncryptionController newPW = new passwordEncryptionController();
                    newPW.generateSalt();
                    var hash = newPW.generateHash(newUser.password, true);
                    var salt = newPW.getSalt();

                    newUser.password = hash;
                    newUser.salt = salt;
                    Console.WriteLine("hash: " + hash);
                    Console.WriteLine("salt: " + salt);


                    // Saves User in user table
                    SqliteDataAccess.SavePerson(newUser);

                    // inserts role application 
                    SqliteDataAccess.RoleApplication(newUser);
                    //first time login sets session
                    HttpContext.Session.SetString("UserSession", "default");
                    HttpContext.Session.SetString("UserName", newUser.fname);


                    return RedirectToAction("Dashboard", "Home");
                }
                else
                {
                    return RedirectToAction("Register", "Home");
                }
            }
            else if(replica.fname == "birthdate duplicate")
            {
                Console.WriteLine("replica: " + replica.fname);
                return RedirectToAction("Register", "Home");

            }
            else if (replica.fname == "email duplicate")
            {
                Console.WriteLine("replica: " + replica.fname);
                return RedirectToAction("Register", "Home");

            }
            else 
            {
                Console.WriteLine("replica: " + replica.fname);
                return RedirectToAction("Register", "Home");

            }

        }

     

            public ActionResult GetUser()
        {
            Console.WriteLine("SetPerson triggered!");

            StringBuilder userListHtml = new StringBuilder("<table id=\"pplTbl\"><tr><th>First Name</th><th>Last Name</th></tr>");

            user = SqliteDataAccess.LoadPeople();

            foreach (var person in user)
            {
                userListHtml.Append("<tr><td>");
                userListHtml.Append(person.fname);
                userListHtml.Append("</td><td>");
                userListHtml.Append(person.lname);
                userListHtml.Append("</td></tr>");
            }

            userListHtml.Append("</table>");

            Console.WriteLine(userListHtml);

            ViewBag.ppl = userListHtml;

            return RedirectToAction("Scoring", "Home");
        }




        public ActionResult Authorize(LoginModel loginUser)
        {
            UserModel u = new UserModel();
            u.email = (string)loginUser.email;
            u.password = (string)loginUser.password;
            UserModel user = SqliteDataAccess.AuthorizeUser(u);


 


            if (ModelState.IsValid && user != null)
            {

                //creates hash of inserted password with the salt that belongs to the username
                passwordEncryptionController hashPW = new passwordEncryptionController();
                hashPW.setSalt(user.salt);
                var hash = hashPW.generateHash(u.password, true);

                Console.WriteLine("salt: " + user.salt);
                Console.WriteLine("hash: " + hash);
                Console.WriteLine("password: " + user.password);

                if (user.password == hash)
                {
                    Console.WriteLine("Authenticated!");
                    HttpContext.Session.SetString("UserSession", user.role);
                    HttpContext.Session.SetString("UserName", user.fname);
                    HttpContext.Session.SetInt32("UserClub", user.club);
                    HttpContext.Session.SetInt32("UserID", user.userID);

                    return RedirectToAction("Dashboard", "Home");

                }
                else
                {
                    Console.WriteLine("Access Denied!");
                    return RedirectToAction("Login", "Home");
                }
            }
            else
            {
                Console.WriteLine("Access Denied!");
                return RedirectToAction("Login", "Home");
            }

        }

        public ActionResult Logout()
        {

            HttpContext.Session.Remove("UserSession");
            HttpContext.Session.Remove("UserName");
            return RedirectToAction("Dashboard", "Home");
        }

        //returns clubinfo to competitor in view application
        public ActionResult CheckClub()
        {

            string query = "select c.clubID, c.clubname from club as c join user as u on c.clubID = u.club where u.userID = @userID";
            string clubinfo;


            ClubModel club = new ClubModel();
            club.userID = (int)HttpContext.Session.GetInt32("UserID");
            club = SqliteDataAccess.SingleObject(club, query);

            if (club != null)
            {
                clubinfo = club.clubname + " ID:" + club.clubID;

            }
            else
            {
                clubinfo = "No current club";
            }
            ViewBag.ClubInfo = clubinfo;
            return View("~/Views/Home/Application.cshtml");
        }




    }
}
