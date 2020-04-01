using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Infinum.ZanP.Core.Interfaces;
using Infinum.ZanP.Core.Models.SQL;
using Infinum.ZanP.Core.Interfaces.Repositories;

namespace Infinum.ZanP.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ContactsController : ControllerBase
    {
        private readonly IContactService m_contactService;
        private readonly IUnitOfWork m_unitOfWork;

        public ContactsController(IContactService p_contactService, IUnitOfWork p_unitOfWork)
        {
            m_contactService = p_contactService;
            m_unitOfWork = p_unitOfWork;
        }

        [HttpGet]
        public async Task<Infinum.ZanP.Core.Models.DTO.ContactSearchEnvelope> GetContacts([FromQuery(Name="filter")] string filter, [FromQuery(Name ="pageNumber")] int pageNumber=1, [FromQuery(Name ="perPage")] int perPage=10)
        {
            bool applyAll = filter == null;

            var contacts = await m_unitOfWork.Contacts.GetAllAsync();
            int totalNr = contacts.Count();

            if(!applyAll)
            {
                filter = filter.ToLower();
                var filtered = contacts.Where(x => x.Name.ToLower().Contains(filter));
                contacts = filtered;
            }

            List<Infinum.ZanP.Core.Models.DTO.Contact> contactsDto = new List<Infinum.ZanP.Core.Models.DTO.Contact>();

            if(contacts != null && contacts.Count() > 0)
            {
                foreach(var contact in contacts)
                {                
                    Infinum.ZanP.Core.Models.DTO.Contact dto = new Infinum.ZanP.Core.Models.DTO.Contact()
                    {
                        Id = contact.Id,
                        Address2String = contact.Address.ToString(),
                        DateOfBirth2String = contact.DateOfBirth.ToString("dd.MM.yyyy"),
                        Name = contact.Name,
                        TelephoneNumbers2String = string.Join(',', contact.TelephoneNumbers.Select(x => x.ToString()))
                    };

                    contactsDto.Add(dto);
                }
            }

            if(contactsDto.Count() > perPage)
            {
                contactsDto = contactsDto.Skip((pageNumber-1)*perPage).Take(perPage).ToList();
            }

            return new Core.Models.DTO.ContactSearchEnvelope()
            {
                Data = contactsDto,
                FilteredNumber = contactsDto.Count,
                TotalNumber = totalNr
            };
        }

        [HttpGet("{p_contactId}")]
        public async Task<Contact> GetDetails(int p_contactId)
        {
            var contact = await m_unitOfWork.Contacts.GetByIdAsync(p_contactId);

            if(contact == null)
                throw new Exception($"Contact with id {p_contactId} does not exist!");

            return contact;
        }

        [HttpPost]
        public async Task<Contact> Insert([FromBody]Contact p_envelope)
        {
            if(p_envelope == null)
                throw new Exception("Received model is not valid.");
            
            if(p_envelope.Address == null)
                throw new Exception("Received address is not valid.");

            if(string.IsNullOrEmpty(p_envelope.Name))
                throw new Exception("Name of contact is empty.");

            return await m_contactService.Create(p_envelope);
        }
    
        [HttpPut()]
        public async Task<Contact> Update([FromBody]Contact p_envelope)
        {
            if(p_envelope == null)
                throw new Exception("Received model is not valid.");
            
            if(p_envelope.Address == null)
                throw new Exception("Received address is not valid.");

            if(string.IsNullOrEmpty(p_envelope.Name))
                throw new Exception("Name of contact is empty.");

            return await m_contactService.Update(p_envelope);
        }
        
        [HttpDelete("{p_contactId}")]
        public async Task Delete(int p_contactId)
        {
            var contact = await m_unitOfWork.Contacts.GetByIdAsync(p_contactId);
            
            if(contact != null)
            {
                m_unitOfWork.Contacts.Remove(contact);
                await m_unitOfWork.CommitAsync();
            }
            else
            {
                throw new Exception("Contact cannot be deleted because it does not exist.");
            }
        }
    }
}