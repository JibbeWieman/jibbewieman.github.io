// ui.js

/**
 * Sets up dynamic filtering for elements.
 * Filters project cards based on selected tags.
 */
export function setupDynamicFiltering() {
    const tags = document.querySelectorAll('.tag');
    const projectCards = document.querySelectorAll('.project-card');

    tags.forEach(tag => {
        tag.addEventListener('click', () => {
            const selectedTag = tag.getAttribute('data-tag');

            projectCards.forEach(card => {
                const tags = card.getAttribute('data-tags');
                card.classList.toggle('hidden', !(selectedTag === 'all' || tags.includes(selectedTag)));
            });

            // Highlight selected tag
            tags.forEach(t => t.classList.toggle('active', t === tag));
        });
    });
}


// Call this function when the DOM is ready or as part of initialisation
//setupDynamicFiltering();