﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;


namespace Module1Projekt
{
    class Retrieve_all_info
    {
        #region Static 
        static string userChoice; //Used in menu
        static string userConnected;
        static string userConnetedPassword;
        #endregion

        static void Main(string[] args)
        {
            CorrectPassword(); //When password is correct we get into this loop
            while (1 == 1) {
                Console.Clear();
                Console.WriteLine("Choose one");
                Console.WriteLine("");
                Console.WriteLine("[1] Find all information about user");
                Console.WriteLine("");
                Console.WriteLine("[2] Find information about all users");
                Console.WriteLine("");
                Console.WriteLine("[3] Update a user");
                Console.WriteLine("");
                Console.Write("I choose: ");
                userChoice = Console.ReadLine();
                Console.Clear();
                Menu();
                Console.ReadLine();
            }
        }

        /// <summary>
        /// Menu so user can choose what he wants
        /// </summary>
        static void Menu()
        {
            var prog = new UpdateUserInfo();
            var find = new FindAllMail();
            var info = new AllInfoAboutUser();
            Console.Clear();
            switch (userChoice)
            {
                case "1":
                    info.FindAllInfo();
                    break;
                case "2":
                    find.FindAllUsers();
                    break;
                case "3":
                    prog.UpdateUser();
                    break;
                default:
                    Console.WriteLine("Wrong input");
                    break;
            }
            return;
        }
        /// <summary>
        /// Checks if userinput is correct "hardcode pass"
        /// I could not get it to work otherwise
        /// </summary>
        static void CorrectPassword()
        {
            bool privilage = true;
            while (privilage)
            {
                Console.Write("Name: ");
                userConnected = Console.ReadLine();
                Console.Write("Password: ");
                ///Dont hate for this
                Console.ForegroundColor = ConsoleColor.Black;
                Console.CursorVisible = false;
                userConnetedPassword = Console.ReadLine();
                Console.ForegroundColor = ConsoleColor.White;
                if (userConnected == "Administrator" && userConnetedPassword == "Emil1234E")
                {
                    privilage = false;
                }
                else
                {
                    Console.WriteLine("Wrong");
                    Console.ReadLine();
                }
                Console.Clear();
            }
        }

        /// <summary>
        /// Connection for our AD
        /// </summary>
        public DirectoryEntry createDirectoryEntry()
        {
            ///Some other way we can connect in the future
            DirectoryEntry ldapConnection = new DirectoryEntry("LDAP://192.168.0.2", userConnected, userConnetedPassword, AuthenticationTypes.Secure);

            //DirectoryEntry ldapConnection = new DirectoryEntry("MMDA.DK");
            ///We only need a path if we want to be very specific in what OU we want to look for
            //ldapConnection.Path = "LDAP://OU=Miljømærkering DK,DC=MMDA,DC=dk"; ///We are searching for it in miljømærkering
            ldapConnection.AuthenticationType = AuthenticationTypes.Secure; ///makes secure connetion?
            
            return ldapConnection;
        }
    }
}
