﻿using System;
using System.Collections.Generic;

namespace Sakan.Models;

public partial class Bed
{
    public int Id { get; set; }

    public int? RoomId { get; set; }

    public string? Label { get; set; }

    public string? Type { get; set; }

    public decimal? Price { get; set; }

    public bool? IsAvailable { get; set; }

    public string? OccupiedByUserId { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<BedPhoto> BedPhotos { get; set; } = new List<BedPhoto>();

    public virtual ICollection<BookingRequest> BookingRequests { get; set; } = new List<BookingRequest>();

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual AspNetUser? OccupiedByUser { get; set; }

    public virtual Room? Room { get; set; }
}
