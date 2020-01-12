using System;
using System.Security.Claims;
using System.Threading.Tasks;
using DatingApp.API.Data;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
namespace DatingApp.API.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
          var ResultContext= await next();
          var UserId = int.Parse(ResultContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
          var Repo=ResultContext.HttpContext.RequestServices.GetService<IDatingRepository>();
          var user= await Repo.GetUser(UserId);
          user.LastActive=DateTime.Now;
          await Repo.SaveAll();
        }
    }
}