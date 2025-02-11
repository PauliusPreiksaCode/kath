using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using organization_back_end.Auth.Model;
using organization_back_end.Enums;
using organization_back_end.Helpers;
using organization_back_end.RequestDtos.Licences;
using organization_back_end.Services;
using Stripe.Checkout;

namespace organization_back_end.Controllers;

[Route("checkout")]
public class CheckoutController : ControllerBase
{
    private readonly LicenceService _licenceService;
    private readonly UserManager<User> _userManager;
    
    
    public CheckoutController(LicenceService licenceService, UserManager<User> userManager)
    {
        _licenceService = licenceService;
        _userManager = userManager;
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CheckoutOrder([FromBody] CheckoutOrderRequest request)
    {
        var licence = await _licenceService.GetLicenceById(request.licenceId);
        if (licence is null)
            return NotFound();

        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user is null)
            return NotFound();
        
        var options = new SessionCreateOptions
        {
            SuccessUrl = "https://www.google.com/",
            CancelUrl = "https://www.youtube.com/",
            PaymentMethodTypes = new List<string>
            {
                "card",
            },
            CustomerEmail = user.Email,
            LineItems = new List<SessionLineItemOptions>
            {
                new()
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmountDecimal = licence.Price * 100,
                        Currency = "EUR",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = licence.Name,
                        },
                    },
                    Quantity = 1,
                }
            },
            Mode = "payment"
        };
        
        var service = new SessionService();
        var session = await service.CreateAsync(options);
        var ledgerEntry = await _licenceService.CreateInitialLicenceLedgerEntry(licence, request.UserId);
        
        return Ok(new { sessionId = session.Id, redirectUrl = session.Url, ledgerEntry = ledgerEntry });
    }
    
    [Authorize]
    [HttpPost("payment-status")]
    public async Task<IActionResult> UpdatePaymentStatus([FromBody] UpdatePaymentStatusRequest request)
    {
        var service = new SessionService();
        var session = await service.GetAsync(request.SessionId);

        if (session.PaymentStatus == "paid")
        {
            await _licenceService.UpdateLicenceLedgerEntry(request.LedgerId, LicencePaymentStatus.Paid, session);
            return Ok();
        }
        else
        {
            await _licenceService.UpdateLicenceLedgerEntry(request.LedgerId, LicencePaymentStatus.Unpaid, session);
            return BadRequest();
        }
    }
}