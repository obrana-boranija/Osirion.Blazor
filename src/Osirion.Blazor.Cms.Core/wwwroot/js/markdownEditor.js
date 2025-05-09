/**
 * Focuses an element
 * @param {HTMLElement} element - The element to focus
 */
export function focusElement(element) {
    if (element) {
        element.focus();
    }
}

/**
 * Gets the caret position in a textarea
 * @param {HTMLTextAreaElement} textarea - The textarea element
 * @returns {number} The caret position
 */
export function getCaretPosition(textarea) {
    if (!textarea) return 0;
    return textarea.selectionStart;
}

/**
 * Sets the caret position in a textarea
 * @param {HTMLTextAreaElement} textarea - The textarea element
 * @param {number} position - The position to set the caret to
 */
export function setCaretPosition(textarea, position) {
    if (!textarea) return;
    textarea.focus();
    textarea.setSelectionRange(position, position);
}

/**
 * Inserts text at the current cursor position in a textarea
 * @param {HTMLTextAreaElement} textarea - The textarea element
 * @param {string} prefix - Text to insert before the cursor or selection
 * @param {string} suffix - Text to insert after the cursor or selection
 * @param {string} placeholder - Text to insert if no text is selected
 * @returns {Object} The new text and caret position
 */
export function insertTextAtCursor(textarea, prefix, suffix, placeholder) {
    if (!textarea) return { text: "", caretPosition: 0 };

    const startPos = textarea.selectionStart;
    const endPos = textarea.selectionEnd;
    const text = textarea.value;
    const selectedText = text.substring(startPos, endPos);

    let newText;
    let newCaretPosition;

    if (selectedText) {
        // If text is selected, wrap it with prefix and suffix
        newText = text.substring(0, startPos) + prefix + selectedText + suffix + text.substring(endPos);
        newCaretPosition = startPos + prefix.length + selectedText.length + suffix.length;
    } else {
        // If no text is selected, insert prefix + placeholder + suffix
        newText = text.substring(0, startPos) + prefix + placeholder + suffix + text.substring(endPos);
        newCaretPosition = startPos + prefix.length + placeholder.length;
    }

    textarea.value = newText;

    // Set the caret position appropriately
    textarea.focus();
    textarea.setSelectionRange(newCaretPosition, newCaretPosition);

    return { text: newText, caretPosition: newCaretPosition };
}

/**
 * Handles the tab key in a textarea for indentation
 * @param {HTMLTextAreaElement} textarea - The textarea element
 * @param {boolean} isShiftKey - Whether shift key was pressed
 * @returns {Object} The new text and caret position
 */
export function handleTabKey(textarea, isShiftKey) {
    if (!textarea) return { text: "", caretPosition: 0 };

    const startPos = textarea.selectionStart;
    const endPos = textarea.selectionEnd;
    const text = textarea.value;

    // If text is selected and spans multiple lines
    if (startPos !== endPos && text.substring(startPos, endPos).includes('\n')) {
        const selectedLines = text.substring(startPos, endPos).split('\n');
        const startLinePos = text.lastIndexOf('\n', startPos - 1) + 1;
        const endLinePos = endPos + text.substring(endPos).indexOf('\n');

        let newText;
        let newSelection;

        if (isShiftKey) {
            // Un-indent selected lines
            const processedLines = selectedLines.map(line => {
                if (line.startsWith('  ')) return line.substring(2);
                if (line.startsWith('\t')) return line.substring(1);
                return line;
            });

            newText = text.substring(0, startLinePos) +
                processedLines.join('\n') +
                text.substring(endLinePos === -1 ? text.length : endLinePos);

            newSelection = { start: startLinePos, end: startLinePos + processedLines.join('\n').length };
        } else {
            // Indent selected lines
            const processedLines = selectedLines.map(line => '  ' + line);

            newText = text.substring(0, startLinePos) +
                processedLines.join('\n') +
                text.substring(endLinePos === -1 ? text.length : endLinePos);

            newSelection = { start: startLinePos, end: startLinePos + processedLines.join('\n').length };
        }

        textarea.value = newText;
        textarea.setSelectionRange(newSelection.start, newSelection.end);

        return { text: newText, caretPosition: newSelection.end };
    } else {
        // If no selection or selection is on a single line
        if (isShiftKey) {
            // If shift is pressed, we try to remove indentation
            const lineStart = text.lastIndexOf('\n', startPos - 1) + 1;
            const linePrefix = text.substring(lineStart, startPos);

            if (linePrefix.startsWith('  ')) {
                // Remove 2 spaces from the line
                const newText = text.substring(0, lineStart) +
                    linePrefix.substring(2) +
                    text.substring(startPos);

                const newCaretPosition = Math.max(startPos - 2, lineStart);
                textarea.value = newText;
                textarea.setSelectionRange(newCaretPosition, newCaretPosition);

                return { text: newText, caretPosition: newCaretPosition };
            } else if (linePrefix.startsWith('\t')) {
                // Remove a tab from the line
                const newText = text.substring(0, lineStart) +
                    linePrefix.substring(1) +
                    text.substring(startPos);

                const newCaretPosition = Math.max(startPos - 1, lineStart);
                textarea.value = newText;
                textarea.setSelectionRange(newCaretPosition, newCaretPosition);

                return { text: newText, caretPosition: newCaretPosition };
            }

            // If there's no indentation to remove, do nothing
            return { text, caretPosition: startPos };
        } else {
            // Insert 2 spaces at the cursor position
            const newText = text.substring(0, startPos) + '  ' + text.substring(endPos);
            const newCaretPosition = startPos + 2;

            textarea.value = newText;
            textarea.setSelectionRange(newCaretPosition, newCaretPosition);

            return { text: newText, caretPosition: newCaretPosition };
        }
    }
}

/**
 * Gets scroll information for an element
 * @param {HTMLElement} element - The element to get scroll info for
 * @returns {Object} Scroll information
 */
export function getScrollInfo(element) {
    if (!element) return { scrollTop: 0, scrollHeight: 0, clientHeight: 0, percentage: 0 };

    const scrollTop = element.scrollTop;
    const scrollHeight = element.scrollHeight;
    const clientHeight = element.clientHeight;
    const percentage = scrollHeight <= clientHeight ? 0 : scrollTop / (scrollHeight - clientHeight);

    return { scrollTop, scrollHeight, clientHeight, percentage };
}

/**
 * Sets the scroll percentage of an element
 * @param {HTMLElement} element - The element to scroll
 * @param {number} percentage - The percentage to scroll to
 */
export function setScrollPercentage(element, percentage) {
    if (!element) return;

    const scrollHeight = element.scrollHeight;
    const clientHeight = element.clientHeight;

    if (scrollHeight <= clientHeight) return;

    const scrollTop = percentage * (scrollHeight - clientHeight);
    element.scrollTop = scrollTop;
}

/**
 * Prevents the default behavior of the current event
 */
export function preventDefault() {
    event.preventDefault();
}

/**
 * Toggles fullscreen mode for an element
 * @param {string} selector - The CSS selector for the element
 * @returns {boolean} Whether the element is now in fullscreen mode
 */
export function toggleFullscreen(selector) {
    const element = document.querySelector(selector);
    if (!element) return false;

    if (!element.classList.contains('fullscreen')) {
        element.classList.add('fullscreen');
        document.body.style.overflow = 'hidden';
        return true;
    } else {
        element.classList.remove('fullscreen');
        document.body.style.overflow = '';
        return false;
    }
}

/**
 * Sets up a resize observer for the textarea to adjust its height
 * @param {HTMLTextAreaElement} textarea - The textarea element
 */
export function initResizeObserver(textarea) {
    if (!textarea || !window.ResizeObserver) return;

    const resizeObserver = new ResizeObserver(() => {
        // Adjust height logic if needed
    });

    resizeObserver.observe(textarea);

    // Return a function to disconnect the observer
    return () => {
        resizeObserver.disconnect();
    };
}

/**
 * Sets up an intersection observer for the editor
 * @param {HTMLElement} element - The element to observe
 * @param {Function} callback - The callback to run when intersection changes
 */
export function initIntersectionObserver(element, callback) {
    if (!element || !window.IntersectionObserver) return;

    const options = {
        root: null,
        rootMargin: '0px',
        threshold: [0, 0.1, 0.5, 1]
    };

    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            const ratio = entry.intersectionRatio;
            callback(ratio);
        });
    }, options);

    observer.observe(element);

    // Return a function to disconnect the observer
    return () => {
        observer.disconnect();
    };
}

/**
 * Initializes the editor with various settings
 * @param {HTMLTextAreaElement} textarea - The textarea element
 * @param {Object} options - Configuration options
 */
export function initEditor(textarea, options = {}) {
    if (!textarea) return;

    // Add tab trap to prevent focus from leaving the textarea when pressing tab
    textarea.addEventListener('keydown', (e) => {
        if (e.key === 'Tab') {
            e.preventDefault();
        }
    });

    // Initialize any observers if needed
    if (options.autoResize) {
        initResizeObserver(textarea);
    }

    // Focus the textarea if needed
    if (options.autoFocus) {
        focusElement(textarea);
    }

    // Add any additional initialization here
}