document.addEventListener('DOMContentLoaded', () => {
    import { setupLazyLoading } from './lazyLoad.js';
    import { setupDynamicFiltering } from './filter.js';

    document.addEventListener('DOMContentLoaded', () => {
        setupDynamicFiltering();
        setupLazyLoading();
    });

});
