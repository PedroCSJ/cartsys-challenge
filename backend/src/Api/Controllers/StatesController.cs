using Cartsys.Api.Extensions;
using Cartsys.Application.DTOs.States;
using Cartsys.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cartsys.Api.Controllers;

[ApiController]
[Route("api/states")]
[Authorize]
public class StatesController(IStateService stateService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] StateFilterRequest filter)
        => (await stateService.GetAllAsync(filter)).ToActionResult();

    [HttpGet("all")]
    public async Task<IActionResult> GetAllSimple()
        => (await stateService.GetAllSimpleAsync()).ToActionResult();

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
        => (await stateService.GetByIdAsync(id)).ToActionResult();

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateStateRequest request)
        => (await stateService.CreateAsync(request)).ToActionResult();

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateStateRequest request)
        => (await stateService.UpdateAsync(id, request)).ToActionResult();

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => (await stateService.DeleteAsync(id)).ToActionResult();
}
