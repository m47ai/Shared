# Update dependencies

🏠 [Go back to README.md](/README.md)

This `dependabot.yml` file contains the configuration for automatic dependency updates using Dependabot in a GitHub repository. It specifies the following updates:

- Weekly **NuGet** package updates, scheduled for Mondays, allowing only dependency updates that begin with `AMAZON*` or `AWS` grouped together in the same branch (`dependabot/dependabot-nuget-aws`)
- Weekly NuGet package updates (root directories), scheduled for Mo ndays at 7:00am Europe/Madrid time, allowing all updates except those for dependencies that begin with "Microsoft.EntityFrameworkCore" and versions greater than 7.0.0, and for "Pomelo.EntityFrameworkCore.MySql" and versions greater than 7.0.0.
- Monthly npm package updates (root directories), scheduled for Mondays at 7:05am Europe/Madrid time.
- Monthly GitHub Actions package updates (root directories), scheduled for Mondays at 7:15am Europe/Madrid time.

Labels, reviewers, and assignees have been specified for each update.
