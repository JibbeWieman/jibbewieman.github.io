function updateFavicon() {
    const favicon = document.querySelector("link[rel='icon']");
    const darkMode = window.matchMedia("(prefers-color-scheme: dark)").matches;

    favicon.href = darkMode 
        ? "assets/images/icons/personal-icon-dark.png" 
        : "assets/images/icons/personal-icon-light.svg";
}

// Run on page load
updateFavicon();

// Update if the theme preference changes
window.matchMedia("(prefers-color-scheme: dark)").addEventListener("change", updateFavicon);