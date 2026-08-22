namespace Libro.Services
{
    public static class FineCalculator
    {
        private const decimal FinePerDay = 25m;

        /// <summary>
        /// بيحسب الغرامة بناءً على تاريخ الاستحقاق وتاريخ الإرجاع الفعلي.
        /// بيرجع null لو مفيش تأخير خالص.
        /// </summary>
        public static decimal? Calculate(DateTime dueDate, DateTime returnDate)
        {
            if (returnDate.Date <= dueDate.Date)
            {
                return null;
            }

            int daysLate = (returnDate.Date - dueDate.Date).Days;
            return daysLate * FinePerDay;
        }
    }
}