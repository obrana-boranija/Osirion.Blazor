'use strict';

function initializeCopyButtons() {
    'use strict';

    // Remove any existing event listeners to prevent duplicates
    document.querySelectorAll('.osirion-copy-button').forEach(function (button) {
        var newButton = button.cloneNode(true);
        button.parentNode.replaceChild(newButton, button);
    });

    // Add event listeners to all copy buttons
    document.querySelectorAll('.osirion-copy-button').forEach(function (button) {
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

function handleCopyClick(event) {
    var button, wrapper, codeBlock, text;
    return regeneratorRuntime.async(function handleCopyClick$(context$1$0) {
        while (1) switch (context$1$0.prev = context$1$0.next) {
            case 0:
                button = event.currentTarget;
                wrapper = button.closest('.osirion-code-wrapper');

                if (wrapper) {
                    context$1$0.next = 5;
                    break;
                }

                console.error('Could not find code wrapper');
                return context$1$0.abrupt('return');

            case 5:
                codeBlock = wrapper.querySelector('code');

                if (codeBlock) {
                    context$1$0.next = 9;
                    break;
                }

                console.error('Could not find code block');
                return context$1$0.abrupt('return');

            case 9:
                text = codeBlock.textContent || codeBlock.innerText;
                context$1$0.prev = 10;

                if (!(navigator.clipboard && window.isSecureContext)) {
                    context$1$0.next = 16;
                    break;
                }

                context$1$0.next = 14;
                return regeneratorRuntime.awrap(navigator.clipboard.writeText(text));

            case 14:
                context$1$0.next = 17;
                break;

            case 16:
                // Fallback for older browsers
                copyTextFallback(text);

            case 17:

                // Success feedback
                button.textContent = 'Copied!';
                button.classList.add('copied');
                button.setAttribute('aria-label', 'Code copied to clipboard');

                // Announce to screen readers
                announceToScreenReader('Code copied to clipboard');

                // Reset button after delay
                setTimeout(function () {
                    button.textContent = 'Copy';
                    button.classList.remove('copied');
                    button.setAttribute('aria-label', 'Copy code to clipboard');
                }, 2000);

                context$1$0.next = 32;
                break;

            case 24:
                context$1$0.prev = 24;
                context$1$0.t0 = context$1$0['catch'](10);

                console.error('Failed to copy:', context$1$0.t0);

                // Error feedback
                button.textContent = 'Error';
                button.classList.add('error');
                button.setAttribute('aria-label', 'Failed to copy code');

                announceToScreenReader('Failed to copy code to clipboard');

                setTimeout(function () {
                    button.textContent = 'Copy';
                    button.classList.remove('error');
                    button.setAttribute('aria-label', 'Copy code to clipboard');
                }, 2000);

            case 32:
            case 'end':
                return context$1$0.stop();
        }
    }, null, this, [[10, 24]]);
}

function copyTextFallback(text) {
    // Create a temporary textarea element
    var textArea = document.createElement('textarea');
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
    var announcement = document.createElement('div');
    announcement.className = 'sr-only';
    announcement.setAttribute('role', 'status');
    announcement.setAttribute('aria-live', 'polite');
    announcement.textContent = message;

    document.body.appendChild(announcement);

    // Remove after announcement
    setTimeout(function () {
        announcement.remove();
    }, 3000);
}

// Export for use in other scripts
window.initializeCopyButtons = initializeCopyButtons;

// Try modern clipboard API first

