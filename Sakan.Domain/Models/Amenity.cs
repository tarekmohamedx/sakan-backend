﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Sakan.Domain.Models;

public partial class Amenity
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string IconUrl { get; set; }

    public virtual ICollection<ListingAmenities> ListingAmenities { get; set; } = new List<ListingAmenities>();
}