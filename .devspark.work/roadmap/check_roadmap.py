#!/usr/bin/env python3
"""Keep roadmap.md and the ps-NNN spec files consistent.

Usage:
    python .devspark.work/roadmap/check_roadmap.py          # report problems, exit 1 if any
    python .devspark.work/roadmap/check_roadmap.py --fix    # move done/dropped specs to specs/complete/,
                                                            # rewrite their links, sync table statuses

Each spec's frontmatter is the source of truth; the roadmap tables mirror it.
Standard library only.
"""

from __future__ import annotations

import argparse
import re
import subprocess
import sys
from dataclasses import dataclass, field
from pathlib import Path

ROADMAP_DIR = Path(__file__).resolve().parent
ROADMAP = ROADMAP_DIR / "roadmap.md"
SPECS = ROADMAP_DIR / "specs"
COMPLETE = SPECS / "complete"

STATUSES = {"proposed", "ready", "specifying", "in-progress", "done", "deferred", "dropped"}
CLOSED = {"done", "dropped"}

ROW = re.compile(
    r"^\| \[(?P<id>ps-\d{3})\]\((?P<link>[^)]+)\) \| (?P<title>[^|]+?) \| (?P<size>[^|]+?) "
    r"\| (?P<deps>[^|]+?) \| (?P<status>[^|]+?) \|\s*$"
)


@dataclass
class Spec:
    path: Path
    meta: dict[str, object] = field(default_factory=dict)

    @property
    def id(self) -> str:
        return str(self.meta.get("id", ""))

    @property
    def rel(self) -> str:
        return self.path.relative_to(ROADMAP_DIR).as_posix()


def parse_frontmatter(path: Path) -> dict[str, object]:
    text = path.read_text(encoding="utf-8")
    match = re.match(r"^---\r?\n(.*?)\r?\n---", text, re.S)
    if not match:
        return {}
    meta: dict[str, object] = {}
    for line in match.group(1).splitlines():
        key, sep, value = line.partition(":")
        if not sep:
            continue
        value = value.strip()
        if value.startswith("[") and value.endswith("]"):
            meta[key.strip()] = [v.strip() for v in value[1:-1].split(",") if v.strip()]
        elif value in ("null", "~", ""):
            meta[key.strip()] = None
        else:
            meta[key.strip()] = value.strip('"')
    return meta


def load_specs() -> list[Spec]:
    files = sorted(SPECS.glob("ps-*.md")) + sorted(COMPLETE.glob("ps-*.md"))
    return [Spec(p, parse_frontmatter(p)) for p in files]


def load_rows(text: str) -> dict[str, dict[str, str]]:
    rows: dict[str, dict[str, str]] = {}
    for line in text.splitlines():
        match = ROW.match(line)
        if match:
            rows[match["id"]] = {k: v.strip() for k, v in match.groupdict().items()}
    return rows


def deps_text(deps: object) -> str:
    return ", ".join(deps) if deps else "—"


def git_mv(src: Path, dst: Path) -> None:
    dst.parent.mkdir(parents=True, exist_ok=True)
    result = subprocess.run(["git", "mv", str(src), str(dst)], cwd=ROADMAP_DIR, capture_output=True, text=True)
    if result.returncode != 0:
        src.rename(dst)


def fix(specs: list[Spec]) -> list[str]:
    actions: list[str] = []
    # Bytes, not read_text: keep the file's line endings exactly as they are
    text = ROADMAP.read_bytes().decode("utf-8")

    for spec in specs:
        closed = spec.meta.get("status") in CLOSED
        in_complete = spec.path.parent == COMPLETE
        if closed != in_complete:
            old_rel = spec.rel
            target = (COMPLETE if closed else SPECS) / spec.path.name
            git_mv(spec.path, target)
            spec.path = target
            text = text.replace(f"]({old_rel})", f"]({spec.rel})")
            actions.append(f"moved {old_rel} -> {spec.rel}")

    by_id = {s.id: s for s in specs}
    lines = text.splitlines(keepends=True)
    for i, line in enumerate(lines):
        match = ROW.match(line.rstrip("\r\n"))
        if not match or match["id"] not in by_id:
            continue
        status = str(by_id[match["id"]].meta.get("status"))
        if match["status"].strip() != status:
            ending = line[len(line.rstrip("\r\n")):]
            lines[i] = line.rstrip("\r\n")[: match.start("status")] + status + " |" + ending
            actions.append(f"{match['id']}: table status {match['status'].strip()} -> {status}")
    text = "".join(lines)

    ROADMAP.write_bytes(text.encode("utf-8"))
    return actions


def check(specs: list[Spec]) -> tuple[list[str], list[str]]:
    errors: list[str] = []
    warnings: list[str] = []
    text = ROADMAP.read_text(encoding="utf-8")
    rows = load_rows(text)
    by_id: dict[str, Spec] = {}

    for spec in specs:
        sid, meta = spec.id, spec.meta
        where = spec.rel
        if not re.fullmatch(r"ps-\d{3}", sid) or not spec.path.name.startswith(sid + "-"):
            errors.append(f"{where}: id '{sid}' does not match the file name")
            continue
        if sid in by_id:
            errors.append(f"{where}: duplicate id {sid} (also {by_id[sid].rel})")
        by_id[sid] = spec

        status = meta.get("status")
        if status not in STATUSES:
            errors.append(f"{where}: unknown status '{status}'")
        in_complete = spec.path.parent == COMPLETE
        if status in CLOSED and not in_complete:
            errors.append(f"{where}: status {status} but not in specs/complete/ (run with --fix)")
        if status not in CLOSED and in_complete:
            errors.append(f"{where}: status {status} but in specs/complete/ (run with --fix)")
        if status == "done" and not meta.get("delivered_in"):
            errors.append(f"{where}: done but delivered_in is empty")
        if status == "in-progress" and not meta.get("spec"):
            warnings.append(f"{where}: in-progress without a spec link")

        row = rows.get(sid)
        if row is None:
            errors.append(f"{sid}: not listed in a roadmap.md table")
            continue
        if row["link"] != where:
            errors.append(f"{sid}: table links {row['link']}, file is {where} (run with --fix)")
        if row["title"] != meta.get("title"):
            errors.append(f"{sid}: title differs (table '{row['title']}', file '{meta.get('title')}')")
        if row["size"] != meta.get("size"):
            errors.append(f"{sid}: size differs (table {row['size']}, file {meta.get('size')})")
        if row["deps"] != deps_text(meta.get("depends_on")):
            errors.append(f"{sid}: depends_on differs (table {row['deps']}, file {deps_text(meta.get('depends_on'))})")
        if row["status"] != status:
            errors.append(f"{sid}: status differs (table {row['status']}, file {status}) (run with --fix)")

    for sid in rows:
        if sid not in by_id:
            errors.append(f"{sid}: in roadmap.md but no spec file exists")

    for spec in by_id.values():
        status = spec.meta.get("status")
        for dep in spec.meta.get("depends_on") or []:
            if dep not in by_id:
                errors.append(f"{spec.id}: depends on unknown {dep}")
            elif status in ("ready", "in-progress") and by_id[dep].meta.get("status") != "done":
                errors.append(f"{spec.id}: {status} but dependency {dep} is {by_id[dep].meta.get('status')}")

    for link in re.findall(r"\]\((specs/[^)#]+)\)", text):
        if not (ROADMAP_DIR / link).exists():
            errors.append(f"roadmap.md: broken link {link}")

    return errors, warnings


def summary(specs: list[Spec]) -> str:
    counts: dict[str, int] = {}
    for spec in specs:
        counts[str(spec.meta.get("status"))] = counts.get(str(spec.meta.get("status")), 0) + 1
    order = ["done", "in-progress", "specifying", "ready", "proposed", "deferred", "dropped"]
    parts = [f"{counts[s]} {s}" for s in order if counts.get(s)]
    return f"{len(specs)} items: " + ", ".join(parts)


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    parser.add_argument("--fix", action="store_true", help="move closed specs, rewrite links, sync table statuses")
    args = parser.parse_args()

    specs = load_specs()
    if args.fix:
        for action in fix(specs):
            print(f"fixed: {action}")
        specs = load_specs()

    errors, warnings = check(specs)
    for warning in warnings:
        print(f"warning: {warning}")
    for error in errors:
        print(f"error: {error}")
    print(summary(specs))
    print("roadmap OK" if not errors else f"{len(errors)} error(s)")
    return 1 if errors else 0


if __name__ == "__main__":
    sys.exit(main())
