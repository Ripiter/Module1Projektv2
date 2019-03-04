using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

namespace Module1Projekt
{
    class AddUser
    {
        static void AddNewUser(string[] args)
        {
            // connect to LDAP  

            DirectoryEntry myLdapConnection = CreateDirectoryEntry();

            // define vars for user  

            String domain = "MMDA.dk";
            Console.WriteLine("enter First Name:");
            String first = Console.ReadLine();
            Console.WriteLine("enter Last Name");
            String last = Console.ReadLine();
            Console.WriteLine("enter Password");
            object[] password = { Console.ReadLine() };
            Console.WriteLine("Enter Group");
            String[] groups = { Console.ReadLine() };
            Console.WriteLine("enter Username");
            String username = Console.ReadLine();
            String homeDrive = "C:";

            Console.WriteLine("Choose home directory \n[1] Blå \n[2] Grøn - Adminstrativ \n[3] Grøn - Konsulenter \n[4] Gul");

            //choosing home directory
            String homeDir;
            string dirInput = Console.ReadLine();
            switch (dirInput)
            {
                case "1":
                    homeDir = @"\\MMDA.dk\Miljømærkering DK\Blå\Medarbejdere\" + username;
                    break;

                case "2":
                    homeDir = @"\\MMDA.dk\Miljømærkering DK\Grøn\Grøn\Adminstrativ\Medarbejdere\";
                    break;

                case "3":
                    homeDir = @"\\MMDA.dk\Miljømærkering DK\Grøn\Grøn\Konsulenter\Medarbejdere\";
                    break;

                case "4":
                    homeDir = @"\\MMDA.dk\Miljømærkering DK\Gul\Medarbejdere\";
                    break;

            }
            


            // create user  

            try
            {
                if (CreateUser(myLdapConnection, domain, first, last,
                         password, groups, username, homeDrive, homeDir, true) == 0)
                {

                    Console.WriteLine("Account created!");
                    Console.ReadLine();
                }

                else
                {
                    Console.WriteLine("Problem creating account :(");
                    Console.ReadLine();
                }
            }

            catch (Exception e)
            {
                Console.WriteLine("Exception caught:\n\n" + e.ToString());
                Console.ReadLine();
            }
        }

        static int CreateUser(DirectoryEntry myLdapConnection, String domain, String first,
                              String last, object[] password,
                              String[] groups, String username, String homeDrive,
                              String homeDir, bool enabled)
        {
            // create new user object and write into AD  

            DirectoryEntry user = myLdapConnection.Children.Add(
                                  "CN=" + first + " " + last, "user");

            // User name (domain based)   
            user.Properties["userprincipalname"].Add(username + "@" + domain);

            // User name (older systems)  
            user.Properties["samaccountname"].Add(username);

            // Surname  
            user.Properties["sn"].Add(last);

            // Forename  
            user.Properties["givenname"].Add(first);

            // Display name  
            user.Properties["displayname"].Add(first + " " + last);

            // E-mail  
            user.Properties["mail"].Add(username + "@" + domain);

            // Home dir (drive letter)  
            user.Properties["homedirectory"].Add(homeDir);

            // Home dir (path)  
            user.Properties["homedrive"].Add(homeDrive);

            user.CommitChanges();



            // set user's password  

            user.Invoke("SetPassword", password);

            // enable account if requested (see http://support.microsoft.com/kb/305144 for other codes)   

            if (enabled)
                user.Invoke("Put", new object[] { "userAccountControl", "512" });

            // add user to specified groups  

            foreach (String thisGroup in groups)
            {
                DirectoryEntry newGroup = myLdapConnection.Parent.Children.Find(
                                          "CN=" + thisGroup, "group");

                if (newGroup != null)
                    newGroup.Invoke("Add", new object[] { user.Path.ToString() });
            }

            user.CommitChanges();

            // make home folder on server  

            Directory.CreateDirectory(homeDir);

            // set permissions on folder, we loop this because if the program  
            // tries to set the permissions straight away an exception will be  
            // thrown as the brand new user does not seem to be available, it takes  
            // a second or so for it to appear and it can then be used in ACLs  
            // and set as the owner  

            bool folderCreated = false;

            while (!folderCreated)
            {
                try
                {
                    // get current ACL  

                    DirectoryInfo dInfo = new DirectoryInfo(homeDir);
                    DirectorySecurity dSecurity = dInfo.GetAccessControl();

                    // Add full control for the user and set owner to them  

                    IdentityReference newUser = new NTAccount(domain + @"\" + username);

                    dSecurity.SetOwner(newUser);

                    FileSystemAccessRule permissions =
                       new FileSystemAccessRule(newUser, FileSystemRights.FullControl,
                                                AccessControlType.Allow);

                    dSecurity.AddAccessRule(permissions);

                    // Set the new access settings.  

                    dInfo.SetAccessControl(dSecurity);
                    folderCreated = true;
                }

                catch (System.Security.Principal.IdentityNotMappedException)
                {
                    Console.Write(".");
                }

                catch (Exception ex)
                {
                    // other exception caught so not problem with user delay as   
                    // commented above  

                    Console.WriteLine("Exception caught:" + ex.ToString());
                    return 1;
                }
            }

            return 0;
        }

        /// <summary>
        /// Connection for our AD
        /// </summary>
        static DirectoryEntry CreateDirectoryEntry()
        {
            // create and return new LDAP connection with desired settings  


            ///Some other way we can connect in the future
            DirectoryEntry ldapConnection = new DirectoryEntry("LDAP://192.168.0.2", "Administrator", "Emil1234E", AuthenticationTypes.Secure);


            //DirectoryEntry ldapConnection = new DirectoryEntry("MMDA.DK");
            ///We only need a path if we want to be very specific in what OU we want to look for
            //ldapConnection.Path = "LDAP://OU=Miljømærkering DK,DC=MMDA,DC=dk"; ///We are searching for it in miljømærkering
            ldapConnection.AuthenticationType = AuthenticationTypes.Secure; ///makes secure connetion?

            return ldapConnection;
        }
    }
}
