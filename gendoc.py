import re
import os

def extract_lua_functions(lua_code):
    # Match functions with optional LuaDoc above
    func_pattern = re.compile(
        r"(?:---@[\s\S]*?)?"          # optional doc comments
        r"function\s+([a-zA-Z0-9_:.]+)\s*\((.*?)\)",  # function name and params
        re.MULTILINE
    )

    # Capture full type info until end of line
    param_pattern = re.compile(r"---@param\s+(\w+)\s+([^\n]+)")
    return_pattern = re.compile(r"---@return\s+([^\n]+)")

    functions = []

    for match in func_pattern.finditer(lua_code):
        full_match = match.group(0)
        name = match.group(1)
        params = [p.strip() for p in match.group(2).split(',')] if match.group(2).strip() else []

        param_docs = {n: t.strip() for n, t in param_pattern.findall(full_match)}
        return_types = [r.strip() for r in return_pattern.findall(full_match)]

        doc_lines = [line.strip()[3:].strip() for line in full_match.splitlines() if line.strip().startswith('---')]
        docstring = "\n".join(doc_lines)

        functions.append({
            'name': name,
            'params': params,
            'param_types': param_docs,
            'return_types': return_types,
            'doc': docstring
        })

    return functions

def generate_md(file_path, functions, lua_folder, output_folder):
    # Generate path inside gen/ preserving folder structure
    rel_path = os.path.relpath(file_path, lua_folder)
    md_path = os.path.join(output_folder, rel_path)
    md_path = md_path.replace('.lua', '.md')
    os.makedirs(os.path.dirname(md_path), exist_ok=True)

    file_name = os.path.basename(file_path)
    with open(md_path, 'w', encoding='utf-8') as f:
        f.write(f"# {file_name}\n\n")
        f.write(f"Source: `{file_path}`\n\n")

        for func in functions:
            f.write(f"## {func['name']}\n\n")
            if func['doc']:
                f.write(f"{func['doc']}\n\n")
            if func['params']:
                f.write("| Parameter | Type |\n")
                f.write("|-----------|------|\n")
                for p in func['params']:
                    type_info = func['param_types'].get(p, 'unknown')
                    f.write(f"| `{p}` | {type_info} |\n")
                f.write("\n")
            if func['return_types']:
                f.write(f"**Returns:** {', '.join(func['return_types'])}\n\n")
            f.write("---\n\n")
    return md_path

def generate_reference_table(md_files, output_file):
    os.makedirs(os.path.dirname(output_file), exist_ok=True)
    with open(output_file, 'w', encoding='utf-8') as f:
        f.write("# MONOE.exe Lua API Reference Table\n\n")
        for md in md_files:
            title = os.path.basename(md).replace('.md', '')
            f.write(f"- [{title}]({os.path.relpath(md, os.path.dirname(output_file))})\n")

def main(lua_folder, output_folder, ref_table_file):
    md_files = []
    for root, _, files in os.walk(lua_folder):
        for file in files:
            if file.endswith('.lua'):
                path = os.path.join(root, file)
                with open(path, 'r', encoding='utf-8') as f:
                    code = f.read()
                funcs = extract_lua_functions(code)
                md_file = generate_md(path, funcs, lua_folder, output_folder)
                md_files.append(md_file)

    generate_reference_table(md_files, ref_table_file)
    print(f"Generated {len(md_files)} MD files in '{output_folder}' and reference table at '{ref_table_file}'")

if __name__ == "__main__":
    lua_folder = "libraries"
    output_folder = "docs/gen"
    ref_table_file = "docs/monoe_lua_api.md"
    main(lua_folder, output_folder, ref_table_file)
