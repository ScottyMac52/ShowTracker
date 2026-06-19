# ShowTracker

A .NET console application and supporting libraries for tracking TV shows and movies. Built as a rapid prototyping/test harness for core tracking logic **before** integrating into an Alexa skill for voice-driven show management.

**Current status:** Early development (v0.0.1.5). Core Trakt search works great. Basic track flow exists. Persistence layer is in place for local SQLite storage. Many tracking features (untrack, upcoming releases, watch progress, next episode) are stubbed or partially wired.

## Why This Project?
I wanted a clean, testable backend for show tracking that I can:
1. Rapidly iterate on in console.
2. Later drop into an Alexa skill (or other voice/UI) without rewriting core logic.
3. Keep local control of my watch data + use Trakt only for discovery/search.

## Features (Implemented)

- **Trakt-powered title search**: Search shows + movies in one call (or separately). Results ranked with exact title match first, then Trakt score, limited to top 10 most relevant.
- **Track Show / Track Movie**: Exact-match search via Trakt, returns structured `TrackedTitle` (with ProviderId from Trakt, type, optional platform like "Netflix").
- **Local persistence**: SQLite (`showtracker.db`) with simple repo pattern for `TrackedTitles` table. Schema created on demand.
- **Command-line interface**: Extensible command router with dedicated commands for search, track, untrack, get tracked, upcoming, mark watched, etc.
- **Strong test coverage**: Unit tests across Domain/Application/Persistence/Providers + integration tests. Recent releases focused on command test coverage.
- **Clean-ish architecture**: Domain models (records), Application services, Persistence (lightweight SQLite), Providers (Trakt), Console frontend. Heavy DI via Microsoft.Extensions.
- **CI/CD**: GitHub Actions workflows for builds, tests, and automated releases/tags.

## Planned / In Progress

- Full implementation of `ITitleTrackingProvider` and Application services for:
  - Persisting tracked titles locally
  - Untracking
  - Retrieving tracked list
  - Upcoming releases / next episode logic
  - Watch progress / mark episode/movie watched
- Deeper Trakt integration (calendars, user history, recommendations?)
- Alexa skill frontend (console is the proving ground)
- Better error handling, resilience (retries, rate limits), logging
- Possibly Spectre.Console for prettier CLI tables/output

## Architecture Overview

```
ShowTracker.Console (CLI entrypoint + CommandRouter + Commands)
        ↓
ShowTracker.Application (Services: TrackShowService, SearchTitlesService, etc. + Interfaces)
        ↓
ShowTracker.Domain (Models: TrackedTitle, TitleSearchResult, TrackedTitleType + Interfaces)
        ↑
ShowTracker.Persistence (SqliteTrackedTitleRepository, SqliteWatchProgressRepository, etc.)
ShowTracker.Providers.Trakt (TraktTitleSearchClient, TraktTitleTrackingProvider)
```

- **Domain**: Pure models/records + core interfaces (e.g. `ITitleTrackingProvider`).
- **Application**: Orchestration / use-case services. This is the "core" that different frontends (Console, Alexa) will consume.
- **Persistence**: Lightweight direct SQLite (no EF Core yet). Fast for console/Alexa.
- **Providers**: External integrations. Currently only Trakt search + partial track.
- **Testing**: Separate test projects per layer + shared `ShowTracker.Testing` + integration tests.

## Command Set
- search <query>
- search-show <query>
- search-movie <query>
- track-show <title> [--platform <platform>]
- track-movie <title> [--platform <platform>]
- tracked
- untrack <provider-id>
- watched-episode <show title> <season> <episode>
- watched-movie <movie title>
- progress <show title>
- next-episode <show title>
- releases
- next-release <title>
- help

