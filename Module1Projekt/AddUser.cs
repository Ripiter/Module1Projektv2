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
        public void AddNewUser()
        {
            // connect to LDAP  
            MainMenu conn = new MainMenu();
            DirectoryEntry myLdapConnection = conn.createDirectoryEntry();

            // define vars for user  

            String domain = "MMDA.dk";
            Console.WriteLine("enter First Name:");
            String first = Console.ReadLine();
            Console.WriteLine("enter Last Name");
            String last = Console.ReadLine();
            Console.WriteLine("enter Initials");
            String initials = Console.ReadLine();
            Console.WriteLine("enter Password");
            object[] password = { Console.ReadLine() };
            Console.WriteLine("Enter Group");
            String[] groups = { Console.ReadLine() };
            Console.WriteLine("enter Username");
            String username = Console.ReadLine();
            String homeDrive = "C:";

            Console.WriteLine("Choose home directory \n[1] Blå \n[2] Grøn - Adminstrativ \n[3] Grøn - Konsulenter \n[4] Gul");

            //choosing home directory
            
            string homeDir = "";

            // create user  

            try
            {
                if (CreateUser(myLdapConnection, domain, first, last, initials,
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

        static int CreateUser(DirectoryEntry myLdapConnection, String domain, String first, String initials,
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

            //Initials
            user.Properties["initials"].Add(initials);

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

    }
}
