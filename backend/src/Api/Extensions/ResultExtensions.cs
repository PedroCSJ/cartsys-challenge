using Cartsys.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace Cartsys.Api.Extensions;

public static class ResultExtensions
{
    public static IActionResult ToActionResult<T>(this Result<T> result)
    {
        if (!result.IsSuccess)
        {
            return result.StatusCode switch
            {
                404 => new NotFoundObjectResult(new { message = result.ErrorMessage }),
                409 => new ConflictObjectResult(new { message = result.ErrorMessage }),
                _ => new BadRequestObjectResult(new { message = result.ErrorMessage })
            };
        }

        return result.StatusCode == 201
            ? new ObjectResult(result.Value) { StatusCode = 201 }
            : new OkObjectResult(result.Value);
    }

    public static IActionResult ToActionResult(this Result result)
    {
        if (!result.IsSuccess)
        {
            return result.StatusCode switch
            {
                404 => new NotFoundObjectResult(new { message = result.ErrorMessage }),
                409 => new ConflictObjectResult(new { message = result.ErrorMessage }),
                _ => new BadRequestObjectResult(new { message = result.ErrorMessage })
            };
        }

        return new NoContentResult();
    }
}
