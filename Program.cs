using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HeroAimPro;

namespace LoserAim
{
    class Program
    {
        private static bool YesNoPrompt()
        {
            ConsoleKey response;

            do
            {
                response = Console.ReadKey(true).Key;
            } while (response != ConsoleKey.Y && response != ConsoleKey.N);

            return response == ConsoleKey.Y;
        }

        static void Main(string[] args)
        {
            Console.Title = "LoserAim lol kek";
            Console.WriteLine("LoserAim for HeroAim CS:GO - made by aequabit, NotOfficer, YatoDev and RaZa");
            Console.WriteLine("-");

            // Check for CS:GO
            if (Process.GetProcessesByName("csgo").Length < 1)
            {
                Console.WriteLine("> Please start CS:GO first.");
                Console.ReadKey();
                return;
            }

            // Payload output path
            var dllPath = Path.Combine(Environment.CurrentDirectory, "heroaim_csgo.dll");
            var injectorPath = Path.Combine(Environment.CurrentDirectory, "injector.exe");

            // Write registry values for the injector and cheat
            if (!HeroAimHelper.WriteRegistry("ClientState", "1") ||
                !HeroAimHelper.WriteRegistry("Install", Environment.CurrentDirectory) ||
                !HeroAimHelper.WriteRegistry("UtilityLocation", dllPath) ||
                !HeroAimHelper.WriteRegistry("HeroAimProInjected", "1"))
            {
                Console.WriteLine("> ERROR: Failed to write to registry. Try restarting as administrator.");
                Console.ReadKey();
                return;
            }

            // Asset paths
            var blobPath = Path.Combine(Environment.CurrentDirectory, "heroaim_csgo_blob.bin");
            var versionPath = Path.Combine(Environment.CurrentDirectory, "version");
            
            var version = "";
            if (!File.Exists(versionPath))
            {
                Console.WriteLine("> ERROR: Failed to read version information.");
                Console.ReadKey();
                return;
            }
            version = File.ReadAllText(versionPath);

            Console.WriteLine("> Would you like to download a current version of HeroAim? [y/n]");
            Console.WriteLine("> WARNING: Newer versions might be unsafe and contain spyware!");
            
            // Get user input
            var doUpdate = YesNoPrompt();

            if (doUpdate)
            {
                blobPath = Path.Combine(Path.GetTempPath(), "heroaim_csgo_blob.bin");

                Console.Write("> Downloading client settings...");

                // Download client settings
                version = HeroAimHelper.DownloadSettings();

                Console.WriteLine("ok");

                Console.Write("> Downloading HeroAim CS:GO...");

                try
                {
                    // Download the CS:GO payload
                    HeroAimHelper.DownloadCsgoPayload(blobPath);

                    Console.WriteLine("ok");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("err");
                    Console.WriteLine("> ERROR: Failed to download CS:GO payload.");
                }
            }

            Console.Write("> Updating client settings...");

            // Download client settings
            HeroAimHelper.UpdateSettings(version);

            Console.WriteLine("ok");

            // Check for cheat updates
            if (HeroAimHelper.Version != HeroAimHelper.HEROAIM_SUPPORTED_VERSION)
            {
                Console.WriteLine("> WARNING: This emulator version does not support the current HeroAim version. Would you like to continue anyways? [y/n]");
                
                if (!YesNoPrompt())
                {
                    Console.WriteLine("Press any key to close...");
                    Console.ReadKey();
                    return;
                }
            }

            Console.Write("> Extracting HeroAim CS:GO...");

            try
            {
                // Extract the CS:GO payload
                HeroAimHelper.ExtractCsgoPayload(blobPath, dllPath, injectorPath);

                Console.WriteLine("ok");
            }
            catch (Exception ex)
            {
                Console.WriteLine("err");
                Console.WriteLine("> ERROR: Failed to extract CS:GO payload.");
            }

            Console.Write("> Injecting...");

            // Inject the CS:GO cheat
            HeroAimHelper.InjectCsgoCheat(injectorPath);

            Console.WriteLine("ok");

            Console.WriteLine("");
            Console.WriteLine("Press any key to close...");
            Console.ReadKey();
        }
    }
}
