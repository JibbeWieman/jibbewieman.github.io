//Immediately call function on page load
addLoadingScreen();
function addLoadingScreen() {
    // Check if the loading screen has already been shown (using sessionStorage to persist across reloads)
    if (sessionStorage.getItem("loadingScreenShown") === "true") {
        return; // Exit early if the loading screen has already been shown
    }

    // Fetch the loading screen HTML
    fetch('loading-screen.html')
        .then((response) => response.text())
        .then((html) => {
            // Inject the loading screen into the current page
            const loadingContainer = document.createElement('div');
            loadingContainer.innerHTML = html;
            document.body.prepend(loadingContainer);

            const loadingBar = document.getElementById("loading-bar");
            const loadingOverlay = document.getElementById("loading-overlay");
            const loadingText = document.getElementById("loading-text");

            // Update loading text with animated dots
            let dotCount = 1;
            const textInterval = setInterval(() => {
                loadingText.textContent = `LOADING${'.'.repeat(dotCount)}`;
                dotCount = (dotCount % 3) + 1; // Cycle through 1 to 3
            }, 50); // Update every 50ms

            // Track progress as resources load
            let progress = 0;
            const updateProgress = () => {
                progress = Math.round((performance.getEntriesByType("resource").length / totalResources) * 100);
                loadingBar.style.width = progress + "%";

                // If loading is complete, delay for at least 1 second before fading out
                if (progress >= 100) {
                    clearInterval(textInterval); // Stop text animation
                    setTimeout(() => {
                        loadingOverlay.style.opacity = "0"; // Fade out
                        setTimeout(() => {
                            loadingOverlay.style.display = "none"; // Hide after fade-out
                            sessionStorage.setItem("loadingScreenShown", "true"); // Mark the loading screen as shown
                        }, 500); // Wait for fade-out to complete
                    }, 1000); // Ensure the loading screen stays visible for at least 1 second
                }
            };

            // Monitor the loading progress
            const totalResources = performance.getEntriesByType("resource").length;
            const interval = setInterval(() => {
                updateProgress();
                if (progress >= 100) clearInterval(interval); // Stop updating once complete
            }, 100);
        })
        .catch((err) => console.error("Error loading the loading screen:", err));
}

// Call the function on page load
/*document.addEventListener("DOMContentLoaded", addLoadingScreen);*/