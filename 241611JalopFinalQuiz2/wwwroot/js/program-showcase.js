// Programs move only in response to a browse, keyboard, or drag action.
const programRail = document.querySelector('.program-rail');
if (programRail) {
    const controls = document.querySelector('.program-controls');
    const buttons = [...document.querySelectorAll('[data-program-direction]')];
    const reduced = matchMedia('(prefers-reduced-motion: reduce)');
    const step = () => programRail.firstElementChild.getBoundingClientRect().width + parseFloat(getComputedStyle(programRail).gap);
    const browse = direction => programRail.scrollBy({left: direction * step(), behavior: reduced.matches ? 'instant' : 'smooth'});
    const updateControls = () => {
        const end = programRail.scrollWidth - programRail.clientWidth;
        if (controls) controls.hidden = end <= 1;
        buttons.forEach(button => {
            button.disabled = Number(button.dataset.programDirection) < 0 ? programRail.scrollLeft <= 1 : programRail.scrollLeft >= end - 1;
        });
    };
    buttons.forEach(button => button.addEventListener('click', () => browse(Number(button.dataset.programDirection))));
    programRail.addEventListener('scroll', updateControls, {passive: true});
    programRail.addEventListener('keydown', event => {
        if (event.target !== programRail || !['ArrowLeft', 'ArrowRight', 'Home', 'End'].includes(event.key)) return;
        event.preventDefault();
        if (event.key === 'Home' || event.key === 'End') {
            programRail.scrollTo({left: event.key === 'Home' ? 0 : programRail.scrollWidth, behavior: reduced.matches ? 'instant' : 'smooth'});
        } else browse(event.key === 'ArrowLeft' ? -1 : 1);
    });
    let dragging = false, moved = false, startX = 0, startScroll = 0;
    programRail.classList.add('is-draggable');
    programRail.addEventListener('pointerdown', event => {
        if (event.button !== 0 || event.pointerType !== 'mouse' || event.target.closest('.program-description')) return;
        dragging = true; moved = false; startX = event.clientX; startScroll = programRail.scrollLeft;
    });
    window.addEventListener('pointermove', event => {
        if (!dragging) return;
        const delta = event.clientX - startX;
        if (Math.abs(delta) > 5) moved = true;
        if (!moved) return;
        event.preventDefault();
        programRail.classList.add('is-dragging');
        programRail.scrollLeft = startScroll - delta;
    });
    const release = () => { dragging = false; programRail.classList.remove('is-dragging'); };
    window.addEventListener('pointerup', release);
    window.addEventListener('pointercancel', release);
    programRail.addEventListener('click', event => {
        if (!moved) return;
        event.preventDefault(); event.stopPropagation(); moved = false;
    }, true);
    const resize = new ResizeObserver(updateControls);
    resize.observe(programRail);
    updateControls();
}
