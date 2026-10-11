using Osirion.Blazor.Cms.Application.Commands;
using Shouldly;

namespace Osirion.Blazor.Cms.Application.Tests.Commands;

public class CommandResultTests
{
    [Fact]
    public void Success_HasNoErrorMessage()
    {
        var result = CommandResult.Success();

        result.IsSuccess.ShouldBeTrue();
        result.ErrorMessage.ShouldBeNull();
    }

    [Fact]
    public void Failure_CarriesTheErrorMessage()
    {
        var result = CommandResult.Failure("Not allowed");

        result.IsSuccess.ShouldBeFalse();
        result.ErrorMessage.ShouldBe("Not allowed");
    }

    [Fact]
    public void GenericSuccess_CarriesTheResult()
    {
        var result = CommandResult<int>.Success(42);

        result.IsSuccess.ShouldBeTrue();
        result.Result.ShouldBe(42);
        result.ErrorMessage.ShouldBeNull();
    }

    [Fact]
    public void GenericFailure_HasNoResult()
    {
        var result = CommandResult<string>.Failure("Not found");

        result.IsSuccess.ShouldBeFalse();
        result.Result.ShouldBeNull();
        result.ErrorMessage.ShouldBe("Not found");
    }
}
