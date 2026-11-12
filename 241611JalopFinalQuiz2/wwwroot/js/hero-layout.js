// Keep the navbar and static hero together within the available viewport.
const viewportHero = document.querySelector('[data-viewport-hero]');
const portalHeader = document.querySelector('.portal-header');
if (viewportHero && portalHeader) {
    const syncHeight = () => viewportHero.style.setProperty('--portal-header-height', `${portalHeader.getBoundingClientRect().height}px`);
    syncHeight();
    const observer = new ResizeObserver(syncHeight);
    observer.observe(portalHeader);
    window.addEventListener('pagehide', () => observer.disconnect(), { once: true });
}
