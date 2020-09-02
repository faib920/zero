using Fireasy.Zero.Models;
using Fireasy.Zero.Services.Impls;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace Fireasy.Zero.AspNetCore.Controllers
{
    [AllowAnonymous]
    public class UsersController : ODataController
    {
        private DbContext _context;

        public UsersController(DbContext context)
        {
            _context = context;
        }

        [EnableQuery(AllowedQueryOptions = AllowedQueryOptions.All)]
        public IQueryable<SysUser> Get(ODataQueryOptions options)
        {
            return _context.SysUsers;
        }
    }
}
