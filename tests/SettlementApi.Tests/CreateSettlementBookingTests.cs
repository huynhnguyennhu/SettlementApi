using System.Threading.Tasks;
using Application.Settlement.Command;
using Domain.Model;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace SettlementApi.IntegrationTests
{
    [TestFixture]
    public class CreateSettlementBookingTests
    {
        private IConfigurationRoot _configuration;
        private IServiceScopeFactory _scopeFactory;
        
        [SetUp]
        public void Setup()
        {
            var builder = new ConfigurationBuilder();
            _configuration = builder.Build();
            var startup = new Startup(_configuration);
            var services = new ServiceCollection();
            startup.ConfigureServices(services);
            _scopeFactory = services.BuildServiceProvider().GetService<IServiceScopeFactory>();
        }
        
        [Test]
        public async Task WhenRequestContainsMatchingNameAndBookingTimeThenOutcomeMustBeAlreadyReserved()
        {
            var command = new BookSettlementCommand
            {
                Name = "A",
                BookingTime = "09:15"
            };
            var bookingResult1 = await SendAsync(command);
            var bookingResult2 = await SendAsync(command);

            bookingResult1.BookingId.Should().NotBeNull();
            bookingResult1.Outcome.Should().Be(BookingStatus.Successful);
            bookingResult2.BookingId.Should().BeNull();
            bookingResult2.Outcome.Should().Be(BookingStatus.AlreadyReserved);
        }

        [Test]
        public async Task WhenRequestBookingTimeInFullyBookedPeriodThenOutcomeMustBeAlreadyReserved()
        {
            SendFourOverlappingBooking();
            var command = new BookSettlementCommand
            {
                Name = "John Smith",
                BookingTime = "10:10"
            };
            var bookingResult = await SendAsync(command);
            bookingResult.BookingId.Should().BeNull();
            bookingResult.Outcome.Should().Be(BookingStatus.AlreadyReserved);
        }

        [Test]
        public async Task WhenRequestHasValidDataAndBookingTimeIsAvailableThenBookingShouldBeMadeSuccesfully()
        {
            SendFourOverlappingBooking();
            var command = new BookSettlementCommand
            {
                Name = "John Smith",
                BookingTime = "10:30"
            };
            var bookingResult = await SendAsync(command);
            bookingResult.BookingId.Should().NotBeNull();
            bookingResult.Outcome.Should().Be(BookingStatus.Successful);
        }

        [Test]
        public async Task WhenRequestHasOverlappingStartAndEndBookingTimeThenBookingShouldBeMadeSuccesfully()
        {
            SendFourOverlappingBooking();
            var command = new BookSettlementCommand
            {
                Name = "C",
                BookingTime = "10:30"
            };
            var bookingResult = await SendAsync(command);
            bookingResult.BookingId.Should().NotBeNull();
            bookingResult.Outcome.Should().Be(BookingStatus.Successful);
        }

        private async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
        {
            using var scope = _scopeFactory.CreateScope();

            var mediator = scope.ServiceProvider.GetService<ISender>();

            return await mediator.Send(request);
        }

        private Task<TResponse> Send<TResponse>(IRequest<TResponse> request)
        {
            using var scope = _scopeFactory.CreateScope();

            var mediator = scope.ServiceProvider.GetService<ISender>();

            return mediator.Send(request);
        }

        private void SendFourOverlappingBooking()
        {
            //setup 4 overlapping settlement bookings
            Send(new BookSettlementCommand() { Name = "A", BookingTime = "09:15" }).Wait();
            Send(new BookSettlementCommand() { Name = "B", BookingTime = "09:20" }).Wait();
            Send(new BookSettlementCommand() { Name = "C", BookingTime = "09:30" }).Wait();
            Send(new BookSettlementCommand() { Name = "D", BookingTime = "09:45" }).Wait();
        }
    }
}
