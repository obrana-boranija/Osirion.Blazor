'use strict';

function initializeLineHighlighting() {
    'use strict';

    // Store highlighted lines for each code block
    var highlightedLines = new Map();

    // Initialize line highlighting for all code blocks with line numbers
    document.querySelectorAll('.line-numbers pre').forEach(function (pre, preIndex) {
        initializeCodeBlock(pre, preIndex, highlightedLines);
    });
}

function initializeCodeBlock(pre, preIndex, highlightedLines) {
    var lineElements = pre.querySelectorAll('.line-numbers-rows > span');

    if (!lineElements.length) {
        return;
    }

    // Calculate line metrics once
    var lineMetrics = calculateLineMetrics(pre);

    lineElements.forEach(function (span, index) {
        var lineNum = index + 1;

        // Make line numbers interactive
        span.setAttribute('role', 'button');
        span.setAttribute('aria-label', 'Toggle highlight for line ' + lineNum);
        span.setAttribute('tabindex', '0');
        span.style.cursor = 'pointer';

        // Remove existing listeners
        span.replaceWith(span.cloneNode(true));
        var newSpan = pre.querySelectorAll('.line-numbers-rows > span')[index];

        // Add click handler
        newSpan.addEventListener('click', function (e) {
            toggleLineHighlight(e, pre, preIndex, lineNum, highlightedLines, lineMetrics);
        });

        // Add keyboard handler
        newSpan.addEventListener('keydown', function (e) {
            if (e.key === 'Enter' || e.key === ' ') {
                e.preventDefault();
                toggleLineHighlight(e, pre, preIndex, lineNum, highlightedLines, lineMetrics);
            }
        });
    });
}

function toggleLineHighlight(event, pre, preIndex, lineNum, highlightedLines, lineMetrics) {
    var span = event.currentTarget;
    var isCtrlPressed = event.ctrlKey || event.metaKey;

    var currentHighlights = highlightedLines.get(preIndex) || [];

    if (span.classList.contains('osirion-line-highlighted')) {
        // Remove highlight
        span.classList.remove('osirion-line-highlighted');
        currentHighlights = currentHighlights.filter(function (n) {
            return n !== lineNum;
        });
        span.setAttribute('aria-pressed', 'false');
    } else {
        // Add highlight
        if (!isCtrlPressed) {
            // Clear all highlights if Ctrl/Cmd not pressed
            pre.querySelectorAll('.osirion-line-highlighted').forEach(function (el) {
                el.classList.remove('osirion-line-highlighted');
                el.setAttribute('aria-pressed', 'false');
            });
            currentHighlights = [];
        }

        span.classList.add('osirion-line-highlighted');
        span.setAttribute('aria-pressed', 'true');

        if (!currentHighlights.includes(lineNum)) {
            currentHighlights.push(lineNum);
        }
    }

    // Sort line numbers
    currentHighlights.sort(function (a, b) {
        return a - b;
    });
    highlightedLines.set(preIndex, currentHighlights);

    // Update visual overlays
    createHighlightOverlays(pre, currentHighlights, lineMetrics);

    // Announce to screen readers
    var action = span.classList.contains('osirion-line-highlighted') ? 'highlighted' : 'unhighlighted';
    announceLineAction(lineNum, action, isCtrlPressed);
}

function calculateLineMetrics(pre) {
    var codeElement = pre.querySelector('code');
    if (!codeElement) return null;

    var computedStyle = window.getComputedStyle(pre);
    var codeStyle = window.getComputedStyle(codeElement);

    return {
        lineHeight: parseFloat(computedStyle.lineHeight) || 20,
        paddingTop: parseFloat(codeStyle.paddingTop) || 0,
        paddingLeft: parseFloat(codeStyle.paddingLeft) || 0
    };
}

function createHighlightOverlays(pre, highlightedLines, lineMetrics) {
    // Remove existing overlays
    pre.querySelectorAll('.osirion-line-highlight-overlay').forEach(function (el) {
        return el.remove();
    });

    if (!lineMetrics || !highlightedLines.length) return;

    highlightedLines.forEach(function (lineNum) {
        var overlay = document.createElement('div');
        overlay.className = 'osirion-line-highlight-overlay';

        // Position the overlay
        var top = lineMetrics.paddingTop + (lineNum - 1) * lineMetrics.lineHeight;

        Object.assign(overlay.style, {
            position: 'absolute',
            top: top + 'px',
            left: '0',
            right: '0',
            height: lineMetrics.lineHeight + 'px',
            backgroundColor: 'rgba(255, 255, 0, 0.1)',
            pointerEvents: 'none',
            zIndex: '1'
        });

        pre.appendChild(overlay);
    });

    // Ensure pre has position relative
    if (window.getComputedStyle(pre).position === 'static') {
        pre.style.position = 'relative';
    }
}

function announceLineAction(lineNum, action, isMultiSelect) {
    var message = isMultiSelect ? 'Line ' + lineNum + ' ' + action + '. Hold Ctrl or Cmd to select multiple lines.' : 'Line ' + lineNum + ' ' + action;

    var announcement = document.createElement('div');
    announcement.className = 'sr-only';
    announcement.setAttribute('role', 'status');
    announcement.setAttribute('aria-live', 'polite');
    announcement.textContent = message;

    document.body.appendChild(announcement);

    setTimeout(function () {
        announcement.remove();
    }, 3000);
}

// Export for use in other scripts
window.initializeLineHighlighting = initializeLineHighlighting;

