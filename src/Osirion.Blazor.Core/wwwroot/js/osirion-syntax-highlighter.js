(function () {
    'use strict';

    // Configuration
    var config = {
        selector: '[class*="language-"]',
        initialized: 'osirion-highlighted',
        maxAttempts: 50,
        retryDelay: 100,
        debounceDelay: 50
    };

    var observer = null;
    var debounceTimer = null;
    var initialized = false;

    // Initialize highlighting for specific elements
    function highlightElements(elements) {
        if (typeof Prism === 'undefined') return false;

        Array.prototype.forEach.call(elements, function (element) {
            // Skip if already highlighted
            if (element.classList.contains(config.initialized)) return;

            // Mark as initialized first to prevent double processing
            element.classList.add(config.initialized);

            // Highlight the element
            Prism.highlightElement(element);
        });

        return true;
    }

    // Find all unhighlighted code blocks
    function findUnhighlightedElements() {
        var allElements = document.querySelectorAll(config.selector);
        var unhighlighted = [];

        Array.prototype.forEach.call(allElements, function (el) {
            if (!el.classList.contains(config.initialized)) {
                unhighlighted.push(el);
            }
        });

        return unhighlighted;
    }

    // Process all code blocks
    function processCodeBlocks() {
        var elements = findUnhighlightedElements();
        if (elements.length === 0) return;

        if (typeof Prism === 'undefined') {
            // Retry if Prism not loaded yet
            setTimeout(processCodeBlocks, config.retryDelay);
            return;
        }

        highlightElements(elements);
    }

    // Debounced refresh function
    function refresh() {
        clearTimeout(debounceTimer);
        debounceTimer = setTimeout(function () {
            processCodeBlocks();

            // Reinitialize other features
            if (window.initializeCopyButtons) {
                window.initializeCopyButtons();
            }
            if (window.initializeLineHighlighting) {
                window.initializeLineHighlighting();
            }
        }, config.debounceDelay);
    }

    // Set up mutation observer
    function setupObserver() {
        if (observer) observer.disconnect();

        observer = new MutationObserver(function (mutations) {
            var shouldRefresh = false;

            for (var i = 0; i < mutations.length; i++) {
                var mutation = mutations[i];

                // Check if code blocks were added
                if (mutation.type === 'childList') {
                    var addedNodes = Array.prototype.slice.call(mutation.addedNodes);

                    var hasNewCode = addedNodes.some(function (node) {
                        if (node.nodeType !== 1) return false; // Not an element node

                        // Check if node matches selector
                        if (node.matches && node.matches(config.selector)) {
                            return true;
                        }

                        // Check if node contains elements matching selector
                        if (node.querySelector && node.querySelector(config.selector)) {
                            return true;
                        }

                        return false;
                    });

                    if (hasNewCode) {
                        shouldRefresh = true;
                        break;
                    }
                }
            }

            if (shouldRefresh) refresh();
        });

        observer.observe(document.body, {
            childList: true,
            subtree: true
        });
    }

    // Initialize
    function init() {
        if (initialized) return;
        initialized = true;

        // Initial processing
        processCodeBlocks();

        // Set up observer
        setupObserver();

        // Listen for Blazor enhanced navigation
        document.addEventListener('enhancedload', refresh);

        // Also listen for standard navigation events
        window.addEventListener('popstate', refresh);
        window.addEventListener('hashchange', refresh);
    }

    // Start when DOM is ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }

    // Public API
    window.OsirionSyntaxHighlighter = {
        init: init,
        refresh: refresh,
        processCodeBlocks: processCodeBlocks
    };
})();