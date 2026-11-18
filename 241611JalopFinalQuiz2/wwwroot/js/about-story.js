// Vertical scroll selects the same sticky chapters on desktop and mobile.
const aboutStory = document.querySelector('[data-about-story]');
if (aboutStory) {
    const reduced = matchMedia('(prefers-reduced-motion: reduce)');
    const markers = [...aboutStory.querySelectorAll('[data-about-marker]')];
    const copies = [...aboutStory.querySelectorAll('[data-about-copy]')];
    const images = [...aboutStory.querySelectorAll('[data-about-image]')];
    let pending = false;
    const update = () => {
        pending = false;
        let active = 0;
        markers.forEach((marker, index) => {
            if (marker.getBoundingClientRect().top <= innerHeight * .5) active = index;
        });
        copies.forEach((copy, index) => {
            copy.classList.toggle('is-active', index === active);
            copy.inert = index !== active;
            copy.setAttribute('aria-hidden', String(index !== active));
        });
        images.forEach((picture, index) => {
            picture.classList.toggle('is-active', index === active);
            picture.setAttribute('aria-hidden', String(index !== active));
        });
    };
    const schedule = () => { if (!pending) { pending = true; requestAnimationFrame(update); } };
    aboutStory.classList.add('is-enhanced');
    addEventListener('scroll', schedule, { passive: true });
    addEventListener('resize', schedule, { passive: true });
    addEventListener('pageshow', schedule);
    document.querySelectorAll('[data-about-jump]').forEach(link => link.addEventListener('click', event => {
        const index = Number(link.dataset.aboutJump);
        if (!Number.isInteger(index) || index < 0 || index >= markers.length) return;
        event.preventDefault();
        scrollTo({ top: scrollY + markers[index].getBoundingClientRect().top - innerHeight / 2 + 2,
            behavior: reduced.matches ? 'instant' : 'smooth' });
    }));
    update();
}
