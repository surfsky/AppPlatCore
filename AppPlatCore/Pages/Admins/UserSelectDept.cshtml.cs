using App.DAL;
using App.Components;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Pages.Admin
{
    [CheckPower("CoreDeptView")]
    public class UserSelectDeptModel : BaseAdminModel
    {
        public IEnumerable<Dept> Depts { get; set; }

        public string DeptSelectedRowID { get; set; }

        public async Task OnGetAsync(string ids)
        {
            ids ??= "";

            DeptSelectedRowID = ids;

            Depts = await DB.Depts.AsNoTracking().ToListAsync();
        }
    }
}