using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UMSV.Schema;
using System.Management;
using Enterprise;
using System.Threading;

namespace UMSV
{
    public static class VoiceFileManager
    {
        public static void SaveVoice(string fileName, byte[] voice)
        {
            string[] paths = new string[]{
                Config.Default.VoiceDirectory,
                DateTime.Now.ToString("yyyy-MM-dd"),
                fileName
            };

            var path = Path.Combine(paths);

            if (!Directory.Exists(Path.GetDirectoryName(path)))
                Directory.CreateDirectory(Path.GetDirectoryName(path));

            File.WriteAllBytes(path, voice);
        }

        public static void Start()
        {
            Logger.WriteInfo("VoiceFileManager Start");
            CheckForHardDiskFreeSpaceTimer = new Timer(CheckForHardDiskFreeSpace, null, 0, 3600 * 1000);

        }

        public static void Stop()
        {
            try
            {
                Logger.WriteInfo("VoiceFileManager Stop");
                CheckForHardDiskFreeSpaceTimer.Dispose();
            }
            catch
            {
            }
        }

        private static Timer CheckForHardDiskFreeSpaceTimer;

        private static void CheckForHardDiskFreeSpace(object state)
        {
            try
            {
                char voiceDirectoryPartition = Config.Default.VoiceDirectory.First();

                ManagementObjectSearcher oSearcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_LogicalDisk where DeviceID = '" + voiceDirectoryPartition + ":'");


                foreach (ManagementObject oReturn in oSearcher.Get())
                {
                    CurrentPartitionSize = Convert.ToDecimal(oReturn["Size"].ToString());
                    CurrentPartitionFreeSize = Convert.ToDecimal(oReturn["FreeSpace"].ToString());

                    Logger.WriteDebug("CheckForHardDiskFreeSpace, freespace: {0}/{1}", CurrentPartitionFreeSize, CurrentPartitionSize);

                    if (CurrentPartitionFreeSize * 100 / CurrentPartitionSize < Config.Default.HardDiskFreeSpaceRadio)
                    {
                        DirectoryInfo directory = new DirectoryInfo(Config.Default.VoiceDirectory);
                        DeleteOldestDirectory(directory);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }

        private static void DeleteOldestDirectory(DirectoryInfo directory)
        {
            try
            {

                string dir;
                
                if (directory.GetDirectories().Count() > 0)
                    dir = directory.GetDirectories().OrderBy(p => p.CreationTime).FirstOrDefault().FullName;
                else
                    dir = directory.FullName;


                foreach (string file in Directory.GetFiles(dir))
                    File.Delete(file);

                if (Directory.GetDirectories(dir).Count() > 0)
                    DeleteOldestDirectory(new DirectoryInfo(Directory.GetDirectories(dir).FirstOrDefault()));

                Directory.Delete(dir);
                CheckForHardDiskFreeSpace(directory);
            }
            catch (Exception ex)
            {
                Logger.WriteException(ex.Message);
            }
        }

        public static decimal CurrentPartitionSize;
        public static decimal CurrentPartitionFreeSize;
    }
}
