using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CentralServer.LobbyServer.Session;
using Discord;
using Discord.Webhook;
using EvoS.Framework;
using EvoS.Framework.Constants.Enums;
using EvoS.Framework.DataAccess;
using EvoS.Framework.Network.NetworkMessages;
using EvoS.Framework.Network.Static;
using log4net;
using WebSocketSharp;

namespace CentralServer.LobbyServer.Discord
{
    public class DiscordManager
    {
        private static DiscordManager _instance;
        private static readonly ILog log = LogManager.GetLogger(typeof(DiscordManager));
        
        
        private static readonly string LINE = "\\_\\_\\_\\_\\_\\_\\_\\_\\_\\_\\_\\_\\_\\_\\_\\_\\_\\_\\_\\_\\_\\_\\_\\_\\_\\_\\_\\_\\_\\_";
        private static readonly string LINE_LONG = LINE + "\\_\\_\\_\\_\\_\\_\\_" + LINE;

        private readonly DiscordConfiguration conf;
        
        private readonly DiscordWebhookClient gameLogChannel;
        private readonly ulong? gameLogThreadId;
        private readonly DiscordWebhookClient adminChannel;
        private readonly bool lobbyEnabled;
        private readonly DiscordWebhookClient lobbyChannel;
        private readonly ulong? lobbyThreadId;
        
        private readonly CancellationTokenSource cancelTokenSource = new CancellationTokenSource();

        private static readonly DiscordStatusUtils.Status NO_STATUS = new DiscordStatusUtils.Status { totalPlayers = -1, inGame = -1, inQueue = -1 };
        private DiscordStatusUtils.Status lastStatus = NO_STATUS;
        
        
        public DiscordManager()
        {
            conf = LobbyConfiguration.GetDiscordConfiguration();
            if (!conf.Enabled)
            {
                log.Info("Discord is not enabled");
                return;
            }

            if (conf.ChannelWebhook.MaybeUri())
            {
                log.Info("Discord game log is enabled");
                gameLogChannel = new DiscordWebhookClient(conf.ChannelWebhook);
                gameLogThreadId = conf.ChannelThreadId;
                gameLogChannel.Log += Log;
            }

            if (conf.AdminChannelWebhook.MaybeUri())
            {
                log.Info("Discord admin is enabled");
                adminChannel = new DiscordWebhookClient(conf.AdminChannelWebhook);
                adminChannel.Log += Log;
            }
            
            if (conf.DiscordLobbyEnabled && conf.LobbyChannelWebhook.MaybeUri())
            {
                log.Info("Discord lobby is enabled");
                lobbyChannel = new DiscordWebhookClient(conf.LobbyChannelWebhook);
                lobbyThreadId = conf.LobbyChannelThreadId;
                lobbyChannel.Log += Log;
            }
        }

        public static DiscordManager Get()
        {
            return _instance ??= new DiscordManager();
        }

        public void Start()
        {
            if (lobbyChannel != null)
            {
                _ = SendServerStatusLoop(cancelTokenSource.Token);
            }
        }

        public void Shutdown()
        {
            cancelTokenSource.Cancel();
            cancelTokenSource.Dispose();
        }

        private async Task SendServerStatusLoop(CancellationToken cancelToken)
        {
            while (true)
            {
                if (cancelToken.IsCancellationRequested) return;
                await SendServerStatus();
                await Task.Delay(conf.LobbyChannelUpdatePeriodSeconds * 1000, cancelToken);
            }
        }

        public async void SendGameReport(LobbyGameInfo gameInfo, string serverName, string serverVersion, LobbyGameSummary gameSummary)
        {
            if (gameLogChannel == null)
            {
                return;
            }
            try
            {
                if (gameSummary.GameResult != GameResult.TeamAWon
                    && gameSummary.GameResult != GameResult.TeamBWon)
                {
                    return;
                }
                await gameLogChannel.SendMessageAsync(
                    null,
                    false,
                    embeds: new[] {
                        MakeGameReportEmbed(gameInfo, serverName, serverVersion, gameSummary)
                    },
                    "Atlas Reactor",
                    threadId: gameLogThreadId);
            }
            catch (Exception e)
            {
                log.Error($"Failed to send report to discord webhook {e.Message}");
            }
        }

        private async Task SendServerStatus()
        {
            if (lobbyChannel == null)
            {
                return;
            }
            DiscordStatusUtils.Status status = DiscordStatusUtils.GetStatus();
            if (conf.LobbyChannelUpdateOnChangeOnly && lastStatus.Equals(status))
            {
                return;
            }
            try
            {
                await lobbyChannel.SendMessageAsync(
                    DiscordStatusUtils.BuildPlayerCountSummary(status),
                    username: "Atlas Reactor",
                    threadId: lobbyThreadId)
                    .ContinueWith(x => lastStatus = status);
            }
            catch (Exception e)
            {
                log.Error($"Failed to send report to discord webhook {e.Message}");
            }
        }

        private static Embed MakeGameReportEmbed(LobbyGameInfo gameInfo, string serverName, string serverVersion,
            LobbyGameSummary gameSummary)
        {
            string map = Maps.GetMapName[gameInfo.GameConfig.Map];
            EmbedBuilder eb = new EmbedBuilder
            {
                Title = $"Game Result for {map ?? gameInfo.GameConfig.Map}",
                Description =
                    $"{(gameSummary.GameResult.ToString() == "TeamAWon" ? "Team A Won" : "Team B Won")} " +
                    $"{gameSummary.TeamAPoints}-{gameSummary.TeamBPoints} ({gameSummary.NumOfTurns} turns)",
                Color = gameSummary.GameResult.ToString() == "TeamAWon" ? Color.Green : Color.Red
            };

            eb.AddField("Team A", LINE, true);
            eb.AddField("│", "│", true);
            eb.AddField("Team B", LINE, true);
            eb.AddField("**[ Takedowns : Deaths : Deathblows ] [ Damage : Healing : Damage Received ]**", LINE_LONG, false);

            GetTeamsFromGameSummary(gameSummary, out List<PlayerGameSummary> teamA, out List<PlayerGameSummary> teamB);
            int n = Math.Max(teamA.Count, teamB.Count);
            for (int i = 0; i < n; i++)
            {
                GameReportAddPlayer(eb, teamA.ElementAtOrDefault(i));
                eb.AddField("│", "│", true);
                GameReportAddPlayer(eb, teamB.ElementAtOrDefault(i));
            }

            EmbedFooterBuilder footer = new EmbedFooterBuilder
            {
                Text = $"{serverName} - {serverVersion} - {new DateTime(gameInfo.CreateTimestamp):yyyy_MM_dd__HH_mm_ss}"
            };
            eb.Footer = footer;
            return eb.Build();
        }
        
        public async void SendPlayerFeedback(long accountId, ClientFeedbackReport message)
        {
            if (adminChannel == null)
            {
                return;
            }
            try
            {
                LobbyServerPlayerInfo playerInfo = SessionManager.GetPlayerInfo(accountId);
                EmbedBuilder eb = new EmbedBuilder
                {
                    Title = $"User Report From: {playerInfo.Handle}",
                    Description = message.Message,
                    Color = 16711680
                };
                eb.AddField("Reason", message.Reason, true);
                if (message.ReportedPlayerHandle != null)
                {
                    eb.AddField("Reported Account", $"{message.ReportedPlayerHandle} #{message.ReportedPlayerAccountId}", true);
                }
                await adminChannel.SendMessageAsync(null, false, embeds: new[] { eb.Build() }, "Atlas Reactor");
            }
            catch (Exception e)
            {
                log.Error($"Failed to send report to discord webhook {e.Message}");
            }
        }

        private static void GameReportAddPlayer(EmbedBuilder eb, PlayerGameSummary? player)
        {
            if (player == null)
            {
                eb.AddField("-", "-", true);
                return;
            }
            
            PersistedAccountData account = DB.Get().AccountDao.GetAccount(player.AccountId);
            eb.AddField(
                $"{account.Handle} ({player.CharacterName})",
                $"**[ {player.NumAssists} : {player.NumDeaths} : {player.NumKills} ] [ {player.TotalPlayerDamage} : " +
                $"{player.GetTotalHealingFromAbility() + player.TotalPlayerAbsorb} : {player.TotalPlayerDamageReceived} ]**",
                true);
        }

        private static void GetTeamsFromGameSummary(
            LobbyGameSummary gameSummary,
            out List<PlayerGameSummary> teamA,
            out List<PlayerGameSummary> teamB)
        {
            teamA = new List<PlayerGameSummary>();
            teamB = new List<PlayerGameSummary>();

            // Sort into teams, ignore spectators if ever
            foreach (PlayerGameSummary player in gameSummary.PlayerGameSummaryList)
            {
                if (player.IsSpectator())
                {
                    continue;
                }

                if (player.IsInTeamA())
                {
                    teamA.Add(player);
                }
                else
                {
                    teamB.Add(player);
                }
            }
        }

        private Task Log(LogMessage msg)
        {
            return DiscordUtils.Log(log, msg);
        }
    }
}