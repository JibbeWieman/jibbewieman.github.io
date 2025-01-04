document.addEventListener("DOMContentLoaded", () => {
    const header = document.querySelector("header.index-header");

    // Get the start time or set it for the first time
    let animationStartTime = sessionStorage.getItem("animationStartTime");
    if (!animationStartTime) {
        animationStartTime = Date.now(); // Save the current time as the animation start time
        sessionStorage.setItem("animationStartTime", animationStartTime);
    }

    // Calculate the elapsed time since the animation started
    const elapsedTime = (Date.now() - animationStartTime) / 1000; // in seconds

    // Set the animation delay based on the elapsed time
    const animationDuration = 15; // Duration of your keyframes animation in seconds
    const animationOffset = elapsedTime % animationDuration;
    header.style.animationDelay = `-${animationOffset}s`;

    // Clear start time if navigating back to avoid reset issues
    window.addEventListener("beforeunload", () => {
        sessionStorage.setItem("animationStartTime", animationStartTime);
    });
});
