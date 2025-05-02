using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace YerelTurlar.Models;

public class User
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string? FirstName { get; set; }

    [Required]
    [StringLength(50)]
    public string? LastName { get; set; }

    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string? Email { get; set; }

    [Required]
    [Phone]
    public string? Phone { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string? Password { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    public bool IsActive { get; set; } = true;
    
    public bool EmailConfirmed { get; set; } = false;
    
    public bool IsAdmin { get; set; } = false;

    public virtual ICollection<Review>? Reviews { get; set; }
    public virtual ICollection<Booking>? Bookings { get; set; }
} 