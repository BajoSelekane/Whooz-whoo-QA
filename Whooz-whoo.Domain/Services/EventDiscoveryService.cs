using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Whooz_whoo.Domain.Entities;
using Whooz_whoo.Domain.Interfaces;
using Whooz_whoo.Domain.Enums;

namespace Whooz_whoo.Domain.Services
{
    public class EventDiscoveryService : IEventDiscoveryService
    {
        private readonly IRepository<Event> _eventRepository;
        private readonly IGeolocationService _geolocationService;
        private readonly ICacheService _cacheService;

        public EventDiscoveryService(
            IRepository<Event> eventRepository,
            IGeolocationService geolocationService,
            ICacheService cacheService)
        {
            _eventRepository = eventRepository;
            _geolocationService = geolocationService;
            _cacheService = cacheService;
        }

        public async Task<List<Event>> FindNearbyEventsAsync(
            Location userLocation,
            double radiusInKm,
            EventType? type = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            int maxResults = 20)
        {
            var cacheKey = $"discovery:{userLocation.Latitude}:{userLocation.Longitude}:{radiusInKm}:{type}:{startDate}:{endDate}";

            var cached = await _cacheService.GetAsync<List<Event>>(cacheKey);
            if (cached != null) return cached;

            var query = _eventRepository.GetQueryable()
                .Where(e => e.Status == EventStatus.Active)
                .Where(e => e.Location.CalculateDistance(userLocation) <= radiusInKm);

            if (type.HasValue)
                query = query.Where(e => e.Categories.Any(c => c.Type == type.Value));

            if (startDate.HasValue)
                query = query.Where(e => e.StartDateTime >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(e => e.EndDateTime <= endDate.Value);

            var events = await query
                .OrderBy(e => e.Location.CalculateDistance(userLocation))
                .Take(maxResults)
                .ToListAsync();

            // Cache for 5 minutes
            await _cacheService.SetAsync(cacheKey, events, TimeSpan.FromMinutes(5));

            return events;
        }

        public async Task<List<Event>> GetPersonalizedRecommendationsAsync(
            User user,
            int maxResults = 10)
        {
            var userInterests = user.Interests.Select(i => i.Category).ToList();

            var query = _eventRepository.GetQueryable()
                .Where(e => e.Status == EventStatus.Active)
                .Where(e => e.StartDateTime > DateTime.UtcNow);

            // Score events based on user interests
            var scoredEvents = await query
                .Select(e => new
                {
                    Event = e,
                    Score = e.Categories
                        .Count(c => userInterests.Contains(c.Type.ToString()))
                })
                .ToListAsync();

            return scoredEvents
                .OrderByDescending(x => x.Score)
                .ThenBy(e => e.Event.StartDateTime)
                .Take(maxResults)
                .Select(x => x.Event)
                .ToList();
        }
    }
}