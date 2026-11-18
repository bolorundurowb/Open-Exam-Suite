# Repository guidance

## Project structure

- The solution is `src/Open-Exam-Suite.slnx`. Build and run commands from the `src/` directory.
- The applications are `src/Apps/Creator` and `src/Apps/Simulator` and both use Avalonia on Windows, macOS, and Linux.
- Keep reusable exam and application behavior in the portable libraries under `src/Libraries/`. Keep UI and platform-specific behavior in the app projects or behind the existing host-port abstractions.
- All projects target .NET 11. Nullable reference types and implicit usings are enabled through `src/Directory.Build.props`.

## Dependencies and file formats

- Use SDK-style project files and `PackageReference` for dependencies.
- Package versions are centrally managed in `src/Directory.Packages.props`. Add package versions there and omit versions from project-level `PackageReference` entries.
- Preserve `.oef` compatibility with older v1 and v2 files. Legacy files should remain readable; loading a file should not rewrite it. Save changes in the current Protobuf-backed format.
- Keep JSON/XML import and export behavior compatible with the Creator workflows.

## Build and verification

Run solution commands from `src/`:

```bash
dotnet restore
dotnet build --configuration Release --no-restore
dotnet test --configuration Release
```

The CI workflow builds and tests on Windows, macOS, and Linux. Linux builds require the Avalonia native libraries installed by `.github/workflows/build-and-test.yml` (`libfontconfig1`, `libx11-6`, `libice6`, and `libsm6`).

## Release packaging and documentation

- Keep platform artifacts combined: Windows x64 installer and portable ZIP; macOS x64 and arm64 DMGs; Linux x64 tarball and combined Debian package.
- Keep `.github/workflows/generate-release.yml`, the packaging scripts under `installer/`, and the README download list in sync when release assets change.
- The Linux Debian package and tarball include both the Simulator and Creator, plus samples.

## Branch names

Use the branch prefixes specified for this project:

- `ft/` — feature work
- `ch/` — changes or maintenance
- `bg/` — bug fixes
- `doc/` — documentation

Follow the prefix with a short kebab-case description, such as `doc/readme-modernization-cleanup`.
