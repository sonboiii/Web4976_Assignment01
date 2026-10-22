#!/usr/bin/env bash
set -euo pipefail

cd -- "$(dirname -- "${BASH_SOURCE[0]}")"
export ASPNETCORE_ENVIRONMENT=Development

dotnet run --project Web4976_Assignment01.csproj --no-launch-profile -- --urls http://localhost:5159 "$@"
