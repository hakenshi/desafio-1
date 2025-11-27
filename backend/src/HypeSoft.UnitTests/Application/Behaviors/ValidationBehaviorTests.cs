using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using HypeSoft.Application.Behaviors;
using MediatR;
using Moq;

namespace HypeSoft.UnitTests.Application.Behaviors;

public class ValidationBehaviorTests
{
    [Fact]
    public async Task Handle_NoValidators_ShouldCallNext()
    {
        // Arrange
        var validators = new List<IValidator<TestRequest>>();
        var behavior = new ValidationBehavior<TestRequest, TestResponse>(validators);
        var request = new TestRequest();
        var nextCalled = false;
        
        Task<TestResponse> Next()
        {
            nextCalled = true;
            return Task.FromResult(new TestResponse());
        }

        // Act
        await behavior.Handle(request, Next, CancellationToken.None);

        // Assert
        nextCalled.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldCallNext()
    {
        // Arrange
        var validatorMock = new Mock<IValidator<TestRequest>>();
        validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var validators = new List<IValidator<TestRequest>> { validatorMock.Object };
        var behavior = new ValidationBehavior<TestRequest, TestResponse>(validators);
        var request = new TestRequest();
        var nextCalled = false;

        Task<TestResponse> Next()
        {
            nextCalled = true;
            return Task.FromResult(new TestResponse());
        }

        // Act
        await behavior.Handle(request, Next, CancellationToken.None);

        // Assert
        nextCalled.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_InvalidRequest_ShouldThrowValidationException()
    {
        // Arrange
        var validationFailure = new ValidationFailure("Property", "Error message");
        var validationResult = new ValidationResult(new[] { validationFailure });

        var validatorMock = new Mock<IValidator<TestRequest>>();
        validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        var validators = new List<IValidator<TestRequest>> { validatorMock.Object };
        var behavior = new ValidationBehavior<TestRequest, TestResponse>(validators);
        var request = new TestRequest();

        Task<TestResponse> Next() => Task.FromResult(new TestResponse());

        // Act
        var act = async () => await behavior.Handle(request, Next, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task Handle_MultipleValidatorsWithErrors_ShouldThrowValidationExceptionWithAllErrors()
    {
        // Arrange
        var failure1 = new ValidationFailure("Property1", "Error 1");
        var failure2 = new ValidationFailure("Property2", "Error 2");

        var validator1Mock = new Mock<IValidator<TestRequest>>();
        validator1Mock
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new[] { failure1 }));

        var validator2Mock = new Mock<IValidator<TestRequest>>();
        validator2Mock
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new[] { failure2 }));

        var validators = new List<IValidator<TestRequest>> { validator1Mock.Object, validator2Mock.Object };
        var behavior = new ValidationBehavior<TestRequest, TestResponse>(validators);
        var request = new TestRequest();

        Task<TestResponse> Next() => Task.FromResult(new TestResponse());

        // Act
        var act = async () => await behavior.Handle(request, Next, CancellationToken.None);

        // Assert
        var exception = await act.Should().ThrowAsync<ValidationException>();
        exception.Which.Errors.Should().HaveCount(2);
    }

    public class TestRequest : IRequest<TestResponse> { }
    public class TestResponse { }
}
