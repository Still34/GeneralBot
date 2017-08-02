namespace GeneralBot.Models.Config
{
    public class Credential
    {
        public string Discord { get; set; } = "";
        public string Google { get; set; } = "";
        public string TimezoneDb { get; set; } = "";
        public (string clientId, string secret) Imgur { get; set; } = ("", "");
        public string DarkSky { get; set; } = "";
    }
}