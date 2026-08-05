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
        this.setEventListener(window, 'resize', () => this.updateFocusedSlides(), { passive: true });

        this.updateFocusedSlides();
        this.trackFocusedSlides();
    }

    beginDrag(event) {
        if (event.button !== undefined && event.button !== 0) {
            return;
        }

        if (event.target.closest('a, button, input, select, textarea, [role="button"]')) {
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
        const distance = Math.abs(event.deltaX) > Math.abs(event.deltaY)
            ? event.deltaX
            : event.deltaY;

        if (distance === 0 || this.getLoopLength() === 0) {
            return;
        }

        event.preventDefault();
        this.shiftAnimation(distance);
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
        this.updateFocusedSlides();
    }

    getLoopLength() {
        const gap = Number.parseFloat(getComputedStyle(this.track).gap) || 0;
        return (this.track?.scrollWidth / 2) + (gap / 2);
    }

    getTrackAnimation() {
        return this.track?.getAnimations().find(animation => animation.animationName.includes('osirion-testimonial-scroll'));
    }

    updateFocusedSlides() {
        if (!this.root || !this.viewport) {
            return;
        }

        const viewportRect = this.viewport.getBoundingClientRect();
        const viewportCenter = viewportRect.left + (viewportRect.width / 2);
        const slides = Array.from(this.root.querySelectorAll('.osirion-testimonial-carousel-slide'));
        const positionedSlides = slides.map(slide => ({ slide, center: this.getCenter(slide) }));
        const centerSlide = positionedSlides
            .sort((left, right) => Math.abs(left.center - viewportCenter) - Math.abs(right.center - viewportCenter))[0];

        if (!centerSlide) {
            return;
        }

        const leftNeighbor = positionedSlides
            .filter(item => item.center < centerSlide.center)
            .sort((left, right) => right.center - left.center)[0];
        const rightNeighbor = positionedSlides
            .filter(item => item.center > centerSlide.center)
            .sort((left, right) => left.center - right.center)[0];
        const focusedSlides = new Set([centerSlide.slide, leftNeighbor?.slide, rightNeighbor?.slide]);

        slides.forEach(slide => slide.classList.toggle('osirion-testimonial-carousel-slide-focus', focusedSlides.has(slide)));
    }

    trackFocusedSlides() {
        window.cancelAnimationFrame(this.focusAnimationFrame);
        const update = () => {
            this.updateFocusedSlides();
            this.focusAnimationFrame = window.requestAnimationFrame(update);
        };

        this.focusAnimationFrame = window.requestAnimationFrame(update);
    }

    getCenter(element) {
        const rect = element.getBoundingClientRect();
        return rect.left + (rect.width / 2);
    }

    dispose() {
        window.cancelAnimationFrame(this.focusAnimationFrame);
        this.getTrackAnimation()?.cancel();
        this.root?.classList.remove('is-enhanced');
        super.dispose();
    }
}
