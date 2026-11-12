// MVC adaptation of the user-supplied React Bits DriftWall reference.
// Razor owns the accessible image list; page scroll drives the flat columns.
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
    let columns = [], frame = 0;

    // Page scroll is the only timeline; there is no idle animation loop.
    const updateRunning = () => {
        const header = parseFloat(getComputedStyle(document.documentElement).getPropertyValue('--portal-header-height')) || 85;
        const travel = Math.max(1, section.offsetHeight - wall.clientHeight);
        const progress = Math.max(0, Math.min(1, (header - section.getBoundingClientRect().top) / travel));
        plane.style.transform = 'translate(-50%, -50%)';
        columns.forEach((column, index) => {
            const distance = Math.min(column.height, wall.clientHeight * .65);
            const offset = ((column.offset + progress * distance * (index % 2 ? -1 : 1)) % column.height + column.height) % column.height;
            column.track.style.transform = `translate3d(0,${-offset}px,0)`;
        });
    };
    const onScroll = () => {
        if (frame) return;
        frame = requestAnimationFrame(() => { frame = 0; updateRunning(); });
    };
    window.addEventListener('scroll', onScroll, {passive:true});
    const build = () => {
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
                    inner.append(image); tile.append(inner); track.append(tile);
                });
            }
            column.append(track); plane.append(column);
            columns.push({track,height,offset:height * ((index * .37) % 1)});
        }
        updateRunning();
    };

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

    const observer = new IntersectionObserver(entries => { updateRunning(); });
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
        window.removeEventListener('scroll', onScroll);
        reduced.removeEventListener('change', build);
        document.removeEventListener('visibilitychange', updateRunning);
    }, {once:true});
});
