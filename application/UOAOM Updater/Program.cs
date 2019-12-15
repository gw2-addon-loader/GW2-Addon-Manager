using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace UOAOM_Updater
{
    class Program
    {
        static string update_zip = "latestRelease\\update.zip";
        static string update_folder = "latestRelease";

        static void Main(string[] args)
        {
            //race condition handicap. TODO: um, MAKE THIS NOT A RACE CONDITION lol
            //(idk, have another helper process that starts on close button pressed in main app, waits until main app's process is closed, then starts this updater and closes itself?)
            //
            //add clause to retry an extraction if it doesn't work?
            System.Threading.Thread.Sleep(3000);

            if (Directory.Exists(update_folder))
            {
                using (FileStream zip = new FileStream(update_zip, FileMode.Open))
                {
                    ZipArchive archive = new ZipArchive(zip);
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        if (string.IsNullOrEmpty(entry.Name))
                            Directory.CreateDirectory(entry.FullName);
                        else if (entry.FullName != "UOAOM updater.exe")
                        {
                            try
                            {
                                entry.ExtractToFile(Path.Combine(Directory.GetCurrentDirectory(), entry.FullName), true);
                            }
                            catch (IOException)
                            {
                                System.Threading.Thread.Sleep(3000);
                                entry.ExtractToFile(Path.Combine(Directory.GetCurrentDirectory(), entry.FullName), true);
                            }
                            
                        }
                            
                    }
                    zip.Close();
                }
                Directory.Delete(update_folder, true);
            }
                
        }
    }
}
