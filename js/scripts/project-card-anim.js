const container = document.querySelector(".projects-container");
const cards = document.querySelectorAll(".project-card");

let isScrolling = false;
const scrollSpeed = 1; // Adjust speed as needed

function startScrolling() {
    isScrolling = true;
    requestAnimationFrame(scroll);
}

function stopScrolling() {
    isScrolling = false;
}

function scroll() {
    if (!isScrolling) return;

    container.scrollBy({ left: scrollSpeed, behavior: 'smooth' });

    // Check if the first card is out of view
    const firstCard = container.firstElementChild;
    if (firstCard.getBoundingClientRect().right <= container.getBoundingClientRect().left) {
        container.appendChild(firstCard); // Move it to the end
        container.scrollLeft -= firstCard.offsetWidth; // Adjust scroll position
    }

    requestAnimationFrame(scroll);
}

container.addEventListener("mouseenter", stopScrolling);
container.addEventListener("mouseleave", startScrolling);

startScrolling();