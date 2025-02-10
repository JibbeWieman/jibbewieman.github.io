function toggleContent(button) {
    let content = button.nextElementSibling;

    if (content && content.classList.contains("expandable-content")) {
        // Toggle display between 'none' and 'block'
        content.style.display = content.style.display === "none" ? "block" : "none";
    }
}

// Ensure all expandable contents start hidden
document.addEventListener("DOMContentLoaded", () => {
    document.querySelectorAll(".expandable-content").forEach(content => {
        content.style.display = "none";
    });
});


// Function to dynamically create expandable sections
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
