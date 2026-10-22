document.querySelector('[data-theme]')?.addEventListener('click', event => {
    const theme = document.documentElement.dataset.bsTheme === 'dark' ? 'light' : 'dark';

    document.documentElement.dataset.bsTheme = theme;
    event.currentTarget.textContent = theme === 'dark' ? 'Light mode' : 'Dark mode';

    try {
        localStorage.setItem('civic-theme', theme);
    } catch {}
});

const themeButton = document.querySelector('[data-theme]');

if (themeButton)
    themeButton.textContent = document.documentElement.dataset.bsTheme === 'dark' ? 'Light mode' : 'Dark mode';

document.querySelector('[data-print]')?.addEventListener('click', () => window.print());

document.querySelectorAll('[data-confirm]').forEach(form => form.addEventListener('submit', event => {
    if (!confirm(form.dataset.confirm))
        event.preventDefault();
}));

const municipality = document.querySelector('[data-candidate-municipality], #municipalityId');
const position = document.querySelector('[data-candidate-position], #positionId');

if (municipality && position) {
    const update = () => {
        for (const option of position.options) {
            if (!option.dataset.municipality)
                continue;

            option.disabled = option.hidden = !!municipality.value
                && municipality.value !== '0'
                && option.dataset.municipality !== municipality.value;
        }

        if (position.selectedOptions[0]?.disabled)
            position.selectedIndex = 0;
    };

    municipality.addEventListener('change', update);
    update();
}
