using DataLayer.Abstract.DataBaseUpdater.Filler;
using DataLayer.Abstract.DataBaseUpdater.Migrator;
using DataLayer.Abstract.DataBaseUpdater.Updater;
using DataLayer.DataBaseUpdater;
using DataLayer.DataBaseUpdater.DataContext;
using Microsoft.Extensions.Logging;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var factory = LoggerFactory.Create(x => x.AddConsole());
var logger = factory.CreateLogger<Program>();

var context = DataBaseMigratorHelpers.GetContext<PostgreDbContext>(x => new PostgreDbContext(x), StringResources.ConnectionString, logger);

if (args.Length > 0)
{
    await DbFiller.Run(context, typeof(Program).Assembly, factory);
}
else
{
    var updater = new DbUpdater(factory.CreateLogger<DbUpdater>(), context);
    await updater.UpdateAsync();
}