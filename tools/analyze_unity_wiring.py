#!/usr/bin/env python3
import argparse
import json
import re
from collections import defaultdict
from pathlib import Path


MONO_RE = re.compile(r"\bclass\s+([A-Za-z_][A-Za-z0-9_]*)\s*:\s*MonoBehaviour\b")
GUID_RE = re.compile(r"guid:\s*([0-9a-f]{32})")


def read_text(path):
    return path.read_text(encoding="utf-8", errors="ignore")


def script_guid(script_path):
    meta_path = script_path.with_suffix(script_path.suffix + ".meta")
    if not meta_path.exists():
        return None
    match = GUID_RE.search(read_text(meta_path))
    return match.group(1) if match else None


def mono_classes(script_path):
    text = read_text(script_path)
    return MONO_RE.findall(text)


def main():
    parser = argparse.ArgumentParser(
        description="Report MonoBehaviour scripts and whether scenes or prefabs reference their Unity GUIDs."
    )
    parser.add_argument("--root", default=".", help="Unity project root")
    parser.add_argument("--json", action="store_true", help="Emit JSON instead of text")
    args = parser.parse_args()

    root = Path(args.root).resolve()
    scripts = sorted(root.glob("Assets/Scripts/**/*.cs"))
    assets = sorted(list(root.glob("Assets/**/*.unity")) + list(root.glob("Assets/**/*.prefab")))

    guid_to_assets = defaultdict(list)
    for asset in assets:
        text = read_text(asset)
        for guid in GUID_RE.findall(text):
            guid_to_assets[guid].append(str(asset.relative_to(root)))

    rows = []
    for script in scripts:
        classes = mono_classes(script)
        if not classes:
            continue
        guid = script_guid(script)
        references = sorted(set(guid_to_assets.get(guid, []))) if guid else []
        rows.append(
            {
                "script": str(script.relative_to(root)),
                "classes": classes,
                "guid": guid,
                "assetReferences": references,
            }
        )

    if args.json:
        print(json.dumps(rows, indent=2))
        return

    for row in rows:
        refs = ", ".join(row["assetReferences"]) if row["assetReferences"] else "-"
        print(f"{row['script']}\t{', '.join(row['classes'])}\t{refs}")


if __name__ == "__main__":
    main()
