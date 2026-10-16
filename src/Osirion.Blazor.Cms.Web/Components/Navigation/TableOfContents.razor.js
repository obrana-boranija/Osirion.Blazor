// Marks the contents link of the section being read. The section being read is the last heading whose top has passed
// the reading line: a quarter of the viewport below the sticky header's anchor offset. At the end of the page the last
// heading wins, so a short final section can still be marked.
export default class extends BlazorJSComponents.Component {
    setParameters(refs) {
        const nav = refs.nav;
        if (!nav) return;

        this.entries = [...nav.querySelectorAll('a.osirion-toc-link')]
            .map(link => ({ link, heading: document.getElementById(decodeURIComponent(link.hash.slice(1))) }))
            .filter(entry => entry.heading);
        if (this.entries.length === 0) return;

        this.ticking = false;
        const schedule = () => {
            if (this.ticking) return;
            this.ticking = true;
            window.requestAnimationFrame(() => {
                this.ticking = false;
                this.update();
            });
        };

        this.setEventListener(window, 'scroll', schedule, { passive: true });
        this.setEventListener(window, 'resize', schedule, { passive: true });
        this.setEventListener(window, 'hashchange', schedule);
        this.update();
    }

    update() {
        const root = getComputedStyle(document.documentElement);
        const declared = root.getPropertyValue('--osirion-anchor-offset').trim();
        const amount = parseFloat(declared) || 0;
        const offset = declared.endsWith('em') ? amount * (parseFloat(root.fontSize) || 16) : amount;
        const line = offset + window.innerHeight * 0.25;
        const atEnd = window.innerHeight + window.scrollY >= document.documentElement.scrollHeight - 2;

        let current = null;
        for (const entry of this.entries) {
            if (entry.heading.getBoundingClientRect().top <= line) current = entry;
        }

        if (atEnd) current = this.entries[this.entries.length - 1];
        this.mark(current);
    }

    mark(current) {
        if (current === this.current) return;
        this.current = current;
        for (const entry of this.entries) {
            const active = entry === current;
            entry.link.classList.toggle('osirion-toc-link-active', active);
            if (active) {
                entry.link.setAttribute('aria-current', 'location');
            } else {
                entry.link.removeAttribute('aria-current');
            }
        }
    }
}
