using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Threading;

namespace Module1Projekt
{
    class MainMenu
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
                Console.WriteLine("Choose one\r\n");
                Console.WriteLine("[1] Find all information about user\r\n");
                Console.WriteLine("[2] Find information about all users\r\n");
                Console.WriteLine("[3] Update a user\r\n");
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
                case "4":
                    Console.WriteLine("DLC comming soon");
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
            int tries = 0;
            while (privilage)
            {

                if (tries == 3)
                {
                    Console.WriteLine("u got locked out of the system for 60 seconds"); 
                    Thread.Sleep(60000);
                }
                Console.Write("Name: ");
                userConnected = Console.ReadLine().ToLower();
                Console.Write("Password: ");
                ///Dont hate for this
                ///Will replace with a "correct version"
                ///This is beta version of hiding password
                
                userConnetedPassword = "";
                while(true)
                {
                    ConsoleKeyInfo key = Console.ReadKey(true);
                    // Backspace Should Not Work
                    if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                    {
                        userConnetedPassword += key.KeyChar;
                        Console.Write("*");
                    }
                    else
                    {
                        if (key.Key == ConsoleKey.Backspace && userConnetedPassword.Length > 0)
                        {
                            userConnetedPassword = userConnetedPassword.Substring(0, (userConnetedPassword.Length - 1));
                            Console.Write("\b \b");
                        }
                        else if (key.Key == ConsoleKey.Enter)
                        {
                            break;
                        }
                    }
                }

                if (userConnected == "administrator" && userConnetedPassword == "Emil1234E")
                {
                    privilage = false;
                }
                else
                {
                    tries++;
                    Console.WriteLine("");
                    Console.WriteLine("Wrong");
                    Console.WriteLine("You wrote username:" + userConnected + ", Password:" + userConnetedPassword);
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
