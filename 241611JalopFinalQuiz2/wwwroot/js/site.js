// Keep failed content images readable without changing or fetching database content.
const siteNavigation = document.querySelector('.site-navigation');
if (siteNavigation) {
    const desktop = window.matchMedia('(min-width: 1001px)');
    const updateNavigation = () => { siteNavigation.open = desktop.matches; };
    updateNavigation();
    desktop.addEventListener('change', updateNavigation);
    siteNavigation.querySelectorAll('a').forEach(link => link.addEventListener('click', () => {
        if (!desktop.matches) siteNavigation.open = false;
    }));
}
const accountMenu = document.querySelector('.account-menu');
if (accountMenu) {
    document.addEventListener('click', event => { if (!accountMenu.contains(event.target)) accountMenu.open = false; });
    document.addEventListener('keydown', event => {
        if (event.key === 'Escape' && accountMenu.open) {
            accountMenu.open = false;
            accountMenu.querySelector('summary').focus();
        }
    });
}

document.querySelectorAll('[data-content-image]').forEach(image => {
    const showFallback = () => {
        image.hidden = true;
        const fallback = image.parentElement.querySelector('[data-image-fallback]');
        if (fallback) fallback.hidden = false;
    };
    image.addEventListener('error', showFallback);
    if (image.complete && image.naturalWidth === 0) showFallback();
});
