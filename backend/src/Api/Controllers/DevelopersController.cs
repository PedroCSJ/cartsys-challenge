using Cartsys.Api.Extensions;
using Cartsys.Api.Reports;
using Cartsys.Application.DTOs.Developers;
using Cartsys.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cartsys.Api.Controllers;

[ApiController]
[Route("api/developers")]
[Authorize]
public class DevelopersController(IDeveloperService developerService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] DeveloperFilterRequest filter)
        => (await developerService.GetAllAsync(filter)).ToActionResult();

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
        => (await developerService.GetByIdAsync(id)).ToActionResult();

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDeveloperRequest request)
        => (await developerService.CreateAsync(request)).ToActionResult();

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDeveloperRequest request)
        => (await developerService.UpdateAsync(id, request)).ToActionResult();

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => (await developerService.DeleteAsync(id)).ToActionResult();

    [HttpGet("report/pdf")]
    public async Task<IActionResult> GeneratePdf()
    {
        var result = await developerService.GetAllForReportAsync();
        if (!result.IsSuccess)
            return BadRequest(new { message = result.ErrorMessage });

        var pdf = DeveloperReportService.Generate(result.Value!);
        return File(pdf, "application/pdf", $"relatorio-desenvolvedores-{DateTime.Now:yyyyMMdd}.pdf");
    }
}
