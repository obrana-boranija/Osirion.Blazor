(function() {
    'use strict';

    const config = {
        containerSelector: '.line-numbers',
        lineSelector: '.line-numbers-rows > span',
        highlightClass: 'osirion-line-highlighted',
        initialized: 'osirion-lines-initialized',
        activeLines: new WeakMap() // Use WeakMap to prevent memory leaks
    };

    let initialized = false;

    // Toggle line highlight
    function toggleLineHighlight(event) {
        const span = event.target.closest(config.lineSelector);
        if (!span) return;

        const pre = span.closest('pre');
        if (!pre) return;

        const isMultiSelect = event.ctrlKey || event.metaKey || event.shiftKey;
        const lineNumber = Array.from(span.parentElement.children).indexOf(span) + 1;

        // Get or create line set for this pre element
        let activeLines = config.activeLines.get(pre) || new Set();

        if (!isMultiSelect) {
            // Clear all highlights
            pre.querySelectorAll(`.${config.highlightClass}`).forEach(el => {
                el.classList.remove(config.highlightClass);
                el.setAttribute('aria-pressed', 'false');
            });
            activeLines.clear();
        }

        // Toggle current line
        if (span.classList.contains(config.highlightClass)) {
            span.classList.remove(config.highlightClass);
            span.setAttribute('aria-pressed', 'false');
            activeLines.delete(lineNumber);
        } else {
            span.classList.add(config.highlightClass);
            span.setAttribute('aria-pressed', 'true');
            activeLines.add(lineNumber);
        }

        // Store updated set
        config.activeLines.set(pre, activeLines);

        // Announce to screen readers
        announceChange(lineNumber, span.classList.contains(config.highlightClass), isMultiSelect);
    }

    // Announce changes for accessibility
    function announceChange(lineNumber, isHighlighted, isMultiSelect) {
        const message = isHighlighted 
            ? `Line ${lineNumber} highlighted${isMultiSelect ? '. Hold Ctrl/Cmd for multiple selection.' : ''}`
            : `Line ${lineNumber} unhighlighted`;

        const announcement = document.createElement('div');
        announcement.className = 'sr-only';
        announcement.setAttribute('role', 'status');
        announcement.setAttribute('aria-live', 'polite');
        announcement.textContent = message;
        
        document.body.appendChild(announcement);
        setTimeout(() => announcement.remove(), 3000);
    }

    // Process line numbers
    function processLineNumbers() {
        const containers = document.querySelectorAll(`${config.containerSelector}:not(.${config.initialized})`);
        
        containers.forEach(container => {
            container.classList.add(config.initialized);
            
            const lines = container.querySelectorAll(config.lineSelector);
            lines.forEach((span, index) => {
                span.setAttribute('role', 'button');
                span.setAttribute('aria-label', `Toggle highlight for line ${index + 1}`);
                span.setAttribute('aria-pressed', 'false');
                span.setAttribute('tabindex', '0');
                span.style.cursor = 'pointer';
            });
        });
    }

    // Initialize with event delegation
    function init() {
        if (initialized) return;
        initialized = true;

        // Click handler with delegation
        document.addEventListener('click', (event) => {
            if (!event.target.closest(config.lineSelector)) return;
            toggleLineHighlight(event);
        });

        // Keyboard handler with delegation
        document.addEventListener('keydown', (event) => {
            if (event.key !== 'Enter' && event.key !== ' ') return;
            if (!event.target.closest(config.lineSelector)) return;
            
            event.preventDefault();
            toggleLineHighlight(event);
        });

        // Process existing elements
        processLineNumbers();

        // Watch for new elements
        const observer = new MutationObserver(() => {
            processLineNumbers();
        });

        observer.observe(document.body, {
            childList: true,
            subtree: true
        });
    }

    // Auto-initialize
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }

    // Public API
    window.initializeLineHighlighting = processLineNumbers;
    window.OsirionLineHighlighting = {
        init: init,
        processLineNumbers: processLineNumbers
    };
})();
