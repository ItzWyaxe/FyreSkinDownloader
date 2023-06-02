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
                        string path = Environment.ExpandEnvironmentVariables(@"C:\Users\%USERNAME%\AppData\Local\fyremc-client\app-0.8.9\logs\latest.log");

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
                                        if (line.Contains("[Tag")) startIndex = line.IndexOf("[Tag") + "[Tag".Length;
                                        if (line.Contains("[Owner")) startIndex = line.IndexOf("[Owner") + "[Owner".Length;
                                        if (line.Contains("[Iron")) startIndex = line.IndexOf("[Iron") + "[Iron".Length;
                                        if (line.Contains("[Emerald")) startIndex = line.IndexOf("[Emerald") + "[Emerald".Length;
                                        if (line.Contains("[Diamond")) startIndex = line.IndexOf("[Diamond") + "[Diamond".Length;
                                        if (line.Contains("[Admin")) startIndex = line.IndexOf("[Admin") + "[Admin".Length;
                                        if (line.Contains("[Moderator")) startIndex = line.IndexOf("[Moderator") + "[Moderator".Length;
                                        if (line.Contains("[Jr.Moderator")) startIndex = line.IndexOf("[Jr.Moderator") + "[Jr.Moderator".Length;
                                        if (line.Contains("[Builder")) startIndex = line.IndexOf("[Builder") + "[Builder".Length;
                                        if (line.Contains("[Jr.Builder")) startIndex = line.IndexOf("[Jr.Builder") + "[Jr.Builder".Length;
                                        if (line.Contains("[Media")) startIndex = line.IndexOf("[Media") + "[Media".Length;
                                        if (line.Contains("[Aqua")) startIndex = line.IndexOf("[Aqua") + "[Aqua".Length;
                                        if (line.Contains("[Booster")) startIndex = line.IndexOf("[Booster") + "[Booster".Length;
                                        if (line.Contains("[Platnium")) startIndex = line.IndexOf("[Platnium") + "[Platnium".Length;
                                        if (line.Contains("[Team")) startIndex = line.IndexOf("[Team") + "[Team".Length;
                                        if (line.Contains("[Netherite")) startIndex = line.IndexOf("[Netherite") + "[Netherite".Length;
                                        if (line.Contains("[Lazurit")) startIndex = line.IndexOf("[Lazurit") + "[Lazurit".Length;
                                        if (line.Contains("[Epic")) startIndex = line.IndexOf("[Epic") + "[Epic".Length;
                                        if (line.Contains("[Wizard")) startIndex = line.IndexOf("[Wizard") + "[Wizard".Length;
                                        if (line.Contains("[Blaze")) startIndex = line.IndexOf("[Blaze") + "[Blaze".Length;
                                        if (line.Contains("[Phoenix")) startIndex = line.IndexOf("[Phoenix") + "[Phoenix".Length;
                                        if (line.Contains("[Veteran")) startIndex = line.IndexOf("[Veteran") + "[Veteran".Length;
                                        if (line.Contains("[Gold")) startIndex = line.IndexOf("[Gold") + "[Gold".Length;

                                        startIndex += 2;
                                        int NChat = startIndex+3;

                                        int plusIndex = line.LastIndexOf('+');
                                        if (plusIndex != -1 && NChat > plusIndex)
                                        {
                                            startIndex += 1;
                                        }
                                        int endIndex = line.IndexOf(' ', startIndex);

                                        if (startIndex != -1 && endIndex != -1)
                                        {
                                            string PlayerName = line.Substring(startIndex, endIndex - startIndex);
                                            Console.WriteLine("FyreMC log: ", PlayerName);
                                            StartDownloadSkin(PlayerName);
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