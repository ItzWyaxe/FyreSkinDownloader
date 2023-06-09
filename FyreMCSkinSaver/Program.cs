using System;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace FyreMCSkinSaver
{
    class Program
    {
        static bool ValidateInput(string text)
        {
            string pattern = @"^[a-zA-Z0-9]+$";
            if (Regex.IsMatch(text, pattern))
            {
                return true;
            }
            return false;
        }

        static string ValidInput(string playername)
        {
            while (true)
            {
                Console.Write(playername);
                string fmcplayer = Console.ReadLine();
                if (ValidateInput(fmcplayer))
                {
                    return fmcplayer;
                }
                Console.WriteLine("Invalid input!");
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("FyreMC skin saver by: ItzWyaxe");
            while (true)
            {
                try
                {
                    Console.Write("FyreMC playername: ");
                    string playername = ValidInput("");
                    if (playername == "log")
                    {
                        Console.Write("Get skins from fyremc log\n");
                        string path = Environment.ExpandEnvironmentVariables(@"C:\Users\%USERNAME%\AppData\Local\fyremc-client\app-0.9.3\logs\latest.log");

                        using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        using (var sr = new StreamReader(fs, System.Text.Encoding.ASCII))
                        {
                            string line;
                            while (true)
                            {
                                while (!sr.EndOfStream)
                                {
                                    line = sr.ReadLine();
                                    if (sr.Peek() == -1)
                                    {
                                        int startIndex = 0;
                                        string[] Ranks = { "[Tag", "[Owner", "[Iron", "[Emerald", "[Diamond", "[Admin", "[Moderator", "[Jr.Moderator",
                    "[Builder", "[Jr.Builder", "[Media", "[Aqua", "[Booster", "[Platnium", "[Team", "[Netherite",
                    "[Lazurit", "[Epic", "[Wizard", "[Blaze", "[Phoenix", "[Veteran", "[Gold" };

                                        bool containsrank = false;
                                        for (int i = 0;  i < Ranks.Length; i++)
                                        {
                                            if (line.Contains(Ranks[i])) {
                                                startIndex = line.IndexOf(Ranks[i]) + Ranks[i].Length;
                                                containsrank = true;
                                            }
                                        }

                                        if (containsrank && line.Contains("[CHAT]"))
                                        {
                                            startIndex += 2;
                                            int NChat = startIndex + 3;

                                            int plusIndex = line.LastIndexOf('+');
                                            if (plusIndex != -1 && NChat > plusIndex)
                                            {
                                                startIndex += 1;
                                            }
                                            int endIndex = line.IndexOf(' ', startIndex);

                                            if (startIndex != -1 && endIndex != -1)
                                            {
                                                string PlayerName = line.Substring(startIndex, endIndex - startIndex);
                                                Console.WriteLine("----------------");
                                                Console.WriteLine($"Playername: {PlayerName}");
                                                Console.WriteLine($"Chat: {line}");
                                                StartDownloadSkin(PlayerName);
                                                Console.WriteLine("----------------");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        StartDownloadSkin(playername);
                    }
                }
                catch (WebException e)
                {
                    Console.WriteLine("Error: " + e.Message);
                }
            }
        }

        static void StartDownloadSkin(string playername)
        {
            string url = "https://account.fyremc.hu/api/player/" + playername;
            string userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.UserAgent = userAgent;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string json = reader.ReadToEnd();
            response.Close();

            dynamic fyremc_player_json = JsonConvert.DeserializeObject(json);
            if ((bool)fyremc_player_json.error)
            {
                Console.WriteLine("Player not found");
            }
            else
            {
                dynamic json_data = fyremc_player_json.data;
                string username = json_data.username;
                string skin_url_2d = json_data.skin;
                skin_url_2d = skin_url_2d.Substring(44);
                string skinFolder = skin_url_2d.Substring(0, 2);
                string skinURL = $"https://account.fyremc.hu/MinecraftSkins/{skinFolder}/{skin_url_2d}";
                HttpWebRequest skinRequest = (HttpWebRequest)WebRequest.Create(skinURL);
                skinRequest.UserAgent = userAgent;
                HttpWebResponse skinResponse = (HttpWebResponse)skinRequest.GetResponse();
                string skinspath = Path.Combine(Directory.GetCurrentDirectory(), "skins");
                if (!Directory.Exists(skinspath))
                {
                    Directory.CreateDirectory(skinspath);
                }

                using (Stream skinStream = skinResponse.GetResponseStream())
                {
                    using (FileStream fileStream = File.Create($"./skins/{username}.png"))
                    {
                        skinStream.CopyTo(fileStream);
                    }
                }

                Console.WriteLine($"Skin saved! ( {username} )");
            }
        }
    }
}
