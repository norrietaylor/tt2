#!/usr/bin/env bash
# Pre-push smoke check for Unity C# project.
# Catches the classes of CI failures we've hit without needing Unity installed.
# Run: ./scripts/preflight.sh

set -euo pipefail

SCRIPTS_DIR="Unity/Assets/Scripts"
TESTS_DIR="Unity/Assets/Tests"
RUNTIME_ASMDEF="$SCRIPTS_DIR/TT2.Runtime.asmdef"
ERRORS=0

red()   { printf '\033[0;31m%s\033[0m\n' "$1"; }
green() { printf '\033[0;32m%s\033[0m\n' "$1"; }
warn()  { printf '\033[0;33m%s\033[0m\n' "$1"; }

fail() { red "FAIL: $1"; ERRORS=$((ERRORS + 1)); }
pass() { green "OK: $1"; }

echo "=== Unity Preflight Checks ==="
echo ""

# 1. Every .cs file has a .meta file
echo "--- Meta files ---"
MISSING_META=0
while IFS= read -r cs_file; do
    if [ ! -f "${cs_file}.meta" ]; then
        fail "Missing .meta for $cs_file"
        MISSING_META=$((MISSING_META + 1))
    fi
done < <(find Unity/Assets -name "*.cs" -not -path "*/Library/*" 2>/dev/null)

while IFS= read -r asmdef_file; do
    if [ ! -f "${asmdef_file}.meta" ]; then
        fail "Missing .meta for $asmdef_file"
        MISSING_META=$((MISSING_META + 1))
    fi
done < <(find Unity/Assets -name "*.asmdef" -not -path "*/Library/*" 2>/dev/null)

if [ "$MISSING_META" -eq 0 ]; then
    pass "All .cs and .asmdef files have .meta files"
fi

# 2. New folders (staged or untracked) have .meta files
echo ""
echo "--- Folder meta files (new folders only) ---"
MISSING_FOLDER_META=0
while IFS= read -r dir; do
    [ -z "$dir" ] && continue
    [ ! -d "$dir" ] && continue
    dir_meta="${dir}.meta"
    if [ ! -f "$dir_meta" ]; then
        fail "Missing folder .meta: $dir_meta"
        MISSING_FOLDER_META=$((MISSING_FOLDER_META + 1))
    fi
done < <(git diff --name-only --diff-filter=A HEAD 2>/dev/null | xargs -I{} dirname {} | sort -u | grep "^Unity/Assets")

if [ "$MISSING_FOLDER_META" -eq 0 ]; then
    pass "All new folders have .meta files"
fi

# 3. No duplicate GUIDs among new/changed .meta files
echo ""
echo "--- GUID uniqueness (new/changed files) ---"
GUID_ERRORS=0
for meta_file in $(git diff --name-only HEAD 2>/dev/null | grep '\.meta$' || true); do
    [ ! -f "$meta_file" ] && continue
    guid=$(grep "^guid:" "$meta_file" 2>/dev/null | head -1 | awk '{print $2}')
    [ -z "$guid" ] && continue
    matches=$(find Unity/Assets -name "*.meta" -exec grep -l "^guid: $guid" {} + 2>/dev/null | wc -l)
    if [ "$matches" -gt 1 ]; then
        fail "$meta_file has duplicate GUID $guid"
        GUID_ERRORS=$((GUID_ERRORS + 1))
    fi
done

if [ "$GUID_ERRORS" -eq 0 ]; then
    pass "No GUID collisions in new/changed .meta files"
fi

# 4. 500-line cap
echo ""
echo "--- 500-line cap ---"
OVER_500=0
while IFS= read -r cs_file; do
    lines=$(wc -l < "$cs_file")
    if [ "$lines" -gt 500 ]; then
        fail "$cs_file is $lines lines (max 500)"
        OVER_500=$((OVER_500 + 1))
    fi
done < <(find Unity/Assets -name "*.cs" -not -path "*/Library/*" 2>/dev/null)

if [ "$OVER_500" -eq 0 ]; then
    pass "All files under 500 lines"
fi

# 5. All using directives in test files reference namespaces available via asmdef
echo ""
echo "--- Namespace references ---"
if [ -f "$RUNTIME_ASMDEF" ]; then
    # Check that test files only import TaekwondoTech.* (covered by TT2.Runtime)
    # or standard Unity/System namespaces
    BAD_IMPORTS=0
    while IFS= read -r test_file; do
        while IFS= read -r import; do
            ns=$(echo "$import" | sed 's/using //;s/;//;s/ //')
            case "$ns" in
                TaekwondoTech*|UnityEngine*|UnityEditor*|System*|NUnit*|TMPro*) ;;
                *) fail "$test_file imports unknown namespace: $ns"
                   BAD_IMPORTS=$((BAD_IMPORTS + 1)) ;;
            esac
        done < <(grep "^using " "$test_file" 2>/dev/null)
    done < <(find "$TESTS_DIR" -name "*.cs" 2>/dev/null)

    if [ "$BAD_IMPORTS" -eq 0 ]; then
        pass "All test imports reference known namespaces"
    fi
else
    warn "SKIP: $RUNTIME_ASMDEF not found — cannot verify namespace references"
fi

# 6. Production asmdef references external packages that code needs
echo ""
echo "--- External package references ---"
if [ -f "$RUNTIME_ASMDEF" ]; then
    # Check if any .cs file uses TMPro but asmdef doesn't reference it
    USES_TMPRO=$(grep -rl "using TMPro;" "$SCRIPTS_DIR" 2>/dev/null | head -1)
    if [ -n "$USES_TMPRO" ]; then
        if ! grep -q "Unity.TextMeshPro" "$RUNTIME_ASMDEF"; then
            fail "Code uses TMPro but TT2.Runtime.asmdef missing Unity.TextMeshPro reference"
        else
            pass "TMPro reference present in asmdef"
        fi
    fi

    # Check if any .cs file uses Timeline but asmdef doesn't reference it
    USES_TIMELINE=$(grep -rl "using UnityEngine.Timeline;" "$SCRIPTS_DIR" 2>/dev/null | head -1 || true)
    if [ -n "$USES_TIMELINE" ]; then
        if ! grep -q "Unity.Timeline" "$RUNTIME_ASMDEF"; then
            fail "Code uses Timeline but TT2.Runtime.asmdef missing Unity.Timeline reference"
        fi
    fi
else
    warn "SKIP: No runtime asmdef found"
fi

# 7. No Object.Destroy() in test code without LogAssert protection
echo ""
echo "--- EditMode test safety ---"
UNSAFE_DESTROY=0
while IFS= read -r test_file; do
    if grep -qn "Object\.Destroy\b" "$test_file" 2>/dev/null; then
        if ! grep -q "LogAssert" "$test_file" 2>/dev/null; then
            fail "$test_file calls Object.Destroy without LogAssert (will fail in EditMode)"
            UNSAFE_DESTROY=$((UNSAFE_DESTROY + 1))
        fi
    fi
done < <(find "$TESTS_DIR" -name "*.cs" 2>/dev/null || true)

if [ "$UNSAFE_DESTROY" -eq 0 ]; then
    pass "No unprotected Object.Destroy in test code"
fi

# Summary
echo ""
echo "==========================="
if [ "$ERRORS" -gt 0 ]; then
    red "PREFLIGHT FAILED: $ERRORS error(s)"
    exit 1
else
    green "PREFLIGHT PASSED"
    exit 0
fi
