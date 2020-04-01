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
    public class ContactRepository : Repository<Contact>
    {
        public ContactRepository(ApplicationDbContext p_context) : base(p_context)
        {
        }

        public override IEnumerable<Contact> Find(Expression<Func<Contact, bool>> predicate)
        {
            return m_context.Set<Contact>().Include(x => x.TelephoneNumbers)
                                            .Include(x => x.Address)
                                            .ThenInclude(x => x.Country)
                                            .Where(predicate);
        }

        public override async Task<IEnumerable<Contact>> GetAllAsync()
        {
            return await m_context.Set<Contact>().Include(x => x.TelephoneNumbers)
                                                .Include(x => x.Address)
                                                .ThenInclude(x => x.Country)
                                                .ToListAsync();
        }

        public override async Task<Contact> SingleOrDefaultAsync(Expression<Func<Contact, bool>> predicate)
        {
            return await m_context.Set<Contact>().Include(x => x.TelephoneNumbers)
                                                .Include(x => x.Address)
                                                .ThenInclude(x => x.Country)
                                                .SingleOrDefaultAsync(predicate);
        }

        public async override ValueTask<Contact> GetByIdAsync(int id)
        {
            return await SingleOrDefaultAsync(x => x.Id == id);
        }
    }
}