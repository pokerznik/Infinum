using Infinum.ZanP.Core.Interfaces;
using Infinum.ZanP.Core.Interfaces.Repositories;
using Infinum.ZanP.Core.Models.SQL;
using System;
using System.Threading.Tasks;
using Infinum.ZanP.Infrastructure.Data;
using Infinum.ZanP.Infrastructure.Services.Repositories;
using System.Collections.Generic;
using System.Linq;
using Infinum.ZanP.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Infinum.ZanP.Infrastructure.Services
{
    public class ContactService: IContactService
    {
        private readonly IUnitOfWork m_unitOfWork;
        private readonly ILiveUpdateService m_liveUpdateService;

        public ContactService(IUnitOfWork p_unitOfWork, ILiveUpdateService p_liveUpdateService)
        {
            m_unitOfWork = p_unitOfWork;
            m_liveUpdateService = p_liveUpdateService;
        }

        private bool ContactExists(Contact p_contact, bool isUpdating=false)
        {
            IEnumerable<Contact> contactsAtAddress = m_unitOfWork.Contacts.Find(x => x.Address.Id == p_contact.Address.Id);

            if(contactsAtAddress != null && contactsAtAddress.Count() > 0)
            {
                List<string> names = contactsAtAddress.Select(x => x.Name.ToLower()).ToList();

                if(names.Contains(p_contact.Name.ToLower()))
                {
                    if(isUpdating)
                    {
                        if(names.Where(x => x == p_contact.Name.ToLower()).Count() > 1)
                        {
                            return true;
                        }
                        return false;
                    }

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

            // send live update
            await m_liveUpdateService.ContactUpdated(p_toCreate);

            return p_toCreate;     
        }

        public async Task<Contact> Update(Contact p_toUpdate)
        {
            if(p_toUpdate == null)
                throw new Exception("Contact cannot be updated because is not valid.");

            var contact = await m_unitOfWork.Contacts.GetByIdAsync(p_toUpdate.Id);

            if(contact != null)
            {
                bool hasNameChanged = !(contact.Name == p_toUpdate.Name);

                contact.Name = p_toUpdate.Name;
                contact.DateOfBirth = p_toUpdate.DateOfBirth;

                Address originalAddress = await m_unitOfWork.Addresses.GetByIdAsync(contact.Address.Id);
                bool hasAddressChanged = !(originalAddress.Equals(p_toUpdate.Address));

                // we should ensure address and name uniqueness, so if name or address is changed, we should
                // chech if the contact doesn't already exist.
                if(hasNameChanged || hasAddressChanged)
                {
                    // if the address has been changed, we should change its reference
                    if(hasAddressChanged)
                    {
                        Address newAddress = GetExistingAddress(p_toUpdate.Address.Street, p_toUpdate.Address.HouseNumber, p_toUpdate.Address.ZIP, p_toUpdate.Address.City, p_toUpdate.Address.Country.Id);
                        
                        if(newAddress == null)
                        {
                            Country updatedCountry = await m_unitOfWork.Countries.GetByIdAsync(p_toUpdate.Address.Country.Id);

                            newAddress = new Address()
                            {
                                Street = p_toUpdate.Address.Street,
                                HouseNumber = p_toUpdate.Address.HouseNumber,
                                ZIP = p_toUpdate.Address.ZIP,
                                City = p_toUpdate.Address.City,
                                Country = updatedCountry
                            };
                            await m_unitOfWork.Addresses.AddAsync(newAddress);
                        }
                        contact.Address = newAddress;
                    }

                    p_toUpdate.Address.Id = contact.Address.Id;
                    if(ContactExists(p_toUpdate, true))
                        throw new Exception("Contact with that name and address already exists.");
                }
                
                

                var originalNumbers = contact.TelephoneNumbers;
                var updatedNumbers = p_toUpdate.TelephoneNumbers;

                m_unitOfWork.TelephoneNumbers.RemoveRange(originalNumbers);
                
                contact.TelephoneNumbers = updatedNumbers;

                await m_unitOfWork.CommitAsync();

                // send live updates
                await m_liveUpdateService.ContactUpdated(p_toUpdate);

                return p_toUpdate;
            }
            throw new Exception("Contact cannot be updated, because it does not exist.");
        }


        public async Task Delete(int p_id)
        {
            var contact = await m_unitOfWork.Contacts.GetByIdAsync(p_id);

            if(contact == null)
                throw new Exception("Contact does not exist!");

            if(contact.TelephoneNumbers != null && contact.TelephoneNumbers.Count() > 0)
                m_unitOfWork.TelephoneNumbers.RemoveRange(contact.TelephoneNumbers);

            var addressUsers = m_unitOfWork.Contacts.Find(x => x.Address.Id == contact.Address.Id);

            if(addressUsers.Count() == 1)
                m_unitOfWork.Addresses.Remove(contact.Address);
            
            m_unitOfWork.Contacts.Remove(contact);
            await m_unitOfWork.CommitAsync();

            // live updates
            await m_liveUpdateService.ContactDeleted(p_id);
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