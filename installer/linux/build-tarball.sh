#!/usr/bin/env bash
#
# Builds the Linux tarball using the same Simulator/ Creator/ Samples/ layout as the
# Windows portable zip.
#
# Usage: build-tarball.sh <staging_dir> <version> <output_dir>
set -euo pipefail

staging_dir="${1:?usage: build-tarball.sh <staging_dir> <version> <output_dir>}"
version="${2:?}"
output_dir="${3:?}"

[[ -d "${staging_dir}" ]] || { echo "Staging directory not found: ${staging_dir}" >&2; exit 1; }

mkdir -p "${output_dir}"
tarball="${output_dir}/OpenExamSuite-${version}-Linux-x64.tar.gz"
rm -f "${tarball}"

# Include only the folders that exist, in a stable order.
include=()
for folder in Simulator Creator Samples; do
  [[ -d "${staging_dir}/${folder}" ]] && include+=("${folder}")
done

if [[ ${#include[@]} -eq 0 ]]; then
  echo "Nothing to package under ${staging_dir}" >&2
  exit 1
fi

tar -C "${staging_dir}" -czf "${tarball}" "${include[@]}"
echo "${tarball}"
