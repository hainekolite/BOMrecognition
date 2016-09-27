using BomRainB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomRainB.Data.Repositories
{
    public class UserRepository : GenericRepository<User>
    {
        public UserRepository(BomRainBirdDbContext context) :base(context)
        {
        }

    }
}
