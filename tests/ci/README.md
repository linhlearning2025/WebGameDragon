# PLAT-001 CI Verification

These checks validate the pull-request workflow without committing a secret fixture or using a
real credential. Run them from the repository root with the approved versions in
`docs/eng/toolchain-matrix.md`.

## Passing Workflow

1. Run `pnpm install --frozen-lockfile`.
2. Run `dotnet tool restore`.
3. Run `dotnet restore apps/api/tests/DragonEnglishKids.Api.Tests/DragonEnglishKids.Api.Tests.csproj --locked-mode`.
4. Run `pnpm build`, `pnpm lint`, and `pnpm test`.
5. Open a pull request and confirm the required build, lint, test, and Gitleaks checks pass.

**Expected result**: every required check reports success; workflow logs contain no connection
strings or credentials.

## Required-Check Failure

1. In a temporary worktree, introduce an intentional failing assertion in an existing test or a
   syntax error in a temporary tracked source file.
2. Run the corresponding test or build command and confirm it exits nonzero.
3. If validating workflow gating, push only to a temporary branch and confirm the required job
   failure blocks the workflow's passing state.
4. Remove the temporary worktree or discard the temporary branch; do not merge the deliberate
   failure.

**Expected result**: a required check failure produces a failed job and cannot be reported as a
successful check set.

## Synthetic Secret Redaction

1. Create a temporary directory outside the checkout.
2. Generate a fake token pattern at runtime so no token-shaped value is committed, for example in
   PowerShell: `$synthetic = 'ghp_' + ('1' * 36)` and write `GITHUB_TOKEN=$synthetic` to a temporary
   `.env` file.
3. Run the exact scanner command and flags configured in `.github/workflows/ci.yml` against that
   temporary directory, with redaction enabled.
4. Confirm the scan exits nonzero, reports a finding, and its output does not contain the synthetic
   value.
5. Delete the temporary directory.

**Expected result**: secret detection fails closed and redacts the finding. Never paste, commit, or
use a real credential for this check.