﻿console.log("Expandable script loaded!");

/* -------------------------------------------------------------
   expandable-sections + lazy code‑snippet loader + line nums
   ----------------------------------------------------------- */

/* ---------- lazy snippet fetcher -------------------------------- */
async function loadSnippet(container, file) {
    // Only load once
    if (container.dataset.loaded) return;

    try {
        // 1) Fetch raw .cs text
        const resp = await fetch(`/assets/snippets/${file}`);
        if (!resp.ok) throw new Error(resp.statusText);
        const rawCode = await resp.text();

        // 2) Escape HTML entities (so highlight.js won't choke)
        const escaped = rawCode
            .replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;");

        // 3) Inject into container
        container.innerHTML = `
      <div class="code-container">
        <pre><code class="language-csharp">${escaped}</code></pre>
      </div>`.trim();

        // Mark as loaded
        container.dataset.loaded = "true";

        // 4) Highlight with Highlight.js
        const codeEl = container.querySelector("pre > code");
        if (window.hljs && codeEl) {
            hljs.highlightElement(codeEl);
        }

        // 5) Add line numbers
        addLineNumbers(container.querySelector(".code-container"));
    }
    catch (err) {
        container.textContent = "Could not load snippet 😞";
        console.error(`Error loading snippet [${file}]:`, err);
    }
}

/* ---------- line-numbering helper -------------------------------- */
function addLineNumbers(codeContainer) {
    const pre = codeContainer.querySelector("pre");
    const code = pre.querySelector("code");
    if (!code) return;

    // Get just the inner HTML of the <code> block (post-highlighted)
    const html = code.innerHTML;

    // Split by lines
    const lines = html.split(/\n/);

    // Clear the <pre> content
    pre.innerHTML = "";

    // Build each line with number + content
    lines.forEach((lineHtml, idx) => {
        const lineDiv = document.createElement("div");
        lineDiv.classList.add("code-line");

        const lineNumber = document.createElement("span");
        lineNumber.classList.add("line-number");
        lineNumber.textContent = idx + 1;

        const content = document.createElement("span");
        content.innerHTML = lineHtml === "" ? "\u200B" : lineHtml;

        lineDiv.appendChild(lineNumber);
        lineDiv.appendChild(content);

        pre.appendChild(lineDiv);
    });
}


/* ---------- expand / collapse ---------------------------------- */
function toggleContent(button, filename = null, event = null) {
    if (event) event.stopPropagation();

    const content = button.nextElementSibling;
    if (!content || !content.classList.contains("expandable-content")) return;

    // ✅ Use getComputedStyle for accurate display check
    const currentDisplay = window.getComputedStyle(content).display;
    const show = currentDisplay === "none";

    content.style.display = show ? "block" : "none";
    button.classList.toggle("flipped", show);

    if (show && filename) loadSnippet(content, filename);
}
//window.toggleContent = toggleContent;

/* ---------- initial setup -------------------------------------- */
document.addEventListener("DOMContentLoaded", () => {
    document.querySelectorAll(".expandable-container").forEach(container => {
        const button = container.querySelector(".expandable-button");
        const content = container.querySelector(".expandable-content");

        if (content) content.style.display = "none";

        /* colour‑hover logic (unchanged) */
        if (button?.dataset.color) {
            const base = button.dataset.color;
            const dark = darkenColor(base, 0.85);
            button.style.background = base;
            button.addEventListener("mouseenter", () => (button.style.background = dark));
            button.addEventListener("mouseleave", () => (button.style.background = base));
        }
    });
});

/* ---------- helpers -------------------------------------------- */
function darkenColor(hex, pct) {
    const num = parseInt(hex.replace("#", ""), 16);
    const r = Math.max(0, (num >> 16) - 255 * (1 - pct));
    const g = Math.max(0, ((num >> 8) & 0xff) - 255 * (1 - pct));
    const b = Math.max(0, (num & 0xff) - 255 * (1 - pct));
    return `rgb(${r | 0},${g | 0},${b | 0})`;
}

/* ---------- image overlay code --------------------------------- */
function createExpandableSection(parentId, buttonText, contentText, imageUrl) {
    let container = document.createElement("div");
    container.classList.add("expandable-container");

    let button = document.createElement("button");
    button.classList.add("expandable-button");
    button.textContent = buttonText;
    button.onclick = function () {
        toggleContent(button);
    };

    // Add the arrow icon to the button
    let img = document.createElement("img");
    img.className = "arrow-icon";
    img.src = "arrow-down.svg";
    img.alt = "Toggle Arrow";
    button.appendChild(img);

    container.appendChild(button);

    let content = document.createElement("div");
    content.classList.add("expandable-content");
    content.style.display = "none"; // Start hidden

    let paragraph = document.createElement("p");
    paragraph.textContent = contentText;
    content.appendChild(paragraph);

    if (imageUrl) {
        let image = document.createElement("img");
        image.src = imageUrl;
        image.alt = "Image";
        image.onclick = function () {
            openOverlay(this);
        };
        content.appendChild(image);
    }

    container.appendChild(content);
    document.getElementById(parentId).appendChild(container);
}


// Image enlargement overlay for expandable sections
window.openOverlay = function (imgElement) {
    const overlay = document.getElementById("image-overlay");
    const overlayImg = document.getElementById("overlay-image");

    overlayImg.src = imgElement.src;
    overlay.style.display = "flex";

    // Lock page scrolling
    document.body.style.overflow = "hidden";

    // Reset zoom state
    overlayImg.classList.remove("zoomed");
    overlayImg.style.cursor = "zoom-in";
    overlayImg.style.transform = "scale(1)";
    overlayImg.style.transformOrigin = "center";
    overlayImg.style.left = "0px";
    overlayImg.style.top = "0px";
    isZoomed = false;
    scrollX = 0;
    scrollY = 0;

    // Remove previous event listeners to prevent duplication
    overlayImg.onclick = null;
    overlay.onclick = null;
    document.onkeydown = null;
    overlay.onwheel = null;

    // Toggle zoom-in and zoom-out
    overlayImg.onclick = function (event) {
        if (isZoomed) {
            resetZoom();
        } else {
            zoomImage(event);
        }
        event.stopPropagation();
    };

    // Close overlay when clicking outside the image
    overlay.onclick = function (event) {
        if (event.target === overlay) {
            closeOverlay();
        }
    };

    // Close on Escape key
    document.onkeydown = function (event) {
        if (event.key === "Escape") {
            closeOverlay();
        }
    };

    // Enable scrolling to pan when zoomed (global listener)
    overlay.onwheel = handleScrollPan;
};

// Track zoom & scroll state
let isZoomed = false;
let scrollX = 0;
let scrollY = 0;
const scrollSpeed = 20; // Adjust panning speed

// Zoom at the click position
function zoomImage(event) {
    const overlayImg = document.getElementById("overlay-image");
    const rect = overlayImg.getBoundingClientRect();

    // Calculate relative click position
    const offsetX = (event.clientX - rect.left) / rect.width;
    const offsetY = (event.clientY - rect.top) / rect.height;

    overlayImg.classList.add("zoomed");
    overlayImg.style.cursor = "zoom-out";
    overlayImg.style.transform = "scale(2)";  // Adjust scale as needed
    overlayImg.style.transformOrigin = `${offsetX * 100}% ${offsetY * 100}%`;

    isZoomed = true;

    // Reset scroll position
    scrollX = 0;
    scrollY = 0;
    updateImagePosition();
}

// Reset zoom
function resetZoom() {
    const overlayImg = document.getElementById("overlay-image");
    overlayImg.classList.remove("zoomed");
    overlayImg.style.cursor = "zoom-in";
    overlayImg.style.transform = "scale(1)";
    overlayImg.style.left = "0px";
    overlayImg.style.top = "0px";
    isZoomed = false;
    scrollX = 0;
    scrollY = 0;
}

// Handle scrolling to pan when zoomed in
function handleScrollPan(event) {
    if (!isZoomed) return;

    event.preventDefault(); // Prevent default page scrolling

    const overlayImg = document.getElementById("overlay-image");
    const parentRect = overlayImg.parentElement.getBoundingClientRect();
    const scaleFactor = 2; // Same as zoom scale

    // Get the scaled image size
    const scaledWidth = overlayImg.naturalWidth * scaleFactor;
    const scaledHeight = overlayImg.naturalHeight * scaleFactor;

    // Calculate max scroll limits
    const maxScrollX = Math.max(0, (scaledWidth - parentRect.width) / 2);
    const maxScrollY = Math.max(0, (scaledHeight - parentRect.height) / 2);

    // Update scroll position within limits
    scrollX = Math.max(-maxScrollX, Math.min(maxScrollX, scrollX - event.deltaX * scrollSpeed));
    scrollY = Math.max(-maxScrollY, Math.min(maxScrollY, scrollY - event.deltaY * scrollSpeed));

    updateImagePosition();
}

// Apply the updated scroll position
function updateImagePosition() {
    const overlayImg = document.getElementById("overlay-image");
    overlayImg.style.left = `${scrollX}px`;
    overlayImg.style.top = `${scrollY}px`;
}

// Close overlay
function closeOverlay() {
    const overlay = document.getElementById("image-overlay");
    resetZoom();
    overlay.style.display = "none";

    // Restore page scrolling
    document.body.style.overflow = "";
}
