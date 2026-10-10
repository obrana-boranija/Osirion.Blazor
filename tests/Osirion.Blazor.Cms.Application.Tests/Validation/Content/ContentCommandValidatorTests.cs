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
