using Infinum.ZanP.Core.Models.SQL;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Infinum.ZanP.Core.Interfaces
{
    public interface IContactService
    {
        Task<Contact> Get(int p_contactId);
        Task<Contact> Create(Contact p_toCreate);
        Task<Contact> Update(Contact p_toUpdate);
        Task Delete(int p_id);
        Task<Contact> UpdateAddress(Contact p_toUpdate);
        Task<Contact> AddPhoneNumber(Contact p_contact, TelephoneNumber p_number);
        void RemovePhoneNumber(TelephoneNumber p_number);
    }
}