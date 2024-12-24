document.addEventListener('DOMContentLoaded', function () {
    const tags = document.querySelectorAll('.tags-container .tag'); // External tags
    const projectTags = document.querySelectorAll('.project-card .tag'); // Internal tags
    const projectCards = document.querySelectorAll('.project-card');

    // Reusable filtering function
    function filterProjects(filter) {
        projectCards.forEach(card => {
            const cardTags = card.getAttribute('data-tags').split(' ');

            if (filter === 'all' || cardTags.includes(filter)) {
                card.style.display = 'block'; // Show the card
            } else {
                card.style.display = 'none'; // Hide the card
            }
        });
    }

    // Add click event to external tags
    tags.forEach(tag => {
        tag.addEventListener('click', function () {
            // Remove active class from all tags
            tags.forEach(tag => tag.classList.remove('active'));

            // Add active class to the clicked tag
            tag.classList.add('active');

            const filter = tag.getAttribute('data-tag');
            filterProjects(filter);
        });
    });

    // Add click event to project card tags
    projectTags.forEach(tag => {
        tag.addEventListener('click', function () {
            const filter = tag.getAttribute('data-tag');

            // Trigger the same logic as external tags
            tags.forEach(tag => tag.classList.remove('active'));
            const matchingExternalTag = [...tags].find(t => t.getAttribute('data-tag') === filter);
            if (matchingExternalTag) {
                matchingExternalTag.classList.add('active');
            }

            filterProjects(filter);
        });
    });
});

// Highlight current page in the navbar
const currentPath = window.location.pathname;
const navLinks = document.querySelectorAll('nav ul li a');

navLinks.forEach(link => {
    if (link.getAttribute('href').includes(currentPath)) {
        link.classList.add('active');
    }
});
