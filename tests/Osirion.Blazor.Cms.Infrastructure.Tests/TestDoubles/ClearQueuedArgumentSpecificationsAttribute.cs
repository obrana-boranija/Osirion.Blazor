using System.Reflection;
using NSubstitute.Core;
using Osirion.Blazor.Cms.Infrastructure.Tests.TestDoubles;
using Xunit.Sdk;

[assembly: ClearQueuedArgumentSpecifications]

namespace Osirion.Blazor.Cms.Infrastructure.Tests.TestDoubles;

/// <summary>
/// Runs every test in this assembly with no NSubstitute argument specifications left over from another test.
/// </summary>
/// <remarks>
/// NSubstitute queues <c>Arg.Any</c> and <c>Arg.Is</c> on the current thread until a call on a substitute takes them.
/// A test that passes them to a call that is not a substitute call, such as a method of the class under test or an
/// extension method, leaves them queued. The next test that runs on the same thread then fails with an argument
/// matcher exception that has nothing to do with it, so the failing test changes from run to run. Known-failing tests
/// in this project do that, for example <c>GitHubDirectoryRepositoryTests.DeleteAsync_RemovesDirectory</c> and
/// <c>InMemoryContentCacheServiceTests</c>. Clearing the queue before and after each test keeps them from failing
/// other tests; they still fail themselves until they are corrected.
/// </remarks>
[AttributeUsage(AttributeTargets.Assembly)]
public sealed class ClearQueuedArgumentSpecificationsAttribute : BeforeAfterTestAttribute
{
    /// <inheritdoc />
    public override void Before(MethodInfo methodUnderTest)
    {
        SubstitutionContext.Current.ThreadContext.DequeueAllArgumentSpecifications();
    }

    /// <inheritdoc />
    public override void After(MethodInfo methodUnderTest)
    {
        SubstitutionContext.Current.ThreadContext.DequeueAllArgumentSpecifications();
    }
}
