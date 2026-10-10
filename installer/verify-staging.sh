#!/usr/bin/env bash
#
# Checks that a packaged tree uses the sibling layout:
#
#   Windows/Linux staging:  {root}/Simulator, {root}/Creator, {root}/Samples
#   macOS bundle:           {app}.app/Contents/Resources
#
# Samples must sit beside the app folders, never inside them ({app}/Simulator/Samples).
#
# Usage: verify-staging.sh <staging_root | app_bundle.app>
set -euo pipefail

target="${1:?usage: verify-staging.sh <staging_root | app_bundle.app>}"

if [[ "${target}" == *.app ]]; then
  [[ -d "${target}/Contents/Resources" ]] || { echo "Bundle is missing Contents/Resources: ${target}" >&2; exit 1; }
  shopt -s nullglob
  resources=("${target}/Contents/Resources/"*.oef)
  shopt -u nullglob
  if [[ ${#resources[@]} -eq 0 ]]; then
    echo "Contents/Resources has no .oef samples: ${target}" >&2
    exit 1
  fi
  for bad in "${target}/Contents/MacOS/Samples" "${target}/Contents/MacOS/Simulator/Samples"; do
    if [[ -d "${bad}" ]]; then
      echo "Samples must not live under Contents/MacOS: ${bad}" >&2
      exit 1
    fi
  done
  echo "Bundle layout OK: ${target}"
  exit 0
fi

[[ -d "${target}" ]] || { echo "Staging root not found: ${target}" >&2; exit 1; }
[[ -d "${target}/Samples" ]] || { echo "Missing Samples beside the app folders: ${target}/Samples" >&2; exit 1; }
shopt -s nullglob
samples=("${target}/Samples/"*.oef)
shopt -u nullglob
if [[ ${#samples[@]} -eq 0 ]]; then
  echo "Samples directory has no .oef files: ${target}/Samples" >&2
  exit 1
fi

for app in Simulator Creator; do
  if [[ -d "${target}/${app}/Samples" ]]; then
    echo "Samples must not live inside ${app}: ${target}/${app}/Samples" >&2
    exit 1
  fi
done

echo "Staging layout OK: ${target}"
