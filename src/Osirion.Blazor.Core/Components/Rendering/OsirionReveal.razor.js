export default class extends BlazorJSComponents.Component {
    attach() {
        this.observer = undefined;
    }

    setParameters(refs, animate) {
        this.observer?.disconnect();
        if (!animate || !refs.root || window.matchMedia('(prefers-reduced-motion: reduce)').matches) return;

        this.observer = new IntersectionObserver(entries => entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('osirion-reveal-visible');
                this.observer.unobserve(entry.target);
            }
        }), { threshold: 0.12 });

        this.observer.observe(refs.root);
    }

    dispose() {
        this.observer?.disconnect();
        super.dispose();
    }
}