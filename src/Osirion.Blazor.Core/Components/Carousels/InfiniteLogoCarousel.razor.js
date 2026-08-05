export default class extends BlazorJSComponents.Component {
    attach() {
        this.isDragging = false;
        this.dragged = false;
        this.focusAnimationFrame = undefined;
    }

    setParameters(refs) {
        this.root = refs.root;
        this.viewport = refs.viewport;
        this.track = refs.track;
        this.lastDragX = 0;

        if (!this.viewport || !this.track) {
            return;
        }

        this.root?.classList.add('is-enhanced');

        this.setEventListener(this.viewport, 'pointerdown', event => this.beginDrag(event));
        this.setEventListener(this.viewport, 'pointermove', event => this.moveDrag(event));
        this.setEventListener(this.viewport, 'pointerup', event => this.endDrag(event));
        this.setEventListener(this.viewport, 'pointercancel', event => this.endDrag(event));
        this.setEventListener(this.viewport, 'lostpointercapture', () => this.endDrag());
        this.setEventListener(this.viewport, 'click', event => this.suppressDraggedClick(event), true);
        this.setEventListener(this.viewport, 'dragstart', event => event.preventDefault());
        this.setEventListener(this.viewport, 'wheel', event => this.scrollWithWheel(event), { passive: false });
        this.setEventListener(window, 'resize', () => this.updateFocusedLogos(), { passive: true });

        this.updateFocusedLogos();
        this.trackFocusedLogos();
    }

    beginDrag(event) {
        if (event.button !== undefined && event.button !== 0) {
            return;
        }

        this.stopAutoScroll();
        this.isDragging = true;
        this.dragged = false;
        this.lastDragX = event.clientX;
        this.viewport.classList.add('is-dragging');
        this.viewport.setPointerCapture?.(event.pointerId);
    }

    moveDrag(event) {
        if (!this.isDragging) {
            return;
        }

        const distance = event.clientX - this.lastDragX;
        if (Math.abs(distance) > 4) {
            this.dragged = true;
        }

        this.shiftAnimation(-distance);
        this.lastDragX = event.clientX;
    }

    endDrag(event) {
        if (!this.isDragging) {
            return;
        }

        this.isDragging = false;
        this.viewport.classList.remove('is-dragging');
        if (event?.pointerId !== undefined && this.viewport.hasPointerCapture?.(event.pointerId)) {
            this.viewport.releasePointerCapture(event.pointerId);
        }

        window.setTimeout(() => {
            this.dragged = false;
        }, 0);
        this.startAutoScroll();
    }

    suppressDraggedClick(event) {
        if (!this.dragged) {
            return;
        }

        event.preventDefault();
        event.stopPropagation();
    }

    scrollWithWheel(event) {
        const delta = Math.abs(event.deltaX) > Math.abs(event.deltaY)
            ? event.deltaX
            : event.deltaY;

        if (delta === 0 || this.getLoopLength() === 0) {
            return;
        }

        event.preventDefault();
        this.shiftAnimation(delta);
    }

    startAutoScroll() {
        this.getTrackAnimation()?.play();
    }

    stopAutoScroll() {
        this.getTrackAnimation()?.pause();
    }

    shiftAnimation(distance) {
        const animation = this.getTrackAnimation();
        const loopLength = this.getLoopLength();
        const duration = Number(animation?.effect?.getTiming().duration);

        if (!animation || !loopLength || !duration) {
            return;
        }

        const currentTime = Number(animation.currentTime) || 0;
        const nextTime = currentTime + ((distance / loopLength) * duration);
        animation.currentTime = ((nextTime % duration) + duration) % duration;
        this.updateFocusedLogos();
    }

    getLoopLength() {
        const firstGroup = this.track?.firstElementChild;
        if (!firstGroup) {
            return 0;
        }

        return firstGroup.getBoundingClientRect().width;
    }

    getTrackAnimation() {
        return this.track?.getAnimations().find(animation => animation.animationName.includes('osirion-carousel-scroll'));
    }

    updateFocusedLogos() {
        if (!this.root || !this.viewport) {
            return;
        }

        const viewportRect = this.viewport.getBoundingClientRect();
        const viewportCenter = viewportRect.left + (viewportRect.width / 2);
        const logos = Array.from(this.root.querySelectorAll('.osirion-logo-item'));
        const positionedLogos = logos.map(logo => ({ logo, center: this.getCenter(logo) }));
        const centerLogo = positionedLogos
            .sort((left, right) => Math.abs(left.center - viewportCenter) - Math.abs(right.center - viewportCenter))[0];

        if (!centerLogo) {
            return;
        }

        const leftNeighbor = positionedLogos
            .filter(item => item.center < centerLogo.center)
            .sort((left, right) => right.center - left.center)[0];
        const rightNeighbor = positionedLogos
            .filter(item => item.center > centerLogo.center)
            .sort((left, right) => left.center - right.center)[0];
        const focusedLogos = new Set([centerLogo.logo, leftNeighbor?.logo, rightNeighbor?.logo]);

        logos.forEach(logo => logo.classList.toggle('osirion-logo-item-focus', focusedLogos.has(logo)));
    }

    trackFocusedLogos() {
        window.cancelAnimationFrame(this.focusAnimationFrame);
        const update = () => {
            this.updateFocusedLogos();
            this.focusAnimationFrame = window.requestAnimationFrame(update);
        };

        this.focusAnimationFrame = window.requestAnimationFrame(update);
    }

    getCenter(element) {
        const rect = element.getBoundingClientRect();
        return rect.left + (rect.width / 2);
    }

    prefersReducedMotion() {
        return window.matchMedia('(prefers-reduced-motion: reduce)').matches;
    }

    dispose() {
        window.cancelAnimationFrame(this.focusAnimationFrame);
        this.getTrackAnimation()?.cancel();
        this.root?.classList.remove('is-enhanced');
        super.dispose();
    }
}
