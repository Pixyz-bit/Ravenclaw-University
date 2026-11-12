// Keep failed content images readable without changing or fetching database content.
const siteNavigation = document.querySelector('.site-navigation');
if (siteNavigation) {
    const desktop = window.matchMedia('(min-width: 1001px)');
    const updateNavigation = () => { siteNavigation.open = desktop.matches; };
    updateNavigation();
    desktop.addEventListener('change', updateNavigation);
    siteNavigation.querySelectorAll('a').forEach(link => link.addEventListener('click', event => {
        if (!desktop.matches) {
            siteNavigation.open = false;
            // Resolve the fragment against the collapsed menu height, before scrolling.
            const header = siteNavigation.closest('.portal-header');
            const height = `${header.getBoundingClientRect().height}px`;
            document.documentElement.style.setProperty('--portal-header-height', height);
            if (!document.body.classList.contains('has-viewport-hero')) {
                document.querySelector('[data-viewport-hero]')?.style.setProperty('--portal-header-height', height);
            }
            const destination = new URL(link.href);
            const target = destination.hash && document.getElementById(destination.hash.slice(1));
            if (target && destination.pathname === location.pathname && destination.search === location.search) {
                event.preventDefault();
                requestAnimationFrame(() => {
                    history.pushState(null, '', destination.hash);
                    target.scrollIntoView({block:'start'});
                });
            }
        }
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
