using System;
using System.Threading.Tasks;
using Domain.Model;

namespace Application.Service
{
    public interface IBookingStoreProvider
    {
        Task<Guid?> AddBooking(Booking booking);        
    }
}
