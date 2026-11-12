// Share the measured sticky-navbar height with hero sizing and section offsets.
const viewportHero = document.querySelector('[data-viewport-hero]');
const portalHeader = document.querySelector('.portal-header');
if (viewportHero && portalHeader) {
    const syncHeight = () => {
        const height = `${portalHeader.getBoundingClientRect().height}px`;
        document.documentElement.style.setProperty('--portal-header-height', height);
        viewportHero.style.setProperty('--portal-header-height', height);
    };
    syncHeight();
    const observer = new ResizeObserver(syncHeight);
    observer.observe(portalHeader);
    window.addEventListener('pagehide', () => observer.disconnect(), { once: true });
}
