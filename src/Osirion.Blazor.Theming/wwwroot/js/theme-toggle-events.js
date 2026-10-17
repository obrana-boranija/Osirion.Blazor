// Bridges the osirion-theme-update browser event to ThemeToggle.OnThemeChanged. It is imported only when a toggle runs
// interactively and has a callback, so a statically rendered page never loads it.
const subscriptions = new Map();

function currentTheme() {
    return document.documentElement.getAttribute('data-osirion-theme') === 'dark' ? 'dark' : 'light';
}

export function subscribe(id, target) {
    unsubscribe(id);
    let last = currentTheme();

    // The event also fires for a palette change; only a change of light or dark is reported.
    const listener = function () {
        const theme = currentTheme();
        if (theme !== last) {
            last = theme;
            target.invokeMethodAsync('NotifyThemeChanged', theme);
        }
    };

    window.addEventListener('osirion-theme-update', listener);
    subscriptions.set(id, listener);
}

export function unsubscribe(id) {
    const listener = subscriptions.get(id);
    if (listener) {
        window.removeEventListener('osirion-theme-update', listener);
        subscriptions.delete(id);
    }
}
