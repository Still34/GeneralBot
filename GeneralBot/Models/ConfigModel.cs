using System.Collections.Generic;

namespace GeneralBot.Models
{
    public class ConfigModel
    {
        public Credential Credentials { get; set; } = new Credential();
        public Command Commands { get; set; } = new Command();
        public ulong[] Owners { get; set; } = {132855517751148544, 168693960628371456};
    }

    public class Command
    {
        public (List<string> Responses, string Image) EightBall { get; set; } = (new List<string>
            {
                "It is certain.",
                "It is decidedly so.",
                "Without a doubt!",
                "Yes, definitely!",
                "You may rely on it.",
                "As I see it, yes.",
                "Most likely.",
                "Outlook good.",
                "Yes.",
                "Signs point to yes.",
                "Don't count on it.",
                "My reply is no.",
                "My sources say no.",
                "Outlook not so good.",
                "Very doubtful."
            },
            "https://i.imgur.com/eAAerQJ.png");
    }

    public class Credential
    {
        public string Discord { get; set; } = "";
        public string Google { get; set; } = "";
        public string TimezoneDb { get; set; } = "";
        public (string clientId, string secret) Imgur { get; set; } = ("", "");
        public string DarkSky { get; set; } = "";
    }
}