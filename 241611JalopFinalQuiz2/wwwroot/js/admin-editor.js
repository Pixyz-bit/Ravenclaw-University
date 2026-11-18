// Shared editing behavior; all persistence and validation remain in MVC controllers.
const adminEditorForms = [...document.querySelectorAll('form[data-admin-editor]')];
let adminLeaving = false;
const editorStates = adminEditorForms.map(form => {
    const preview = form.closest('.admin-editor-grid')?.querySelector('[data-admin-preview]');
    const signature = () => JSON.stringify([...form.elements]
        .filter(input => input.name && ['INPUT', 'SELECT', 'TEXTAREA'].includes(input.tagName) && !['hidden', 'submit', 'button'].includes(input.type))
        .map(input => [input.name, input.type === 'file' ? [...input.files].map(file => [file.name, file.size, file.lastModified]) : input.type === 'checkbox' ? input.checked : input.value]));
    const initial = signature();
    const recovered = form.dataset.recovered === 'true';
    let dirty = recovered;
    const update = () => {
        dirty = recovered || signature() !== initial;
        preview?.querySelectorAll('[data-preview-field]').forEach(target => {
            const input = [...form.elements].find(element => element.name === target.dataset.previewField && element.type !== 'hidden');
            if (!input) return;
            const value = input.type === 'checkbox' ? (input.checked ? 'Active' : 'Inactive')
                : input.tagName === 'SELECT' ? input.selectedOptions[0]?.textContent.trim() : input.value.trim();
            target.textContent = value || target.dataset.previewEmpty;
        });
        const image = preview?.querySelector('[data-preview-image]');
        if (image) {
            const selected = form.querySelector('[data-upload-preview]:not([hidden])');
            const source = selected?.getAttribute('src') || image.dataset.originalSrc;
            if (source) image.setAttribute('src', source); else image.removeAttribute('src');
            image.hidden = !source;
            preview.querySelector('[data-preview-image-empty]').hidden = Boolean(source);
        }
    };
    form.addEventListener('input', update);
    form.addEventListener('change', update);
    form.addEventListener('submit', event => {
        // Unobtrusive validation uses data-val rules, not HTML required attributes.
        if (window.jQuery?.fn.valid && !window.jQuery(form).valid()) {
            event.preventDefault();
            form.querySelector('.input-validation-error')?.focus();
            return;
        }
        // Let native and unobtrusive validation finish before showing a saving state.
        queueMicrotask(() => {
            if (event.defaultPrevented || !form.checkValidity() || form.querySelector('.input-validation-error')) return;
            adminLeaving = true;
            form.querySelectorAll('button[type=submit]').forEach(button => { button.disabled = true; button.textContent = 'Saving…'; });
        });
    });
    update();
    return { form, update, isDirty: () => dirty };
});
const hasUnsavedAdminChanges = () => !adminLeaving && editorStates.some(state => state.isDirty());
const adminUnsavedDialog = document.querySelector('[data-admin-unsaved]');
let adminPendingNavigation;
const requestAdminLeave = action => {
    adminPendingNavigation = action;
    adminUnsavedDialog.showModal();
    adminUnsavedDialog.querySelector('[data-keep-editing]').focus();
};
adminUnsavedDialog?.querySelector('[data-keep-editing]').addEventListener('click', () => {
    adminPendingNavigation = null;
    adminUnsavedDialog.close();
});
adminUnsavedDialog?.querySelector('[data-discard-draft]').addEventListener('click', () => {
    const navigate = adminPendingNavigation;
    adminPendingNavigation = null;
    adminUnsavedDialog.close();
    adminLeaving = true;
    navigate?.();
});
adminUnsavedDialog?.addEventListener('cancel', () => { adminPendingNavigation = null; });
document.addEventListener('click', event => {
    const link = event.target.closest('a[href]');
    if (!link || event.defaultPrevented || event.button !== 0 || event.ctrlKey || event.metaKey || event.shiftKey || event.altKey || link.target === '_blank' || link.hasAttribute('download')) return;
    const destination = new URL(link.href, location.href);
    if (destination.pathname === location.pathname && destination.search === location.search && destination.hash) return;
    if (hasUnsavedAdminChanges()) {
        event.preventDefault();
        requestAdminLeave(() => location.assign(destination.href));
    }
});
document.addEventListener('submit', event => {
    if (editorStates.some(state => state.form === event.target)) return;
    if (hasUnsavedAdminChanges()) {
        event.preventDefault();
        const form = event.target;
        const submitter = event.submitter;
        requestAdminLeave(() => form.requestSubmit(submitter));
    }
});
window.addEventListener('beforeunload', event => {
    if (hasUnsavedAdminChanges()) { event.preventDefault(); event.returnValue = ''; }
});
window.addEventListener('pageshow', event => {
    if (!event.persisted) return;
    adminLeaving = false;
    // A restored document must not leave its save controls disabled.
    adminEditorForms.forEach(form => form.querySelectorAll('button[type=submit]').forEach(button => {
        if (button.dataset.saveLabel) { button.disabled = false; button.textContent = button.dataset.saveLabel; }
    }));
    editorStates.forEach(state => state.update());
});
adminEditorForms.forEach(form => form.querySelectorAll('button[type=submit]').forEach(button => {
    if (!button.disabled) button.dataset.saveLabel = button.textContent;
}));
const firstInvalidField = document.querySelector('.admin-main .input-validation-error');
if (firstInvalidField) firstInvalidField.focus();
