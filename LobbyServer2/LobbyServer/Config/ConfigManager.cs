namespace CentralServer.LobbyServer.Config
{
    static class ConfigManager
    {
        public static string MOTDPopUpText = "";
        public static string MOTDText = "Lobby server version 2";

        public static string PatchNotesHeader = "Evos Emulator v0.1";
        public static string PatchNotesDescription = "The resurrection";
        public static string PatchNotesText = "";

        public static bool GameTypePracticeAvailable = false;
        public static bool GameTypeCoopAvailable = false;
        public static bool GameTypePvPAvailable = true;
        public static bool GameTypeRankedAvailable = false;
        public static bool GameTypeCustomAvailable = false;
    }
}
