using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Durados.Windows.Utilities.AzureUploader
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (args.Length > 0 && args[0].ToLower() == "backup")
            {
                Backup(args);
            }
            else
            {
                Application.Run(new MainMenuForm());
            }
            
        }

        private static BackupArgs backupArgs;
        static void Backup(string[] args)
        {
            try
            {
                backupArgs = GetBackupArgs(args);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }

            try
            { 
                Backup(backupArgs);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        private static BackupArgs GetBackupArgs(string[] args)
        {
            BackupArgs backupArgs = new BackupArgs();

            backupArgs.WriteToConsole = true;
            backupArgs.StorageName = args[1];
            backupArgs.StorageKey = args[2];
            backupArgs.BackupStorageName = args[3];
            backupArgs.BackupStorageKey = args[4];
            if (args.Length > 5)
                backupArgs.Copies = Convert.ToInt32(args[5]);
            else
                backupArgs.Copies = 14;
            if (args.Length > 6)
                backupArgs.SendEmailOnSuccess = Convert.ToBoolean(args[6]);
            else
                backupArgs.SendEmailOnSuccess = true;
            if (args.Length > 7)
                backupArgs.SendEmailOnFailure = Convert.ToBoolean(args[7]);
            else
                backupArgs.SendEmailOnFailure = true;
            if (args.Length > 8)
                backupArgs.Emails = Convert.ToString(args[8]).Split('|');
            else
                backupArgs.Emails = new string[2] { "itay@backand.com", "relly@backand.com" };
            if (args.Length > 9)
                backupArgs.SmtpHost = args[9];
            else
                backupArgs.SmtpHost = "smtp.mandrillapp.com";
            if (args.Length > 10)
                backupArgs.SmtpUsername = args[10];
            else
                backupArgs.SmtpUsername = "itay@modubiz.com";
            if (args.Length > 11)
                backupArgs.SmtpPassword = args[11];
            else
                backupArgs.SmtpPassword = "RmsCSyEKZNSLWdw1GMOO_A";
            if (args.Length > 12)
                backupArgs.SmtpPort = Convert.ToInt32(args[12]);
            else
                backupArgs.SmtpPort = 587;
            if (args.Length > 13)
                backupArgs.SmtpFrom = args[13];
            else
                backupArgs.SmtpFrom = "teamqa@backand.com";
            
            return backupArgs;
        }

        static void Backup(BackupArgs backupArgs)
        {
            try
            {
                Backup backup = new Backup();

                backup.StorageCred = new StorageCred() { Name = backupArgs.StorageName, Key = backupArgs.StorageKey };
                backup.BackupStorageCred = new StorageCred() { Name = backupArgs.BackupStorageName, Key = backupArgs.BackupStorageKey };
                backup.Copies = backupArgs.Copies;

                backup.BackupStarted += backup_BackupStarted;
                backup.BackupContainerStarted += backup_BackupContainerStarted;
                backup.BackupContainerEnded += backup_BackupContainerEnded;
                backup.BackupEnded += backup_BackupEnded;
                backup.All();
            }
            catch (Exception exception)
            {
                Send(backupArgs.Emails, "Backup Ended with unhandled exception: {0}", Environment.NewLine + Environment.NewLine + exception.Message + Environment.NewLine + Environment.NewLine + exception.StackTrace);
            }
            finally
            {
                Application.Exit();
            }
        }

        static void backup_BackupStarted(object sender, BackupStartedEventArgs e)
        {
            if (backupArgs.WriteToConsole)
            {
                Console.WriteLine(e.ToString());
            }
            if (backupArgs.SendEmailOnSuccess)
            {
                Send(backupArgs.Emails, "Backup Started", e.ToString());
            }
        }

        static void backup_BackupContainerStarted(object sender, BackupContainerStartedEventArgs e)
        {
            if (backupArgs.WriteToConsole)
            {
                Console.WriteLine(e.ToString());
            }
        }

        static void backup_BackupContainerEnded(object sender, BackupContainerEndedEventArgs e)
        {
            if (backupArgs.WriteToConsole)
            {
                Console.WriteLine(e.ToString());
            }
        }

        static void backup_BackupEnded(object sender, BackupEndedEventArgs e)
        {
            if (backupArgs.WriteToConsole)
            {
                Console.WriteLine(e.ToString());
            }
            if (backupArgs.SendEmailOnSuccess && e.Success)
            {
                Send(backupArgs.Emails, "Backup Ended", e.ToString());
            }
            else if (backupArgs.SendEmailOnFailure && !e.Success)
            {
                Send(backupArgs.Emails, "Backup Ended with Failures", e.ToString() + Environment.NewLine + Environment.NewLine + e.GetExceptionsReport());
            }
        }

        static void Send(string[] to, string subject, string message)
        {
            Durados.Cms.DataAccess.Email.Send(backupArgs.SmtpHost, false, backupArgs.SmtpPort, backupArgs.SmtpUsername, backupArgs.SmtpPassword, false, to, null, null, subject, message, backupArgs.SmtpFrom, backupArgs.SmtpFrom, null, false, null, null, true);
        }

        public class BackupArgs
        {
            public string StorageName { get; set; }
            public string StorageKey { get; set; }
            public string BackupStorageName { get; set; }
            public string BackupStorageKey { get; set; }
            public int Copies { get; set; }
            public bool WriteToConsole { get; set; }
            public bool SendEmailOnSuccess { get; set; }
            public bool SendEmailOnFailure { get; set; }
            public string[] Emails { get; set; }

            public string SmtpHost { get; set; }
            public string SmtpUsername { get; set; }
            public string SmtpPassword { get; set; }
            public int SmtpPort { get; set; }
            public string SmtpFrom { get; set; }



        }
    }
}
