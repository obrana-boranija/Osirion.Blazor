// Markdown Editor Interop Functions
// This module provides JavaScript functionality for the MarkdownEditor and MarkdownPreview components

/**
 * Initializes the markdown editor
 * @param {HTMLElement} editorElement - The textarea element
 * @param {DotNetReference} dotNetRef - Reference to the .NET component
 */
export function initializeMarkdownEditor(editorElement, dotNetRef) {
    if (!editorElement) return;

    // Track scroll events on the editor
    editorElement.addEventListener('scroll', () => {
        const position = calculateScrollPosition(editorElement);
        dotNetRef.invokeMethodAsync('UpdateScrollPosition', position);
    });
}

/**
 * Initializes the markdown preview
 * @param {HTMLElement} previewElement - The preview container element
 * @param {DotNetReference} dotNetRef - Reference to the .NET component
 */
export function initializeMarkdownPreview(previewElement, dotNetRef) {
    if (!previewElement) return;

    // Track scroll events on the preview
    previewElement.addEventListener('scroll', () => {
        const position = calculateScrollPosition(previewElement);
        dotNetRef.invokeMethodAsync('UpdateScrollPosition', position);
    });
}

/**
 * Calculates the scroll position as a percentage (0-1)
 * @param {HTMLElement} element - The element to calculate scroll position for
 * @returns {number} - The scroll position as a percentage
 */
function calculateScrollPosition(element) {
    if (!element) return 0;

    const scrollTop = element.scrollTop;
    const scrollHeight = element.scrollHeight;
    const clientHeight = element.clientHeight;

    // Calculate the scroll percentage (0 to 1)
    let position = 0;
    if (scrollHeight > clientHeight) {
        position = scrollTop / (scrollHeight - clientHeight);
    }

    return position;
}

/**
 * Gets detailed scroll information for an element
 * @param {HTMLElement} element - The element to get scroll info for
 * @returns {Object} - Scroll information object
 */
export function getScrollInfo(element) {
    if (!element) return { position: 0, scrollTop: 0, scrollHeight: 0, clientHeight: 0 };

    const scrollTop = element.scrollTop;
    const scrollHeight = element.scrollHeight;
    const clientHeight = element.clientHeight;

    // Calculate the scroll percentage (0 to 1)
    let position = 0;
    if (scrollHeight > clientHeight) {
        position = scrollTop / (scrollHeight - clientHeight);
    }

    return {
        position,
        scrollTop,
        scrollHeight,
        clientHeight
    };
}

/**
 * Sets the scroll position of an element
 * @param {HTMLElement} element - The element to scroll
 * @param {number} position - The position as a percentage (0-1)
 */
export function setScrollPosition(element, position) {
    if (!element) return;

    const scrollHeight = element.scrollHeight;
    const clientHeight = element.clientHeight;

    if (scrollHeight <= clientHeight) return;

    // Calculate the target scroll position
    const targetScrollTop = position * (scrollHeight - clientHeight);

    // Set the scroll position
    element.scrollTop = targetScrollTop;
}

/**
 * Gets the selection information from a textarea
 * @param {HTMLTextAreaElement} textarea - The textarea element
 * @returns {Object} - Selection information
 */
export function getTextAreaSelection(textarea) {
    if (!textarea) return { text: '', start: 0, end: 0 };

    const start = textarea.selectionStart;
    const end = textarea.selectionEnd;
    const text = textarea.value.substring(start, end);

    return { text, start, end };
}

/**
 * Inserts text at the current cursor position in a textarea
 * @param {HTMLTextAreaElement} textarea - The textarea element
 * @param {string} text - The text to insert
 */
export function insertTextAtCursor(textarea, text) {
    if (!textarea) return;

    const start = textarea.selectionStart;
    const end = textarea.selectionEnd;
    const before = textarea.value.substring(0, start);
    const after = textarea.value.substring(end);

    // Set new text and update cursor position
    textarea.value = before + text + after;

    // Set selection after inserted text
    const newCursorPos = start + text.length;
    textarea.selectionStart = newCursorPos;
    textarea.selectionEnd = newCursorPos;

    // Focus the textarea
    textarea.focus();
}

/**
 * Wraps selected text in a textarea with prefix and suffix
 * @param {HTMLTextAreaElement} textarea - The textarea element
 * @param {string} prefix - Text to add before selection
 * @param {string} suffix - Text to add after selection
 * @param {string} defaultText - Default text if no selection
 */
export function wrapTextSelection(textarea, prefix, suffix, defaultText) {
    if (!textarea) return;

    const start = textarea.selectionStart;
    const end = textarea.selectionEnd;
    const selectedText = textarea.value.substring(start, end);
    const before = textarea.value.substring(0, start);
    const after = textarea.value.substring(end);

    // Use selected text or default text if no selection
    const textToWrap = selectedText.length > 0 ? selectedText : defaultText;

    // Set new text with wrapping
    textarea.value = before + prefix + textToWrap + suffix + after;

    // Set cursor position and selection
    if (selectedText.length > 0) {
        // Select the wrapped text
        textarea.selectionStart = start + prefix.length;
        textarea.selectionEnd = start + prefix.length + textToWrap.length;
    } else {
        // Place cursor after the default text
        const newPosition = start + prefix.length + defaultText.length;
        textarea.selectionStart = newPosition;
        textarea.selectionEnd = newPosition;
    }

    // Focus the textarea
    textarea.focus();
}

/**
 * Handles tab key in textarea for indentation
 * @param {HTMLTextAreaElement} textarea - The textarea element
 * @param {boolean} isShiftKey - Whether shift key is pressed
 */
export function handleTabKey(textarea, isShiftKey) {
    if (!textarea) return;

    const start = textarea.selectionStart;
    const end = textarea.selectionEnd;

    // If selection spans multiple lines
    if (start !== end) {
        const selectedText = textarea.value.substring(start, end);

        // Check if selection contains newlines
        if (selectedText.indexOf('\n') !== -1) {
            const before = textarea.value.substring(0, start);
            const after = textarea.value.substring(end);

            let newText;

            if (isShiftKey) {
                // Remove tab or 2 spaces from the beginning of each line
                newText = selectedText.replace(/^(\t|  )/gm, '');
            } else {
                // Add tab to the beginning of each line
                newText = selectedText.replace(/^/gm, '\t');
            }

            textarea.value = before + newText + after;

            // Update selection to cover the new text
            textarea.selectionStart = start;
            textarea.selectionEnd = start + newText.length;
            return;
        }
    }

    // Single line or no selection
    if (!isShiftKey) {
        // Insert tab character
        const before = textarea.value.substring(0, start);
        const after = textarea.value.substring(end);

        textarea.value = before + '\t' + after;

        // Move cursor position
        textarea.selectionStart = textarea.selectionEnd = start + 1;
    }
}

/**
 * Gets the value of an element
 * @param {HTMLElement} element - The element to get value from
 * @returns {string} - The element's value
 */
export function getElementValue(element) {
    return element ? element.value : '';
}

/**
 * Focuses an element
 * @param {HTMLElement} element - The element to focus
 */
export function focusElement(element) {
    if (element && typeof element.focus === 'function') {
        element.focus();
    }
}