// Hanging pennants: a small damped spring gives the supplied rope layout gentle sway.
document.querySelectorAll('[data-collegiate-scroll]').forEach(scroll => {
    const pennants = [...scroll.querySelectorAll('[data-collegiate-pennant]')];
    const reduced = matchMedia('(prefers-reduced-motion: reduce)');
    const states = pennants.map((element,index) => ({element,index,angle:0,velocity:0,impulse:0}));
    let visible = false, frame = 0, last = 0, elapsed = 0, lastWidth = 0;

    const tick = time => {
        const dt = last ? Math.min((time - last) / 1000,.04) : 0; last = time; elapsed += dt;
        states.forEach(state => {
            const breeze = Math.sin(elapsed * .8 + state.index * .9) * .75;
            state.velocity += ((breeze + state.impulse - state.angle) * 22 - state.velocity * 7) * dt;
            state.angle += state.velocity * dt;
            state.element.style.transform = `rotate(${state.angle}deg)`;
        });
        frame = requestAnimationFrame(tick);
    };
    const update = () => {
        if (visible && !document.hidden && !reduced.matches) {
            if (!frame) frame = requestAnimationFrame(tick);
        } else {
            cancelAnimationFrame(frame); frame = 0; last = 0;
            if (reduced.matches) states.forEach(state => { state.angle = 0; state.velocity = 0; state.element.style.transform = ''; });
        }
    };
    states.forEach(state => {
        state.element.addEventListener('pointermove', event => {
            if (event.pointerType !== 'mouse') return;
            const rect = state.element.getBoundingClientRect();
            state.impulse = ((event.clientX - rect.left) / rect.width - .5) * 5;
        });
        state.element.addEventListener('pointerleave', () => state.impulse = 0);
    });
    const observer = new IntersectionObserver(entries => { visible = entries[0].isIntersecting; update(); }); observer.observe(scroll);
    const resize = new ResizeObserver(() => {
        if (lastWidth !== scroll.clientWidth) {
            lastWidth = scroll.clientWidth;
            scroll.scrollLeft = Math.max(0,(scroll.scrollWidth - scroll.clientWidth) / 2);
        }
    }); resize.observe(scroll);
    reduced.addEventListener('change',update);
    document.addEventListener('visibilitychange',update);
    window.addEventListener('pagehide',() => {
        cancelAnimationFrame(frame); observer.disconnect(); resize.disconnect();
        reduced.removeEventListener('change',update); document.removeEventListener('visibilitychange',update);
    },{once:true});
});
