using System;
using System.Collections.Generic;
using System.Text;
using Whooz_whoo.Domain.Entities;

namespace Whooz_whoo.Domain.Interfaces
{
    public interface IEventStreamingService
    {
        Task<string> CreateStreamAsync(Guid eventId, string streamName);
        Task<string> GetStreamUrlAsync(string streamId);
        Task<bool> StartStreamAsync(string streamId);
        Task<bool> StopStreamAsync(string streamId);
        Task<bool> IsStreamingAsync(string streamId);
        Task<int> GetViewerCountAsync(string streamId);
        Task<List<ViewerInfo>> GetActiveViewersAsync(string streamId);
    }
}
