document.addEventListener("DOMContentLoaded", () => {
    console.log("📢 Waiting for nav to load...");

    // Check localStorage for the saved theme and apply it
    if (localStorage.getItem("theme") === "dark") {
        document.body.classList.add("dark-mode");
        console.log("🌙 Dark mode applied from localStorage");
    }

    // Create an observer to watch for changes in #nav-placeholder
    const observer = new MutationObserver(() => {
        const toggleButton = document.getElementById("theme-toggle");
        if (toggleButton) {
            console.log("✅ Theme toggle button found! Adding event listener.");

            toggleButton.addEventListener("click", () => {
                document.body.classList.toggle("dark-mode");
                const isDarkMode = document.body.classList.contains("dark-mode");

                // Save the theme preference to localStorage
                localStorage.setItem("theme", isDarkMode ? "dark" : "light");

                console.log(`🌗 Theme toggled! Now in ${isDarkMode ? "Dark" : "Light"} mode`);
            });

            observer.disconnect(); // Stop observing once we've found and initialized the button
        }
    });

    // Start observing changes inside #nav-placeholder
    const navPlaceholder = document.getElementById("nav-placeholder");
    if (navPlaceholder) {
        observer.observe(navPlaceholder, { childList: true, subtree: true });
    } else {
        console.error("❌ #nav-placeholder not found! Make sure it's in your HTML.");
    }
});
