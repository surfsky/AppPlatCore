using App.DAL;

using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Pages.Admin
{
    [CheckPower("CoreRoleView")]
    public class UserSelectRoleModel : BaseAdminModel
    {
        public IEnumerable<Role> Roles { get; set; }

        public string[] RoleSelectedValueArray { get; set; }

        public async Task OnGetAsync(string ids)
        {
            ids ??= "";

            RoleSelectedValueArray = ids.Split(',');

            Roles = await DB.Roles.AsNoTracking().ToListAsync();
        }
    }
}