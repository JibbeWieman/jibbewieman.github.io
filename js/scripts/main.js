document.addEventListener('DOMContentLoaded', async () => {
    const { setupLazyLoading } = await import('./modules/lazyLoad.js');
    const { setupDynamicFiltering } = await import('./modules/filter.js');

    setupDynamicFiltering();
    setupLazyLoading();
});

