'use strict';

(function () {
    'use strict';

    var config = {
        containerSelector: '.line-numbers',
        lineSelector: '.line-numbers-rows > span',
        highlightClass: 'osirion-line-highlighted',
        initialized: 'osirion-lines-initialized'
    };

    var initialized = false;
    var activeLines = {}; // Use object instead of WeakMap for ES5

    // Get unique ID for element
    function getElementId(element) {
        if (!element._osirionId) {
            element._osirionId = 'osirion-' + Math.random().toString(36).substr(2, 9);
        }
        return element._osirionId;
    }

    // Toggle line highlight
    function toggleLineHighlight(event) {
        var target = event.target;
        var span = null;

        // Find the span element
        while (target && target !== document) {
            if (target.matches && target.matches(config.lineSelector)) {
                span = target;
                break;
            }
            target = target.parentElement;
        }

        if (!span) return;

        var pre = span.closest('pre');
        if (!pre) return;

        var isMultiSelect = event.ctrlKey || event.metaKey || event.shiftKey;
        var allLines = Array.prototype.slice.call(span.parentElement.children);
        var lineNumber = allLines.indexOf(span) + 1;
        var preId = getElementId(pre);

        // Get or create line set for this pre element
        if (!activeLines[preId]) {
            activeLines[preId] = [];
        }

        if (!isMultiSelect) {
            // Clear all highlights
            var highlighted = pre.querySelectorAll('.' + config.highlightClass);
            Array.prototype.forEach.call(highlighted, function (el) {
                el.classList.remove(config.highlightClass);
                el.setAttribute('aria-pressed', 'false');
            });
            activeLines[preId] = [];
        }

        // Toggle current line
        if (span.classList.contains(config.highlightClass)) {
            span.classList.remove(config.highlightClass);
            span.setAttribute('aria-pressed', 'false');
            activeLines[preId] = activeLines[preId].filter(function (num) {
                return num !== lineNumber;
            });
        } else {
            span.classList.add(config.highlightClass);
            span.setAttribute('aria-pressed', 'true');
            if (activeLines[preId].indexOf(lineNumber) === -1) {
                activeLines[preId].push(lineNumber);
            }
        }

        // Announce to screen readers
        announceChange(lineNumber, span.classList.contains(config.highlightClass), isMultiSelect);
    }

    // Announce changes for accessibility
    function announceChange(lineNumber, isHighlighted, isMultiSelect) {
        var message = isHighlighted ? 'Line ' + lineNumber + ' highlighted' + (isMultiSelect ? '. Hold Ctrl/Cmd for multiple selection.' : '') : 'Line ' + lineNumber + ' unhighlighted';

        var announcement = document.createElement('div');
        announcement.className = 'sr-only';
        announcement.setAttribute('role', 'status');
        announcement.setAttribute('aria-live', 'polite');
        announcement.textContent = message;

        document.body.appendChild(announcement);
        setTimeout(function () {
            announcement.parentNode.removeChild(announcement);
        }, 3000);
    }

    // Process line numbers
    function processLineNumbers() {
        var containers = document.querySelectorAll(config.containerSelector + ':not(.' + config.initialized + ')');

        Array.prototype.forEach.call(containers, function (container) {
            container.classList.add(config.initialized);

            var lines = container.querySelectorAll(config.lineSelector);
            Array.prototype.forEach.call(lines, function (span, index) {
                span.setAttribute('role', 'button');
                span.setAttribute('aria-label', 'Toggle highlight for line ' + (index + 1));
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
        document.addEventListener('click', function (event) {
            var target = event.target;
            while (target && target !== document) {
                if (target.matches && target.matches(config.lineSelector)) {
                    toggleLineHighlight(event);
                    break;
                }
                target = target.parentElement;
            }
        });

        // Keyboard handler with delegation
        document.addEventListener('keydown', function (event) {
            if (event.key !== 'Enter' && event.key !== ' ') return;

            var target = event.target;
            while (target && target !== document) {
                if (target.matches && target.matches(config.lineSelector)) {
                    event.preventDefault();
                    toggleLineHighlight(event);
                    break;
                }
                target = target.parentElement;
            }
        });

        // Process existing elements
        processLineNumbers();

        // Watch for new elements
        if (window.MutationObserver) {
            var observer = new MutationObserver(function () {
                processLineNumbers();
            });

            observer.observe(document.body, {
                childList: true,
                subtree: true
            });
        }
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

