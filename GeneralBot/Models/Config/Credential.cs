namespace GeneralBot.Models.Config
{
    public class Credential
    {
        public string DarkSky { get; set; } = "";
        public string Discord { get; set; } = "";
        public string Google { get; set; } = "";
        public (string ClientId, string Secret) Imgur { get; set; } = ("", "");
        public (string ClientId, string Secret) Gfycat { get; set; } = ("", "");
        public string TimezoneDb { get; set; } = "";
    }
}