(function() {
    'use strict';

    // Configuration
    const config = {
        selector: '[class*="language-"]',
        initialized: 'osirion-highlighted',
        maxAttempts: 50,
        retryDelay: 100,
        debounceDelay: 50
    };

    let observer = null;
    let debounceTimer = null;
    let initialized = false;

    // Initialize highlighting for specific elements
    function highlightElements(elements) {
        if (typeof Prism === 'undefined') return false;

        elements.forEach(element => {
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
        return Array.from(document.querySelectorAll(config.selector))
            .filter(el => !el.classList.contains(config.initialized));
    }

    // Process all code blocks
    function processCodeBlocks() {
        const elements = findUnhighlightedElements();
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
        debounceTimer = setTimeout(() => {
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

        observer = new MutationObserver((mutations) => {
            let shouldRefresh = false;

            for (const mutation of mutations) {
                // Check if code blocks were added
                if (mutation.type === 'childList') {
                    const hasNewCode = Array.from(mutation.addedNodes).some(node => {
                        if (node.nodeType !== Node.ELEMENT_NODE) return false;
                        return node.matches?.(config.selector) || 
                               node.querySelector?.(config.selector);
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
