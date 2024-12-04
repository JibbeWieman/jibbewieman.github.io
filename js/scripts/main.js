document.addEventListener('DOMContentLoaded', async () => {
    const { setupLazyLoading } = await import('./modules/lazyLoad.js');
    const { setupDynamicFiltering } = await import('./modules/filter.js');

    setupDynamicFiltering();
    setupLazyLoading();
});

document.addEventListener('DOMContentLoaded', function () {
    const tags = document.querySelectorAll('.tag');
    const projectCards = document.querySelectorAll('.project-card');

    tags.forEach(tag => {
        tag.addEventListener('click', function () {
            // Remove active class from all tags
            tags.forEach(tag => tag.classList.remove('active'));

            // Add active class to the clicked tag
            tag.classList.add('active');

            const filter = tag.getAttribute('data-tag');

            projectCards.forEach(card => {
                // Get the tags of the project card
                const cardTags = card.getAttribute('data-tags').split(' ');

                if (filter === 'all' || cardTags.includes(filter)) {
                    card.style.display = 'block'; // Show the card
                } else {
                    card.style.display = 'none'; // Hide the card
                }
            });
        });
    });
});


// Is supposed to underline the current page but doesn't
// Get the current URL path
const currentPath = window.location.pathname;

// Select all navigation links
const navLinks = document.querySelectorAll('nav ul li a');

navLinks.forEach(link => {
    // Check if the link's href matches the current path
    if (link.getAttribute('href').includes(currentPath)) {
        link.classList.add('active');
    }
});