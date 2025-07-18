﻿using Microsoft.AspNetCore.Mvc;
using Sakan.Infrastructure.Services; // make sure this matches your namespace
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sakan.Application.Interfaces.User;
using Sakan.Application.DTOs.User;
using Sakan.Infrastructure.Services.User;

namespace Sakan.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListingDetailsController : ControllerBase
    {
        private readonly IListingDetailsService _listingService;
        private readonly IBookingRequestService _bookingService;

        public ListingDetailsController(IListingDetailsService listingService, IBookingRequestService bookingService)
        {
            _listingService = listingService;
            _bookingService = bookingService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetListingById(int id)
        {
            var listing = await _listingService.GetListingDetails(id);
            if (listing == null) return NotFound();
            return Ok(listing);
        }


        [HttpGet("booked-months/{listingId}")]
        public async Task<IActionResult> GetBookedMonths(int listingId)
        {
            var result = await _listingService.GetBookedMonthsAsync(listingId);
            return Ok(result);
        }

        [HttpPost("request")]
        public async Task<IActionResult> CreateBookingRequest([FromBody] BookingRequestsDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var result = await _bookingService.CreateAsync(dto);
                return Ok(new { result.requestId, result.hostId });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("listing-reviews/{listingId}")]
        public async Task<IActionResult> GetListingReviews(int listingId)
        {
            var reviews = await _listingService.GetReviewsForListingAsync(listingId);
            return Ok(reviews);
        }



        //[HttpGet("{listingId}/amenities")]
        //public async Task<IActionResult> GetAmenities(int listingId)
        //{
        //    var amenities = await _listingService.GetAmenitiesForListingAsync(listingId);
        //    return Ok(amenities);
        //}


        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetListingById(int id, [FromQuery] string lang = "en")
        //{
        //    var listing = await _listingService.GetListingDetails(id);
        //    if (listing == null) return NotFound();

        //    var translator = new TranslationService();

        //    listing.Title = await translator.TranslateTextAsync(listing.Title, lang);
        //    listing.Description = await translator.TranslateTextAsync(listing.Description, lang);
        //    listing.Location = await translator.TranslateTextAsync(listing.Location, lang);

        //    // Translate bedrooms
        //    if (listing.BedroomList != null)
        //    {
        //        foreach (var room in listing.BedroomList)
        //        {
        //            room.Name = await translator.TranslateTextAsync(room.Name, lang);
        //        }
        //    }

        //    // Translate host fields if needed
        //    if (listing.Host?.Languages != null)
        //    {
        //        for (int i = 0; i < listing.Host.Languages.Count; i++)
        //        {
        //            listing.Host.Languages[i] = await translator.TranslateTextAsync(listing.Host.Languages[i], lang);
        //        }
        //    }


        //    return Ok(listing);
        //}

        //[HttpGet("translate")]
        //public async Task<IActionResult> TranslateSample(string text, string lang = "ar")
        //{
        //    var service = new TranslationService();
        //    var translated = await service.TranslateTextAsync(text, lang);
        //    return Ok(new { Original = text, Translated = translated });
        //}

    }
}
