namespace GeneralBot.Models.Config
{
    public class Icon
    {
        public string Announce { get; set; } =
            "https://emojipedia-us.s3.amazonaws.com/thumbs/120/twitter/103/public-address-loudspeaker_1f4e2.png";

        public string Calendar { get; set; } = "https://i.imgur.com/Igurn2H.png";

        public string EightBall { get; set; } = "https://i.imgur.com/eAAerQJ.png";

        public string Warning { get; set; } = "https://i.imgur.com/euWbiQP.png";
        public Weather Weather { get; set; } = new Weather();
    }

    public class Weather
    {
        public string ClearDay { get; set; } = "https://i.imgur.com/GF0URKg.png";
        public string ClearNight { get; set; } = "https://i.imgur.com/ne9f8z5.png";
        public string Cloudy { get; set; } = "https://i.imgur.com/W1qPad4.png";
        public string Default { get; set; } = "https://i.imgur.com/rUYrc0E.png";
        public string Fog { get; set; } = "";
        public string PartlyCloudy { get; set; } = "https://i.imgur.com/qIVnCth.png";
        public string Rain { get; set; } = "https://i.imgur.com/hxWU8V1.png";
        public string Sleet { get; set; } = "https://i.imgur.com/BKyZYLL.png";
        public string Snow { get; set; } = "https://i.imgur.com/BKyZYLL.png";
        public string Wind { get; set; } = "https://i.imgur.com/ObyCzM8.png";
    }
}