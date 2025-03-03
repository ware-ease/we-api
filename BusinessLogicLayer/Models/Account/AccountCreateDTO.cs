using BusinessLogicLayer.Models.Profile;

namespace BusinessLogicLayer.Models.Account
{
    public class AccountCreateDTO
    {
        public string Username { get; set; } = string.Empty;
        //public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? CreatedBy { get; set; }
        public ProfileCreateDTO Profile { get; set; }
        public List<string>? groupIds { get; set; }

    }
}
