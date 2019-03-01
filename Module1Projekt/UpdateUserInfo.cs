using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;

namespace Module1Projekt
{ 
    class UpdateUserInfo
    {
        
        public void UpdateUser()
        {
            UpdateUserMenu();
            string whattoupdate = Console.ReadLine();

            string property = "";
            switch (whattoupdate)
            {
                case "1":
                    property = "mail";
                    break;
                case "2":
                    property = "displayname";
                    break;
                case "3":
                    property = "title";
                    break;
                default:
                    Console.WriteLine("Wrong input");
                    return;
                    break;
            }



            Console.Write("Enter user fx. jakob jawa. waidow: ");
            String username = Console.ReadLine();

            try
            {
                var conn = new Retrieve_all_info();
                
                DirectoryEntry myLdapConnection = conn.createDirectoryEntry(); /// makes the connection

                DirectorySearcher search = new DirectorySearcher(myLdapConnection);
                search.Filter = "(cn=" + username + ")"; /// search for common name username
                search.PropertiesToLoad.Add(property); //loads the properti we want to update 

                SearchResult result = search.FindOne(); // finds user 

                if (result != null) // if it found anything run this code
                {
                    // create new object from search result  

                    DirectoryEntry entryToUpdate = result.GetDirectoryEntry();

                    // show existing properti  

                    Console.WriteLine("Current " + property + " : " + // writes our current property
                                      entryToUpdate.Properties[property][0].ToString());


                    Console.Write("\n\nEnter new " + property + " : ");

                    // get new title and write to AD  

                    String newProperty = Console.ReadLine();

                    entryToUpdate.Properties[property].Value = newProperty; /// update the property
                    entryToUpdate.CommitChanges(); /// commit the changes to ad

                    Console.WriteLine("\n\n...new " + property + " saved"); /// new property saved
                }

                else Console.WriteLine("User not found!"); //if we dident find any user
            }

            catch (Exception e)
            {
                Console.WriteLine("Cant be empty");
            }
        }
        /// <summary>
        /// Used in UpdateUser
        /// </summary>
        void UpdateUserMenu() /// just a menu we can choose from
        {
            Console.WriteLine("What do u wish to update?");
            Console.WriteLine();
            Console.WriteLine("1. Mail");
            Console.WriteLine();
            Console.WriteLine("2. Display Name");
            Console.WriteLine();
            Console.WriteLine("3. title");
            Console.WriteLine();
            Console.WriteLine();
            Console.Write("choose one: ");
        }

    }
}
