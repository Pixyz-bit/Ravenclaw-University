// Profile drafts stay local until the ordinary, antiforgery-protected MVC save.
const profileForm = document.querySelector('[data-profile-editor]');
if (profileForm) {
    const state = profileForm.querySelector('[data-profile-dirty]');
    const save = profileForm.querySelector('button[type="submit"]');
    const originallyDisabled = save.disabled;
    const signature = () => JSON.stringify([...profileForm.elements]
        .filter(input => input.name && ['INPUT', 'SELECT', 'TEXTAREA'].includes(input.tagName) && input.type !== 'hidden')
        .map(input => [input.name, input.type === 'file' ? [...input.files].map(file => [file.name, file.size, file.lastModified]) : input.value]));
    const initial = signature();
    const recovered = profileForm.dataset.recovered === 'true';
    let leaving = false;
    const dirty = () => recovered || signature() !== initial;
    const update = () => {
        const message = dirty() ? 'Unsaved changes' : 'No unsaved changes';
        if (state.textContent !== message) state.textContent = message;
        state.classList.toggle('is-dirty', dirty());
    };
    profileForm.addEventListener('input', update);
    profileForm.addEventListener('change', update);
    const choose = profileForm.querySelector('[data-profile-choose]');
    const imageInput = profileForm.querySelector('[data-image-upload]');
    imageInput.hidden = true;
    profileForm.querySelector('[data-profile-photo-label]').hidden = true;
    choose.hidden = false;
    choose.addEventListener('click', () => imageInput.click());
    profileForm.addEventListener('submit', event => {
        if (window.jQuery?.fn.valid && !window.jQuery(profileForm).valid()) {
            event.preventDefault();
            profileForm.querySelector('.input-validation-error')?.focus();
            return;
        }
        queueMicrotask(() => {
            if (event.defaultPrevented || !profileForm.checkValidity() || profileForm.querySelector('.input-validation-error')) return;
            leaving = true; state.textContent = 'Saving changes…'; save.textContent = 'Saving…'; save.disabled = true;
        });
    });
    const dialog = document.querySelector('[data-profile-unsaved]');
    let pending;
    const requestLeave = action => {
        pending = action;
        dialog.showModal();
        dialog.querySelector('[data-profile-keep]').focus();
    };
    dialog.querySelector('[data-profile-keep]').addEventListener('click', () => { pending = null; dialog.close(); });
    dialog.addEventListener('cancel', () => { pending = null; });
    dialog.querySelector('[data-profile-discard]').addEventListener('click', () => {
        const action = pending; pending = null; leaving = true; dialog.close(); action?.();
    });
    document.addEventListener('click', event => {
        const link = event.target.closest('a[href]');
        if (!link || leaving || !dirty() || event.defaultPrevented || event.button !== 0
            || event.ctrlKey || event.metaKey || event.shiftKey || event.altKey || link.target === '_blank' || link.hasAttribute('download')) return;
        const destination = new URL(link.href, location.href);
        if (destination.pathname === location.pathname && destination.search === location.search && destination.hash) return;
        event.preventDefault(); requestLeave(() => location.assign(destination.href));
    });
    document.addEventListener('submit', event => {
        if (event.target === profileForm || event.defaultPrevented || leaving || !dirty()) return;
        event.preventDefault();
        const form = event.target, submitter = event.submitter;
        requestLeave(() => form.requestSubmit(submitter));
    });
    window.addEventListener('beforeunload', event => {
        if (!leaving && dirty()) { event.preventDefault(); event.returnValue = ''; }
    });
    window.addEventListener('pageshow', event => {
        if (!event.persisted) return;
        leaving = false; save.disabled = originallyDisabled; save.textContent = 'Save changes';
        // The upload helper revokes its preview URL on pagehide; rebuild it on Back.
        imageInput.dispatchEvent(new Event('change', { bubbles: true }));
        update();
    });
    update();
}
