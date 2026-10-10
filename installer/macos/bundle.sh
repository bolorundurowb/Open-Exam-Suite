#!/usr/bin/env bash
#
# Assembles a macOS .app bundle from a self-contained publish output.
#
# Usage:
#   bundle.sh <publish_dir> <app_name> <executable> <bundle_id> <version> <output_dir>
#             [--document-types] [--samples <dir>]
#
#   publish_dir   Directory produced by `dotnet publish`.
#   app_name      Bundle display name, e.g. "Open Exam Suite Simulator".
#   executable    Executable file name inside Contents/MacOS.
#   bundle_id     Reverse-DNS bundle identifier.
#   version       Full version, e.g. 5.0.0-preview.1.
#   output_dir    Directory that will receive <app_name>.app.
#   --document-types  Register the .oef document type (Simulator only).
#   --samples <dir>   Copy the sample exams into Contents/Resources.
set -euo pipefail

publish_dir="${1:?usage: bundle.sh <publish_dir> <app_name> <executable> <bundle_id> <version> <output_dir> [--document-types] [--samples <dir>]}"
app_name="${2:?}"
executable="${3:?}"
bundle_id="${4:?}"
version="${5:?}"
output_dir="${6:?}"
shift 6

document_types=""
samples_dir=""
while [[ $# -gt 0 ]]; do
  case "$1" in
    --document-types) document_types="yes"; shift ;;
    --samples) samples_dir="${2:?--samples needs a directory}"; shift 2 ;;
    *) echo "Unknown argument: $1" >&2; exit 1 ;;
  esac
done

script_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

# Apple wants a numeric x.y.z for CFBundleShortVersionString; drop the prerelease tag.
short_version="${version%%-*}"

app_bundle="${output_dir}/${app_name}.app"
contents="${app_bundle}/Contents"
macos_dir="${contents}/MacOS"
resources_dir="${contents}/Resources"

rm -rf "${app_bundle}"
mkdir -p "${macos_dir}" "${resources_dir}"

cp -R "${publish_dir}/." "${macos_dir}/"
chmod +x "${macos_dir}/${executable}"

if [[ -n "${samples_dir}" && -d "${samples_dir}" ]]; then
  cp -R "${samples_dir}/." "${resources_dir}/"
fi

if [[ "${document_types}" == "yes" ]]; then
  doc_block="$(cat <<'XML'
    <key>CFBundleDocumentTypes</key>
    <array>
        <dict>
            <key>CFBundleTypeName</key>
            <string>Open Exam Suite Exam</string>
            <key>CFBundleTypeRole</key>
            <string>Viewer</string>
            <key>LSHandlerRank</key>
            <string>Owner</string>
            <key>CFBundleTypeExtensions</key>
            <array>
                <string>oef</string>
            </array>
        </dict>
    </array>
XML
)"
else
  doc_block=""
fi

sed \
  -e "s|{{APP_NAME}}|${app_name}|g" \
  -e "s|{{EXECUTABLE}}|${executable}|g" \
  -e "s|{{BUNDLE_ID}}|${bundle_id}|g" \
  -e "s|{{VERSION}}|${version}|g" \
  -e "s|{{SHORT_VERSION}}|${short_version}|g" \
  "${script_dir}/Info.plist" > "${contents}/Info.plist"

# Replace the document-types placeholder. The block is several lines, so this
# cannot be a single sed substitution.
doc_file="${contents}/.document-types"
plist_tmp="${contents}/Info.plist.tmp"
printf '%s\n' "${doc_block}" > "${doc_file}"
: > "${plist_tmp}"
while IFS= read -r line || [[ -n "${line}" ]]; do
  if [[ "${line}" == *"{{DOCUMENT_TYPES}}"* ]]; then
    if [[ -s "${doc_file}" ]]; then
      cat "${doc_file}" >> "${plist_tmp}"
    fi
  else
    printf '%s\n' "${line}" >> "${plist_tmp}"
  fi
done < "${contents}/Info.plist"
mv "${plist_tmp}" "${contents}/Info.plist"
rm -f "${doc_file}"

cp "${script_dir}/entitlements.plist" "${contents}/entitlements.plist"

echo "Created ${app_bundle}"
