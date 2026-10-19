#!/usr/bin/env bash
set -euo pipefail

repository_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
cd "$repository_root"

expected_node="$(tr -d '\r\n' < .node-version)"
actual_node="$(node --version | sed 's/^v//')"
if [[ "$actual_node" != "$expected_node" ]]; then
  printf 'Node.js %s is required; found %s.\n' "$expected_node" "$actual_node" >&2
  exit 1
fi

expected_pnpm="$(node -p "JSON.parse(require('fs').readFileSync('package.json', 'utf8')).packageManager.split('@').pop()")"
actual_pnpm="$(pnpm --version)"
if [[ "$actual_pnpm" != "$expected_pnpm" ]]; then
  printf 'pnpm %s is required; found %s.\n' "$expected_pnpm" "$actual_pnpm" >&2
  exit 1
fi

expected_dotnet="$(node -p "JSON.parse(require('fs').readFileSync('global.json', 'utf8')).sdk.version")"
actual_dotnet="$(dotnet --version)"
if [[ "$actual_dotnet" != "$expected_dotnet" ]]; then
  printf '.NET SDK %s is required; found %s.\n' "$expected_dotnet" "$actual_dotnet" >&2
  exit 1
fi

if [[ -z "${ConnectionStrings__Default:-}" ]]; then
  printf 'Required environment variable ConnectionStrings__Default is missing.\n' >&2
  exit 1
fi

pnpm install --frozen-lockfile
dotnet tool restore
dotnet restore apps/api/DragonEnglishKids.Api.csproj --locked-mode

printf 'Prerequisites validated and dependencies restored.\n'
printf 'Apply migrations with: pnpm db:migrate\n'
printf 'Start the API with: pnpm dev:api\n'
printf 'Start the web app with: pnpm dev:web\n'
printf 'Start the content tool with: pnpm dev:content\n'