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
    document.querySelectorAll('[data-about-jump]').forEach(link => link.addEventListener('click', event => {
        const index = Number(link.dataset.aboutJump);
        if (!Number.isInteger(index) || index < 0 || index >= copies.length) return;
        event.preventDefault();
        const behavior = reduced.matches ? 'instant' : 'smooth';
        if (desktop.matches) {
            // Chapter selection follows the same viewport midpoint used by update().
            window.scrollTo({ top: window.scrollY + markers[index].getBoundingClientRect().top - window.innerHeight / 2 + 2, behavior });
        } else {
            rail.scrollTo({ left: rail.children[index].offsetLeft - rail.children[0].offsetLeft, behavior });
            aboutStory.scrollIntoView({ block: 'start', behavior });
        }
    }));
    let activeSlide = 0;
    const sizeRail = () => {
        if (desktop.matches) { rail.style.height = ''; return; }
        const style = getComputedStyle(rail);
        const slide = rail.children[activeSlide];
        const height = slide.offsetHeight + parseFloat(style.paddingTop) + parseFloat(style.paddingBottom);
        const previousHeight = parseFloat(rail.style.height) || height;
        const bounds = rail.getBoundingClientRect();
        rail.style.height = `${height}px`;
        if (height < previousHeight - 1 && bounds.top < 0 && bounds.bottom > 0) {
            // Keep the chosen chapter in view when a much taller chapter collapses.
            requestAnimationFrame(() => slide.scrollIntoView({block:'start',inline:'nearest',behavior:'instant'}));
        }
    };
    dots.forEach((dot, i) => dot.addEventListener('click', () => rail.scrollTo({ left: rail.children[i].offsetLeft - rail.children[0].offsetLeft, behavior: reduced.matches ? 'instant' : 'smooth' })));
    rail.addEventListener('scroll', () => {
        let active = 0; let distance = Infinity;
        [...rail.children].forEach((slide, i) => { const current = Math.abs(slide.getBoundingClientRect().left - rail.getBoundingClientRect().left); if (current < distance) { distance = current; active = i; } });
        dots.forEach((dot, i) => dot.setAttribute('aria-current', String(i === active)));
        activeSlide = active;
        sizeRail();
    }, { passive: true });
    const mobileResize = new ResizeObserver(sizeRail);
    [...rail.children].forEach(slide => mobileResize.observe(slide));
    window.addEventListener('resize', sizeRail, { passive:true });
    desktop.addEventListener('change', sizeRail);
    window.addEventListener('pagehide', () => mobileResize.disconnect(), {once:true});
    sizeRail();
    aboutStory.querySelector('.about-dots').hidden = false;
}
