(function() {
    'use strict';

    const config = {
        buttonSelector: '.osirion-copy-button',
        wrapperSelector: '.osirion-code-wrapper, .osirion-code-block, pre',
        codeSelector: 'code',
        initialized: 'osirion-copy-initialized',
        copiedClass: 'copied',
        errorClass: 'error',
        timeout: 2000
    };

    let initialized = false;

    // Copy text to clipboard with fallback
    async function copyToClipboard(text) {
        // Try modern clipboard API
        if (navigator.clipboard && window.isSecureContext) {
            try {
                await navigator.clipboard.writeText(text);
                return true;
            } catch (err) {
                console.warn('Clipboard API failed, using fallback:', err);
            }
        }

        // Fallback method
        const textarea = document.createElement('textarea');
        textarea.value = text;
        textarea.style.position = 'fixed';
        textarea.style.left = '-999999px';
        textarea.style.top = '-999999px';
        document.body.appendChild(textarea);
        
        try {
            textarea.select();
            document.execCommand('copy');
            return true;
        } catch (err) {
            console.error('Fallback copy failed:', err);
            return false;
        } finally {
            document.body.removeChild(textarea);
        }
    }

    // Handle copy button click
    async function handleCopyClick(button) {
        // Ensure we have a valid button element
        if (!button || typeof button.closest !== 'function') {
            console.error('Invalid button element');
            return;
        }

        const wrapper = button.closest(config.wrapperSelector);
        
        if (!wrapper) {
            console.error('Could not find code wrapper');
            return;
        }

        const codeElement = wrapper.querySelector(config.codeSelector);
        if (!codeElement) {
            console.error('Could not find code element');
            return;
        }

        const text = codeElement.textContent || codeElement.innerText || '';
        const textSpan = button.querySelector('.osirion-copy-text') || button;
        const originalText = textSpan.textContent;

        try {
            const success = await copyToClipboard(text);
            
            if (success) {
                textSpan.textContent = 'Copied!';
                button.classList.add(config.copiedClass);
                button.setAttribute('aria-label', 'Code copied to clipboard');
                
                setTimeout(() => {
                    textSpan.textContent = originalText;
                    button.classList.remove(config.copiedClass);
                    button.setAttribute('aria-label', 'Copy code to clipboard');
                }, config.timeout);
            } else {
                throw new Error('Copy failed');
            }
        } catch (err) {
            console.error('Failed to copy:', err);
            textSpan.textContent = 'Error';
            button.classList.add(config.errorClass);
            
            setTimeout(() => {
                textSpan.textContent = originalText;
                button.classList.remove(config.errorClass);
                button.setAttribute('aria-label', 'Copy code to clipboard');
            }, config.timeout);
        }
    }

    // Initialize copy buttons using event delegation
    function init() {
        if (initialized) return;
        initialized = true;

        // Use event delegation on document level
        document.addEventListener('click', async (event) => {
            // Find the button element, checking if the click was on the button or its child
            const button = event.target.closest(config.buttonSelector);
            if (!button) return;
            
            // Prevent default action
            event.preventDefault();
            
            // Mark as initialized if not already
            if (!button.classList.contains(config.initialized)) {
                button.classList.add(config.initialized);
                button.setAttribute('aria-label', 'Copy code to clipboard');
                button.setAttribute('role', 'button');
                button.setAttribute('tabindex', '0');
            }
            
            // Handle the copy action
            await handleCopyClick(button);
        });

        // Keyboard support
        document.addEventListener('keydown', async (event) => {
            if (event.key !== 'Enter' && event.key !== ' ') return;
            
            const button = event.target.closest(config.buttonSelector);
            if (!button) return;
            
            event.preventDefault();
            await handleCopyClick(button);
        });

        // Process existing buttons
        processButtons();
    }

    // Find and mark new buttons
    function processButtons() {
        const buttons = document.querySelectorAll(`${config.buttonSelector}:not(.${config.initialized})`);
        buttons.forEach(button => {
            button.classList.add(config.initialized);
            button.setAttribute('aria-label', 'Copy code to clipboard');
            button.setAttribute('role', 'button');
            button.setAttribute('tabindex', '0');
        });
    }

    // Auto-initialize
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }

    // Public API
    window.initializeCopyButtons = processButtons;
    window.OsirionCopyButtons = {
        init: init,
        processButtons: processButtons
    };
})();
