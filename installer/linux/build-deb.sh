#!/usr/bin/env bash
#
# Builds one combined Linux package containing the Simulator, Creator, and samples.
#
# Usage: build-deb.sh <staging_dir> <version> <output_dir> [arch]
set -euo pipefail

staging_dir="${1:?usage: build-deb.sh <staging_dir> <version> <output_dir> [arch]}"
version="${2:?}"
output_dir="${3:?}"
arch="${4:-amd64}"

[[ -d "${staging_dir}/Simulator" ]] || { echo "Simulator staging directory not found: ${staging_dir}/Simulator" >&2; exit 1; }
[[ -d "${staging_dir}/Creator" ]] || { echo "Creator staging directory not found: ${staging_dir}/Creator" >&2; exit 1; }

script_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
repo_root="$(cd "${script_dir}/../.." && pwd)"
deb_version="$(printf '%s' "${version}" | sed -E 's/-(alpha|beta|preview|rc)/~\1/')"
root="$(mktemp -d)"
trap 'rm -rf "${root}"' EXIT

install -d \
  "${root}/opt/open-exam-suite/Simulator" \
  "${root}/opt/open-exam-suite/Creator" \
  "${root}/opt/open-exam-suite/Samples" \
  "${root}/usr/bin" \
  "${root}/usr/share/applications" \
  "${root}/usr/share/mime/packages" \
  "${root}/usr/share/icons/hicolor/scalable/apps" \
  "${root}/DEBIAN"

cp -R "${staging_dir}/Simulator/." "${root}/opt/open-exam-suite/Simulator/"
cp -R "${staging_dir}/Creator/." "${root}/opt/open-exam-suite/Creator/"
if [[ -d "${staging_dir}/Samples" ]]; then
  cp -R "${staging_dir}/Samples/." "${root}/opt/open-exam-suite/Samples/"
fi

ln -s /opt/open-exam-suite/Simulator/OpenExamSuite.Simulator "${root}/usr/bin/open-exam-suite"
ln -s /opt/open-exam-suite/Creator/OpenExamSuite.Creator "${root}/usr/bin/open-exam-suite-creator"
install -m 0644 "${script_dir}/open-exam-suite.desktop" "${root}/usr/share/applications/open-exam-suite.desktop"
install -m 0644 "${script_dir}/open-exam-suite-creator.desktop" "${root}/usr/share/applications/open-exam-suite-creator.desktop"
install -m 0644 "${script_dir}/open-exam-suite.xml" "${root}/usr/share/mime/packages/open-exam-suite.xml"
install -m 0644 "${repo_root}/assets/icons/simulator.svg" "${root}/usr/share/icons/hicolor/scalable/apps/open-exam-suite.svg"
install -m 0644 "${repo_root}/assets/icons/creator.svg" "${root}/usr/share/icons/hicolor/scalable/apps/open-exam-suite-creator.svg"

cat > "${root}/DEBIAN/control" <<EOF
Package: open-exam-suite
Version: ${deb_version}
Section: education
Priority: optional
Architecture: ${arch}
Maintainer: Winner-Timothy Bolorunduro
Depends: libfontconfig1, libx11-6, libice6, libsm6, shared-mime-info, desktop-file-utils
Recommends: libgtk-3-0 | libgtk-3-0t64
Description: Open Exam Suite Simulator and Creator
 Take exams with the Simulator and create exams with Creator.
EOF

mkdir -p "${output_dir}"
dpkg-deb --build --root-owner-group "${root}" "${output_dir}/open-exam-suite_${deb_version}_${arch}.deb"
