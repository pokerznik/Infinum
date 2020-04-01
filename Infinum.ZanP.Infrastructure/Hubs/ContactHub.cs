using System.Threading.Tasks;  
using Microsoft.AspNetCore.SignalR;
using Infinum.ZanP.Core.Interfaces.Repositories;
using Infinum.ZanP.Core.Models.DTO;
using System.Collections.Generic;
using System.Linq;

namespace Infinum.ZanP.Infrastructure.Hubs
{
    public class ContactHub: Hub
    {
        private readonly IUnitOfWork m_unitOfWork;

        public ContactHub(IUnitOfWork p_unitOfWork)
        {
            m_unitOfWork = p_unitOfWork;
        }

        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("MessageReceived", message);
        }

        public override async Task OnConnectedAsync()
        {
            var allContacts = await m_unitOfWork.Contacts.GetAllAsync();
            List<Contact> contactsDto = new List<Contact>();

            foreach(var contact in allContacts)
            {                
                Contact dto = new Contact()
                {
                    Id = contact.Id,
                    Address2String = contact.Address.ToString(),
                    DateOfBirth2String = contact.DateOfBirth.ToString("dd.MM.yyyy"),
                    Name = contact.Name,
                    TelephoneNumbers2String = string.Join(',', contact.TelephoneNumbers.Select(x => x.ToString()))
                };

                contactsDto.Add(dto);
            }

            await Clients.Caller.SendAsync("Connected", contactsDto);
        }
    }
}