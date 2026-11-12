// Overlay the hero transparently; retain measured navbar offsets for later sections.
const viewportHero = document.querySelector('[data-viewport-hero]');
const portalHeader = document.querySelector('.portal-header');
if (viewportHero && portalHeader) {
    document.body.classList.add('has-viewport-hero');
    viewportHero.style.setProperty('--portal-header-height', '0px');
    const syncAppearance = () => {
        // Include the anchor's 16px breathing room, so About links reveal solid navy.
        const boundary = portalHeader.getBoundingClientRect().height + 17;
        portalHeader.classList.toggle('is-over-hero', viewportHero.getBoundingClientRect().bottom > boundary);
    };
    const syncHeight = () => {
        const height = `${portalHeader.getBoundingClientRect().height}px`;
        document.documentElement.style.setProperty('--portal-header-height', height);
        syncAppearance();
    };
    syncHeight();
    const observer = new ResizeObserver(syncHeight);
    observer.observe(portalHeader);
    let pending = false;
    const onScroll = () => {
        if (pending) return;
        pending = true;
        requestAnimationFrame(() => { pending = false; syncAppearance(); });
    };
    window.addEventListener('scroll', onScroll, { passive: true });
    window.addEventListener('resize', syncAppearance, { passive: true });
    window.addEventListener('pageshow', syncAppearance);
    window.addEventListener('pagehide', event => {
        if (event.persisted) return;
        observer.disconnect();
        window.removeEventListener('scroll', onScroll);
        window.removeEventListener('resize', syncAppearance);
        window.removeEventListener('pageshow', syncAppearance);
    });
}
