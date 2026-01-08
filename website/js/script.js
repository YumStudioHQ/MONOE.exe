async function loadVersion() {
  try {
    const base = window.location.pathname.replace(/\/index\.html$/, "");
    const res = await fetch(`${base}/project.godot`);
    const text = await res.text();

    const match = text.match(/config\/version\s*=\s*"([^"]+)"/);
    const version = match ? match[1] : "unknown";

    document.getElementById("version").textContent =
      `v${version}`;
  } catch (e) {
    document.getElementById("version").textContent =
      "version unavailable";
  }
}

async function loadReadme() {
  const container = document.getElementById("readme");

  try {
    const res = await fetch("./README.md");
    const md = await res.text();

    container.classList.remove("loading");
    container.innerHTML = marked.parse(md);
  } catch (e) {
    container.textContent = "Failed to load README.md";
  }
}

loadVersion();
loadReadme();
