using DataLayer.Abstract.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace DataLayer.Models.Models
{
    public class User : UpdatedEntity
    {
        [Required]
        public string Username { get; set; } = null!;

        [Required]
        public string PasswordHash { get; set; } = null!;

        [Required]
        public string Role { get; set; } = null!; // SystemAdmin, StoreAdmin, DepartmentHead, WarehouseAdmin

        public int? StoreNumber { get; set; }

        public string? DepartmentName { get; set; }

        public string? WarehouseName { get; set; }

        public Store? Store { get; set; }
        public Department? Department { get; set; }
        public Warehouse? Warehouse { get; set; }
    }
}
