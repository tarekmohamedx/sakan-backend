﻿using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Sakan.Application.DTOs;
using Sakan.Application.DTOs.Host;
using Sakan.Application.DTOs.User;
using Sakan.Application.Interfaces;
using Sakan.Application.Interfaces.User;
using Sakan.Domain.Interfaces;
using Sakan.Domain.Models;
using Sakan.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sakan.Infrastructure.Services
{
    // BookingRequestService.cs
    public class BookingRequestService : IBookingRequestService
    {
        private readonly sakanContext _context;
        private readonly IBookingRequestRepository bookingRequestRepository;

        public BookingRequestService(sakanContext context, IBookingRequestRepository bookingRequestRepository)
        {
            _context = context;
            this.bookingRequestRepository = bookingRequestRepository;
        }

        public async Task<(int requestId, string hostId)> CreateAsync(BookingRequestsDto dto)
        {
            int firstRequestId = 0;

            // ✅ Get hostId first
            var hostId = await _context.Listings
                .Where(l => l.Id == dto.ListingId)
                .Select(l => l.HostId)
                .FirstOrDefaultAsync();

            // ✅ Check if guest is the host
            if (hostId == dto.GuestId)
            {
                throw new InvalidOperationException("You cannot book your own apartment.");
            }

            // Check for duplicate request
            bool isDuplicate = await _context.BookingRequests.AnyAsync(r =>
                r.GuestId == dto.GuestId &&
                r.FromDate == dto.FromDate &&
                r.ToDate == dto.ToDate &&
                (
                    (dto.BedIds == null || !dto.BedIds.Any()) // whole room booking
                        ? (r.RoomId == dto.RoomId && r.BedId == null && r.ListingId == dto.ListingId)
                        : (dto.BedIds.Contains(r.BedId ?? 0) && r.RoomId == dto.RoomId && r.ListingId == dto.ListingId)
                )
            );

            if (isDuplicate)
            {
                throw new InvalidOperationException("You have already sent this booking request.");
            }

            // Create booking(s)
            if (dto.BedIds == null || !dto.BedIds.Any())
            {
                var booking = new BookingRequest
                {
                    GuestId = dto.GuestId,
                    ListingId = dto.ListingId ?? 0,
                    RoomId = dto.RoomId,
                    BedId = null,
                    FromDate = dto.FromDate,
                    ToDate = dto.ToDate,
                    HostApproved = null,
                    GuestApproved = null,
                    CreatedAt = DateAndTime.Now
                };

                _context.BookingRequests.Add(booking);
                await _context.SaveChangesAsync();
                firstRequestId = booking.Id;
            }
            else
            {
                foreach (var bedId in dto.BedIds)
                {
                    var booking = new BookingRequest
                    {
                        GuestId = dto.GuestId,
                        ListingId = dto.ListingId ?? 0,
                        RoomId = dto.RoomId,
                        BedId = bedId,
                        FromDate = dto.FromDate,
                        ToDate = dto.ToDate,
                        HostApproved = null,
                        GuestApproved = null,
                        CreatedAt = dto.CreatedAt
                    };

                    _context.BookingRequests.Add(booking);
                    await _context.SaveChangesAsync();

                    if (firstRequestId == 0)
                        firstRequestId = booking.Id;
                }
            }

            return (firstRequestId, hostId);
        }


        //use the BookingRequestsDTO then return all the booking requests for the user with the given userId
        public async Task<IEnumerable<BookingRequestsDTO>> GetBookingRequestsByUserIdAsync(string userId)
        {
            var requests = await _context.BookingRequests
                .Where(br => br.GuestId == userId)
                .Select(br => new BookingRequestsDTO
                {
                    GuestId = br.GuestId,
                    HostId = _context.Listings
                        .Where(l => l.Id == br.ListingId)
                        .Select(l => l.HostId)
                        .FirstOrDefault(),
                    BookingRequestId = br.Id,
                    ListingTitle = _context.Listings
                        .Where(l => l.Id == br.ListingId)
                        .Select(l => l.Title)
                        .FirstOrDefault(),
                    BedPrice = _context.Beds
                        .Where(b => b.Id == br.BedId)
                        .Select(b => b.Price)
                        .FirstOrDefault(),
                    ListingLocation = _context.Listings
                        .Where(l => l.Id == br.ListingId)
                        .Select(l => l.Governorate + " - " + l.District)
                        .FirstOrDefault(),
                    FromDate = (DateTime)br.FromDate,
                    ToDate = (DateTime)br.ToDate,
                    Status = br.IsActive == true ? "Accepted" :
                             br.IsActive == false ? "Rejected" :
                             "Pending"
                })
                .ToListAsync();

            return requests;
        }

        public async Task<bool> UpdateBookingRequestAsync(int requestId, bool isAccepted)
        {
            var bookingRequest = await _context.BookingRequests.FindAsync(requestId);
            if (bookingRequest == null) return false;
            bookingRequest.HostApproved = isAccepted;
            if (bookingRequest.GuestApproved == true && isAccepted)
                bookingRequest.IsActive = true;
            _context.BookingRequests.Update(bookingRequest);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<HostBookingRequestDTO>> GetBookingRequestsByHostIdAsync(string hostId)
        {
            var requests = await _context.BookingRequests
                .Where(br => _context.Listings.Any(l => l.Id == br.ListingId && l.HostId == hostId))
                .Select(br => new HostBookingRequestDTO
                {
                    BookingRequestId = br.Id,
                    GuestId = br.GuestId,
                   
                    GuestName = _context.Users.Where(u => u.Id == br.GuestId).Select(u => u.UserName).FirstOrDefault(),
                    ListingTitle = _context.Listings.Where(l => l.Id == br.ListingId).Select(l => l.Title).FirstOrDefault(),
                    RoomTitle = _context.Rooms.Where(r => r.Id == br.RoomId).Select(r => r.Name).FirstOrDefault(),
                    BedTitle = _context.Beds.Where(b => b.Id == br.BedId).Select(b => b.Label).FirstOrDefault(),
                    ListingLocation = _context.Listings.Where(l => l.Id == br.ListingId)
                        .Select(l => l.Governorate + " - " + l.District).FirstOrDefault(),
                    FromDate = (DateTime)br.FromDate,
                    ToDate = (DateTime)br.ToDate,
                    CreatedAt = (DateTime)br.CreatedAt,
                    IsApproved = br.HostApproved == true ? "Accepted" :
                             br.HostApproved == false ? "Rejected" : 

                             "Pending"

                }).OrderByDescending(s => s.CreatedAt)
                .ToListAsync();

            return requests;
        }

        public async Task<ChatWithHostDTO> GetLatestBookingRequestAsync(int listingId, string guestId)
        {
            var latestRequest = await _context.BookingRequests
       .Where(br => br.ListingId == listingId && br.GuestId == guestId)
       .OrderByDescending(br => br.FromDate)
       .Select(br => new ChatWithHostDTO
       {
           ListingId = br.ListingId,
           ListingTitle = br.Listing.Title ?? "Unknown",
           HostId = br.Listing.HostId,
           HostName = br.Listing.Host.UserName ?? "Unknown",
           GuestId = br.GuestId,
           GuestName = br.Guest.UserName ?? "Unknown"
       })
       .FirstOrDefaultAsync();

            return latestRequest;
        }

        private decimal CalculatePrice(BookingRequest request)
        {
            // 1. إذا كان السرير محدداً
            if (request.BedId.HasValue && request.Bed != null)
            {
                return request.Bed.Price ?? 0;
            }
            // 2. إذا كانت الغرفة محددة (بدون سرير)
            else if (request.RoomId.HasValue && request.Room != null)
            {
                // نفترض أن السعر هنا لليلة الواحدة
                var nights = (request.ToDate.Value - request.FromDate.Value).Days;
                return (request.Room.PricePerNight ?? 0) * nights;
            }
            // 3. إذا كانت الشقة كاملة محددة
            else if (request.ListingId.HasValue && request.Listing != null)
            {
                return request.Listing.PricePerMonth ?? 0; // أو أي منطق آخر للشقة كاملة
            }

            throw new InvalidOperationException("Could not determine the price for the booking request.");
        }

        public async Task<BookingRQSDTO> getbookingrequestbyid(int id)
        {
            var entity = await bookingRequestRepository.GetByIdAsync(id);
            if (entity == null) return null;

            return new BookingRQSDTO
            {
                Id = entity.Id,
                GuestId = entity.GuestId,
                ListingId = entity.ListingId,
                RoomId = entity.RoomId,
                BedId = entity.BedId,
                FromDate = entity.FromDate,
                ToDate = entity.ToDate,
                IsActive = entity.IsActive,
            };
        }
    }

}
