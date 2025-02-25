using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using EvoS.Framework.Constants.Enums;
using log4net;

namespace EvoS.Framework
{
    public static class Extensions
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Extensions));
        
        public static MemoryStream ReadStream(this Stream source)
        {
            var ms = new MemoryStream();

            source.CopyTo(ms);
            ms.Position = 0;

            return ms;
        }

        public static string ReadBinary(this Stream source)
        {
            if (source == null)
                return "NULL";

            try
            {
                var message = source.ReadStream();
                var buffer = new byte[message.Length];

                message.Read(buffer, 0, buffer.Length);
                message.Dispose();

                return BitConverter.ToString(buffer).Replace("-", " ");
            }
            catch
            {
                return "NULL";
            }
        }

        public static string ReadText(this Stream source)
        {
            if (source == null)
                return "NULL";

            try
            {
                var message = source.ReadStream();
                var buffer = new byte[message.Length];

                message.Read(buffer, 0, buffer.Length);
                message.Dispose();

                return Encoding.UTF8.GetString(buffer);
            }
            catch
            {
                return "NULL";
            }
        }
        
        public static PlayerGameResult ToPlayerGameResult(this GameResult gameResult, Team team)
        {
            return gameResult switch
            {
                GameResult.NoResult => PlayerGameResult.NoResult,
                GameResult.TieGame => PlayerGameResult.Tie,
                GameResult.TeamAWon => team == Team.TeamA ? PlayerGameResult.Win : PlayerGameResult.Lose,
                GameResult.TeamBWon => team == Team.TeamB ? PlayerGameResult.Win : PlayerGameResult.Lose,
                _ => PlayerGameResult.NoResult
            };
        }
        
        public static IPAddress GetSubnet(this IPAddress address, int subnet)
        {
            byte[] ipAddressBytes = address.GetAddressBytes();
            if (subnet == 0)
            {
                return new IPAddress(ipAddressBytes);
            }
            
            if (subnet > 32)
            {
                throw new ArgumentException("Bad IP address mask");
            }

            byte[] broadcastAddress = new byte[ipAddressBytes.Length];
            int maskPow = subnet;
            for (int i = broadcastAddress.Length - 1; i >= 0; i--)
            {
                int maskBytePow = Math.Max(0, Math.Min(maskPow, 8));
                maskPow -= 8;
                byte maskByte = (byte)((1 << maskBytePow) - 1);
                broadcastAddress[i] = (byte)(ipAddressBytes[i] | maskByte);
            }
            return new IPAddress(broadcastAddress);
        }
        
        public static bool IsSameSubnet(this IPAddress address, IPAddress otherAddress, int subnet)
        {
            return address.GetSubnet(subnet).Equals(otherAddress.GetSubnet(subnet));
        }

        public static string FormatMinutesSeconds(this TimeSpan timeSpan)
        {
            return $"{Math.Truncate(timeSpan.TotalMinutes)}:{timeSpan.Seconds:00}";
        }

        public static async Task LogError(this Task task)
        {
            log.Debug($"Starting task: {task}");
            try
            {
                await task;
                log.Debug($"Task finished: {task}");
            }
            catch (Exception e)
            {
                log.Error($"Task failed: {task}", e);
            }
        }
    }
}
