// ThemePicker for Osirion.Blazor: applies the chosen palette at once and keeps it in a cookie.
// One listener on the document serves every picker, including those enhanced navigation adds later.
(function () {
    'use strict';

    if (window.__osirionThemePicker__) {
        return;
    }
    window.__osirionThemePicker__ = true;

    // The ids of ThemePalette; 'framework' means no palette attribute.
    const PALETTES = ['framework', 'osirion', 'commit', 'editor', 'studio', 'studio-blue', 'contrast'];
    const COOKIE = 'osirion-preferred-palette';
    const PICKER = 'select[data-osirion-palette-picker]';

    function readPalette() {
        const match = document.cookie.match(/(?:^|; )osirion-preferred-palette=([^;]*)/);
        const value = match ? decodeURIComponent(match[1]).toLowerCase() : '';
        return PALETTES.indexOf(value) >= 0 ? value : null;
    }

    function ensureStylesheet(href) {
        if (!href || document.querySelector('link[data-osirion-palettes]')) {
            return;
        }
        const link = document.createElement('link');
        link.rel = 'stylesheet';
        link.href = href;
        link.setAttribute('data-osirion-palettes', '');
        document.head.appendChild(link);
    }

    function applyPalette(palette, href) {
        const html = document.documentElement;
        if (palette === 'framework') {
            html.removeAttribute('data-osirion-palette');
        } else {
            ensureStylesheet(href);
            html.setAttribute('data-osirion-palette', palette);
        }

        const secure = window.location.protocol === 'https:' ? '; Secure' : '';
        document.cookie = COOKIE + '=' + palette + '; path=/; max-age=' + (365 * 24 * 60 * 60) + '; SameSite=Lax' + secure;
        window.dispatchEvent(new CustomEvent('osirion-theme-update'));
    }

    // A static page, or a page cached before the visitor chose, renders the default; show the palette in effect.
    function syncPickers() {
        const palette = readPalette() || document.documentElement.getAttribute('data-osirion-palette') || 'framework';
        document.querySelectorAll(PICKER).forEach(function (select) {
            if (select.value !== palette && select.querySelector('option[value="' + palette + '"]')) {
                select.value = palette;
            }
        });
    }

    document.addEventListener('change', function (event) {
        const select = event.target;
        if (!select || !select.matches || !select.matches(PICKER)) {
            return;
        }
        const palette = PALETTES.indexOf(select.value) >= 0 ? select.value : 'framework';
        applyPalette(palette, select.getAttribute('data-osirion-palettes-href'));
        syncPickers();
    });

    window.addEventListener('osirion-theme-update', syncPickers);
    if (typeof Blazor !== 'undefined' && Blazor.addEventListener) {
        Blazor.addEventListener('enhancedload', syncPickers);
    }
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', syncPickers);
    } else {
        syncPickers();
    }
})();
