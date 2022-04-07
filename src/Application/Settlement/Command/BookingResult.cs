using System;
using Domain.Model;

namespace Application.Settlement.Command
{
    public class BookingResult
    {
        public Guid? BookingId { get; set; }
        public BookingStatus Outcome { get; set; }
    }
}
