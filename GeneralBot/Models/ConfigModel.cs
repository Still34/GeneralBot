namespace GeneralBot.Models
{
    public class ConfigModel
    {
        public Credential Credentials { get; set; } = new Credential();
        public ulong[] Owners { get; set; } = {132855517751148544, 168693960628371456};
    }

    public class Credential
    {
        public string Discord { get; set; } = "";
        public string TimezoneDb { get; set; } = "";
        public string Imgur { get; set; } = "";
        public string DarkSky { get; set; } = "";
    }
}