﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sakan.Application.DTOs.User;
using Sakan.Application.Interfaces.User;
using System.Security.Claims;

namespace Sakan.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListingController : ControllerBase
    {
        private readonly IListingService _listingService;

        public ListingController(IListingService listingService)
        {
            _listingService = listingService;
        }

        [HttpPost("{hostId}")]
        public async Task<IActionResult> CreateListing([FromForm] CreateListingDTO dto , [FromRoute]string hostId)
        {
            try
            {
                //var hostId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _listingService.CreateListingAsync(dto, hostId);
                return Ok(new { message = "Listing created successfully." });
            }
            catch (Exception ex)
            {
                // Log the error if needed
                return BadRequest(new
                {
                    error = true,
                    message = ex.Message
                });
            }
        }

    }
}
