using Infinum.ZanP.Core.Interfaces;
using Infinum.ZanP.Core.Interfaces.Repositories;
using Infinum.ZanP.Core.Models.SQL;
using System;
using System.Threading.Tasks;
using Infinum.ZanP.Infrastructure.Data;
using Infinum.ZanP.Infrastructure.Services.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace Infinum.ZanP.Infrastructure.Services
{
    public class ContactService: IContactService
    {
        private readonly IUnitOfWork m_unitOfWork;

        public ContactService(IUnitOfWork p_unitOfWork)
        {
            m_unitOfWork = p_unitOfWork;
        }

        private bool ContactExists(Contact p_contact)
        {
            IEnumerable<Contact> contactsAtAddress = m_unitOfWork.Contacts.Find(x => x.Address.Id == p_contact.Address.Id);

            if(contactsAtAddress != null && contactsAtAddress.Count() > 0)
            {
                IEnumerable<string> names = contactsAtAddress.Select(x => x.Name.ToLower());

                if(names.Contains(p_contact.Name.ToLower()))
                {
                    return true;
                }
            }
            return false;
        }

        private Address GetExistingAddress(string p_street, string p_houseNumber, string p_zip, string p_city, int p_countryId)
        {
            var existings = m_unitOfWork.Addresses.Find(x => x.Country.Id == p_countryId && x.City.ToLower() == p_city.ToLower() && x.ZIP.ToLower() == p_zip.ToLower() && x.Street.ToLower() == p_street.ToLower() && x.HouseNumber.ToLower() == p_houseNumber.ToLower());
            
            if(existings != null && existings.Count() > 0)
            {
                return existings.First();
            }

            return null;

        }

        public async Task<Contact> Create(Contact p_toCreate)
        {
            Address existingAddress =  GetExistingAddress(p_toCreate.Address.Street, p_toCreate.Address.HouseNumber, p_toCreate.Address.ZIP, p_toCreate.Address.City, p_toCreate.Address.Country.Id);
            
            var country = await m_unitOfWork.Countries.SingleOrDefaultAsync(x => x.Id == p_toCreate.Address.Country.Id);
            p_toCreate.Address.Country = country;
            
            if(existingAddress != null)
            {
                p_toCreate.Address = existingAddress;

                if(ContactExists(p_toCreate))
                {
                    throw new Exception("Contact with that name and address already exists.");
                } 
            }

            await m_unitOfWork.Contacts.AddAsync(p_toCreate);
            await m_unitOfWork.CommitAsync();
    
            return p_toCreate;     
        }

        public async Task<Contact> Update(Contact p_toUpdate)
        {
            if(p_toUpdate == null)
                throw new Exception("Contact cannot be updated because is not valid.");

            var contact = await m_unitOfWork.Contacts.GetByIdAsync(p_toUpdate.Id);
            
            contact.Name = p_toUpdate.Name;
            contact.DateOfBirth = p_toUpdate.DateOfBirth;
            await m_unitOfWork.CommitAsync();

            return p_toUpdate;
            
        }

        public async void Delete(int p_id)
        {
            var contact = await m_unitOfWork.Contacts.GetByIdAsync(p_id);

            if(contact == null)
                throw new Exception("Contact does not exist!");

            m_unitOfWork.Contacts.Remove(contact);
            await m_unitOfWork.CommitAsync();
        }

        public async Task<Contact> UpdateAddress(Contact p_toUpdate)
        {
            if(p_toUpdate.Address == null)
                throw new Exception("Address cannot be updated beacuse is not valid.");

            var contact = await m_unitOfWork.Contacts.GetByIdAsync(p_toUpdate.Id);
            contact.Address = p_toUpdate.Address;
            // we are gonna check if there is uniqueness by address and contact name provided

            if(ContactExists(contact))
                throw new Exception("Contact cannot be updated because is already in database.");
            
            await m_unitOfWork.CommitAsync();
            return p_toUpdate;
        }

        public async Task<Contact> Get(int p_contactId)
        {
            if(p_contactId <= 0)
                throw new Exception("Cannot get contact, ID is invalid.");

            var contact = await m_unitOfWork.Contacts.GetByIdAsync(p_contactId);
            if(contact == null)
                throw new Exception("Couldn't gathered the contact.");
            
            return contact;
        }

        public async Task<Contact> AddPhoneNumber(Contact p_contact, TelephoneNumber p_number)
        {
            if(p_contact == null)
                throw new Exception("Cannot add phone number, because the contact is invaild.");

            if(p_number == null)
                throw new Exception("Cannot add phone number, because the number is invalid.");

            var contact = await m_unitOfWork.Contacts.GetByIdAsync(p_contact.Id);
            List<TelephoneNumber> numbers = new List<TelephoneNumber>();

            if((contact.TelephoneNumbers != null) && (contact.TelephoneNumbers.Count() > 0))
            {
                numbers = contact.TelephoneNumbers.ToList();
            }

            numbers.Add(p_number);
            await m_unitOfWork.CommitAsync();

            return p_contact;
        }

        public async void RemovePhoneNumber(TelephoneNumber p_number)
        {
            if(p_number != null)
                throw new Exception("Cannot remove phone number, beacause is not valid.");

            m_unitOfWork.TelephoneNumbers.Remove(p_number);
        }
    }
}