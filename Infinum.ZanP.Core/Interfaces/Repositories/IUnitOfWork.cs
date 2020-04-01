using System;
using System.Threading.Tasks;
using Infinum.ZanP.Core.Models.SQL;

namespace Infinum.ZanP.Core.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Address> Addresses { get; }
        IRepository<Contact> Contacts { get; }
        IRepository<Country> Countries { get; }
        IRepository<TelephoneNumber> TelephoneNumbers { get; }

        Task<int> CommitAsync();
    }
}