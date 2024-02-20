using LogiceaCardDomain;
using LogiceaCardsApplication.Services;
using LogiceaDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LogiceaCardsApplication.Controllers
{
    [Route("api/cards")]
    [ApiController]
    [Authorize]
    public class CardsController : ControllerBase
    {
        private readonly LogiceaCardDbContext _dbContext;
        private readonly LookUpService _lookUpService;
        private readonly JwtService _jwtService;

        public CardsController(LogiceaCardDbContext dbContext, LookUpService lookUpService, JwtService jwtService)
        {
            _dbContext = dbContext;
            _lookUpService = lookUpService;
            _jwtService = jwtService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCard([FromBody] CardDTO cardDTO)
        {
            UserDTO? userDto = await IsAuthorized();

            if (userDto == null)
            {
                return Forbid();
            }

            // Map DTO to entity
            var card = new Card
            {
                Name = cardDTO.Name,
                Description = cardDTO.Description,
                Color = cardDTO.Color,
                StatusId = await _lookUpService.GetStatusId(cardDTO.Status),
                UserId = userDto.Id,       // Assumption made is admins should also be able to create cards
                CreatedAt = DateTime.UtcNow,
            };

            // Enforce constraints
            if (string.IsNullOrWhiteSpace(card.Name))
            {
                return BadRequest("Card name is mandatory.");
            }

            if (!IsValidColorFormat(card.Color))
            {
                return BadRequest("Invalid color format. It should be 6 alphanumeric characters prefixed with #.");
            }

            // Set default values
            card.StatusId = _dbContext.Statuses.FirstOrDefault(s => s.Name == "To Do")?.Id ?? 1;

            _dbContext.Cards.Add(card);
            await _dbContext.SaveChangesAsync();

            // Map entity back to DTO for response
            var responseDTO = new CardDTO(card.Id)
            {
                Name = card.Name,
                Description = card.Description,
                Color = card.Color,
                Status = await _lookUpService.GetStatusName(card.StatusId),
                CreatedAt = card.CreatedAt
            };

            return Ok(responseDTO);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCards()
        {
             UserDTO? userDto = await IsAuthorized();
 
             if (userDto == null)
             {
                 return Forbid();
             }
 
             List<Card> cards = await _dbContext.Cards
                     .Include(c => c.Status)
                     .Include(c => c.User)
                     .Where(c => (userDto.Role == "Admin" || c.UserId == userDto.Id))
                     .ToListAsync();
 
             List<CardDTO> cardDTOs = new List<CardDTO>();
             foreach (var card in cards)
             {
                 cardDTOs.Add(new CardDTO(card.Id)
                 {
                     Color = card.Color,
                     CreatedAt = card.CreatedAt,
                     Description = card.Description,
                     Name = card.Name,
                     Status = card.Status.Name
                 });
             }
             
             return Ok(cardDTOs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCardAsync(int id)
        {
            UserDTO? userDto = await IsAuthorized();

            if (userDto == null)
            {
                return Forbid();
            }

            // Retrieve the card with the specified id from the database
            var card = _dbContext.Cards
                            .Include(c => c.Status)
                            .Include(c => c.User)
                            .FirstOrDefault(c => c.Id == id && c.UserId == userDto.Id);

            if (card == null)
            {
                return NotFound();
            }

            // Map entity back to DTO for response
            var responseDTO = new CardDTO(card.Id)
            {
                Name = card.Name,
                Description = card.Description,
                Color = card.Color,
                Status = card.Status.Name,
                CreatedAt = card.CreatedAt
            };

            return Ok(responseDTO);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCard(int id, [FromBody] CardDTO updatedCardDTO)
        {
            UserDTO? userDto = await IsAuthorized();

            if (userDto == null)
            {
                return Forbid();
            }

            var existingCard = await _dbContext.Cards
                                    .Include(c => c.Status) 
                                    .Include(c => c.User)
                                    .FirstOrDefaultAsync(c =>
                                        (userDto.Role == "Admin" || c.UserId == userDto.Id) && c.Id == id);

            if (existingCard == null)
            {
                return NotFound();
            }

            // Admins can update any card, members can only update their own cards
            if (userDto.Role != "Admin" && existingCard.UserId != userDto.Id)
            {
                return Forbid();
            }

            // Validate and update the card properties
            if (!string.IsNullOrWhiteSpace(updatedCardDTO.Name))
            {
                existingCard.Name = updatedCardDTO.Name;
            }
            else
            {
                return BadRequest("Card name is mandatory.");
            }

            if (!string.IsNullOrWhiteSpace(updatedCardDTO.Description))
            {
                existingCard.Description = updatedCardDTO.Description;
            }

            if (!string.IsNullOrWhiteSpace(updatedCardDTO.Color) && IsValidColorFormat(updatedCardDTO.Color))
            {
                existingCard.Color = updatedCardDTO.Color;
            }
            else if (!string.IsNullOrWhiteSpace(updatedCardDTO.Color))
            {
                return BadRequest("Invalid color format. It should be 6 alphanumeric characters prefixed with #.");
            }

            int statusId = await _lookUpService.GetStatusId(updatedCardDTO.Status);

            if (statusId > 0)
            {
                existingCard.StatusId = statusId;
            }

            await _dbContext.SaveChangesAsync();

            // Map entity back to DTO for response
            var responseDTO = new CardDTO(existingCard.Id)
            {
                Name = existingCard.Name,
                Description = existingCard.Description,
                Color = existingCard.Color,
                Status = await _lookUpService.GetStatusName(existingCard.StatusId),
                CreatedAt = existingCard.CreatedAt
            };

            return Ok(responseDTO);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCard(int id)
        {
            UserDTO? userDto = await IsAuthorized();

            if (userDto == null)
            {
                return Forbid();
            }

            var cardToDelete = await _dbContext.Cards
                                    .FirstOrDefaultAsync(c =>
                                        (userDto.Role == "Admin" || c.UserId == userDto.Id) && c.Id == id);

            if (cardToDelete == null)
            {
                return NotFound();
            }

            // Admins can delete any card, members can only delete their own cards
            if (userDto.Role != "Admin" && cardToDelete.UserId != userDto.Id)
            {
                return Forbid();
            }

            _dbContext.Cards.Remove(cardToDelete);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchCards(
            [FromQuery] CardSearchDTO searchDTO,
            [FromQuery] int page = 1,
            [FromQuery] int size = 10)
        {
            UserDTO? userDto = await IsAuthorized();

            if (userDto == null)
            {
                return Forbid();
            }

            // Get cards based on user access
            var query = _dbContext.Cards
                            .Include(c => c.Status)
                            .Where(c => userDto.Role == "Admin" || c.UserId == userDto.Id);

            // Apply filters
            if (!string.IsNullOrWhiteSpace(searchDTO.Name))
            {
                query = query.Where(c => c.Name.Contains(searchDTO.Name));
            }

            if (!string.IsNullOrWhiteSpace(searchDTO.Color))
            {
                query = query.Where(c => c.Color == searchDTO.Color);
            }

            if (!string.IsNullOrWhiteSpace(searchDTO.Status))
            {
                query = query.Where(c => c.Status.Name == searchDTO.Status);
            }

            if (searchDTO.CreatedAt != null)
            {
                query = query.Where(c => c.CreatedAt.Date == searchDTO.CreatedAt.Value.Date);
            }

            if (searchDTO.SortBy != null && searchDTO.SortBy.Any())
            {
                foreach (var sortBy in searchDTO.SortBy)
                {
                    switch (sortBy.ToLower())
                    {
                        case "name":
                            query = query.OrderBy(c => c.Name);
                            break;
                        case "color":
                            query = query.OrderBy(c => c.Color);
                            break;
                        case "status":
                            query = query.OrderBy(c => c.StatusId);
                            break;
                        case "createdat":
                            query = query.OrderBy(c => c.CreatedAt);
                            break;
                    }
                }
            }

            // Apply pagination
            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / size);

            if (page > totalPages)
            {
                return NotFound("Page not found");
            }

            var results = await query
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync();

            var paginatedResult = new PaginatedResult
            {
                Page = page,
                Size = size,
                TotalCount = totalCount,
                TotalPages = totalPages
            };

            List<CardDTO> cardDTOs = new List<CardDTO>();

            foreach (Card card in results)
            {
                cardDTOs.Add(new CardDTO(card.Id)
                {
                    Color = card.Color,
                    CreatedAt = card.CreatedAt,
                    Description = card.Description,
                    Name = card.Name,
                    Status = card.Status.Name
                });
            }

            paginatedResult.Data = cardDTOs;

            return Ok(paginatedResult);
        }

        #region Private Methods
        private async Task<UserDTO?> IsAuthorized()
        {
            // Extract JWT from Authorization header
            var jwt = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            // Validate and extract user info from JWT
            var principal = _jwtService.ValidateToken(jwt);

            // Get user info from claims
            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userEmail = principal.FindFirst(ClaimTypes.Email)?.Value;
            var userRole = principal.FindFirst(ClaimTypes.Role)?.Value;

            if (userId != null && userEmail != null && userRole != null)
            {
                var user = await _lookUpService.GetUser(userId);

                return user;
            }

            return null;
        }
        private bool IsValidColorFormat(string color)
        {
            if (string.IsNullOrWhiteSpace(color) || color.Length != 7 || color[0] != '#')
            {
                return false;
            }

            // Check if the remaining characters are valid hexadecimal values
            for (int i = 1; i < 7; i++)
            {
                if (!char.IsDigit(color[i]) && (color[i] < 'A' || color[i] > 'F') && (color[i] < 'a' || color[i] > 'f'))
                {
                    return false;
                }
            }

            return true;
        }

        #endregion

    }
}
