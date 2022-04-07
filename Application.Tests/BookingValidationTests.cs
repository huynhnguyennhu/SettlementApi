using Application.Settlement.Command;
using FluentAssertions;
using NUnit.Framework;

namespace Application.Tests
{
    [TestFixture]
    public class Tests
    {
        private const string NotEmptyErrorCode = "NotEmptyValidator";
        private const string CustomErrorCode = "AsyncPredicateValidator";
        private BookSettlementCommandValidator _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new BookSettlementCommandValidator();
        }


        [Test]
        public void WhenRequestDoesNotHaveNameThenRelevantMissingRequiredFieldErrorShouldBeRaised()
        {
            var command = new BookSettlementCommand
            {
                BookingTime = "11:00"
            };
            var validationResult = _validator.Validate(command);
            validationResult.IsValid.Should().Be(false);
            validationResult.Errors[0].ErrorCode.Should().Be(NotEmptyErrorCode);
        }

        [Test]
        public void WhenRequestDoesNotHaveBookingTimeThenRelevantMissingRequiredFieldErrorShouldBeRaised()
        {
            var command = new BookSettlementCommand
            {
                Name = "John Smith"
            };
            var validationResult = _validator.Validate(command);
            validationResult.IsValid.Should().Be(false);
            validationResult.Errors[0].ErrorCode.Should().Be(NotEmptyErrorCode);
        }

        [Test]
        public void WhenRequestDoesNotHaveValidBookingTimeThenInvalidDataErrorShouldBeRaised()
        {
            var command = new BookSettlementCommand
            {
                Name = "John Smith",
                BookingTime = "16:01"
            };
            var validationResult = _validator.Validate(command);
            validationResult.IsValid.Should().Be(false);
            validationResult.Errors[0].ErrorCode.Should().Be(CustomErrorCode);
        }
    }
}