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

    // The link's address is the page as the server saw it, which differs from the browser's address where a page is
    // served in place of another (a not-found page, a static 404.html). With script running there is no reason to
    // follow it at all: move focus to the main element, which also scrolls it into view, as the browser would.
    attachSkipLink(refs) {
        if (!refs.skip || !refs.main) return;

        this.setEventListener(refs.skip, 'click', event => {
            event.preventDefault();
            refs.main.focus();
        });
    }
}