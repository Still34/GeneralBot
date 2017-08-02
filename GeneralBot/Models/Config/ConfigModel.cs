namespace GeneralBot.Models.Config
{
    public class ConfigModel
    {
        public Credential Credentials { get; set; } = new Credential();
        public Command Commands { get; set; } = new Command();
        public string CurrencySymbol { get; set; } = ":moneybag:";
        public ulong[] Owners { get; set; } = {132855517751148544, 168693960628371456};
    }
}