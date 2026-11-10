#!/usr/bin/env bash
set -euo pipefail
if [[ $# -ne 1 ]]; then
  echo 'Usage: ./build.sh /path/to/RimWorld1.7' >&2
  exit 2
fi
cd "$(dirname "$0")"
dotnet build Source/JoJoStand/JoJoStand.csproj -c Release "-p:RimWorldDir=$1"
echo 'Built 1.7/Assemblies/JoJoStand.dll.'
