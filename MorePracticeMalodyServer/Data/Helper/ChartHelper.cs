using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MorePracticeMalodyServer.Model.DataModel;
using MorePracticeMalodyServer.Model.DbModel;

namespace MorePracticeMalodyServer.Data.Helper
{
    public static class ChartHelper
    {
        private static async Task<IEnumerable<ChartInfo>> InternalQueryCharts(DataContext context, int sid, int beta,
            int mode,
            int from, int maxCount)
        {
            var result = await context.Songs
                .Include(s => s.Charts)
                .AsNoTracking()
                .FirstAsync(s => s.SongId == sid);

            var charts = result.Charts;

            // Select stable if don't want to see beta.
            if (beta == 0)
                charts
                    .Where(c => c.Type == ChartState.Stable);

            // Select mode if give a mode.
            if (mode != -1)
                charts
                    .Where(c => c.Mode == mode);

            // 目前返回所有的chart，下次重构合并进入判断是否有更多后再分片返回.
            var list = new List<ChartInfo>();
            foreach (var chart in charts)
                list.Add(new ChartInfo
                {
                    Cid = chart.ChartId,
                    Creator = chart.Creator,
                    Length = chart.Length,
                    Level = chart.Level,
                    Mode = chart.Mode,
                    Size = chart.Size,
                    Type = chart.Type,
                    Uid = chart.UserId,
                    Version = chart.Version
                });

            return list;
        }

        private static async Task<ChartInfo> InternalQueryChart(DataContext context, int cid)
        {
            var result = await context.Charts
                .AsSplitQuery()
                .AsNoTracking()
                .FirstAsync(c => c.ChartId == cid);

            return new ChartInfo
            {
                Cid = result.ChartId,
                Creator = result.Creator,
                Length = result.Length,
                Level = result.Level,
                Mode = result.Mode,
                Size = result.Size,
                Type = result.Type,
                Uid = result.UserId,
                Version = result.Version
            };
        }

        internal static async Task<IEnumerable<ChartInfo>> QueryChartsFromSid(this DataContext context, int sid,
            int beta,
            int mode)
        {
            return await InternalQueryCharts(context, sid, beta, mode, 0, 0);
        }

        internal static async Task<ChartInfo> QueryChartFromCid(this DataContext context, int cid)
        {
            return await InternalQueryChart(context, cid);
        }
    }
}