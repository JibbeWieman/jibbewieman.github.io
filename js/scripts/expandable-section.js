function toggleContent(button) {
    let content = button.nextElementSibling;

    if (content && content.classList.contains("expandable-content")) {
        // Toggle display between 'none' and 'block'
        content.style.display = content.style.display === "none" ? "block" : "none";
    }
}

document.addEventListener("DOMContentLoaded", () => {
    document.querySelectorAll(".expandable-container").forEach(container => {
        let button = container.querySelector(".expandable-button");
        let content = container.querySelector(".expandable-content");

        if (content) {
            content.style.display = "none"; // Ensure content is hidden initially
        }

        if (button) {
            let color = button.dataset.color;
            if (color) {
                button.style.background = color;

                // Generate and apply darker color on hover
                let darkenedColor = darkenColor(color, 0.85);

                button.addEventListener("mouseenter", () => {
                    button.style.background = darkenedColor;
                });

                button.addEventListener("mouseleave", () => {
                    button.style.background = color;
                });
            }
        }
    });
});

function darkenColor(hex, percent) {
    let num = parseInt(hex.replace("#", ""), 16);
    let r = Math.max(0, (num >> 16) - (255 * (1 - percent)));
    let g = Math.max(0, ((num >> 8) & 0x00FF) - (255 * (1 - percent)));
    let b = Math.max(0, (num & 0x0000FF) - (255 * (1 - percent)));
    return `rgb(${Math.round(r)}, ${Math.round(g)}, ${Math.round(b)})`;
}

function createExpandableSection(parentId, buttonText, contentText, imageUrl) {
    let container = document.createElement("div");
    container.classList.add("expandable-container");

    let button = document.createElement("button");
    button.classList.add("expandable-button");
    button.textContent = buttonText;
    button.onclick = function () {
        toggleContent(button);
    };
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
        content.appendChild(image);
    }

    container.appendChild(content);
    document.getElementById(parentId).appendChild(container);
}
