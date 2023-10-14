namespace zooma_api.Repositories
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
