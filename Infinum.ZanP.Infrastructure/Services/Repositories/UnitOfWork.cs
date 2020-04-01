using Infinum.ZanP.Core.Interfaces.Repositories;
using Infinum.ZanP.Infrastructure.Data;
using Infinum.ZanP.Core.Models.SQL;
using System.Threading.Tasks;
using System;

namespace Infinum.ZanP.Infrastructure.Services.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext m_context;
        
        public UnitOfWork(ApplicationDbContext p_context)
        {
            m_context = p_context;
        }

        public IRepository<Address> Addresses => new AddressRepository(m_context);
        public IRepository<Contact> Contacts => new ContactRepository(m_context);
        public IRepository<Country> Countries => new Repository<Country>(m_context);
        public IRepository<TelephoneNumber> TelephoneNumbers => new Repository<TelephoneNumber>(m_context);
        
        public async Task<int> CommitAsync()
        {
            try{
                return await m_context.SaveChangesAsync();
            }
            catch(Exception e)
            {
                if(e.InnerException != null)
                    throw e.InnerException;
                else
                    throw e;
            }
        }

        public void Dispose()
        {
            m_context.Dispose();
        }
    }
}