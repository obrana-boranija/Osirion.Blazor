using Osirion.Blazor.Cms.Application.Commands.Content;
using Osirion.Blazor.Cms.Application.Validation.Content;
using Shouldly;

namespace Osirion.Blazor.Cms.Application.Tests.Validation.Content;

public class ContentCommandValidatorTests
{
    [Theory]
    [InlineData("/etc/cron.d/job")]
    [InlineData("/blog/post.md")]
    [InlineData("../post.md")]
    [InlineData("blog/../../post.md")]
    [InlineData("blog/./post.md")]
    [InlineData("blog/..")]
    [InlineData("blog//post.md")]
    [InlineData("blog/")]
    [InlineData("blog\\post.md")]
    [InlineData("C:post.md")]
    [InlineData("blog/post\t.md")]
    [InlineData("blog/post\0.md")]
    [InlineData("blog/post?.md")]
    public void CreateContentCommandValidator_UnsafePath_IsInvalid(string path)
    {
        var result = new CreateContentCommandValidator().Validate(CreateCommand(path));

        result.Errors.ShouldContain(error => error.PropertyName == nameof(CreateContentCommand.Path));
    }

    [Theory]
    [InlineData("post.md")]
    [InlineData("blog/post.md")]
    [InlineData("blog/2026/post.md")]
    [InlineData("releases/v1..2.md")]
    [InlineData("blog/članak.md")]
    public void CreateContentCommandValidator_RelativePath_IsValid(string path)
    {
        var result = new CreateContentCommandValidator().Validate(CreateCommand(path));

        result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("/etc/cron.d/job")]
    [InlineData("../post.md")]
    [InlineData("blog/./post.md")]
    [InlineData("blog\\post.md")]
    public void UpdateContentCommandValidator_UnsafePath_IsInvalid(string path)
    {
        var result = new UpdateContentCommandValidator().Validate(UpdateCommand(path));

        result.Errors.ShouldContain(error => error.PropertyName == nameof(UpdateContentCommand.Path));
    }

    [Theory]
    [InlineData("blog/post.md")]
    [InlineData("releases/v1..2.md")]
    public void UpdateContentCommandValidator_RelativePath_IsValid(string path)
    {
        var result = new UpdateContentCommandValidator().Validate(UpdateCommand(path));

        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void CreateContentCommandValidator_MissingTitleContentAndPath_ReportsEach()
    {
        var result = new CreateContentCommandValidator().Validate(new CreateContentCommand());

        result.Errors.Select(error => error.PropertyName).Distinct().ShouldBe(
            [nameof(CreateContentCommand.Title), nameof(CreateContentCommand.Content), nameof(CreateContentCommand.Path)],
            ignoreOrder: true);
    }

    [Fact]
    public void CreateContentCommandValidator_TitleOver200Characters_IsInvalid()
    {
        var command = CreateCommand("blog/post.md");
        command.Title = new string('a', 201);

        var result = new CreateContentCommandValidator().Validate(command);

        result.Errors.ShouldContain(error => error.ErrorMessage == "Title cannot exceed 200 characters");
    }

    [Theory]
    [InlineData("my-post", true)]
    [InlineData("post-2026", true)]
    [InlineData("My Post", false)]
    [InlineData("post_one", false)]
    public void CreateContentCommandValidator_Slug_AllowsOnlyLowercaseLettersDigitsAndHyphens(string slug, bool isValid)
    {
        var command = CreateCommand("blog/post.md");
        command.Slug = slug;

        var result = new CreateContentCommandValidator().Validate(command);

        result.IsValid.ShouldBe(isValid);
    }

    [Fact]
    public void UpdateContentCommandValidator_MissingIdTitleAndContent_ReportsEach()
    {
        var result = new UpdateContentCommandValidator().Validate(new UpdateContentCommand { Path = "blog/post.md" });

        result.Errors.Select(error => error.PropertyName).Distinct().ShouldBe(
            [nameof(UpdateContentCommand.Id), nameof(UpdateContentCommand.Title), nameof(UpdateContentCommand.Content)],
            ignoreOrder: true);
    }

    [Theory]
    [InlineData("renamed", true)]
    [InlineData("Renamed!", false)]
    public void UpdateContentCommandValidator_Slug_AllowsOnlyLowercaseLettersDigitsAndHyphens(string slug, bool isValid)
    {
        var command = UpdateCommand("blog/post.md");
        command.Slug = slug;

        var result = new UpdateContentCommandValidator().Validate(command);

        result.IsValid.ShouldBe(isValid);
    }

    private static CreateContentCommand CreateCommand(string path) => new()
    {
        Title = "Post",
        Content = "Body",
        Path = path
    };

    private static UpdateContentCommand UpdateCommand(string path) => new()
    {
        Id = "post-1",
        Title = "Post",
        Content = "Body",
        Path = path
    };
}
