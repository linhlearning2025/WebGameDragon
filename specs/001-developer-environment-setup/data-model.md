# PLAT-001 Data Model

PLAT-001 introduces no product-domain database entities or persisted child data. The following
entities describe development and verification artifacts only; they are not PostgreSQL tables.

## Development Environment Profile

Represents a developer's local configuration for starting and checking the project.

| Attribute | Meaning | Validation |
|---|---|---|
| environment mode | Identifies local development configuration | Must select a non-production mode for the default local setup |
| database connection setting | Connection to local PostgreSQL | Required for API startup; missing value fails fast without value disclosure |
| media mode | Local or mock media selection | Must not require production R2 credentials in the default setup |
| secret source | Location from which any optional provider credentials are read | Secret values must not be committed or logged |

## Toolchain Matrix Entry

Represents an approved prerequisite documented for reproducible setup.

| Attribute | Meaning | Validation |
|---|---|---|
| tool name | Runtime, SDK, package manager, database client, or development prerequisite | Must be named in the setup guide |
| approved version | Version selected by the technical owner | Must be explicit and pinned before implementation is considered complete |
| support channel | Support status relevant to the project | Must follow the approved toolchain policy |
| verification command | Command a developer runs to confirm the installed version | Must be documented and reproducible |

Exact approved toolchain values are recorded in `docs/eng/toolchain-matrix.md` and
runtime/package-lock configuration.

## Service Readiness Result

Represents the non-persistent result returned when checking local API readiness.

| Attribute | Meaning | Validation |
|---|---|---|
| overall status | Whether the API is ready to serve dependent local workflows | Ready only when required dependencies are available |
| dependency checks | Status of required services, including PostgreSQL | A database outage produces a not-ready result |
| diagnostic detail | Safe explanation of the failed check | Must not contain credentials, connection strings, or secret values |

## CI Check Run

Represents a pull-request verification run.

| Attribute | Meaning | Validation |
|---|---|---|
| revision | Proposed repository change being checked | Must identify the revision under test |
| check name | Build, lint, test, readiness smoke, or secret scan | Must correspond to a required check in the workflow |
| outcome | Pass or fail | Any failed required check prevents the check set from passing |
| safe failure detail | Information needed to diagnose a failed check | Must redact secret values |

## Relationships and Lifecycle

- A developer profile selects toolchain matrix entries and local service configuration.
- An API readiness result is computed for a check and is not persisted as product state.
- A CI check run evaluates one revision and contains results for the required checks.
- The toolchain matrix records owner-approved values; runtime and dependency lock files must match
  those values for reproducible setup.