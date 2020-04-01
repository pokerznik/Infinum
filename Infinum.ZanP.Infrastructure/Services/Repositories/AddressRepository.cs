using Infinum.ZanP.Core.Models.SQL;
using Infinum.ZanP.Infrastructure.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace Infinum.ZanP.Infrastructure.Services.Repositories
{
    public class AddressRepository : Repository<Address>
    {
        public AddressRepository(ApplicationDbContext p_context) : base(p_context)
        {
        }

        public override IEnumerable<Address> Find(Expression<Func<Address, bool>> predicate)
        {
            return m_context.Set<Address>().Include(x => x.Country)
                                            .Where(predicate);
        }

        public override async Task<IEnumerable<Address>> GetAllAsync()
        {
            return await m_context.Set<Address>().Include(x => x.Country)
                                                .ToListAsync();
        }

        public override async Task<Address> SingleOrDefaultAsync(Expression<Func<Address, bool>> predicate)
        {
            return await m_context.Set<Address>().Include(x => x.Country)
                                                .SingleOrDefaultAsync(predicate);
        }

        public async override ValueTask<Address> GetByIdAsync(int id)
        {
            return await SingleOrDefaultAsync(x => x.Id == id);
        }
    }
}