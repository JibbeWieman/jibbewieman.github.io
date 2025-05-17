
import { renderProjects } from "/js/scripts/projects-renderer.js";  

if (location.protocol === "file:") {
    console.warn("insertHTML: fetch skipped (file:// protocol)");
} else {
    document.addEventListener("DOMContentLoaded", () => {
        // nav
        fetch("nav.html")
            .then(r => r.text())
            .then(html => {
                const slot = document.getElementById("nav-placeholder");
                if (slot) slot.innerHTML = html;
            })
            .catch(err => console.error("nav.html →", err));

        // footer
        fetch("footer.html")
            .then(r => r.text())
            .then(html => {
                const slot = document.getElementById("footer-placeholder");
                if (slot) slot.innerHTML = html;
            })
            .catch(err => console.error("footer.html →", err));

        // projects
        renderProjects();          // <-- call the renderer once the DOM is ready
    });
}
