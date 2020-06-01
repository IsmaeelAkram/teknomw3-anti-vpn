﻿using System;
using InfinityScript;
using Tiny.RestClient;
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
            if (isVPN("67.85.105.1"))
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
            //http://v2.api.iphub.info/ip/{ip}
            //"X-Key", "OTE0NjpPc2pseU5pS1FuVVU2RDVQRmFObGJra3c4S2hMMHFteg=="

            var client = new TinyRestClient(new HttpClient(), "http://v2.api.iphub.info");

            return false;
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
