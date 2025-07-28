using DataLayer.DataAccess.Context;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.DataBaseUpdater.DataContext
{
    public class PostgreDbContext : TradeContext
    {
        public PostgreDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}
