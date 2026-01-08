import re
import os
from dataclasses import dataclass, field
from typing import List, Optional
import shutil

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

def render_html(cls: ClassDoc, output_path: str, css_path: str = "/website/style/style.css"):
    os.makedirs(os.path.dirname(output_path), exist_ok=True)

    def esc(s: str) -> str:
        return (
            s.replace("&", "&amp;")
             .replace("<", "&lt;")
             .replace(">", "&gt;")
        )

    with open(output_path, "w", encoding="utf-8") as f:
        f.write(f"""<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="utf-8">
  <title>{cls.name} – Lua API</title>
  <link rel="stylesheet" href="{css_path}">
</head>
<body>

<div class="topbar">
  <div class="title">
    <h1><a href="/docs/html/index.html">reference</a> {cls.name}</h1>
    <div class="version">Lua API Reference</div>
  </div>
</div>

<main>
  <div class="panel">
""")

        # Summary
        if cls.summary.strip():
            f.write(f"<p>{esc(cls.summary.strip())}</p>\n")

        # Fields
        if cls.fields:
            f.write("<h2>Properties</h2>\n<table>\n")
            f.write("<tr><th>Name</th><th>Type</th><th>Description</th></tr>\n")
            for field in cls.fields:
                f.write(
                    f"<tr>"
                    f"<td><code>{field.name}</code></td>"
                    f"<td><code>{field.type}</code></td>"
                    f"<td>{esc(field.description)}</td>"
                    f"</tr>\n"
                )
            f.write("</table>\n")

        # Functions
        for fn in cls.functions:
            f.write(f"<h2>{fn.name}</h2>\n")

            if fn.summary.strip():
                f.write(f"<p>{esc(fn.summary.strip())}</p>\n")

            if fn.params:
                f.write("<h3>Parameters</h3>\n<table>\n")
                f.write("<tr><th>Name</th><th>Type</th><th>Description</th></tr>\n")
                for p in fn.params:
                    f.write(
                        f"<tr>"
                        f"<td><code>{p.name}</code></td>"
                        f"<td><code>{p.type}</code></td>"
                        f"<td>{esc(p.description)}</td>"
                        f"</tr>\n"
                    )
                f.write("</table>\n")

            if fn.returns:
                f.write("<h3>Returns</h3>\n<ul>\n")
                for r in fn.returns:
                    f.write(f"<li><code>{esc(r)}</code></li>\n")
                f.write("</ul>\n")

            for note in fn.notes:
                title = note.kind.capitalize()
                f.write(
                    f"<blockquote>"
                    f"<strong>{title}:</strong> {esc(note.text)}"
                    f"</blockquote>\n"
                )

            f.write("<hr>\n")

        f.write("""
  </div>
</main>

<footer>
  Generated by MONOE LuaDoc
</footer>

</body>
</html>
""")

def render_html_index(generated, output_path: str, css_path: str = "/website/style/style.css"):
    os.makedirs(os.path.dirname(output_path), exist_ok=True)

    with open(output_path, "w", encoding="utf-8") as f:
        f.write(f"""<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="utf-8">
  <title>MONOE Lua API</title>
  <link rel="stylesheet" href="{css_path}">
</head>
<body>

<header class="topbar">
  <a href="/index.html">
    <img src="/icon.png" alt="MONOE.exe Logo" class="logo" />
  </a>
  <div class="title">
    <h1>MONOE.exe — API Reference</h1>
  </div>
</header>

<main>
  <div class="panel">
    <h2>Classes</h2>
    <ul>
""")

        for name, path in sorted(generated):
            rel = os.path.relpath(path, os.path.dirname(output_path))
            f.write(f'<li><a href="{rel}">{name}</a></li>\n')

        f.write("""
    </ul>
  </div>
</main>

<footer>
  Generated by MONOE LuaDoc
</footer>
<script src="/website/js/script.js"></script>
</body>
</html>
""")

def main(
    lua_root: str = "monoelib",
    output_md_root: str = "docs/gen",
    output_html_root: str = "docs/html",
    index_md_file: str = "docs/monoe_lua_api.md",
    index_html_file: str = "docs/html/index.html",
    generate_md: bool = True,
    generate_html: bool = True,
):
    generated_md = []
    generated_html = []

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

            rel = os.path.relpath(lua_path, lua_root)

            # -----------------------------
            # Markdown output
            # -----------------------------
            if generate_md:
                md_path = os.path.join(output_md_root, rel).replace(".lua", ".md")
                render_markdown(cls, md_path)
                generated_md.append((cls.name, md_path))
                print(f"✓ MD {cls.name} → {md_path}")

            # -----------------------------
            # HTML output
            # -----------------------------
            if generate_html:
                html_path = os.path.join(output_html_root, rel).replace(".lua", ".html")
                # CSS path relative to each HTML file
                depth = len(rel.split(os.sep)) - 1
                css_path = "../" * depth + "style.css" if depth > 0 else "style.css"
                render_html(cls, html_path, css_path=css_path)
                generated_html.append((cls.name, html_path))
                print(f"✓ HTML {cls.name} → {html_path}")

    # -----------------------------
    # Generate index files
    # -----------------------------
    if generate_md:
        os.makedirs(os.path.dirname(index_md_file), exist_ok=True)
        with open(index_md_file, "w", encoding="utf-8") as f:
            f.write("# MONOE.exe Lua API Reference\n\n")
            for name, path in sorted(generated_md):
                rel_path = os.path.relpath(path, os.path.dirname(index_md_file))
                f.write(f"- [{name}]({rel_path})\n")
        print(f"✔ Markdown index written to {index_md_file}")

    if generate_html:
        render_html_index(generated_html, index_html_file)
        # Copy CSS file to the output HTML directory
        css_source = "website/style/style.css"
        css_destination = os.path.join(output_html_root, "style.css")
        os.makedirs(os.path.dirname(css_destination), exist_ok=True)
        if os.path.exists(css_source):
            with open(css_source, "r", encoding="utf-8") as src, open(css_destination, "w", encoding="utf-8") as dst:
                dst.write(src.read())
            print(f"✔ Copied CSS to {css_destination}")
        else:
            print(f"✗ CSS file not found at {css_source}")
        print(f"✔ HTML index written to {index_html_file}")
        

    print(f"\n✔ Generated {len(generated_md)} Markdown docs" if generate_md else "")
    print(f"✔ Generated {len(generated_html)} HTML docs" if generate_html else "")

    shutil.make_archive("build/gen/docs-html", 'zip', "docs/html")
    print(f"✔ ziped HTML docs" if generate_html else "")
    shutil.make_archive("build/gen/docs-md", 'zip', "docs/gen")
    print(f"✔ ziped MD docs" if generate_html else "")

if __name__ == "__main__": main()