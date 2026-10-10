#!/usr/bin/env bash
#
# Builds the two Linux .deb packages from a staging tree:
#
#   open-exam-suite          Simulator, samples, desktop entry, MIME type, /usr/bin symlink
#   open-exam-suite-creator  Creator only; depends on open-exam-suite
#
# Usage: build-deb.sh <staging_dir> <version> <output_dir> [arch]
set -euo pipefail

staging_dir="${1:?usage: build-deb.sh <staging_dir> <version> <output_dir> [arch]}"
version="${2:?}"
output_dir="${3:?}"
arch="${4:-amd64}"

[[ -d "${staging_dir}" ]] || { echo "Staging directory not found: ${staging_dir}" >&2; exit 1; }

script_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
repo_root="$(cd "${script_dir}/../.." && pwd)"

# Debian versions cannot contain '-preview'; map to '~preview' so a prerelease sorts before the final.
deb_version="$(printf '%s' "${version}" | sed -E 's/-(alpha|beta|preview|rc)/~\1/')"

maintainer="Winner-Timothy Bolorunduro"

build_simulator() {
  local root
  root="$(mktemp -d)"
  install -d \
    "${root}/opt/open-exam-suite/Simulator" \
    "${root}/opt/open-exam-suite/Samples" \
    "${root}/usr/bin" \
    "${root}/usr/share/applications" \
    "${root}/usr/share/mime/packages" \
    "${root}/usr/share/icons/hicolor/scalable/apps" \
    "${root}/DEBIAN"

  cp -R "${staging_dir}/Simulator/." "${root}/opt/open-exam-suite/Simulator/"
  [[ -d "${staging_dir}/Samples" ]] && cp -R "${staging_dir}/Samples/." "${root}/opt/open-exam-suite/Samples/"

  ln -s /opt/open-exam-suite/Simulator/OpenExamSuite.Simulator "${root}/usr/bin/open-exam-suite"
  install -m 0644 "${script_dir}/open-exam-suite.desktop" "${root}/usr/share/applications/open-exam-suite.desktop"
  install -m 0644 "${script_dir}/open-exam-suite.xml" "${root}/usr/share/mime/packages/open-exam-suite.xml"
  install -m 0644 "${repo_root}/assets/icons/simulator.svg" "${root}/usr/share/icons/hicolor/scalable/apps/open-exam-suite.svg"

  cat > "${root}/DEBIAN/control" <<EOF
Package: open-exam-suite
Version: ${deb_version}
Section: education
Priority: optional
Architecture: ${arch}
Maintainer: ${maintainer}
Depends: libfontconfig1, libx11-6, libice6, libsm6, shared-mime-info, desktop-file-utils
Recommends: libgtk-3-0 | libgtk-3-0t64
Description: Open Exam Suite Simulator
 Take Open Exam Suite exams. Registers the .oef file type with the Simulator.
EOF

  chmod 755 "${root}"
  dpkg-deb --build --root-owner-group "${root}" "${output_dir}/open-exam-suite_${deb_version}_${arch}.deb"
  rm -rf "${root}"
}

build_creator() {
  local root
  root="$(mktemp -d)"
  install -d \
    "${root}/opt/open-exam-suite/Creator" \
    "${root}/usr/bin" \
    "${root}/usr/share/applications" \
    "${root}/usr/share/icons/hicolor/scalable/apps" \
    "${root}/DEBIAN"

  cp -R "${staging_dir}/Creator/." "${root}/opt/open-exam-suite/Creator/"

  ln -s /opt/open-exam-suite/Creator/OpenExamSuite.Creator "${root}/usr/bin/open-exam-suite-creator"
  install -m 0644 "${script_dir}/open-exam-suite-creator.desktop" "${root}/usr/share/applications/open-exam-suite-creator.desktop"
  install -m 0644 "${repo_root}/assets/icons/creator.svg" "${root}/usr/share/icons/hicolor/scalable/apps/open-exam-suite-creator.svg"

  cat > "${root}/DEBIAN/control" <<EOF
Package: open-exam-suite-creator
Version: ${deb_version}
Section: education
Priority: optional
Architecture: ${arch}
Depends: open-exam-suite (= ${deb_version})
Maintainer: ${maintainer}
Description: Open Exam Suite Creator
 Create and edit Open Exam Suite exams. Installs only the Creator beside the Simulator.
EOF

  chmod 755 "${root}"
  dpkg-deb --build --root-owner-group "${root}" "${output_dir}/open-exam-suite-creator_${deb_version}_${arch}.deb"
  rm -rf "${root}"
}

mkdir -p "${output_dir}"
build_simulator
build_creator
