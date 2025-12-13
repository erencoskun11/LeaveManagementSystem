namespace Application.Common.Extensions
{
    public static class DateExtensions
    {
        public static int CalculateBusinessDays(this DateTime startDate, DateTime endDate)
        {
            int businessDays = 0;
            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                {
                    businessDays++;
                }
            }
            return businessDays;
        }
    }
}