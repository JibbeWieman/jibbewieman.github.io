document.addEventListener('DOMContentLoaded', () => {
    // Existing initialisation logic in main.js
    if (typeof initExistingFeatures === 'function') {
        initExistingFeatures(); // Hypothetical example
    }

    // Add dynamic filtering
    setupDynamicFiltering();

    // Add lazy loading
    setupLazyLoading();
});

/**
 * Sets up dynamic filtering for project cards.
 */
function setupDynamicFiltering() {
    document.querySelectorAll('.tag, .tags span').forEach(tag => {
        tag.addEventListener('click', () => {
            const selectedTag = tag.getAttribute('data-tag');
            document.querySelectorAll('.project-card').forEach(card => {
                if (selectedTag === 'all' || card.getAttribute('data-tags').includes(selectedTag)) {
                    card.style.display = 'block';
                } else {
                    card.style.display = 'none';
                }
            });
        });
    });
}

/**
 * Sets up lazy loading for images.
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
