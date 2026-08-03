(() => {
    const initialized = new WeakSet();

    function initialize(root) {
        if (initialized.has(root)) return;
        initialized.add(root);

        const items = [...root.querySelectorAll('[data-gallery-index]')];
        const lightbox = root.querySelector('[data-gallery-lightbox]');
        const image = root.querySelector('[data-gallery-image]');
        const caption = root.querySelector('[data-gallery-caption]');
        const counter = root.querySelector('[data-gallery-counter]');
        let currentIndex = 0;
        let previousFocus = null;

        const update = () => {
            const item = items[currentIndex];
            const thumbnail = item?.querySelector('img');
            if (!thumbnail || !image) return;

            image.src = thumbnail.src;
            image.alt = thumbnail.alt;
            if (caption) caption.textContent = item.querySelector('figcaption')?.textContent ?? '';
            if (counter) counter.textContent = `${currentIndex + 1} / ${items.length}`;
        };

        const close = () => {
            if (!lightbox) return;
            lightbox.hidden = true;
            document.body.style.overflow = '';
            previousFocus?.focus();
            previousFocus = null;
        };

        const open = (index) => {
            if (!lightbox || items.length === 0) return;
            currentIndex = index;
            previousFocus = document.activeElement;
            update();
            lightbox.hidden = false;
            document.body.style.overflow = 'hidden';
            lightbox.querySelector('[data-gallery-close]')?.focus();
        };

        const move = (direction) => {
            currentIndex = (currentIndex + direction + items.length) % items.length;
            update();
        };

        items.forEach((item, index) => item.querySelector('button')?.addEventListener('click', () => open(index)));
        root.querySelectorAll('[data-gallery-close]').forEach(button => button.addEventListener('click', close));
        root.querySelector('[data-gallery-previous]')?.addEventListener('click', () => move(-1));
        root.querySelector('[data-gallery-next]')?.addEventListener('click', () => move(1));

        root.addEventListener('keydown', event => {
            if (lightbox?.hidden) return;
            if (event.key === 'Escape') close();
            if (event.key === 'ArrowLeft') move(-1);
            if (event.key === 'ArrowRight') move(1);
        });
    }

    document.querySelectorAll('[data-osirion-gallery]').forEach(initialize);
})();