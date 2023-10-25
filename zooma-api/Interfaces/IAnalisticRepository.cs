using zooma_api.Repositories;

namespace zooma_api.Interfaces
{
    public interface IAnalisticRepository
    {
        float GetTotalRevenues();
        List<MonthRevenue> GetSixMonthsRevenues();
        List<TicketsQuantity> GetSixMonthsTicketQuantity();
        float RevenuesInDay { get; }

        TicketsQuantity TicketsQuantityInDay { get; }
    }
}
