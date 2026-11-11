// Keep failed content images readable without changing or fetching database content.
document.querySelectorAll('[data-content-image]').forEach(image => {
    const showFallback = () => {
        image.hidden = true;
        const fallback = image.parentElement.querySelector('[data-image-fallback]');
        if (fallback) fallback.hidden = false;
    };
    image.addEventListener('error', showFallback);
    if (image.complete && image.naturalWidth === 0) showFallback();
});
