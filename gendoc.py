import re
import os
from dataclasses import dataclass, field
from typing import List, Dict, Optional

# -----------------------------
# Data models
# -----------------------------

@dataclass
class Note:
    kind: str  # note | warning | see
    text: str

@dataclass
class Param:
    name: str
    type: str
    description: str

@dataclass
class FunctionDoc:
    name: str
    params: List[Param] = field(default_factory=list)
    returns: List[str] = field(default_factory=list)
    summary: str = ""
    notes: List[Note] = field(default_factory=list)
    is_method: bool = False

@dataclass
class FieldDoc:
    name: str
    type: str
    description: str

@dataclass
class ClassDoc:
    name: str
    summary: str = ""
    fields: List[FieldDoc] = field(default_factory=list)
    functions: List[FunctionDoc] = field(default_factory=list)

# -----------------------------
# LuaDoc parsing
# -----------------------------

CLASS_RE = re.compile(r"---@class\s+([\w\.]+)")
FIELD_RE = re.compile(r"---@field\s+(\w+)\s+([\w\<\>\.\|]+)\s*(.*)")
PARAM_RE = re.compile(r"---@param\s+(\w+)\s+([\w\<\>\.\|]+)\s*(.*)")
RETURN_RE = re.compile(r"---@return\s+(.+)")
NOTE_RE = re.compile(r"---@(?P<kind>note|warning|see)\s+(.*)")
FUNC_RE = re.compile(r"function\s+([\w\.\:]+)\s*\((.*?)\)")

def parse_lua_file(code: str) -> Optional[ClassDoc]:
    lines = code.splitlines()
    class_doc = None
    current_doc = []

    def flush_doc():
        nonlocal current_doc
        doc = current_doc
        current_doc = []
        return doc

    for i, line in enumerate(lines):
        line = line.rstrip()

        if line.startswith('---'):
            current_doc.append(line)
            continue

        # CLASS
        m = CLASS_RE.search("\n".join(current_doc))
        if m and not class_doc:
            class_name = m.group(1)
            class_doc = ClassDoc(name=class_name)

            for l in current_doc:
                if l.startswith('---') and not l.startswith('---@'):
                    class_doc.summary += l[3:].strip() + "\n"

                fm = FIELD_RE.match(l)
                if fm:
                    class_doc.fields.append(FieldDoc(*fm.groups()))

            flush_doc()
            continue

        # FUNCTION
        fm = FUNC_RE.search(line)
        if fm and class_doc:
            name = fm.group(1)
            is_method = ':' in name

            fn = FunctionDoc(name=name, is_method=is_method)

            for l in current_doc:
                if l.startswith('---') and not l.startswith('---@'):
                    fn.summary += l[3:].strip() + "\n"

                pm = PARAM_RE.match(l)
                if pm:
                    fn.params.append(Param(*pm.groups()))

                rm = RETURN_RE.match(l)
                if rm:
                    fn.returns.append(rm.group(1))

                nm = NOTE_RE.match(l)
                if nm:
                    fn.notes.append(Note(nm.group("kind"), nm.group(2)))

            class_doc.functions.append(fn)
            flush_doc()

    return class_doc

def render_markdown(cls: ClassDoc, output_path: str):
    os.makedirs(os.path.dirname(output_path), exist_ok=True)

    with open(output_path, "w", encoding="utf-8") as f:
        f.write(f"# {cls.name}\n\n")
        f.write(cls.summary.strip() + "\n\n")

        if cls.fields:
            f.write("## Properties\n\n")
            for field in cls.fields:
                f.write(f"- **{field.name}** (`{field.type}`): {field.description}\n")
            f.write("\n")

        for fn in cls.functions:
            f.write(f"## {fn.name}\n\n")
            f.write(fn.summary.strip() + "\n\n")

            if fn.params:
                f.write("### Parameters\n\n")
                f.write("| Name | Type | Description |\n")
                f.write("|------|------|-------------|\n")
                for p in fn.params:
                    f.write(f"| `{p.name}` | `{p.type}` | {p.description} |\n")
                f.write("\n")

            if fn.returns:
                f.write("### Returns\n\n")
                for r in fn.returns:
                    f.write(f"- `{r}`\n")
                f.write("\n")

            for note in fn.notes:
                title = note.kind.capitalize()
                f.write(f"> **{title}:** {note.text}\n\n")

            f.write("---\n\n")

def main(
    lua_root: str = "monoelib",
    output_root: str = "docs/gen",
    index_file: str = "docs/monoe_lua_api.md"
):
    generated = []

    for root, _, files in os.walk(lua_root):
        for file in files:
            if not file.endswith(".lua"):
                continue

            lua_path = os.path.join(root, file)

            with open(lua_path, "r", encoding="utf-8") as f:
                code = f.read()

            cls = parse_lua_file(code)
            if not cls:
                continue  # no @class → ignore

            # Preserve folder structure
            rel = os.path.relpath(lua_path, lua_root)
            md_path = os.path.join(output_root, rel).replace(".lua", ".md")

            render_markdown(cls, md_path)
            generated.append((cls.name, md_path))

            print(f"✓ {cls.name} → {md_path}")

    # -----------------------------
    # Generate index table
    # -----------------------------
    os.makedirs(os.path.dirname(index_file), exist_ok=True)
    with open(index_file, "w", encoding="utf-8") as f:
        f.write("# MONOE.exe Lua API Reference\n\n")

        for name, path in sorted(generated):
            rel = os.path.relpath(path, os.path.dirname(index_file))
            f.write(f"- [{name}]({rel})\n")

    print(f"\n✔ Generated {len(generated)} class docs")
    print(f"✔ Index written to {index_file}")

if __name__ == "__main__": main()