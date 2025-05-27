(function () {
    'use strict';

    // Configuration
    const config = {
        maxAttempts: 50,
        retryDelay: 100,
        enableCopyButton: true,
        enableLineHighlighting: true,
        showLineNumbers: true
    };

    let attempts = 0;

    function initializeHighlighting() {
        attempts++;

        if (typeof Prism === 'undefined') {
            if (attempts < config.maxAttempts) {
                setTimeout(initializeHighlighting, config.retryDelay);
            } else {
                console.error('Prism.js failed to load after', config.maxAttempts, 'attempts');
            }
            return;
        }

        // Highlight all code blocks
        Prism.highlightAll();

        // Initialize features based on configuration
        if (config.enableCopyButton) {
            initializeCopyButtons();
        }

        if (config.enableLineHighlighting && config.showLineNumbers) {
            initializeLineHighlighting();
        }
    }

    // Start initialization
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', function () {
            setTimeout(initializeHighlighting, config.retryDelay);
        });
    } else {
        setTimeout(initializeHighlighting, config.retryDelay);
    }

    // Make configuration available globally if needed
    window.OsirionSyntaxHighlighter = {
        config: config,
        reinitialize: initializeHighlighting
    };
})();