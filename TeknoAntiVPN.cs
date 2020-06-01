using System;
using InfinityScript;
using Tiny.RestClient;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace TeknoAntiVPN
{
    public class TeknoAntiVPN : BaseScript
    {
        string apiKeyTxt = "scripts\\TeknoAntiVPN\\apikey.txt";
        string ignoredPlayersTxt = "scripts\\TeknoAntiVPN\\ignoredPlayers.txt";
        string playersInGameTxt = "scripts\\TeknoAntiVPN\\playersInGame.txt";

        string API_KEY;
        string[] ignoredPlayers;
        string[] playersInGame;

        public TeknoAntiVPN()
        {
            OnServerStart();
        }

        void OnServerStart()
        {
            PlayerConnected += OnPlayerConnected;
            PlayerDisconnected += OnPlayerDisconnected;

            if (!System.IO.Directory.Exists("scripts\\TeknoAntiVPN")) System.IO.Directory.CreateDirectory("scripts\\TeknoAntiVPN");
            if (!System.IO.File.Exists(apiKeyTxt))
            {
                WriteLog.Warning("Api key file doesn't exist. Creating... Please fill this out and restart the server");
                System.IO.File.Create(apiKeyTxt);
            }
            if (!System.IO.File.Exists(ignoredPlayersTxt))
            {
                WriteLog.Warning("Ignored Players file doesn't exist. Creating... ");
                System.IO.File.Create(ignoredPlayersTxt);
            }
            if (!System.IO.File.Exists(playersInGameTxt))
            {
                WriteLog.Warning("PlayersInGame file doesn't exist. Creating... ");
                System.IO.File.Create(playersInGameTxt);
            }
            WriteLog.Info("AntiVPN by Mahjestic successfully started.");
            API_KEY = System.IO.File.ReadAllText(apiKeyTxt);
        }

        public void OnPlayerConnected(Entity player)
        {

            playersInGame = System.IO.File.ReadAllLines(playersInGameTxt);
            foreach (string playerInGame in playersInGame)
            {
                if (player.Name == playerInGame)
                {
                    WriteLog.Info($"{player.Name} has already been scanned...ignoring them.");
                    return;
                }
            }

            WriteLog.Info($"Detecting if player {player.Name} has a VPN...");
            if (isVPN(player.IP.Address.ToString(), player))
            {
                AfterDelay(2000, () => Utilities.ExecuteCommand($"kick \"{player.Name}\" \"^1Proxies and VPNs are not allowed in this server.\""));
                WriteLog.Warning($"Player {player.Name} has a VPN. Kicking them out.");
            }
            else
            {
                WriteLog.Info($"Player {player.Name} does not have a VPN. Allowing them in.");
            }
        }

        void addToPlayersInGame(Entity player)
        {
            WriteLog.Info($"Adding {player.Name} to playersInGame.txt...");
            System.IO.File.AppendAllText(playersInGameTxt, "\n" + player.Name);
        }
        void removeFromPlayersInGame(Entity player)
        {
            WriteLog.Info($"Removing {player.Name} from playersInGame.txt...");
            string playersInGame_ = System.IO.File.ReadAllText(playersInGameTxt);
            System.IO.File.WriteAllText(playersInGameTxt, playersInGame_.Replace($"\n{player.Name}", ""));
        }

        bool isVPN(string ip, Entity player)
        { 

            ignoredPlayers = System.IO.File.ReadAllLines(ignoredPlayersTxt);
            foreach (string ignoredPlayer in ignoredPlayers) { 
                if (player.Name == ignoredPlayer)
                {
                    WriteLog.Info($"{player.Name} has a VPN but they have been ignored.");
                    return false;
                }
            }

            var client = new TinyRestClient(new HttpClient(), "http://v2.api.iphub.info");

            var response = client.GetRequest($"ip/{ip}")
                .AddHeader("X-Key", API_KEY)
                .ExecuteAsStringAsync();

            Dictionary<string, object> result = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Result);

            if(result["block"].ToString() == "1")
            {
                return true;
            }
            else
            {
                addToPlayersInGame(player);
                return false;
            }
        }

        public void OnPlayerDisconnected(Entity player)
        {
            removeFromPlayersInGame(player);
        }
    }

    public static class WriteLog
    {
        public static void None(string message)
        {
            Log.Write(LogLevel.None, message);
        }
        public static void Info(string message)
        {
            Log.Write(LogLevel.Info, message);
        }

        public static void Error(string message)
        {
            Log.Write(LogLevel.Error, message);
        }

        public static void Warning(string message)
        {
            Log.Write(LogLevel.Warning, message);
        }
    }
}
