using System;
using System.IO;
using System.Text;
using EvoS.Framework.Constants.Enums;

namespace EvoS.Framework
{
    public static class Extensions
    {
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
    }
}
