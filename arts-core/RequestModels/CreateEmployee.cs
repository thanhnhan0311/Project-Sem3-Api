﻿namespace arts_core.RequestModels
{
    public class CreateEmployee
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public string FullName {  get; set; }

        public string? Phone { get; set; }

        public string? Address { get; set; }

        public IFormFile? Avatar { get; set; }
    }
}
