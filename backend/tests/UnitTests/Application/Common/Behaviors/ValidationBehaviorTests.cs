using FluentValidation;
using FluentValidation.Results;
using LeadEnrichment.Application.Common.Behaviors;
using Mediator;
using NSubstitute;
using Shouldly;
using Xunit;

namespace LeadEnrichment.UnitTests.Application.Common.Behaviors;

// Note: ValidationBehavior's `.Where(failure => failure is not null)` guard
// is intentionally not exercised here — FluentValidation's ValidationResult
// never actually populates Errors with null entries, so that line is
// defensive dead code rather than a real branch worth a dedicated test.
public class ValidationBehaviorTests
{
    public sealed record TestMessage : IRequest<string>;

    [Fact]
    public async Task Handle_WithNoValidators_CallsNext()
    {
        var behavior = new ValidationBehavior<TestMessage, string>([]);
        var nextCalled = false;

        ValueTask<string> Next(TestMessage msg, CancellationToken ct)
        {
            nextCalled = true;
            return ValueTask.FromResult("ok");
        }

        var result = await behavior.Handle(new TestMessage(), Next, CancellationToken.None);

        nextCalled.ShouldBeTrue();
        result.ShouldBe("ok");
    }

    [Fact]
    public async Task Handle_WithPassingValidator_CallsNext()
    {
        var validator = Substitute.For<IValidator<TestMessage>>();
        validator
            .ValidateAsync(Arg.Any<ValidationContext<TestMessage>>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        var behavior = new ValidationBehavior<TestMessage, string>([validator]);
        var nextCalled = false;

        ValueTask<string> Next(TestMessage msg, CancellationToken ct)
        {
            nextCalled = true;
            return ValueTask.FromResult("ok");
        }

        var result = await behavior.Handle(new TestMessage(), Next, CancellationToken.None);

        nextCalled.ShouldBeTrue();
        result.ShouldBe("ok");
    }

    [Fact]
    public async Task Handle_WithFailingValidator_ThrowsValidationExceptionAndDoesNotCallNext()
    {
        var failure = new ValidationFailure("Property", "Error message");
        var validator = Substitute.For<IValidator<TestMessage>>();
        validator
            .ValidateAsync(Arg.Any<ValidationContext<TestMessage>>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult([failure]));

        var behavior = new ValidationBehavior<TestMessage, string>([validator]);
        var nextCalled = false;

        ValueTask<string> Next(TestMessage msg, CancellationToken ct)
        {
            nextCalled = true;
            return ValueTask.FromResult("ok");
        }

        var exception = await Should.ThrowAsync<ValidationException>(
            async () => await behavior.Handle(new TestMessage(), Next, CancellationToken.None));

        exception.Errors.ShouldContain(failure);
        nextCalled.ShouldBeFalse();
    }

    [Fact]
    public async Task Handle_WithMultipleFailingValidators_AggregatesAllErrors()
    {
        var failureA = new ValidationFailure("PropertyA", "Error A");
        var failureB = new ValidationFailure("PropertyB", "Error B");

        var validatorA = Substitute.For<IValidator<TestMessage>>();
        validatorA
            .ValidateAsync(Arg.Any<ValidationContext<TestMessage>>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult([failureA]));

        var validatorB = Substitute.For<IValidator<TestMessage>>();
        validatorB
            .ValidateAsync(Arg.Any<ValidationContext<TestMessage>>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult([failureB]));

        var behavior = new ValidationBehavior<TestMessage, string>([validatorA, validatorB]);

        ValueTask<string> Next(TestMessage msg, CancellationToken ct) => ValueTask.FromResult("ok");

        var exception = await Should.ThrowAsync<ValidationException>(
            async () => await behavior.Handle(new TestMessage(), Next, CancellationToken.None));

        exception.Errors.Count().ShouldBe(2);
        exception.Errors.ShouldContain(failureA);
        exception.Errors.ShouldContain(failureB);
    }
}
