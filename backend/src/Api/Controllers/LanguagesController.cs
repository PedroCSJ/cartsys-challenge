using Cartsys.Api.Extensions;
using Cartsys.Application.DTOs.Languages;
using Cartsys.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cartsys.Api.Controllers;

[ApiController]
[Route("api/languages")]
[Authorize]
public class LanguagesController(ILanguageService languageService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] LanguageFilterRequest filter)
        => (await languageService.GetAllAsync(filter)).ToActionResult();

    [HttpGet("all")]
    public async Task<IActionResult> GetAllSimple()
        => (await languageService.GetAllSimpleAsync()).ToActionResult();

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
        => (await languageService.GetByIdAsync(id)).ToActionResult();

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateLanguageRequest request)
        => (await languageService.CreateAsync(request)).ToActionResult();

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateLanguageRequest request)
        => (await languageService.UpdateAsync(id, request)).ToActionResult();

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => (await languageService.DeleteAsync(id)).ToActionResult();
}
