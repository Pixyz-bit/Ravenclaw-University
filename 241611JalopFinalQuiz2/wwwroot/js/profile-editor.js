// Both profile drafts remain local, including when changing tabs.
const profilePage = document.querySelector('[data-profile-page]');
if (profilePage) {
    const tabs = [...profilePage.querySelectorAll('[data-profile-tab]')];
    const forms = [...profilePage.querySelectorAll('.profile-form')];
    const imageInput = profilePage.querySelector('[data-image-upload]');
    const choose = profilePage.querySelector('[data-profile-choose]');
    const dialog = profilePage.querySelector('[data-profile-unsaved]');
    let leaving = false, pending;
    const activate = tab => {
        tabs.forEach(item => {
            const selected = item === tab;
            item.setAttribute('aria-selected', String(selected));
            item.tabIndex = selected ? 0 : -1;
            document.getElementById(item.getAttribute('aria-controls')).hidden = !selected;
        });
    };
    tabs.forEach((tab, index) => {
        tab.addEventListener('click', event => { event.preventDefault(); activate(tab); });
        tab.addEventListener('keydown', event => {
            let next;
            if (event.key === 'ArrowRight' || event.key === 'ArrowLeft') next = tabs[1 - index];
            if (event.key === 'Home') next = tabs[0];
            if (event.key === 'End') next = tabs[tabs.length - 1];
            if (!next) return;
            event.preventDefault(); activate(next); next.focus();
        });
    });
    imageInput.hidden = true;
    profilePage.querySelector('[data-profile-photo-label]').hidden = true;
    choose.hidden = false;
    choose.addEventListener('click', () => { activate(tabs[0]); imageInput.click(); });
    const resetVisibility = form => form.querySelectorAll('[data-password-show]').forEach(button => {
        document.getElementById(button.getAttribute('aria-controls')).type = 'password';
        button.textContent = 'Show'; button.setAttribute('aria-pressed', 'false');
        button.setAttribute('aria-label', button.getAttribute('aria-label').replace(/^Hide /, 'Show '));
    });
    profilePage.querySelectorAll('[data-password-show]').forEach(button => {
        button.hidden = false;
        button.addEventListener('click', () => {
            const input = document.getElementById(button.getAttribute('aria-controls'));
            const show = input.type === 'password';
            input.type = show ? 'text' : 'password'; button.textContent = show ? 'Hide' : 'Show';
            button.setAttribute('aria-pressed', String(show));
            button.setAttribute('aria-label', button.getAttribute('aria-label').replace(/^(Show|Hide) /, show ? 'Hide ' : 'Show '));
        });
    });
    const states = forms.map(form => {
        const save = form.querySelector('button[type=submit]');
        const disabled = save.disabled, label = save.textContent;
        // Check secrets in memory without making additional serialized copies.
        const controls = [...form.elements].filter(input => input.name && ['INPUT','SELECT','TEXTAREA'].includes(input.tagName) && input.type !== 'hidden');
        const initial = controls.map(input => input.type === 'file' ? '' : input.value);
        let recovered = form.dataset.recovered === 'true';
        const dirty = () => recovered || controls.some((input, index) => input.type === 'file' ? input.files.length > 0 : input.value !== initial[index]);
        const reset = () => {
            recovered = false;
            if (window.jQuery?.fn.validate) window.jQuery(form).validate().resetForm();
            form.querySelectorAll('.input-validation-error').forEach(input => input.classList.remove('input-validation-error'));
            form.querySelectorAll('.field-validation-error').forEach(error => { error.textContent = ''; error.classList.replace('field-validation-error', 'field-validation-valid'); });
            resetVisibility(form);
            if (form.hasAttribute('data-profile-editor')) imageInput.dispatchEvent(new Event('change', { bubbles: true }));
        };
        // Reset default actions run after listeners (including their microtasks).
        form.addEventListener('reset', () => setTimeout(reset, 0));
        form.addEventListener('submit', event => {
            if (window.jQuery?.fn.valid && !window.jQuery(form).valid()) {
                event.preventDefault(); form.querySelector('.input-validation-error')?.focus(); return;
            }
            if (!leaving && states.some(other => other.form !== form && other.dirty())) {
                event.preventDefault();
                requestLeave(() => { states.filter(other => other.form !== form).forEach(other => other.form.reset()); form.requestSubmit(event.submitter); });
                return;
            }
            queueMicrotask(() => {
                if (event.defaultPrevented || !form.checkValidity()) return;
                leaving = true; save.textContent = 'Saving…'; save.disabled = true;
            });
        });
        return { form, dirty, restore: () => { save.disabled = disabled; save.textContent = label; resetVisibility(form); } };
    });
    const dirty = () => states.some(state => state.dirty());
    const requestLeave = action => { pending = action; dialog.showModal(); dialog.querySelector('[data-profile-keep]').focus(); };
    dialog.querySelector('[data-profile-keep]').addEventListener('click', () => { pending = null; dialog.close(); });
    dialog.addEventListener('cancel', () => { pending = null; });
    dialog.querySelector('[data-profile-discard]').addEventListener('click', () => {
        const action = pending; pending = null; leaving = true; dialog.close(); action?.();
    });
    document.addEventListener('click', event => {
        const link = event.target.closest('a[href]');
        if (!link || link.hasAttribute('data-profile-tab') || leaving || !dirty() || event.defaultPrevented || event.button !== 0
            || event.ctrlKey || event.metaKey || event.shiftKey || event.altKey || link.target === '_blank' || link.hasAttribute('download')) return;
        const destination = new URL(link.href, location.href);
        if (destination.pathname === location.pathname && destination.search === location.search && destination.hash) return;
        event.preventDefault(); requestLeave(() => location.assign(destination.href));
    });
    document.addEventListener('submit', event => {
        if (forms.includes(event.target) || event.defaultPrevented || leaving || !dirty()) return;
        event.preventDefault(); const form = event.target, submitter = event.submitter;
        requestLeave(() => form.requestSubmit(submitter));
    });
    window.addEventListener('beforeunload', event => { if (!leaving && dirty()) { event.preventDefault(); event.returnValue = ''; } });
    window.addEventListener('pageshow', event => {
        if (!event.persisted) return;
        leaving = false; states.forEach(state => state.restore());
        profilePage.querySelector('[data-profile-password]').reset();
        imageInput.dispatchEvent(new Event('change', { bubbles: true }));
    });
}
