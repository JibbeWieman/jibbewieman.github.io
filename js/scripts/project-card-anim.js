class PanelScroll {
    constructor(containerSelector, scrollSpeed = 1) {
        this.container = document.querySelector(containerSelector);
        this.cards = Array.from(this.container.children);
        this.scrollSpeed = scrollSpeed;
        this.isPaused = false;
        this.isDragging = false;
        this.lastMouseX = 0;
        this.cardWidth = this.cards[0].offsetWidth + (1 * window.innerWidth / 100); // 1vw is equivalent to 1% of the viewport width // Include margin
        this.init();
    }

    init() {
        this.positionCards();
        this.container.addEventListener("mousedown", this.startDrag.bind(this));
        document.addEventListener("mousemove", this.handleDrag.bind(this));
        document.addEventListener("mouseup", this.stopDrag.bind(this));

        this.container.addEventListener("mouseenter", () => (this.isPaused = true));
        this.container.addEventListener("mouseleave", () => (this.isPaused = false));

        requestAnimationFrame(this.update.bind(this));
    }

    positionCards() {
        this.cards.forEach((card, index) => {
            card.style.left = `${index * this.cardWidth}px`;
        });
    }

    update() {
        if (!this.isDragging && !this.isPaused) {
            this.cards.forEach(card => {
                let currentX = parseFloat(card.style.left);
                card.style.left = `${currentX - this.scrollSpeed}px`;
            });

            // Check if first card is fully off-screen
            this.checkAndRecycleCards();
        }
        requestAnimationFrame(this.update.bind(this));
    }

    startDrag(event) {
        this.isDragging = true;
        this.lastMouseX = event.clientX;
    }

    handleDrag(event) {
        if (!this.isDragging) return;
        let deltaX = event.clientX - this.lastMouseX;

        // Get the leftmost card's position
        let firstCard = this.cards[0];
        let firstCardLeft = parseFloat(firstCard.style.left);

        // Prevent dragging left beyond the first card's position
        if (firstCardLeft + deltaX > 0) {
            deltaX = -firstCardLeft; // Stop movement at the boundary
        }

        this.cards.forEach(card => {
            let currentX = parseFloat(card.style.left);
            card.style.left = `${currentX + deltaX}px`;
        });

        this.lastMouseX = event.clientX;

        // 🔹 Check for off-screen cards **while dragging**
        this.checkAndRecycleCards();
    }


    checkAndRecycleCards() {
        let firstCard = this.cards[0];
        if (parseFloat(firstCard.style.left) + this.cardWidth < 0) {
            this.recycleCard(firstCard);
        }
    }

    stopDrag() {
        this.isDragging = false;
    }

    recycleCard(card) {
        let lastCard = this.cards[this.cards.length - 1];
        card.style.left = `${parseFloat(lastCard.style.left) + this.cardWidth}px`;

        // Move first card to end of array
        this.cards.push(this.cards.shift());
    }
}

document.addEventListener("DOMContentLoaded", () => {
    if (window.matchMedia("(min-width: 769px)").matches) {
        new PanelScroll(".projects-container", .5);
    }
});

//const container = document.querySelector(".projects-container");
//const cards = document.querySelectorAll(".project-card");

//let isScrolling = false;
//const scrollSpeed = 1; // Adjust speed as needed

//function startScrolling() {
//    isScrolling = true;
//    requestAnimationFrame(scroll);
//}

//function stopScrolling() {
//    isScrolling = false;
//}

//function scroll() {
//    if (!isScrolling) return;

//    container.scrollBy({ left: scrollSpeed, behavior: 'smooth' });

//    // Check if the first card is out of view
//    const firstCard = container.firstElementChild;
//    if (firstCard.getBoundingClientRect().right <= container.getBoundingClientRect().left) {
//        container.appendChild(firstCard); // Move it to the end
//        container.scrollLeft -= firstCard.offsetWidth; // Adjust scroll position
//    }

//    requestAnimationFrame(scroll);
//}

//container.addEventListener("mouseenter", stopScrolling);
//container.addEventListener("mouseleave", startScrolling);

//startScrolling();