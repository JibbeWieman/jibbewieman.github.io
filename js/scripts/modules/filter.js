// ui.js

/**
 * Sets up dynamic filtering for elements.
 * Filters project cards based on selected tags.
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

// Call this function when the DOM is ready or as part of initialisation
setupDynamicFiltering();