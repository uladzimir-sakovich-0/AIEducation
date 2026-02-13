# Documentation Files Rename Procedure

**Date**: February 13, 2026  
**Purpose**: Document the standardization of documentation file naming convention in the Docs folder

## Overview

This document describes the procedure performed to standardize all documentation files in the Docs folder with a date-based naming convention for better organization and traceability.

## Naming Convention Implemented

**Format**: `yyyyMMdd-XX-{OriginalFileName}.md`

Where:
- `yyyyMMdd` = File creation date (e.g., 20260213)
- `XX` = Sequential number (01, 02, 03...) for files created on the same day
- `{OriginalFileName}` = Original file name preserved

**Benefits**:
- Chronological sorting by default
- Easy identification of file creation date
- Sequential numbering for same-day files
- Original filename preserved for context
- Better file organization and history tracking

## Files Renamed

### Step 1: Analysis

Retrieved file creation dates:

```powershell
Get-ChildItem c:\_Projects\_Godel\AIEducation\Docs\*.md | Select-Object Name, CreationTime | Format-Table -AutoSize
```

**Results**:
| Original File Name | Creation Date | New File Name |
|-------------------|---------------|---------------|
| FINANCETRACKER_SETUP_SUMMARY.md | 2026-02-09 19:43:19 | 20260209-01-FINANCETRACKER_SETUP_SUMMARY.md |
| TASK_COMPLETE_SUMMARY.md | 2026-02-09 19:43:19 | 20260209-02-TASK_COMPLETE_SUMMARY.md |
| POSTGRESQL_DOCKER_SETUP.md | 2026-02-13 19:50:22 | 20260213-01-POSTGRESQL_DOCKER_SETUP.md |

### Step 2: Rename Operations

Executed PowerShell rename commands:

```powershell
# Navigate to Docs folder
cd c:\_Projects\_Godel\AIEducation\Docs

# Rename first file from 2026-02-09 (sequential number 01)
Rename-Item -Path "FINANCETRACKER_SETUP_SUMMARY.md" -NewName "20260209-01-FINANCETRACKER_SETUP_SUMMARY.md"

# Rename second file from 2026-02-09 (sequential number 02)
Rename-Item -Path "TASK_COMPLETE_SUMMARY.md" -NewName "20260209-02-TASK_COMPLETE_SUMMARY.md"

# Rename file from 2026-02-13 (sequential number 01)
Rename-Item -Path "POSTGRESQL_DOCKER_SETUP.md" -NewName "20260213-01-POSTGRESQL_DOCKER_SETUP.md"
```

### Step 3: Verification

Confirmed successful renames:

```powershell
Get-ChildItem *.md | Select-Object Name | Format-Table -AutoSize
```

**Final File List**:
- ✅ `20260209-01-FINANCETRACKER_SETUP_SUMMARY.md`
- ✅ `20260209-02-TASK_COMPLETE_SUMMARY.md`
- ✅ `20260213-01-POSTGRESQL_DOCKER_SETUP.md`
- ✅ `20260213-02-DOCS_RENAME_PROCEDURE.md` (this file)

## Sequential Numbering Logic

For files created on the same day:
1. Files are numbered starting from `01`
2. Numbers increment sequentially (`01`, `02`, `03`, etc.)
3. Numbering is based on the order of renaming or creation
4. Each day starts fresh with `01`

**Example**:
- 2026-02-09: Had 2 files → numbered 01 and 02
- 2026-02-13: Has 2 files → numbered 01 and 02

## Future File Naming Guidelines

When creating new documentation files:

1. **Determine the creation date**: Use current date in `yyyyMMdd` format
2. **Check existing files**: List files from the same date
   ```powershell
   Get-ChildItem 20260213-*.md | Sort-Object Name
   ```
3. **Assign next sequential number**: Increment from the highest number found
4. **Apply naming format**: `yyyyMMdd-XX-{DescriptiveFileName}.md`
5. **Use descriptive names**: Keep original filename meaningful and uppercase with underscores

## Automated Rename Script (for future use)

If additional files need to be renamed with this convention:

```powershell
# Get all markdown files without date prefix
$files = Get-ChildItem "c:\_Projects\_Godel\AIEducation\Docs\*.md" | Where-Object { $_.Name -notmatch '^\d{8}-\d{2}-' }

# Group by creation date
$grouped = $files | Group-Object { $_.CreationTime.ToString('yyyyMMdd') }

# Rename each group
foreach ($group in $grouped) {
    $date = $group.Name
    $counter = 1
    
    foreach ($file in $group.Group) {
        $sequentialNum = "{0:D2}" -f $counter
        $newName = "$date-$sequentialNum-$($file.Name)"
        Rename-Item -Path $file.FullName -NewName $newName
        Write-Host "Renamed: $($file.Name) -> $newName"
        $counter++
    }
}
```

## Benefits Achieved

✅ **Organization**: Files now sort chronologically automatically  
✅ **Traceability**: Creation date visible in filename  
✅ **Consistency**: Standardized naming across all documentation  
✅ **Scalability**: Sequential numbering handles multiple files per day  
✅ **Clarity**: Original filenames preserved for easy identification  
✅ **History**: Easy to track when documentation was created  

## Related Files

- [20260209-01-FINANCETRACKER_SETUP_SUMMARY.md](20260209-01-FINANCETRACKER_SETUP_SUMMARY.md) - Initial project setup
- [20260209-02-TASK_COMPLETE_SUMMARY.md](20260209-02-TASK_COMPLETE_SUMMARY.md) - Task completion summary
- [20260213-01-POSTGRESQL_DOCKER_SETUP.md](20260213-01-POSTGRESQL_DOCKER_SETUP.md) - PostgreSQL Docker setup

## Maintenance Notes

- Always check for existing files with the same date before creating new documentation
- Preserve the sequential numbering pattern
- Keep original filename descriptive and meaningful
- Use uppercase with underscores for traditional documentation file naming
- Update this list when new documentation files are added

## Changelog

**2026-02-13** - Initial Rename Procedure
- Renamed 3 existing documentation files
- Implemented date-based naming convention
- Added sequential numbering for same-day files
- Created this procedure document
- Verified all renames successful

---

**Maintained by**: Development Team  
**Last Updated**: February 13, 2026  
**Status**: ✅ Complete
