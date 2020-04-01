using Infinum.ZanP.Core.Interfaces;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Infinum.ZanP.Infrastructure.Hubs;
using Infinum.ZanP.Core.Models.SQL;

namespace Infinum.ZanP.Infrastructure.Services
{
    public class LiveUpdateService : ILiveUpdateService
    {
        private readonly IHubContext<ContactHub> m_contactHub;

        public LiveUpdateService(IHubContext<ContactHub> p_contactHub)
        {
            m_contactHub = p_contactHub;
        }

        public async Task ContactUpdated(Contact p_updated)
        {
            await m_contactHub.Clients.All.SendAsync("ContactUpdated", p_updated);
        }

        public async Task ContactDeleted(int p_contactId)
        {
            await m_contactHub.Clients.All.SendAsync("ContactDeleted", p_contactId);
        }
    }
}