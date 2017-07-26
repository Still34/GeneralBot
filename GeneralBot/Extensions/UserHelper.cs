using System.Linq;
using Discord;
using Discord.WebSocket;

namespace GeneralBot.Extensions
{
    public static class UserHelper
    {
        /// <summary>
        ///     Gets the highest role of the user whose color is set.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static Color GetHighestRoleColor(this SocketGuildUser user) => user.Roles.Where(x => !Equals(x.Color, Color.Default)).OrderByDescending(x => x.Position).Select(x => x.Color).FirstOrDefault();

        /// <summary>
        ///     Gets the managed role for the bot user.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static IRole GetManagedRole(this SocketGuildUser user) => user.Roles.FirstOrDefault(x => x.IsManaged);

        /// <summary>
        ///     Gets the nickname of the specified user, or username if the user isn't in a guild.
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Returns the nickname for the user, or username, or "InvalidUser" if none suffice.</returns>
        public static string GetNicknameOrDefault(this IUser user) => (user as IGuildUser)?.Nickname ?? user.Username ?? "InvalidUser";

        /// <summary>
        ///     Gets the full Discord tag of the specified user.
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Returns the Discord tag for the user, or "InvalidUser#0000" if it is null.</returns>
        public static string GetFullnameOrDefault(this IUser user) => user == null ? "InvalidUser#0000" : user.ToString();

        /// <summary>
        ///     Gets the user's avatar. This will return a default one if the user hasn't specified one.
        /// </summary>
        /// <param name="user">The user to show the avatar for.</param>
        /// <param name="size">The size of the avatar (default avatar will always be 256x256).</param>
        /// <returns></returns>
        public static string GetAvatarUrlOrDefault(this IUser user, ushort size = 128) => user?.GetAvatarUrl(size: size) ?? "https://i.imgur.com/o5vbqOB.png";
    }
}