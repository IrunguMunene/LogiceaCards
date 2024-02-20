using LogiceaCardDomain;
using LogiceaDTO;
using Microsoft.EntityFrameworkCore;

namespace LogiceaCardsApplication.Services
{
    public class LookUpService
    {
        private readonly LogiceaCardDbContext _dbContext;

        public LookUpService(LogiceaCardDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<int> GetStatusId(string statusName)
        {
            var status = await _dbContext.Statuses.FirstOrDefaultAsync(s => s.Name == statusName);
            return status?.Id ?? 1; // Default to "To Do" if not found
        }

        public async Task<string> GetStatusName(int statusId)
        {
            var status = await _dbContext.Statuses.FirstOrDefaultAsync(s => s.Id == statusId);
            return status?.Name ?? "To Do"; // Default to "To Do" if not found;
        }
        public async Task<UserDTO?> GetUser(string userEmail) 
        {
            var user = await _dbContext.Users
                                .Include(u => u.Role)
                                .FirstOrDefaultAsync(u => u.Email.Equals(userEmail));

            if (user == null)
            {
                return null;
            }

            return new UserDTO
            {
                Email = user.Email,
                Id = user.Id,
                Role = user.Role.Name
            };
        }
        public async Task<UserDTO?> AuthenticateUser(string userEmail, string password)
        {
            var user = await _dbContext.Users
                                .Include(u => u.Role)
                                .FirstOrDefaultAsync(u => u.Email.Equals(userEmail) && u.Password.Equals(password));
            
            if (user == null) return null;
            
            return new UserDTO 
            {
                Id = user.Id,
                Email = user.Email,
                Role = user.Role.Name
            };
        }
    }
}
