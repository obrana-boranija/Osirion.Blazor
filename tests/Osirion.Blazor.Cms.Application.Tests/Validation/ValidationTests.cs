using FluentValidation;
using Osirion.Blazor.Cms.Application.Validation;
using Shouldly;
using ValidationResult = Osirion.Blazor.Cms.Application.Validation.ValidationResult;

namespace Osirion.Blazor.Cms.Application.Tests.Validation;

public class ValidationTests
{
    [Fact]
    public void ValidationResult_Success_HasNoErrors()
    {
        var result = ValidationResult.Success();

        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
    }

    [Fact]
    public void ValidationResult_FailureForOneProperty_ListsThatError()
    {
        var result = ValidationResult.Failure("Title", "Title is required");

        result.IsValid.ShouldBeFalse();
        result.Errors["Title"].ShouldBe(["Title is required"]);
    }

    [Fact]
    public async Task FluentValidationAdapter_ValidInstance_ReturnsSuccess()
    {
        var adapter = new FluentValidationAdapter<Sample>(new SampleValidator());

        var result = await adapter.ValidateAsync(new Sample { Name = "ok", Code = "ab" });

        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public async Task FluentValidationAdapter_InvalidInstance_GroupsErrorsByProperty()
    {
        var adapter = new FluentValidationAdapter<Sample>(new SampleValidator());

        var result = await adapter.ValidateAsync(new Sample { Name = "", Code = "abcdef" });

        result.IsValid.ShouldBeFalse();
        result.Errors["Name"].ShouldBe(["Name is required", "Name is too short"]);
        result.Errors["Code"].ShouldBe(["Code is too long"]);
    }

    public sealed class Sample
    {
        public string Name { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;
    }

    private sealed class SampleValidator : AbstractValidator<Sample>
    {
        public SampleValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required").MinimumLength(2).WithMessage("Name is too short");
            RuleFor(x => x.Code).MaximumLength(3).WithMessage("Code is too long");
        }
    }
}
