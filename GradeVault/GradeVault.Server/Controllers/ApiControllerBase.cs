using Microsoft.AspNetCore.Mvc;

namespace GradeVault.Server.Controllers
{
    [ApiController]
    [AutoValidateAntiforgeryToken]
    public abstract class ApiControllerBase : ControllerBase
    {
        
    }
}
