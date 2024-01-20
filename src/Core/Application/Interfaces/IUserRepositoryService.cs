﻿namespace Application.Interfaces
{
    public interface IUserRepositoryService
    {
        public string Username { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
}
