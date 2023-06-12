# Contribution Guidelines

The Ethar GeoPose library is under the [Apache2.0 License](https://github.com/EtharInc/Ethar.GeoPose/blob/main/LICENSE.txt). By contributing to the Ethar GeoPose library, you assert that:

* The contribution is your own original work.
* The contribution adheres to the [Coding Guidelines](articles/appendices/A01-CodingGuidelines.md)
* You have the right to assign the copyright for the work (it is not owned by your employer, or
  you have been given copyright assignment in writing).

## Finding Existing Issues

Before filing a new issue, please search our [open issues](https://github.com/dotnet/runtime/issues) to check if it already exists.

If you do find an existing issue, please include your own feedback in the discussion. Do consider upvoting (👍 reaction) the original post, as this helps us prioritize popular issues in our backlog.

## Writing a Good Bug Report

Good bug reports make it easier for maintainers to verify and root cause the underlying problem. The better a bug report, the faster the problem will be resolved. Ideally, a bug report should contain the following information:

* A high-level description of the problem.
* A _minimal reproduction_, i.e. the smallest size of code/configuration required to reproduce the wrong behavior.
* A description of the _expected behavior_, contrasted with the _actual behavior_ observed.
* Information on the environment: OS/distro, CPU arch, SDK version, etc.
* Additional information, e.g. is it a regression from previous versions? are there any known workarounds?

When ready to submit a bug report, please use the [Bug Report issue template](https://github.com/dotnet/runtime/issues/new?assignees=&labels=&template=01_bug_report.yml).

### Why are Minimal Reproductions Important?

A reproduction lets maintainers verify the presence of a bug, and diagnose the issue using a debugger. A _minimal_ reproduction is the smallest possible console application demonstrating that bug. Minimal reproductions are generally preferable since they:

1. Focus debugging efforts on a simple code snippet,
2. Ensure that the problem is not caused by unrelated dependencies/configuration,
3. Avoid the need to share production codebases.

### Are Minimal Reproductions Required?

In certain cases, creating a minimal reproduction might not be practical (e.g. due to nondeterministic factors, external dependencies). In such cases you would be asked to provide as much information as possible, for example by sharing a memory dump of the failing application. If maintainers are unable to root cause the problem, they might still close the issue as not actionable. While not required, minimal reproductions are strongly encouraged and will significantly improve the chances of your issue being prioritized and fixed by the maintainers.

### How to Create a Minimal Reproduction

The best way to create a minimal reproduction is gradually removing code and dependencies from a reproducing app, until the problem no longer occurs. A good minimal reproduction:

* Excludes all unnecessary types, methods, code blocks, source files, NuGet dependencies and project configurations.
* Contains documentation or code comments illustrating expected vs actual behavior.
* If possible, avoids performing any unneeded IO or system calls. For example, can the ASP.NET based reproduction be converted to a plain old console app?

## DOs and DON'Ts

Please do:

* **DO** follow our [coding style](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions) (C# code-specific).
* **DO** give priority to the current style of the project or file you're changing even if it diverges from the general guidelines.
* **DO** include tests when adding new features. When fixing bugs, start with
  adding a test that highlights how the current behavior is broken.
* **DO** keep the discussions focused. When a new or related topic comes up
  it's often better to create new issue than to side track the discussion.
* **DO** clearly state on an issue that you are going to take on implementing it.
* **DO** blog and tweet (or whatever) about your contributions, frequently!

Please do not:

* **DON'T** make PRs for style changes.
* **DON'T** surprise us with big pull requests. Instead, file an issue and start
  a discussion so we can agree on a direction before you invest a large amount
  of time.
* **DON'T** commit code that you didn't write. If you find code that you think is a good fit to add to the .NET runtime, file an issue and start a discussion before proceeding.
* **DON'T** submit PRs that alter licensing related files or headers. If you believe there's a problem with them, file an issue and we'll be happy to discuss it.
* **DON'T** add API additions without filing an issue and discussing with us first. The project is governed by the [GeoPose standard](https://docs.ogc.org/dis/21-056r10/21-056r10.html) and changes need to be ratified first.

## How to contribute

### Prerequisites

* A GitHub Account
* Familiarization with projects with Git Source control versioning. Atlassian has a wonderful guide for [getting started with Git](https://www.atlassian.com/git).
* Install Git on your local machine and have git assigned as an environment variable.
* Install a Git client like [Fork](https://git-fork.com/) or [GitHub for Desktop](https://desktop.github.com/) for staging and committing code to source control
* Follow any [Getting Started Guidelines](articles/00-GettingStarted.md#prerequisites) for setting up your development environment not covered here.

### Steps

1. Fork the repository to open a pull request for.
2. Clone or sync any changes from the source repository to your local disk.
    > **Note:** When initially cloning the repository be sure to recursively check out all submodules!
3. Create a new branch based on the last source development commit.
4. Make the changes you'd like to contribute.
5. Stage and commit your changes with thoughtful messages detailing the work.
6. Push your local changes to your fork's remote server.
7. Navigate to the repository's source repository on GitHub.
    > **Note:** by now a prompt to open a new pull request should be available on the repository's main landing page.
8. Open a pull request detailing the changes and fill out the Pull Request Template.
9. Typically Code Reviews are performed in around 24-48 hours.
10. Iterate on any feedback from reviews.
    > **Note:** Typically you can push the changes directly to the branch you've opened the pull request for.
11. Once the pull request is accepted and the build validation passes, changes are then squashed and merged into the target branch, and the process is repeated.

### Branching Strategies

The main, development, and any feature branches are all protected by branch policies. Each branch must be up to date and pass test and build validation before it can be merged.

* All merges to the feature and development branches are squashed and merged into a single commit to keep the history clean and focused on specific pull request changes.
* All merges from the main and development branches are just traditionally merged together to ensure they stay in sync and share the same histories.

---

If there is anything not mentioned in this document or you simply want to know more, raise an [RFI (Request for Information) request here](https://github.com/EtharInc/Ethar.GeoPose/issues/new?assignees=&labels=question&template=04_request_for_information.md&title=).
