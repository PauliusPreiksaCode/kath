using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using organization_back_end.Helpers;
using organization_back_end.RequestDtos.Licences;
using organization_back_end.Services;

namespace organization_back_end.Controllers;

[Route("licences")]
public class LicenceController : ControllerBase
{
    private readonly LicenceService _licenceService;

    public LicenceController(LicenceService licenceService)
    {
        _licenceService = licenceService;
    }

    [HttpGet]
    [Authorize]
    [Route("licences")]
    public async Task<IActionResult> GetAllLicences()
    {
        var licences = await _licenceService.GetAllLicences();
        return Ok(licences);
    }
    
    [HttpPost]
    [Authorize]
    [Route("generate")]
    public async Task<IActionResult> GenerateLicence([FromBody] CreateLicenceRequest request)
    {
        var errorResult = CheckErrors();
        if (errorResult != null)
            return errorResult;
        
        await _licenceService.GenerateLicense(request);
        return Ok();
    }
    
    [HttpGet]
    [Authorize]
    [Route("licenceLedgerEntries")]
    public async Task<IActionResult> GetLicenceLedgerEntries()
    {
        var errorResult = CheckErrors();
        if (errorResult != null)
            return errorResult;
        
        var userId = User.GetUserId();
       
        var licenceLedgerEntries = await _licenceService.GetLicenceLedgerEntries(userId);
        return Ok(licenceLedgerEntries);
    }
    
    [HttpDelete]
    [Authorize]
    [Route("remove")]
    public async Task<IActionResult> RemoveLicence([FromBody] RemoveLicenceRequest request)
    {
        try
        {
            var userId = User.GetUserId();

            var errorResult = CheckErrors();
            if (errorResult != null)
                return errorResult;
        
            await _licenceService.RemoveLicence(request.LicenceLedgerId, userId);
            return Ok();
        }
        catch (Exception e)
        {
            return StatusCode(400, "Cannot remove licence");
        }
    }
    
    [HttpPost]
    [Authorize]
    [Route("transfer")]
    public async Task<IActionResult> TransferLicence([FromBody] TransferLicenceRequest request)
    {
        try
        {
            var userId = User.GetUserId();
        
            var errorResult = CheckErrors();
            if (errorResult != null)
                return errorResult;
        
            await _licenceService.TransferLicence(userId, request.NewUserId, request.LedgerEntryId);
            return Ok();
        }
        catch (Exception e)
        {
            return StatusCode(400, "Cannot transfer licence");
        }

    }
    
    private ObjectResult? CheckErrors()
    {
        if (!ModelState.IsValid){
            var errors = ModelState.Values.SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            var wrongJson = errors.Any(e => e.Contains("JSON"));
            return wrongJson ? StatusCode(400, "Wrong JSON format") : StatusCode(422, new { errors });
        }

        return null;
    }
    
}