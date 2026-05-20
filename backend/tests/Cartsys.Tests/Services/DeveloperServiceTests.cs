using Cartsys.Application.DTOs.Developers;
using Cartsys.Application.Interfaces;
using Cartsys.Application.Services;
using Cartsys.Domain.Entities;
using Cartsys.Domain.Enums;
using Cartsys.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace Cartsys.Tests.Services;

public class DeveloperServiceTests
{
    private readonly Mock<IDeveloperRepository> _developerRepoMock = new();
    private readonly Mock<IRepository<ProgrammingLanguage>> _languageRepoMock = new();
    private readonly Mock<IRepository<City>> _cityRepoMock = new();
    private readonly DeveloperService _service;

    public DeveloperServiceTests()
    {
        _service = new DeveloperService(
            _developerRepoMock.Object,
            _languageRepoMock.Object,
            _cityRepoMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldFail_WhenNoLanguagesProvided()
    {
        var request = new CreateDeveloperRequest(
            "Pedro", "pedro@test.com", Seniority.Pleno,
            Guid.NewGuid(), [], null);

        var result = await _service.CreateAsync(request);

        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Contain("ao menos uma linguagem");
    }

    [Fact]
    public async Task CreateAsync_ShouldFail_WhenEmailAlreadyExists()
    {
        var cityId = Guid.NewGuid();
        var langId = Guid.NewGuid();

        _developerRepoMock
            .Setup(r => r.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Developer, bool>>>()))
            .ReturnsAsync(true);

        var request = new CreateDeveloperRequest(
            "Pedro", "pedro@test.com", Seniority.Pleno,
            cityId, [langId], null);

        var result = await _service.CreateAsync(request);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(409);
        result.ErrorMessage.Should().Contain("e-mail");
    }

    [Fact]
    public async Task CreateAsync_ShouldFail_WhenCityNotFound()
    {
        var cityId = Guid.NewGuid();
        var langId = Guid.NewGuid();

        _developerRepoMock
            .Setup(r => r.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Developer, bool>>>()))
            .ReturnsAsync(false);

        _cityRepoMock
            .Setup(r => r.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<City, bool>>>()))
            .ReturnsAsync(false);

        var request = new CreateDeveloperRequest(
            "Pedro", "pedro@test.com", Seniority.Pleno,
            cityId, [langId], null);

        var result = await _service.CreateAsync(request);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNotFound_WhenDeveloperDoesNotExist()
    {
        _developerRepoMock
            .Setup(r => r.GetByIdWithRelationsAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Developer?)null);

        var result = await _service.GetByIdAsync(Guid.NewGuid());

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnNotFound_WhenDeveloperDoesNotExist()
    {
        _developerRepoMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Developer?)null);

        var result = await _service.DeleteAsync(Guid.NewGuid());

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(404);
    }
}
