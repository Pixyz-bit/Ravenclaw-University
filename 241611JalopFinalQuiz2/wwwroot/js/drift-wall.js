// MVC adaptation of the user-supplied React Bits DriftWall reference.
// Razor owns the accessible image list; this module adds a decorative perspective wall.
document.querySelectorAll('[data-university-gallery]').forEach(section => {
    const wall = section.querySelector('[data-drift-wall]');
    const sourceList = wall.querySelector('.drift-wall__items');
    const sources = [...sourceList.querySelectorAll('a')];
    const canvas = wall.querySelector('.drift-wall__canvas');
    const plane = wall.querySelector('.drift-wall__plane');
    const dialog = section.querySelector('dialog');
    const fullImage = dialog.querySelector('[data-gallery-full]');
    const caption = dialog.querySelector('#gallery-image-title');
    const reduced = matchMedia('(prefers-reduced-motion: reduce)');
    let columns = [], active = null, visible = false, frame = 0, last = 0;
    let pointer = {x:0,y:0}, damped = {x:0,y:0};

    const release = () => {
        active?.classList.remove('is-active');
        active = null;
        pointer = {x:0,y:0};
    };

    const tick = time => {
        const dt = last ? Math.min((time - last) / 1000, .05) : 0;
        last = time;
        const damping = 1 - Math.exp(-dt / .12);
        damped.x += (pointer.x * 4.8 - damped.x) * damping;
        damped.y += (-pointer.y * 4.8 - damped.y) * damping;
        plane.style.transform = `translate(-50%, -50%) scale(1.18) rotateX(${16 + damped.y}deg) rotateY(${-14 + damped.x}deg) translateZ(-120px)`;
        const focused = sourceList.contains(document.activeElement);
        columns.forEach((column, index) => {
            const stopped = dialog.open || focused || Number(active?.dataset.column) === index;
            const target = stopped ? 0 : column.speed;
            const ease = 1 - Math.exp(-dt / (stopped ? .16 : .28));
            column.velocity += (target - column.velocity) * ease;
            column.offset = ((column.offset + column.velocity * dt) % column.height + column.height) % column.height;
            column.track.style.transform = `translate3d(0,${-column.offset}px,0)`;
        });
        frame = requestAnimationFrame(tick);
    };

    const updateRunning = () => {
        const shouldRun = visible && !document.hidden && !reduced.matches;
        if (shouldRun && !frame) frame = requestAnimationFrame(tick);
        else if (!shouldRun && frame) { cancelAnimationFrame(frame); frame = 0; last = 0; }
    };

    const build = () => {
        release();
        plane.replaceChildren();
        columns = [];
        wall.classList.toggle('is-enhanced', !reduced.matches);
        canvas.hidden = reduced.matches;
        if (reduced.matches) { updateRunning(); return; }
        const count = wall.clientWidth < 601 ? 3 : 5;
        const tileWidth = Math.max(150, wall.clientWidth / count * 1.1);
        wall.style.setProperty('--dw-tile-w', `${tileWidth}px`);
        wall.style.setProperty('--dw-tile-h', `${tileWidth * .66}px`);
        const style = getComputedStyle(wall);
        const unit = parseFloat(style.getPropertyValue('--dw-tile-h')) + parseFloat(style.getPropertyValue('--dw-gap'));
        for (let index = 0; index < count; index++) {
            let items = sources.filter((_, position) => position % count === index);
            if (!items.length) items = [sources[index % sources.length]];
            const height = items.length * unit;
            const copies = Math.max(2, Math.ceil(wall.clientHeight * 1.6 / height) + 1);
            const column = document.createElement('div'); column.className = 'drift-wall__col';
            const track = document.createElement('div'); track.className = 'drift-wall__track';
            for (let copy = 0; copy < copies; copy++) {
                items.forEach(source => {
                    const tile = document.createElement('a');
                    tile.href = source.href; tile.tabIndex = -1;
                    tile.className = 'drift-wall__tile'; tile.dataset.column = index;
                    const inner = document.createElement('span'); inner.className = 'drift-wall__inner';
                    const image = source.querySelector('img').cloneNode(); image.draggable = false;
                    const overlay = document.createElement('span'); overlay.className = 'drift-wall__overlay';
                    inner.append(image, overlay); tile.append(inner); track.append(tile);
                });
            }
            column.append(track); plane.append(column);
            const factor = 1 + .45 * (((index * .6180339887 + .35) % 1) * 2 - 1);
            columns.push({track,height,offset:height * ((index * .37) % 1),velocity:0,speed:42 * factor * (index % 2 ? -1 : 1)});
        }
        updateRunning();
    };

    wall.addEventListener('pointermove', event => {
        if (event.pointerType !== 'mouse' || reduced.matches) return;
        const rect = wall.getBoundingClientRect();
        pointer = {x:(event.clientX - rect.left) / rect.width - .5,y:(event.clientY - rect.top) / rect.height - .5};
        const tile = document.elementFromPoint(event.clientX, event.clientY)?.closest('.drift-wall__tile');
        if (tile !== active) {
            active?.classList.remove('is-active'); active = tile;
            active?.classList.add('is-active');
        }
    });
    wall.addEventListener('pointerleave', release);
    wall.addEventListener('click', event => {
        const link = event.target.closest('a');
        if (!link || typeof dialog.showModal !== 'function') return;
        event.preventDefault();
        const image = link.querySelector('img');
        fullImage.src = link.href; fullImage.alt = image.alt;
        caption.textContent = image.alt;
        dialog.showModal();
    });
    dialog.addEventListener('click', event => {
        if (event.target !== dialog) return;
        const rect = dialog.getBoundingClientRect();
        if (event.clientX < rect.left || event.clientX > rect.right || event.clientY < rect.top || event.clientY > rect.bottom) dialog.close();
    });

    const observer = new IntersectionObserver(entries => { visible = entries[0].isIntersecting; updateRunning(); });
    observer.observe(wall);
    let size = '';
    const resize = new ResizeObserver(() => {
        const next = `${wall.clientWidth}:${wall.clientHeight}`;
        if (next !== size) { size = next; build(); }
    });
    resize.observe(wall);
    reduced.addEventListener('change', build);
    document.addEventListener('visibilitychange', updateRunning);
    window.addEventListener('pagehide', () => {
        cancelAnimationFrame(frame); observer.disconnect(); resize.disconnect();
        reduced.removeEventListener('change', build);
        document.removeEventListener('visibilitychange', updateRunning);
    }, {once:true});
});
