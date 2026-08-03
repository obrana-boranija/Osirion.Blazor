(() => {
    document.querySelectorAll('[data-osirion-metric="True"], [data-osirion-metric="true"]').forEach(element => {
        if (element.dataset.osirionMetricInitialized) return;
        element.dataset.osirionMetricInitialized = 'true';

        if (window.matchMedia('(prefers-reduced-motion: reduce)').matches) return;

        const observer = new IntersectionObserver(entries => {
            if (!entries.some(entry => entry.isIntersecting)) return;
            observer.disconnect();

            const raw = element.dataset.count || '';
            const parts = raw.split(/[-\u2013]/);
            const prefix = element.dataset.prefix || '';
            const suffix = element.dataset.suffix || '';
            const targets = parts.map(Number);
            const decimals = parts.map(part => (part.split('.')[1] || '').length);
            const start = performance.now();

            const tick = now => {
                const progress = Math.min((now - start) / 1200, 1);
                const eased = 1 - Math.pow(1 - progress, 4);
                const value = targets.map((target, index) => {
                    const amount = target * eased;
                    return decimals[index] ? amount.toFixed(decimals[index]) : Math.round(amount);
                }).join('–');
                element.textContent = `${prefix}${value}${suffix}`;
                if (progress < 1) requestAnimationFrame(tick);
            };
            requestAnimationFrame(tick);
        }, { threshold: 0.3 });
        observer.observe(element);
    });
})();