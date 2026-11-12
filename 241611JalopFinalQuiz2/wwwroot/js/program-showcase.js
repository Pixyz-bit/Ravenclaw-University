// Measure wrapped titles so the resting caption shows exactly the subject name.
const titleSizes = new ResizeObserver(entries => entries.forEach(({target}) => {
    target.closest('.program-card').style.setProperty('--program-title-height', `${target.offsetHeight}px`);
}));
document.querySelectorAll('.program-card-title').forEach(title => titleSizes.observe(title));
const programRail = document.querySelector('.program-rail');
if (programRail && programRail.children.length > 1) {
    const originals = [...programRail.children];
    const reduced = window.matchMedia('(prefers-reduced-motion: reduce)');
    let groupWidth = 0, hovering = false, dragging = false, startX = 0, startScroll = 0, moved = false, visible = false, last = 0;
    const clones = [];
    const addCopies = () => {
        clones.forEach(clone => { titleSizes.unobserve(clone.querySelector('.program-card-title')); clone.remove(); }); clones.length = 0;
        if (reduced.matches) { programRail.scrollLeft = 0; return; }
        for (const position of ['before', 'after']) {
            const fragment = document.createDocumentFragment();
            originals.forEach((card, index) => {
                const clone = card.cloneNode(true);
                clone.removeAttribute('open'); clone.setAttribute('aria-hidden', 'true');
                clone.querySelectorAll('summary,[tabindex]').forEach(node => node.tabIndex = -1);
                clone.addEventListener('click', event => {
                    event.preventDefault();
                    const card = originals[index];
                    programRail.scrollLeft = card.offsetLeft - 24;
                    card.open = !card.open;
                    card.querySelector('summary').focus({preventScroll:true});
                });
                titleSizes.observe(clone.querySelector('.program-card-title'));
                clones.push(clone); fragment.append(clone);
            });
            if (position === 'before') programRail.prepend(fragment); else programRail.append(fragment);
        }
        groupWidth = originals[originals.length - 1].offsetLeft + originals[0].getBoundingClientRect().width + parseFloat(getComputedStyle(programRail).gap) - originals[0].offsetLeft;
        programRail.scrollLeft = groupWidth;
    };
    programRail.classList.add('is-draggable');
    addCopies();
    reduced.addEventListener('change', addCopies);
    const resize = new ResizeObserver(() => {
        if (!reduced.matches) groupWidth = originals[originals.length - 1].offsetLeft + originals[0].getBoundingClientRect().width + parseFloat(getComputedStyle(programRail).gap) - originals[0].offsetLeft;
        const open = originals.find(card => card.open);
        if (open) programRail.scrollLeft = open.offsetLeft - programRail.offsetLeft - 24;
    }); resize.observe(programRail);
    const intersection = new IntersectionObserver(entries => { visible = entries[0].isIntersecting; }); intersection.observe(programRail);
    programRail.addEventListener('pointerenter', () => hovering = true);
    programRail.addEventListener('pointerleave', () => hovering = false);
    programRail.addEventListener('pointerdown', event => { if (event.button !== 0 || event.pointerType !== 'mouse') return; dragging = true; moved = false; startX = event.clientX; startScroll = programRail.scrollLeft; });
    window.addEventListener('pointermove', event => { if (!dragging) return; const delta = event.clientX - startX; if (Math.abs(delta) > 5) moved = true; if (moved) { programRail.scrollLeft = startScroll - delta; programRail.classList.add('is-dragging'); } });
    window.addEventListener('pointerup', () => { dragging = false; programRail.classList.remove('is-dragging'); });
    programRail.addEventListener('click', event => { if (moved) { event.preventDefault(); event.stopPropagation(); moved = false; } }, true);
    const tick = time => {
        const delta = last ? Math.min(time - last, 50) : 0; last = time;
        if (visible && !document.hidden && !hovering && !dragging && !programRail.contains(document.activeElement) && !programRail.querySelector('details[open]') && !reduced.matches && groupWidth > 0) {
            programRail.scrollLeft += delta * .025;
            if (programRail.scrollLeft >= groupWidth * 2) programRail.scrollLeft -= groupWidth;
            if (programRail.scrollLeft < groupWidth - 1) programRail.scrollLeft += groupWidth;
        }
        frame = requestAnimationFrame(tick);
    };
    let frame = requestAnimationFrame(tick);
    window.addEventListener('pagehide', () => { cancelAnimationFrame(frame); resize.disconnect(); intersection.disconnect(); }, {once:true});
}
