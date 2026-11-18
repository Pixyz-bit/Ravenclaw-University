// Shared public/admin drawer. Native details remains usable without JavaScript.
document.querySelectorAll('[data-navigation-drawer]').forEach(drawer => {
    const desktop = matchMedia(drawer.dataset.desktopQuery);
    const trigger = drawer.querySelector(':scope > summary');
    const panel = drawer.querySelector('.navigation-panel');
    let active = false;
    let hiddenSiblings = [];
    const release = () => {
        hiddenSiblings.forEach(([element, wasInert]) => { element.inert = wasInert; });
        hiddenSiblings = [];
        document.documentElement.classList.remove('navigation-drawer-open');
        panel.removeAttribute('role');
        panel.removeAttribute('aria-modal');
        active = false;
    };
    const close = (restoreFocus = true) => {
        drawer.open = false;
        release();
        trigger.setAttribute('aria-expanded', 'false');
        if (restoreFocus) trigger.focus({ preventScroll: true });
    };
    const sync = () => {
        trigger.setAttribute('aria-expanded', String(drawer.open));
        if (desktop.matches || !drawer.open) { release(); return; }
        if (active) return;
        active = true;
        document.querySelectorAll('.account-menu[open]').forEach(menu => { menu.open = false; });
        panel.setAttribute('role', 'dialog');
        panel.setAttribute('aria-modal', 'true');
        document.documentElement.classList.add('navigation-drawer-open');
        // Inert siblings along the ancestor path, leaving only the drawer active.
        let branch = drawer;
        while (branch.parentElement && branch !== document.body) {
            [...branch.parentElement.children].filter(element => element !== branch && element instanceof HTMLElement).forEach(element => {
                hiddenSiblings.push([element, element.inert]);
                element.inert = true;
            });
            branch = branch.parentElement;
        }
        panel.querySelector('[data-drawer-close]').focus({ preventScroll: true });
    };
    drawer.classList.add('drawer-ready');
    const resize = () => {
        const wasActive = active;
        release();
        drawer.classList.toggle('drawer-mobile', !desktop.matches);
        drawer.open = desktop.matches;
        sync();
        if (!desktop.matches && wasActive) trigger.focus({ preventScroll: true });
    };
    drawer.addEventListener('toggle', sync);
    drawer.querySelectorAll('[data-drawer-close]').forEach(button => button.addEventListener('click', () => close()));
    drawer.addEventListener('click', event => {
        if (!desktop.matches && event.target.closest('a')) close();
    });
    // Release modal background state before the editor's unsaved-change dialog.
    drawer.addEventListener('submit', () => { if (!desktop.matches) close(); });
    drawer.addEventListener('keydown', event => {
        if (!active) return;
        if (event.key === 'Escape') { event.preventDefault(); close(); }
        if (event.key !== 'Tab') return;
        const controls = [...panel.querySelectorAll('a[href],button:not([disabled]),summary,input:not([disabled]),[tabindex="0"]')]
            .filter(element => element.getClientRects().length && !element.closest('[inert]'));
        const first = controls[0], last = controls.at(-1);
        if (event.shiftKey && (document.activeElement === first || document.activeElement === trigger)) {
            event.preventDefault(); last.focus();
        } else if (!event.shiftKey && (document.activeElement === last || document.activeElement === trigger)) {
            event.preventDefault(); first.focus();
        }
    });
    desktop.addEventListener('change', resize);
    window.addEventListener('pageshow', resize);
    resize();
});
