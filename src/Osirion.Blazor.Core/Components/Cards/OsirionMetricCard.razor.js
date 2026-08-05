export default class extends BlazorJSComponents.Component {
    attach() {
        this.observer = undefined;
    }

    setParameters(refs, animate) {
        this.observer?.disconnect();
        const valueElement = refs.value;
        if (!animate || !valueElement || window.matchMedia('(prefers-reduced-motion: reduce)').matches) return;

        const parts = (valueElement.dataset.count || '').split(/[-\u2013]/);
        const targets = parts.map(Number);
        if (targets.length === 0 || targets.some(Number.isNaN)) return;

        const prefix = valueElement.dataset.prefix || '';
        const suffix = valueElement.dataset.suffix || '';
        const decimals = parts.map(part => (part.split('.')[1] || '').length);
        this.observer = new IntersectionObserver(entries => {
            if (!entries.some(entry => entry.isIntersecting)) return;
            this.observer.disconnect();

            const start = performance.now();
            const tick = now => {
                const progress = Math.min((now - start) / 1200, 1);
                const eased = 1 - Math.pow(1 - progress, 4);
                const value = targets.map((target, index) => {
                    const amount = target * eased;
                    return decimals[index] ? amount.toFixed(decimals[index]) : Math.round(amount);
                }).join('–');

                valueElement.textContent = `${prefix}${value}${suffix}`;
                if (progress < 1) requestAnimationFrame(tick);
            };

            requestAnimationFrame(tick);
        }, { threshold: 0.3, rootMargin: '0px 0px -30px 0px' });

        this.observer.observe(valueElement);
    }

    dispose() {
        this.observer?.disconnect();
        this.observer = undefined;
    }
}