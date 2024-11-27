// lazyLoad.js

/**
 * Sets up lazy loading for images.
 * Observes elements with `data-src` attribute and loads them when in view.
 */
function setupLazyLoading() {
    const lazyImages = document.querySelectorAll('img[data-src]');
    const imageObserver = new IntersectionObserver((entries, observer) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                const img = entry.target;
                img.src = img.getAttribute('data-src');
                img.removeAttribute('data-src');
                observer.unobserve(img);
            }
        });
    });

    lazyImages.forEach(image => {
        imageObserver.observe(image);
    });
}

// Call this function when the DOM is ready or as part of initialisation
setupLazyLoading();