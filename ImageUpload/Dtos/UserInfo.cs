﻿namespace ImageUpload.Dtos
{
  
    public sealed class UserInfo
    {
        public required string Username { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public List<string> GrantedPermissions { get; set; } = [];
    }
}
