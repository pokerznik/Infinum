using System.Threading.Tasks;
using Infinum.ZanP.Core.Models.SQL;

namespace Infinum.ZanP.Core.Interfaces
{
    public interface ILiveUpdateService
    {
        Task ContactUpdated(Contact p_updated);
        Task ContactDeleted(int p_contactId);
    }
}