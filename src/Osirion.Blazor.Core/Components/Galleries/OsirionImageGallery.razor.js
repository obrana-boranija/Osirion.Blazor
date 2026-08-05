export default class extends BlazorJSComponents.Component {
    attach() {
        this.currentIndex = 0;
        this.previousFocus = null;
    }

    setParameters(refs) {
        this.root = refs.root;
        if (!this.root) return;

        this.items = [...this.root.querySelectorAll('[data-gallery-index]')];
        this.lightbox = this.root.querySelector('[data-gallery-lightbox]');
        this.image = this.root.querySelector('[data-gallery-image]');
        this.caption = this.root.querySelector('[data-gallery-caption]');
        this.counter = this.root.querySelector('[data-gallery-counter]');

        this.items.forEach((item, index) => this.setEventListener(
            item.querySelector('button'), 'click', () => this.open(index)));
        this.root.querySelectorAll('[data-gallery-close]').forEach(button =>
            this.setEventListener(button, 'click', () => this.close()));
        this.setEventListener(this.root.querySelector('[data-gallery-previous]'), 'click', () => this.move(-1));
        this.setEventListener(this.root.querySelector('[data-gallery-next]'), 'click', () => this.move(1));
        this.setEventListener(this.root, 'keydown', event => this.handleKeyDown(event));
    }

    update() {
        const item = this.items[this.currentIndex];
        const thumbnail = item?.querySelector('img');
        if (!thumbnail || !this.image) return;

        this.image.src = thumbnail.src;
        this.image.alt = thumbnail.alt;
        if (this.caption) this.caption.textContent = item.querySelector('figcaption')?.textContent ?? '';
        if (this.counter) this.counter.textContent = `${this.currentIndex + 1} / ${this.items.length}`;
    }

    open(index) {
        if (!this.lightbox || this.items.length === 0) return;
        this.currentIndex = index;
        this.previousFocus = document.activeElement;
        this.update();
        this.lightbox.hidden = false;
        document.body.style.overflow = 'hidden';
        this.lightbox.querySelector('[data-gallery-close]')?.focus();
    }

    close() {
        if (!this.lightbox) return;
        this.lightbox.hidden = true;
        document.body.style.overflow = '';
        this.previousFocus?.focus();
        this.previousFocus = null;
    }

    move(direction) {
        this.currentIndex = (this.currentIndex + direction + this.items.length) % this.items.length;
        this.update();
    }

    handleKeyDown(event) {
        if (this.lightbox?.hidden) return;
        if (event.key === 'Escape') this.close();
        if (event.key === 'ArrowLeft') this.move(-1);
        if (event.key === 'ArrowRight') this.move(1);
    }

    dispose() {
        this.close();
        super.dispose();
    }
    document.querySelectorAll('[data-osirion-gallery]').forEach(initialize);
})();