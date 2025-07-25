﻿using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sakan.Application.Interfaces.User;

namespace Sakan.Controllers.User
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] 
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

     //   [Authorize]
        [HttpPost("create-intent")]
        public async Task<IActionResult> CreatePaymentIntent([FromBody] CreatePaymentIntentRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //var userId = "8569ce99-18f5-49a1-88e8-1549e42c0d83";
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            try
            {
                var clientSecret = await _paymentService.CreatePaymentIntentAsync(request.BookingRequestId, userId);
                return Ok(new { clientSecret });
            }
            catch (Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }
    }

    public class CreatePaymentIntentRequest
    {
        public int BookingRequestId { get; set; }
    }
}
