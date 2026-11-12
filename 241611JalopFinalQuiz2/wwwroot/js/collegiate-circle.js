// Keep the static pennant row centered on Hogwarts when its viewport changes.
document.querySelectorAll('[data-collegiate-scroll]').forEach(scroll => {
    let lastWidth = 0;
    const resize = new ResizeObserver(() => {
        if (lastWidth === scroll.clientWidth) return;
        lastWidth = scroll.clientWidth;
        scroll.scrollLeft = Math.max(0, (scroll.scrollWidth - scroll.clientWidth) / 2);
    });
    resize.observe(scroll);
});
