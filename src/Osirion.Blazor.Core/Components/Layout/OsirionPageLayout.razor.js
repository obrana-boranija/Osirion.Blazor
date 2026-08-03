const header = document.querySelector('.osirion-page-header-scroll');

if (header) {
    let lastScrollY = window.scrollY;
    let isHeaderVisible = true;
    let ticking = false;

    const updateHeader = () => {
        const currentScrollY = window.scrollY;
        const scrollDifference = currentScrollY - lastScrollY;

        if (currentScrollY < 100 || scrollDifference < -10) {
            header.style.transform = 'translateY(0)';
            isHeaderVisible = true;
        } else if (scrollDifference > 10 && isHeaderVisible) {
            header.style.transform = 'translateY(-100%)';
            isHeaderVisible = false;
        }

        lastScrollY = currentScrollY;
        ticking = false;
    };

    window.addEventListener('scroll', () => {
        if (!ticking) {
            window.requestAnimationFrame(updateHeader);
            ticking = true;
        }
    }, { passive: true });
}