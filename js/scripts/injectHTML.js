// insertHTML.js
if (location.protocol !== 'file:') {
    document.addEventListener('DOMContentLoaded', () => {
        // load the nav
        fetch('nav.html')
            .then(r => r.text())
            .then(html => {
                const slot = document.getElementById('nav-placeholder');
                if (slot) slot.innerHTML = html;
            })
            .catch(err => console.error('nav.html →', err));

        // load the footer
        fetch('footer.html')
            .then(r => r.text())
            .then(html => {
                const slot = document.getElementById('footer-placeholder');
                if (slot) slot.innerHTML = html;
            })
            .catch(err => console.error('footer.html →', err));
    });
} else {
    console.warn('fetch skipped on file://');
}
