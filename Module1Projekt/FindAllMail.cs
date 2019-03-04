using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;

namespace Module1Projekt
{
    class FindAllMail
    {
        /// <summary>
        /// Finds all infomation fx. name 
        /// from all users
        /// </summary>
        public void FindAllUsers()
        {
            MainMenu connection = new MainMenu();
            Console.Write("Enter property example initials or mail: ");
            String property = Console.ReadLine(); /// enters a property like a mail or ini 

            try
            {
                DirectoryEntry myLdapConnection = connection.createDirectoryEntry();

                DirectorySearcher search = new DirectorySearcher(myLdapConnection);
                search.PropertiesToLoad.Add("cn"); /// adding all the common names in the domain
                search.PropertiesToLoad.Add(property); /// adds the item we search for in our property


                SearchResultCollection allUsers = search.FindAll(); /// makes a collection out of our search

                foreach (SearchResult result in allUsers)  // foreach result in our all users collection 
                {
                    if (result.Properties["cn"].Count > 0 && result.Properties[property].Count > 0)
                    {
                        Console.WriteLine(String.Format("{0,-20} : {1}", /// prints all of our results 
                                      result.Properties["cn"][0].ToString(),
                                      result.Properties[property][0].ToString()));
                    }
                }
            }

            catch (Exception e)
            {
                Console.WriteLine("Exception caught:\n\n" + e.ToString());
            }
        }
    }
}
