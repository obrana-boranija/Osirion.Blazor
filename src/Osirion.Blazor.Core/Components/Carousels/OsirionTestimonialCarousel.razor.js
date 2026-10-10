export default class extends BlazorJSComponents.Component {
    attach() {
        this.isDragging = false;
        this.dragged = false;
        this.focusAnimationFrame = undefined;
        this.userPaused = false;
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

        this.userPaused = false;
        this.createPauseControl();
        this.listenForPlayStateChanges();
        this.observeVisibility();
        this.updateFocusedSlides();
    }

    // Shared motion control (kept identical in both carousels, see the sibling .razor.js file).

    createPauseControl() {
        this.pauseButton?.remove();
        if (!this.getTrackAnimation()) {
            return;
        }

        const button = document.createElement('button');
        button.type = 'button';
        button.className = 'osirion-carousel-pause';
        button.setAttribute('aria-pressed', 'false');
        button.setAttribute('aria-label', this.root.dataset.pauseLabel || 'Pause animation');
        button.innerHTML = '<svg aria-hidden="true" focusable="false" viewBox="0 0 16 16" width="14" height="14"><path class="osirion-carousel-pause-icon" d="M4 3h3v10H4zM9 3h3v10H9z" fill="currentColor"/><path class="osirion-carousel-play-icon" d="M4 2.5v11l9-5.5z" fill="currentColor"/></svg>';
        this.root.appendChild(button);
        this.pauseButton = button;
        this.setEventListener(button, 'click', () => this.toggleUserPause());
    }

    toggleUserPause() {
        this.userPaused = !this.userPaused;
        this.pauseButton.setAttribute('aria-pressed', String(this.userPaused));
        this.pauseButton.setAttribute('aria-label', this.userPaused
            ? (this.root.dataset.playLabel || 'Play animation')
            : (this.root.dataset.pauseLabel || 'Pause animation'));

        if (this.userPaused) {
            this.stopAutoScroll();
        } else {
            this.getTrackAnimation()?.play();
        }

        this.syncFocusTracking();
    }

    observeVisibility() {
        this.visibilityObserver?.disconnect();
        this.isVisible = true;
        if (typeof IntersectionObserver === 'undefined') {
            return;
        }

        this.isVisible = false;
        this.visibilityObserver = new IntersectionObserver(entries => {
            this.isVisible = entries[entries.length - 1].isIntersecting;
            this.syncFocusTracking();
        });
        this.visibilityObserver.observe(this.root);
    }

    isMotionActive() {
        return this.isVisible
            && !this.isDragging
            && !window.matchMedia('(prefers-reduced-motion: reduce)').matches
            && this.getTrackAnimation()?.playState === 'running';
    }

    // The highlight only changes while the rail moves, so the frame loop runs only while the carousel is on screen
    // and actually animating. It stops itself when the animation pauses and is restarted by the events below.
    syncFocusTracking() {
        if (!this.isMotionActive()) {
            this.stopFocusTracking();
            this.updateFocusedSlides();
            return;
        }

        if (this.focusAnimationFrame !== undefined) {
            return;
        }

        let lastUpdate = 0;
        const frame = timestamp => {
            if (!this.isMotionActive()) {
                this.focusAnimationFrame = undefined;
                return;
            }

            if (timestamp - lastUpdate >= 100) {
                lastUpdate = timestamp;
                this.updateFocusedSlides();
            }

            this.focusAnimationFrame = window.requestAnimationFrame(frame);
        };

        this.focusAnimationFrame = window.requestAnimationFrame(frame);
    }

    stopFocusTracking() {
        if (this.focusAnimationFrame !== undefined) {
            window.cancelAnimationFrame(this.focusAnimationFrame);
            this.focusAnimationFrame = undefined;
        }
    }

    listenForPlayStateChanges() {
        const resync = () => window.setTimeout(() => this.syncFocusTracking(), 0);
        this.setEventListener(this.root, 'pointerenter', resync, { passive: true });
        this.setEventListener(this.root, 'pointerleave', resync, { passive: true });
        this.setEventListener(this.root, 'focusin', resync);
        this.setEventListener(this.root, 'focusout', resync);
        this.motionQuery?.removeEventListener('change', this.resync);
        this.resync = resync;
        this.motionQuery = window.matchMedia('(prefers-reduced-motion: reduce)');
        this.motionQuery.addEventListener('change', resync);
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
        this.syncFocusTracking();
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
        this.syncFocusTracking();
    }

    suppressDraggedClick(event) {
        if (!this.dragged) {
            return;
        }

        event.preventDefault();
        event.stopPropagation();
    }

    scrollWithWheel(event) {
        // Only a horizontal gesture moves the rail. Vertical wheel input is never cancelled, so the page can always
        // scroll while the pointer is over the carousel.
        if (Math.abs(event.deltaX) <= Math.abs(event.deltaY) || this.getLoopLength() === 0) {
            return;
        }

        event.preventDefault();
        this.shiftAnimation(event.deltaX);
    }

    startAutoScroll() {
        if (!this.userPaused) {
            this.getTrackAnimation()?.play();
        }
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

    getCenter(element) {
        const rect = element.getBoundingClientRect();
        return rect.left + (rect.width / 2);
    }

    dispose() {
        this.stopFocusTracking();
        this.visibilityObserver?.disconnect();
        this.motionQuery?.removeEventListener('change', this.resync);
        this.pauseButton?.remove();
        this.getTrackAnimation()?.cancel();
        this.root?.classList.remove('is-enhanced');
        super.dispose();
    }
}
