namespace M47.Shared.Infrastructure.Database.Maintenance;

using System;
using System.Threading.Tasks;

public interface IMaintenanceRepository
{
    Task TransferToHistoryAsync(DateTime date);
}