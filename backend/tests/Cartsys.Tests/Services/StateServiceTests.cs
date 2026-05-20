using Cartsys.Application.DTOs.States;
using Cartsys.Application.Services;
using Cartsys.Domain.Entities;
using Cartsys.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace Cartsys.Tests.Services;

public class StateServiceTests
{
    private readonly Mock<IRepository<State>> _repoMock = new();
    private readonly StateService _service;

    public StateServiceTests()
    {
        _service = new StateService(_repoMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldFail_WhenUFAlreadyExists()
    {
        _repoMock
            .Setup(r => r.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<State, bool>>>()))
            .ReturnsAsync(true);

        var result = await _service.CreateAsync(new CreateStateRequest("Minas Gerais", "MG"));

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(409);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNotFound_WhenStateDoesNotExist()
    {
        _repoMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((State?)null);

        var result = await _service.GetByIdAsync(Guid.NewGuid());

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task CreateAsync_ShouldConvertUFToUpperCase()
    {
        _repoMock
            .Setup(r => r.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<State, bool>>>()))
            .ReturnsAsync(false);

        _repoMock
            .Setup(r => r.AddAsync(It.IsAny<State>()))
            .Returns(Task.CompletedTask)
            .Callback<State>(s => s.Id = Guid.NewGuid());

        _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await _service.CreateAsync(new CreateStateRequest("Minas Gerais", "mg"));

        result.IsSuccess.Should().BeTrue();
        result.Value!.UF.Should().Be("MG");
    }
}
