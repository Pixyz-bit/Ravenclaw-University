// Manual navigation loads each large background on demand.
const heroGallery = document.querySelector('[data-hero-gallery]');
if (heroGallery) {
    const header = document.querySelector('.portal-header');
    const syncHeight = () => heroGallery.style.setProperty('--portal-header-height', `${header.getBoundingClientRect().height}px`);
    syncHeight();
    const headerObserver = new ResizeObserver(syncHeight);
    headerObserver.observe(header);
    window.addEventListener('pagehide', () => headerObserver.disconnect(), { once: true });
    const slides = [...heroGallery.querySelectorAll('.hero-slide')];
    const controls = heroGallery.querySelector('.hero-controls');
    const buttons = [...controls.querySelectorAll('[data-slide]')];
    const status = controls.querySelector('[data-hero-status]');
    let requested = 0;
    const show = async index => {
        requested = index;
        const slide = slides[index];
        slide.src = slide.dataset.src;
        try {
            await slide.decode();
            if (requested !== index) return;
            slides.forEach((image, i) => image.classList.toggle('is-current', i === index));
            buttons.forEach((button, i) => button.setAttribute('aria-pressed', String(i === index)));
            status.textContent = buttons[index].getAttribute('aria-label').replace(/^Show /, '') + ' background selected.';
        } catch {
            if (requested === index) status.textContent = 'This background could not be loaded. Please choose another image.';
        }
    };
    buttons.forEach((button, i) => button.addEventListener('click', () => show(i)));
    controls.addEventListener('keydown', event => {
        if (!['ArrowLeft', 'ArrowRight', 'Home', 'End'].includes(event.key)) return;
        event.preventDefault();
        const current = buttons.indexOf(document.activeElement);
        const next = event.key === 'Home' ? 0 : event.key === 'End' ? buttons.length - 1
            : (current + (event.key === 'ArrowRight' ? 1 : -1) + buttons.length) % buttons.length;
        buttons[next].focus();
        show(next);
    });
    controls.hidden = false;
}
