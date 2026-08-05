export default class extends BlazorJSComponents.Component {
    attach() {
        this.lastScrollY = window.scrollY;
        this.isHeaderVisible = true;
        this.ticking = false;
    }

    setParameters(refs, enabled) {
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
                } else if (difference > 10 && this.isHeaderVisible) {
                    refs.header.style.transform = 'translateY(-100%)';
                    this.isHeaderVisible = false;
                }
                this.lastScrollY = currentScrollY;
                this.ticking = false;
            });
        }, { passive: true });
    }
}