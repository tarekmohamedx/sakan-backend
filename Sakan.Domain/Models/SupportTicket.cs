﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Sakan.Domain.Models;

public partial class SupportTicket
{
    public int Id { get; set; }

    public string UserId { get; set; }

    public string GuestName { get; set; }

    public string GuestEmail { get; set; }

    public string Subject { get; set; }

    public string Description { get; set; }

    public string Category { get; set; }

    public string Status { get; set; }

    public string Priority { get; set; }

    public int? BookingId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? LastUpdatedAt { get; set; }

    public virtual Booking Booking { get; set; }

    public virtual ICollection<TicketReply> TicketReplies { get; set; } = new List<TicketReply>();

    public virtual ApplicationUser User { get; set; }
    public string? GuestAccessToken { get; set; }
}