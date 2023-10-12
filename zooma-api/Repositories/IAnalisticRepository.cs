namespace zooma_api.Repositories
{
    public interface IAnalisticRepository
    {
        float getTotalRevenues();
        List<MonthRevenue> GetGetSixMonthsRevenues();
        List<TicketsQuantity> GetGetSixMonthsTicketQuantity();
        float RevenuesInDay { get; }

        TicketsQuantity TicketsQuantityInDay { get; }
    }
}
