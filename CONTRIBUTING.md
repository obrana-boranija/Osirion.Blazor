# Contributing to Osirion.Blazor

We love your input! We want to make contributing to Osirion.Blazor as easy and transparent as possible, whether it's:

- Reporting a bug
- Discussing the current state of the code
- Submitting a fix
- Proposing new features
- Becoming a maintainer

## We Develop with Github

We use GitHub to host code, to track issues and feature requests, as well as accept pull requests.

## We Use [Github Flow](https://guides.github.com/introduction/flow/index.html)

Pull requests are the best way to propose changes to the codebase. We actively welcome your pull requests:

1. Fork the repo and create your branch from `master`.
2. If you've added code that should be tested, add tests.
3. If you've changed APIs, update the documentation.
4. Ensure the test suite passes.
5. Make sure your code follows the existing style.
6. Issue that pull request!

## Any contributions you make will be under the MIT Software License

In short, when you submit code changes, your submissions are understood to be under the same [MIT License](http://choosealicense.com/licenses/mit/) that covers the project. Feel free to contact the maintainers if that's a concern.

## Report bugs using Github's [issue tracker](https://github.com/obrana-boranija/Osirion.Blazor/issues)

We use GitHub issues to track public bugs. Report a bug by [opening a new issue](https://github.com/obrana-boranija/Osirion.Blazor/issues/new); it's that easy!

## Write bug reports with detail, background, and sample code

**Great Bug Reports** tend to have:

- A quick summary and/or background
- Steps to reproduce
  - Be specific!
  - Give sample code if you can
- What you expected would happen
- What actually happens
- Notes (possibly including why you think this might be happening, or stuff you tried that didn't work)

## Development Process

1. Clone the repository
2. Create a new branch: `git checkout -b feature/my-feature`
3. Make your changes
4. Run the tests: `dotnet test`
5. Commit your changes: `git commit -am 'Add some feature'`
6. Push to the branch: `git push origin feature/my-feature`
7. Submit a pull request

## Coding Style

- We follow standard C# coding conventions
- Use modern C# features when appropriate
- Keep components simple and focused (SOLID principles)
- Write clear, self-documenting code
- Add XML documentation for public APIs
- Use async/await for asynchronous operations
- Avoid unnecessary dependencies

## Testing

- Write unit tests for all new components
- Use xUnit for tests
- Aim for high code coverage
- Test both positive and negative scenarios
- Ensure SSR compatibility in tests

## Documentation

- Update README.md for significant changes
- Document all public APIs with XML comments
- Include examples for new components
- Update the changelog
- Create or update migration guides when needed

## License

By contributing, you agree that your contributions will be licensed under its MIT License.

## References

This document was adapted from the open-source contribution guidelines for [Facebook's Draft](https://github.com/facebook/draft-js/blob/master/CONTRIBUTING.md)