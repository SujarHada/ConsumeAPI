﻿namespace ConsumeWebAPI.Models
{
    public class UserEntity
    {
        public Guid Id { get; set; }
        public string fullName { get; set; } 
        public string email { get; set; }
        public long phone { get; set; }
        public string address { get; set; }

    }
}
