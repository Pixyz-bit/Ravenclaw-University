// Shared eye controls never submit, persist or copy the password value.
document.querySelectorAll('[data-password-toggle]').forEach(button => {
    const input = document.getElementById(button.getAttribute('aria-controls'));
    if (!input || input.type !== 'password') return;
    const label = (input.labels?.[0]?.textContent || 'password').replace(/\s*\*\s*$/, '').trim().toLowerCase();
    const setVisible = visible => {
        input.type = visible ? 'text' : 'password';
        button.setAttribute('aria-pressed', String(visible));
        button.setAttribute('aria-label', `${visible ? 'Hide' : 'Show'} ${label}`);
        button.querySelector('[data-eye-open]').toggleAttribute('hidden', visible);
        button.querySelector('[data-eye-closed]').toggleAttribute('hidden', !visible);
    };
    setVisible(false);
    button.hidden = false;
    button.addEventListener('click', () => setVisible(input.type === 'password'));
    input.form?.addEventListener('reset', () => setTimeout(() => setVisible(false), 0));
    window.addEventListener('pageshow', () => setVisible(false));
});
