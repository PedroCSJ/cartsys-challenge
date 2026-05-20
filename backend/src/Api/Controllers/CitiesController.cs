using Cartsys.Api.Extensions;
using Cartsys.Application.DTOs.Cities;
using Cartsys.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cartsys.Api.Controllers;

[ApiController]
[Route("api/cities")]
[Authorize]
public class CitiesController(ICityService cityService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] CityFilterRequest filter)
        => (await cityService.GetAllAsync(filter)).ToActionResult();

    [HttpGet("by-state/{stateId:guid}")]
    public async Task<IActionResult> GetByState(Guid stateId)
        => (await cityService.GetByStateAsync(stateId)).ToActionResult();

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
        => (await cityService.GetByIdAsync(id)).ToActionResult();

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCityRequest request)
        => (await cityService.CreateAsync(request)).ToActionResult();

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCityRequest request)
        => (await cityService.UpdateAsync(id, request)).ToActionResult();

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => (await cityService.DeleteAsync(id)).ToActionResult();
}
