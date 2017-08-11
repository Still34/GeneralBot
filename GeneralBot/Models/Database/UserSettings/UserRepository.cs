using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using GeneralBot.Models.Context;

namespace GeneralBot.Models.Database.UserSettings
{
    public class UserRepository : IUserRepository
    {
        private readonly UserContext _userContext;

        public UserRepository(UserContext userContext) => _userContext = userContext;


        public async Task SaveRepositoryAsync() => await _userContext.SaveChangesAsync().ConfigureAwait(false);

        public Games GetGame(IUser user) => _userContext.Games.SingleOrDefault(x => x.UserId == user.Id);
        public async Task<Games> GetOrCreateGameAsync(IUser user)
        {
            var record = GetGame(user);
            if (record != null) return record;

            record = new Games(){UserId = user.Id};
            await _userContext.Games.AddAsync(record).ConfigureAwait(false);
            await SaveRepositoryAsync().ConfigureAwait(false);
            return record;
        }

        public IEnumerable<Games> GetAllGames() => _userContext.Games;
        public async Task AddReminderAsync(Reminder reminder)
        {
            await _userContext.Reminders.AddAsync(reminder).ConfigureAwait(false);
            await SaveRepositoryAsync().ConfigureAwait(false);
        }

        public async Task<Profile> GetOrCreateProfileAsync(IUser user)
        {
            var record = _userContext.Profiles.SingleOrDefault(x => x.UserId == user.Id);
            if (record != null) return record;

            record = new Profile {UserId = user.Id};
            await _userContext.AddAsync(record).ConfigureAwait(false);
            await SaveRepositoryAsync().ConfigureAwait(false);
            return record;
        }

        public Profile GetProfile(IUser user) => _userContext.Profiles.SingleOrDefault(x => x.UserId == user.Id);

        public IEnumerable<Profile> GetProfiles() => _userContext.Profiles;

        public IEnumerable<Reminder> GetReminders(IUser user) =>
            _userContext.Reminders.Where(x => x.UserId == user.Id);

        public IEnumerable<Reminder> GetAllReminders() => _userContext.Reminders;
        public async Task RemoveReminderAsync(Reminder reminder)
        {
            if (reminder != null)
            {
                _userContext.Reminders.Remove(reminder);
                await SaveRepositoryAsync().ConfigureAwait(false);
            }
        }

        public async Task RemoveRemindersAsync(IUser user)
        {
            var records = _userContext.Reminders.Where(x => x.UserId == user.Id);
            if (records != null)
            {
                _userContext.Reminders.RemoveRange(records);
                await SaveRepositoryAsync().ConfigureAwait(false);
            }
        }

        public async Task<Coordinate> AddOrUpdateCoordinatesAsync(IUser user, double longitude, double latitude)
        {
            var record = GetCoordinates(user);
            if (record == null)
            {
                record = new Coordinate {UserId = user.Id, Latitude = latitude, Longitude = longitude};
                await _userContext.Coordinates.AddAsync(record).ConfigureAwait(false);
            }
            else
            {
                record.Longitude = longitude;
                record.Latitude = latitude;
            }
            await SaveRepositoryAsync().ConfigureAwait(false);
            return record;
        }

        public Coordinate GetCoordinates(IUser user) =>
            _userContext.Coordinates.SingleOrDefault(x => x.UserId == user.Id);

        public async Task RemoveCoordinatesAsync(IUser user)
        {
            var record = _userContext.Coordinates.Where(x => x.UserId == user.Id);
            if (record != null)
            {
                _userContext.Coordinates.RemoveRange(record);
                await SaveRepositoryAsync().ConfigureAwait(false);
            }
        }

        public IEnumerable<Coordinate> GetAllCoordinates() => _userContext.Coordinates;
    }
}