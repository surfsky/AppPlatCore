using App.Models;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App
{
    [Authorize]
    public class BaseAdminModel : BaseModel
    {

    }
}
