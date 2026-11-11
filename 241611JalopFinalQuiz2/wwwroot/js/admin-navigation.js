// Native details keeps navigation usable without JS; collapse it initially on phones.
const adminNavigation = document.querySelector('.admin-navigation');
if (adminNavigation) {
    const desktop = window.matchMedia('(min-width: 901px)');
    const updateNavigation = () => { adminNavigation.open = desktop.matches; };
    updateNavigation();
    desktop.addEventListener('change', updateNavigation);
}
