﻿namespace Project_1.Models
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string? Normalizedname { get; set; }
        public List<UserRole> UserRoles { get; set; }
        public List<RolePermission> RolePermissions { get; set; }
    }
}
