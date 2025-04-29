using Microsoft.AspNetCore.Mvc;

namespace GradeVault.Server.Controllers
{
    /// <summary>
    /// Base controller class for all API controllers in the GradeVault system.
    /// </summary>
    /// <remarks>
    /// This abstract class provides common functionality and configurations 
    /// for all API controllers. It inherits from ASP.NET Core's ControllerBase
    /// and applies the [ApiController] attribute for API-specific behaviors.
    /// </remarks>
    [ApiController]
    public abstract class ApiControllerBase : ControllerBase
    {
        
    }
}