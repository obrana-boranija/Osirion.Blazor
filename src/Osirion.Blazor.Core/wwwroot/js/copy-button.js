function initializeCopyButtons() {
    'use strict';

    // Remove any existing event listeners to prevent duplicates
    document.querySelectorAll('.osirion-copy-button').forEach(button => {
        const newButton = button.cloneNode(true);
        button.parentNode.replaceChild(newButton, button);
    });

    // Add event listeners to all copy buttons
    document.querySelectorAll('.osirion-copy-button').forEach(button => {
        button.addEventListener('click', handleCopyClick);

        // Add keyboard support
        button.addEventListener('keydown', function (e) {
            if (e.key === 'Enter' || e.key === ' ') {
                e.preventDefault();
                handleCopyClick.call(this, e);
            }
        });
    });
}

async function handleCopyClick(event) {
    const button = event.currentTarget;
    const wrapper = button.closest('.osirion-code-wrapper');

    if (!wrapper) {
        console.error('Could not find code wrapper');
        return;
    }

    const codeBlock = wrapper.querySelector('code');

    if (!codeBlock) {
        console.error('Could not find code block');
        return;
    }

    const text = codeBlock.textContent || codeBlock.innerText;

    try {
        // Try modern clipboard API first
        if (navigator.clipboard && window.isSecureContext) {
            await navigator.clipboard.writeText(text);
        } else {
            // Fallback for older browsers
            copyTextFallback(text);
        }

        // Success feedback
        button.textContent = 'Copied!';
        button.classList.add('copied');
        button.setAttribute('aria-label', 'Code copied to clipboard');

        // Announce to screen readers
        announceToScreenReader('Code copied to clipboard');

        // Reset button after delay
        setTimeout(() => {
            button.textContent = 'Copy';
            button.classList.remove('copied');
            button.setAttribute('aria-label', 'Copy code to clipboard');
        }, 2000);

    } catch (err) {
        console.error('Failed to copy:', err);

        // Error feedback
        button.textContent = 'Error';
        button.classList.add('error');
        button.setAttribute('aria-label', 'Failed to copy code');

        announceToScreenReader('Failed to copy code to clipboard');

        setTimeout(() => {
            button.textContent = 'Copy';
            button.classList.remove('error');
            button.setAttribute('aria-label', 'Copy code to clipboard');
        }, 2000);
    }
}

function copyTextFallback(text) {
    // Create a temporary textarea element
    const textArea = document.createElement('textarea');
    textArea.value = text;
    textArea.style.position = 'fixed';
    textArea.style.left = '-999999px';
    textArea.style.top = '-999999px';
    document.body.appendChild(textArea);

    // Select and copy
    textArea.focus();
    textArea.select();

    try {
        document.execCommand('copy');
    } catch (err) {
        throw new Error('Fallback copy failed');
    } finally {
        textArea.remove();
    }
}

function announceToScreenReader(message) {
    const announcement = document.createElement('div');
    announcement.className = 'sr-only';
    announcement.setAttribute('role', 'status');
    announcement.setAttribute('aria-live', 'polite');
    announcement.textContent = message;

    document.body.appendChild(announcement);

    // Remove after announcement
    setTimeout(() => {
        announcement.remove();
    }, 3000);
}

// Export for use in other scripts
window.initializeCopyButtons = initializeCopyButtons;