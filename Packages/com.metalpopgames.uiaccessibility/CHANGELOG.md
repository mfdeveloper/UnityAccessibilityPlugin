# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [1.1.5]
### Added
- Rollback `Samples/UINavigationExample/Unity UI Navigation Example.unity` scene to the original forked repo
- Moved previous disabled button example from the scene above to: `com.metalpopgames.uiaccessibility/Samples/UINavigationExample` folder.

## [1.1.4]
### Fixed
- ScriptableObject `LocalizationStrategy` that allows external plugins integrations for localization keys (e.g com.unity.localization package)
- Async localization loadings using `UAP_AccessibilityManager.LocalizeAsync()` method with `Task<T>` async/await approach (better for **WebGL** and **Mobile** builds)

## [1.1.3]
### Fixed
- Bug Fix: Fixed NullPointer error on `Accessibility_Toggle_Inspector` when click on **"Speech Output"** inspector section

## [1.1.2]
### Added
- Expose the global delay timers on `UAP_AccessibilityManager` inspector
- Added a new field `m_SkipIfDisabled` to skip disabled elements from TTS talk

## [1.1.1]
### Added
- Support for TextMeshPro Button, DropDown and Input
- Support for Screen Space Canvas
- Added default option for popups to automatically read from the top
- Added helper script to link Back/Close buttons to the scrub gesture
- Added keyboard shortcuts for Windows and Mac to toggle plugin
- Added LayoutElement with IngoreLayout setting to the unit frame to remove ipact on layout groups

### Changed
- Bug Fix: No voice output on windows 64 Bit Standalone
- Bug Fix: Android text-to-speed stuttering when moving fast

## [1.1.0]
### Added
 - Added support for selecting 3D objects in the world
 - Added TextMeshPro supportZeliosAriex).

## [0.2.0] - 2015-10-06
### Changed
- Remove exclusionary mentions of "open source" since this project can
benefit both "open" and "closed" source projects equally.

## [0.1.0] - 2015-10-06
### Added
- Answer "Should you ever rewrite a change log?".

### Changed
- Improve argument against commit logs.
- Start following [SemVer](https://semver.org) properly.

## [0.0.8] - 2015-02-17
### Changed
- Update year to match in every README example.
- Reluctantly stop making fun of Brits only, since most of the world
  writes dates in a strange way.

### Fixed
- Fix typos in recent README changes.
- Update outdated unreleased diff link.

## [0.0.7] - 2015-02-16
### Added
- Link, and make it obvious that date format is ISO 8601.

### Changed
- Clarified the section on "Is there a standard change log format?".

### Fixed
- Fix Markdown links to tag comparison URL with footnote-style links.

## [0.0.6] - 2014-12-12
### Added
- README section on "yanked" releases.

## [0.0.5] - 2014-08-09
### Added
- Markdown links to version tags on release headings.
- Unreleased section to gather unreleased changes and encourage note
keeping prior to releases.

## [0.0.4] - 2014-08-09
### Added
- Better explanation of the difference between the file ("CHANGELOG")
and its function "the change log".

### Changed
- Refer to a "change log" instead of a "CHANGELOG" throughout the site
to differentiate between the file and the purpose of the file — the
logging of changes.

### Removed
- Remove empty sections from CHANGELOG, they occupy too much space and
create too much noise in the file. People will have to assume that the
missing sections were intentionally left out because they contained no
notable changes.

## [0.0.3] - 2014-08-09
### Added
- "Why should I care?" section mentioning The Changelog podcast.

## [0.0.2] - 2014-07-10
### Added
- Explanation of the recommended reverse chronological release ordering.

## [0.0.1] - 2014-05-31
### Added
- This CHANGELOG file to hopefully serve as an evolving example of a
  standardized open source project CHANGELOG.
- CNAME file to enable GitHub Pages custom domain
- README now contains answers to common questions about CHANGELOGs
- Good examples and basic guidelines, including proper date formatting.
- Counter-examples: "What makes unicorns cry?"