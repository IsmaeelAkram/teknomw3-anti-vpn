using System;
using Tiny.RestClient;
using InfinityScript;
using System.Net.Http;

namespace TeknoAntiVPN
{
    public class TeknoAntiVPN : BaseScript
    {
        public TeknoAntiVPN()
        {
            OnServerStart();
        }

        void OnServerStart()
        {
            PlayerConnected += OnPlayerConnected;
            WriteLog.Info("AntiVPN by Mahjestic successfully started.");
        }

        public void OnPlayerConnected(Entity player)
        {
            Players.Add(player);
            string playerIP = player.IP.Address.ToString();
            if (isVPN(playerIP))
            {
                AfterDelay(2000, () => Utilities.ExecuteCommand($"kick \"{player.Name}\" \"^1Proxies and VPNs are not allowed in this server.\""));
                WriteLog.Info($"Player {player.Name} has a VPN. Kicking them out.");
            }
            else
            {
                WriteLog.Info($"Player {player.Name} does not have a VPN. Allowing them in.");
            }
        }

        bool isVPN(string ip)
        {
            //check.php?ip={ip}&format=json&contact=null@null.com&flags=m
            string url = $"http://check.getipintel.net/";
            var client = new TinyRestClient(new HttpClient(), url);
            var response = client.GetRequest("check.php")
                .AddQueryParameter("ip", ip)
                .AddQueryParameter("format", "json")
                .AddQueryParameter("contact", "null@null.com")
                .AddQueryParameter("flags", "m")
                .ExecuteAsStringAsync();

            WriteLog.Info(response.ToString());

            return true;
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
