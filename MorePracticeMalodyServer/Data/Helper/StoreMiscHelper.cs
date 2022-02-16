using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MorePracticeMalodyServer.Model.DbModel;

namespace MorePracticeMalodyServer.Data.Helper;

public static class StoreMiscHelper
{
    internal static async Task<IEnumerable<Song>> QueryAllPromotion(this DataContext context)
    {
        return await InternalQueryPromotion(context, 0, 0, 0, -1);
    }

    internal static async Task<IEnumerable<Song>> QueryPromotion(this DataContext context, int from)
    {
        return await InternalQueryPromotion(context, 0, 0, from, Consts.MaxItem);
    }

    private static async Task<IEnumerable<Song>> InternalQueryPromotion(DataContext context, int org, int mode,
        int from, int maxCount)
    {
        // Query promotes from database.
        // TODO: original mode as query parameter.
        List<Promotion> result;
        if (maxCount > 0)
            result = await context.Promotions
                .Include(p => p.Song)
                .OrderBy(p => p.Id)
                .Skip(maxCount * from)
                .Take(maxCount)
                .AsNoTracking()
                .ToListAsync();
        else
            result = await context.Promotions
                .Include(p => p.Song)
                .AsNoTracking()
                .ToListAsync();

        var list = new List<Song>();
        foreach (var item in result) list.Add(item.Song);

        return list;
    }

    internal static async Task<IEnumerable<Event>> QueryAllEvents(this DataContext context, int active)
    {
        return await InternalQueryEvents(context, active, 0, -1); // -1 makes a query without number limit.
    }

    internal static async Task<IEnumerable<Event>> QueryEvents(this DataContext context, int active, int from)
    {
        return await InternalQueryEvents(context, active, from, Consts.MaxItem);
    }

    internal static async Task<Event> QueryEventById(this DataContext context, int eid)
    {
        var result = await context.Events
            .Include(e => e.EventCharts)
            .ThenInclude(c => c.Chart)
            .ThenInclude(c => c.Song)
            .AsNoTracking()
            .AsSplitQuery()
            .FirstAsync(e => e.EventId == eid); // TODO: Save event to cache?

        return result;
    }

    private static async Task<IEnumerable<Event>> InternalQueryEvents(DataContext context, int active, int from,
        int maxCount)
    {
        var query = context.Events
            .OrderBy(e => e.EventId)
            .AsNoTracking();

        if (active == 1)
            query = query.Where(e => e.Active);

        if (maxCount > 0)
            query.Skip(maxCount * from)
                .Take(maxCount);

        return await query.ToListAsync();
    }
}