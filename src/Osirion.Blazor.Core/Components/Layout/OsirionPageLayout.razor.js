export default class extends BlazorJSComponents.Component {
    attach() {
        this.lastScrollY = window.scrollY;
        this.isHeaderVisible = true;
        this.ticking = false;
    }

    setParameters(refs, enabled) {
        this.attachSkipLink(refs);

        if (!enabled || !refs.header || window.matchMedia('(prefers-reduced-motion: reduce)').matches) return;

        this.setEventListener(window, 'scroll', () => {
            if (this.ticking) return;
            this.ticking = true;
            window.requestAnimationFrame(() => {
                const currentScrollY = window.scrollY;
                const difference = currentScrollY - this.lastScrollY;
                if (currentScrollY < 100 || difference < -10) {
                    refs.header.style.transform = 'translateY(0)';
                    this.isHeaderVisible = true;
                } else if (difference > 10 && this.isHeaderVisible && !refs.header.matches(':hover, :focus-within')) {
                    // A header the reader is using stays: hiding it would carry an open menu panel or drawer away.
                    refs.header.style.transform = 'translateY(-100%)';
                    this.isHeaderVisible = false;
                }
                this.lastScrollY = currentScrollY;
                this.ticking = false;
            });
        }, { passive: true });
    }

    // Blazor's enhanced navigation scrolls to a fragment on the same page without moving focus, so the skip link
    // would leave keyboard focus in the header. Move it to the main element, as the browser does without Blazor.
    attachSkipLink(refs) {
        if (!refs.skip || !refs.main) return;

        this.setEventListener(refs.skip, 'click', () => {
            window.requestAnimationFrame(() => refs.main.focus());
        });
    }
}