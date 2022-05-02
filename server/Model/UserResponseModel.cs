using System;

namespace server.Model
{
    public class UserResponseModel
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
}
