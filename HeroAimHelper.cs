using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LoserAim
{
    class HeroAimHelper
    {
        public const string HEROAIM_SERIAL_KEY = "COHXCETYPSSNDEUTECYGAQLLVRYXFYEHKQGWVTXTDSHRGRAFRNJECQRMDVAKGWTQ";
        public const string HEROAIM_XOR_KEY = "r8kVu6wgbWeSllDFd0NedIjdFC4UMYkUm8D8x6XNeTHCDfG30y4P3cb0QGwlUzaq";
        public const string HEROAIM_CSGO_PAYLOAD = "https://heroaim.com/stage/patch2";
        public const string HEROAIM_SUPPORTED_VERSION = "170";

        private static WebClient _webClient = new WebClient();

        public static string Version = null;

        public static void InjectCsgoCheat(string loaderPath)
        {
            Process process = new Process();
            process.StartInfo.FileName = loaderPath;
            process.StartInfo.Arguments = "";
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.Start();
            process.WaitForExit();

            try
            {
                if (File.Exists(loaderPath))
                {
                    File.Delete(loaderPath);
                }
            }
            catch
            {
            }
        }

        public static void DownloadCsgoPayload(string outputPath)
        {
            // Download the CS:GO payload
            _webClient.DownloadFile(HEROAIM_CSGO_PAYLOAD, outputPath);
        }

        public static void ExtractCsgoPayload(string blobPath, string dllOutputPath, string injectorOutputPath)
        {
            // XOR the serial key
            var serialKey = HeroAimPro.StringCipher.XORCipher(HEROAIM_SERIAL_KEY, HEROAIM_XOR_KEY);

            // Extract the CS:GO DLL and injector from the payload
            HeroAimPro.Client.UnpackPayload(
                blobPath, // path to encrypted blob
                dllOutputPath, // dll output path
                injectorOutputPath, // injector output path
                serialKey); // serial key to decrypt with
        }

        public static bool WriteRegistry(string key, string value)
        {
            var registryKey = Registry.CurrentUser.CreateSubKey("SOFTWARE\\HeroAim");

            if (registryKey == null)
            {
                return false;
            }

            registryKey.SetValue(key, value);
            
            registryKey.Close();

            return true;
        }

        public static string DownloadSettings()
        {
            return _webClient.DownloadString("https://heroaim.com/version");
        }

        public static void UpdateSettings(string settings)
        {
            string[] array = settings.Split(new char[]
            {
                ','
            });

            int num = (array.Length - 1) / 2;

            try
            {
                RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Software\\HeroAim", true);

                for (int i = 0; i < num; i++)
                {
                    registryKey.SetValue(array[1 + i * 2], array[2 + i * 2]);
                }

                registryKey.Close();
            }
            catch
            {
            }

            Version = array[0];
        }

    }
}
