using Cartsys.Domain.Entities;
using Cartsys.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Cartsys.Infrastructure.Data.Seed;

public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        await context.Database.MigrateAsync();

        if (!await context.Users.AnyAsync())
            await SeedUsersAsync(context);

        if (!await context.States.AnyAsync())
            await SeedStatesAndCitiesAsync(context);

        if (!await context.ProgrammingLanguages.AnyAsync())
            await SeedLanguagesAndDevelopersAsync(context);
    }

    private static async Task SeedUsersAsync(AppDbContext context)
    {
        var admin = new User
        {
            Name = "Administrador",
            Email = "admin@cartsys.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123")
        };

        await context.Users.AddAsync(admin);
        await context.SaveChangesAsync();
    }

    private static async Task SeedStatesAndCitiesAsync(AppDbContext context)
    {
        var states = new List<State>
        {
            new() { Name = "Minas Gerais", UF = "MG" },
            new() { Name = "São Paulo", UF = "SP" },
            new() { Name = "Rio de Janeiro", UF = "RJ" },
            new() { Name = "Bahia", UF = "BA" },
            new() { Name = "Rio Grande do Sul", UF = "RS" }
        };

        await context.States.AddRangeAsync(states);
        await context.SaveChangesAsync();

        var mg = states[0];
        var sp = states[1];
        var rj = states[2];

        var cities = new List<City>
        {
            new() { Name = "Belo Horizonte", StateId = mg.Id },
            new() { Name = "Uberlândia", StateId = mg.Id },
            new() { Name = "Contagem", StateId = mg.Id },
            new() { Name = "São Paulo", StateId = sp.Id },
            new() { Name = "Campinas", StateId = sp.Id },
            new() { Name = "Guarulhos", StateId = sp.Id },
            new() { Name = "Rio de Janeiro", StateId = rj.Id },
            new() { Name = "Niterói", StateId = rj.Id }
        };

        await context.Cities.AddRangeAsync(cities);
        await context.SaveChangesAsync();
    }

    private static async Task SeedLanguagesAndDevelopersAsync(AppDbContext context)
    {
        var languages = new List<ProgrammingLanguage>
        {
            new() { Name = "C#", Type = LanguageType.Backend },
            new() { Name = "TypeScript", Type = LanguageType.Frontend },
            new() { Name = "React", Type = LanguageType.Frontend },
            new() { Name = "Python", Type = LanguageType.Backend },
            new() { Name = "SQL Server", Type = LanguageType.Database },
            new() { Name = "PostgreSQL", Type = LanguageType.Database },
            new() { Name = "Docker", Type = LanguageType.DevOps },
            new() { Name = "Flutter", Type = LanguageType.Mobile },
            new() { Name = "Node.js", Type = LanguageType.Backend },
            new() { Name = "Vue.js", Type = LanguageType.Frontend }
        };

        await context.ProgrammingLanguages.AddRangeAsync(languages);
        await context.SaveChangesAsync();

        var bh = await context.Cities.FirstAsync(c => c.Name == "Belo Horizonte");
        var sp = await context.Cities.FirstAsync(c => c.Name == "São Paulo");

        var developers = new List<Developer>
        {
            new()
            {
                Name = "Carlos Eduardo Lima",
                Email = "carlos.lima@email.com",
                Seniority = Seniority.Senior,
                CityId = bh.Id,
                Notes = "Especialista em arquitetura de sistemas.",
                Languages = [languages[0], languages[4]]
            },
            new()
            {
                Name = "Ana Paula Ferreira",
                Email = "ana.ferreira@email.com",
                Seniority = Seniority.Pleno,
                CityId = sp.Id,
                Notes = "Foco em desenvolvimento frontend.",
                Languages = [languages[1], languages[2], languages[9]]
            },
            new()
            {
                Name = "Lucas Mendes Santos",
                Email = "lucas.santos@email.com",
                Seniority = Seniority.Junior,
                CityId = bh.Id,
                Languages = [languages[0], languages[1], languages[2]]
            }
        };

        await context.Developers.AddRangeAsync(developers);
        await context.SaveChangesAsync();
    }
}
