(() => {
    if (window.matchMedia('(prefers-reduced-motion: reduce)').matches) return;
    const observer = new IntersectionObserver(entries => entries.forEach(entry => {
        if (entry.isIntersecting) {
            entry.target.classList.add('osirion-reveal-visible');
            observer.unobserve(entry.target);
        }
    }), { threshold: 0.12 });
    document.querySelectorAll('[data-osirion-reveal="True"], [data-osirion-reveal="true"]').forEach(element => observer.observe(element));
})();