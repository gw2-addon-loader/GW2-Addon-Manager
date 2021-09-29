using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Windows;

namespace GW2AddonManager.Updater
{
    class Program
    {
        private static string DownloadPath => Path.Combine(Path.GetTempPath(), "addon-manager.zip");

        static void Main(string[] _)
        {
            try {
                bool retry = true;
                while(retry) {
                    try {
                        using (var mutex = new NamedMutex("GW2AddonManager", true)) {
                            using (var zip = new FileStream(DownloadPath, FileMode.Open)) {
                                var archive = new ZipArchive(zip);
                                foreach (var entry in archive.Entries) {
                                    if (string.IsNullOrEmpty(entry.Name))
                                        Directory.CreateDirectory(entry.FullName);
                                    else if (entry.FullName != "Updater.exe") {
                                        try {
                                            entry.ExtractToFile(Path.Combine(Directory.GetCurrentDirectory(), entry.FullName), true);
                                        }
                                        catch (IOException e) {
                                            MessageBox.Show($"Error while attempting to extract file '{entry.FullName}': {e.Message}", "Update Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                            break;
                                        }
                                    }
                                }
                                zip.Close();
                            }
                        }
                    }
                    catch (TimeoutException) {
                        retry = MessageBox.Show($"The addon manager appears to still be running! Please close the addon manager.\n\nWould you like to try updating again?", "Update Error", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes;
                    }
                }
            }
            catch (Exception e) {
                MessageBox.Show($"Error while attempting to update: {e.Message}", "Update Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
