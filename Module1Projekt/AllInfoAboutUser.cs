using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;

namespace Module1Projekt
{
    class AllInfoAboutUser
    {
        static DirectoryEntry myLdapConnection;
        static SearchResult result;
        /// <summary>
        /// Find all the information about the user
        /// </summary>
        public void FindAllInfo()
        {
            var test = new Retrieve_all_info();
            DirectorySearcher search;
            Console.Write("Enter user fx. Jakob JAWA. Waidow: ");
            String username = Console.ReadLine();

            try
            {
                // create LDAP connection object  

                //mLdapConnetion is our connection to AD if there is no result comming out
                ///Check for the right connection string
                myLdapConnection = test.createDirectoryEntry();


                // create search object which operates on LDAP connection object  
                // and set search object to only find the user specified  

                ///Search filter becomes what we typed at the start                
                search = new DirectorySearcher(myLdapConnection);
                search.Filter = "(cn=" + username + ")";

                // create results objects from search object  
                result = search.FindOne();

                IsFound();
            }


            catch (Exception)
            {
                //  Console.WriteLine("Exception caught:\n\n" + e.ToString());
                Console.WriteLine("Wrong password or username");
            }


            return;
        }
        /// <summary>
        /// Used in FindAllInfo
        /// </summary>
        public void IsFound()
        {
            ///When result is found 
            if (result != null)
            {
                // user exists, cycle through LDAP fields (cn, telephonenumber etc.)  

                ResultPropertyCollection fields = result.Properties;

                ///Searches for all results that have pur input inside?
                foreach (String ldapField in fields.PropertyNames)
                {
                    // cycle through objects in each field e.g. group membership  
                    // (for many fields there will only be one object such as name)  

                    foreach (Object myCollection in fields[ldapField])
                        Console.WriteLine(String.Format("{0,-20} : {1}",
                                      ldapField, myCollection.ToString()));
                }
            }
            ///If it cant find the user
            ///IT will not say this if connetion string is wrong
            else
            {
                // user does not exist  
                Console.WriteLine("User not found!");
            }

        }
    }
}
