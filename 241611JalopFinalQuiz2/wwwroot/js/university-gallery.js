// The gallery stays still; selecting an image opens its existing native preview.
document.querySelectorAll('[data-university-gallery]').forEach(section => {
    const grid = section.querySelector('.university-gallery-grid');
    const dialog = section.querySelector('dialog');
    const fullImage = dialog.querySelector('[data-gallery-full]');
    const caption = dialog.querySelector('#gallery-image-title');
    grid.addEventListener('click', event => {
        const link = event.target.closest('.university-gallery-image');
        if (!link || typeof dialog.showModal !== 'function') return;
        event.preventDefault();
        fullImage.src = link.href;
        fullImage.alt = link.querySelector('img').alt;
        caption.textContent = fullImage.alt;
        dialog.showModal();
    });
    dialog.addEventListener('click', event => {
        if (event.target !== dialog) return;
        const rect = dialog.getBoundingClientRect();
        if (event.clientX < rect.left || event.clientX > rect.right || event.clientY < rect.top || event.clientY > rect.bottom) dialog.close();
    });
});
