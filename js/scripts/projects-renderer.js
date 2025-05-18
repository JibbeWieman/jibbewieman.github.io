import { projects } from "./projects-data.js";

const tagHTML = tag => `<span class="tag ${tag.toLowerCase()}" data-tag="${tag}">${tag}</span>`;

function cardHTML(p) {
    return `
    <div class="project-card" data-tags="${p.tags.join(" ")}">
        <a href="${p.slug}.html">
            <img src="${p.banner}" alt="${p.title} banner" loading="lazy">
            <div>
                <h3>${p.title}</h3>
                <p>${p.blurb}</p>
                <div class="tags">${p.tags.map(tagHTML).join("")}</div>
            </div>
        </a>
    </div>`;
}

export function renderProjects() {
    const container = document.querySelector(".projects-container");
    if (!container) return; // Exit if the container is missing

    container.innerHTML = projects.map(cardHTML).join("");
}
