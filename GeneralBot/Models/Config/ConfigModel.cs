namespace GeneralBot.Models.Config
{
    public class ConfigModel
    {
        public Command Commands { get; set; } = new Command();
        public Credential Credentials { get; set; } = new Credential();
        public string CurrencySymbol { get; set; } = ":moneybag:";
        public Icon Icons { get; set; } = new Icon();
        public ulong[] Owners { get; set; } = {132855517751148544, 168693960628371456};
    }
}