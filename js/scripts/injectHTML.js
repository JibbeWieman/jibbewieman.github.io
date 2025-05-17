/* insertHTML.js — inject nav, footer, projects and tech‑stack section */

import { renderProjects } from "/js/scripts/projects-renderer.js";

const snippets = {
    nav: { placeholder: "nav-placeholder", file: "nav.html" },
    foot: { placeholder: "footer-placeholder", file: "footer.html" },
    tech: { placeholder: "technical-experience", file: "technical-experience.html" }
    //  ↑ create this tiny HTML fragment once and keep it beside nav/footer
};

if (location.protocol === "file:") {
    console.warn("insertHTML: fetch skipped (file:// protocol)");
} else {
    document.addEventListener("DOMContentLoaded", () => {

        /* helper that fetches a fragment and drops it into its slot */
        const inject = ({ placeholder, file }) => {
            const slot = document.getElementById(placeholder);
            if (!slot) return;                            // page doesn’t need it
            fetch(file, { cache: "no-cache" })
                .then(r => r.text())
                .then(html => (slot.innerHTML = html))
                .catch(err => console.error(`${file} →`, err));
        };

        // nav + footer (always present)
        inject(snippets.nav);
        inject(snippets.foot);

        // tech experience (only if that id exists on this page)
        inject(snippets.tech);

        // render the project cards
        renderProjects();
    });
}
