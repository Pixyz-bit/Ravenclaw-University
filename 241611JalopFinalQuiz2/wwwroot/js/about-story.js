const aboutStory = document.querySelector('[data-about-story]');
if (aboutStory) {
    const desktop = window.matchMedia('(min-width: 1024px)');
    const reduced = window.matchMedia('(prefers-reduced-motion: reduce)');
    const markers = [...aboutStory.querySelectorAll('[data-about-marker]')];
    const copies = [...aboutStory.querySelectorAll('[data-about-copy]')];
    const images = [...aboutStory.querySelectorAll('[data-about-image]')];
    let pending = false;
    const update = () => {
        pending = false;
        aboutStory.classList.toggle('is-enhanced', desktop.matches);
        if (!desktop.matches) return;
        let active = 0;
        markers.forEach((marker, index) => { if (marker.getBoundingClientRect().top <= window.innerHeight * .5) active = index; });
        copies.forEach((copy, i) => { copy.classList.toggle('is-active', i === active); copy.inert = i !== active; });
        images.forEach((image, i) => { image.classList.toggle('is-active', i === active); image.setAttribute('aria-hidden', String(i !== active)); });
    };
    const schedule = () => { if (!pending) { pending = true; requestAnimationFrame(update); } };
    window.addEventListener('scroll', schedule, { passive: true });
    window.addEventListener('resize', schedule, { passive: true });
    desktop.addEventListener('change', schedule);
    update();
    const rail = aboutStory.querySelector('[data-about-rail]');
    const dots = [...aboutStory.querySelectorAll('[data-about-dot]')];
    dots.forEach((dot, i) => dot.addEventListener('click', () => rail.scrollTo({ left: rail.children[i].offsetLeft - rail.children[0].offsetLeft, behavior: reduced.matches ? 'instant' : 'smooth' })));
    rail.addEventListener('scroll', () => {
        let active = 0; let distance = Infinity;
        [...rail.children].forEach((slide, i) => { const current = Math.abs(slide.getBoundingClientRect().left - rail.getBoundingClientRect().left - 24); if (current < distance) { distance = current; active = i; } });
        dots.forEach((dot, i) => dot.setAttribute('aria-current', String(i === active)));
    }, { passive: true });
    aboutStory.querySelector('.about-dots').hidden = false;
}
