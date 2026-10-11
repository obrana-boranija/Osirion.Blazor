using Microsoft.Extensions.Logging;
using Osirion.Blazor.Cms.Admin.Application.Core;
using Osirion.Blazor.Cms.Admin.Core.Events;
using Osirion.Blazor.Cms.Admin.Domain.Events;
using Osirion.Blazor.Cms.Admin.Services.Events;
using Osirion.Blazor.Cms.Admin.Tests.TestDoubles;
using Shouldly;

namespace Osirion.Blazor.Cms.Admin.Tests.Common;

public class AdminPrimitivesTests
{
    [Fact]
    public void Result_SuccessAndFailure_ReportTheirOutcome()
    {
        var success = Result.Success();
        var failure = Result.Failure("broken");

        success.IsSuccess.ShouldBeTrue();
        success.IsFailure.ShouldBeFalse();
        success.Error.ShouldBeEmpty();
        failure.IsSuccess.ShouldBeFalse();
        failure.IsFailure.ShouldBeTrue();
        failure.Error.ShouldBe("broken");
    }

    [Fact]
    public void ResultOfT_SuccessAndFailure_CarryValueOrError()
    {
        var success = Result<int>.Success(42);
        var failure = Result<string>.Failure("broken");

        success.IsSuccess.ShouldBeTrue();
        success.Value.ShouldBe(42);
        failure.IsFailure.ShouldBeTrue();
        failure.Value.ShouldBeNull();
        failure.Error.ShouldBe("broken");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void ParseQuery_Blank_ReturnsAnEmptyDictionary(string query)
    {
        QueryHelpers.ParseQuery(query).ShouldBeEmpty();
    }

    [Fact]
    public void ParseQuery_DecodesNamesAndValuesWithAndWithoutTheQuestionMark()
    {
        var withMark = QueryHelpers.ParseQuery("?code=a%2Bb&state=x+y");
        var withoutMark = QueryHelpers.ParseQuery("code=abc");

        withMark["code"].ShouldBe(["a+b"]);
        withMark["STATE"].ShouldBe(["x y"]);
        withoutMark["code"].ShouldBe(["abc"]);
    }

    [Fact]
    public void ParseQuery_KeyWithoutValueAndEqualsInALaterPair_GetsAnEmptyValue()
    {
        var result = QueryHelpers.ParseQuery("flag&code=abc");

        result["flag"].ShouldBe([string.Empty]);
        result["code"].ShouldBe(["abc"]);
    }

    [Fact]
    public void ParseQuery_RepeatedKey_KeepsEveryValueInOrder()
    {
        var result = QueryHelpers.ParseQuery("tag=a&tag=b&tag=c");

        result["tag"].ShouldBe(["a", "b", "c"]);
    }

    [Fact]
    public void TryGetValue_ReturnsTheFirstValueOrFalse()
    {
        var result = QueryHelpers.ParseQuery("tag=a&tag=b");

        result.TryGetValue("tag", out string first).ShouldBeTrue();
        first.ShouldBe("a");
        result.TryGetValue("missing", out string missing).ShouldBeFalse();
        missing.ShouldBeEmpty();
    }

    [Fact]
    public void TryGetValue_KeyWithNoValues_ReturnsFalse()
    {
        var query = new Dictionary<string, string[]> { ["empty"] = [] };

        query.TryGetValue("empty", out string value).ShouldBeFalse();
        value.ShouldBeEmpty();
    }

    [Fact]
    public void ContentSavedDomainEvent_StampsTheCurrentUtcTimeAndAUniqueId()
    {
        var before = DateTime.UtcNow;

        var first = new ContentSavedDomainEvent("a.md", "sha1");
        var second = new ContentSavedDomainEvent("a.md", "sha1");

        first.Path.ShouldBe("a.md");
        first.Sha.ShouldBe("sha1");
        first.Timestamp.ShouldBeInRange(before, DateTime.UtcNow);
        first.Timestamp.Kind.ShouldBe(DateTimeKind.Utc);
        first.Id.ShouldNotBe(second.Id);
    }

    [Fact]
    public void CmsEventMediator_FailingHandler_IsLoggedAndLaterHandlersStillRun()
    {
        var logger = new RecordingLogger<CmsEventMediator>();
        var mediator = new CmsEventMediator(logger);
        var received = new List<string>();
        mediator.Subscribe<ContentSavedEvent>(_ => throw new InvalidOperationException("handler failed"));
        mediator.Subscribe<ContentSavedEvent>(saved => received.Add(saved.Path));

        mediator.Publish(new ContentSavedEvent("a.md"));

        received.ShouldBe(["a.md"]);
        logger.Entries.ShouldContain(entry => entry.Level == LogLevel.Error && entry.Exception is InvalidOperationException);
    }
}
