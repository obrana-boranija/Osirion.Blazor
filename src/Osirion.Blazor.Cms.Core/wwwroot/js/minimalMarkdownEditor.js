// minimalMarkdownEditor.js
// A minimal JavaScript module for Markdown editor functionality that can't be done in C#

/**
 * Focus the given element
 * @param {HTMLElement} element - The element to focus
 */
export function focusElement(element) {
    element.focus();
}

/**
 * Get scroll information about an element
 * @param {HTMLElement} element - The element to get scroll info for
 * @returns {Object} Scroll information including percentage
 */
export function getScrollInfo(element) {
    const scrollTop = element.scrollTop;
    const scrollHeight = element.scrollHeight;
    const clientHeight = element.clientHeight;
    const percentage = scrollHeight <= clientHeight
        ? 0
        : scrollTop / (scrollHeight - clientHeight);

    return {
        scrollTop,
        scrollHeight,
        clientHeight,
        percentage
    };
}

/**
 * Set scroll position by percentage
 * @param {HTMLElement} element - The element to scroll
 * @param {number} percentage - The scroll percentage (0-1)
 */
export function setScrollPercentage(element, percentage) {
    const scrollHeight = element.scrollHeight;
    const clientHeight = element.clientHeight;

    if (scrollHeight > clientHeight) {
        element.scrollTop = percentage * (scrollHeight - clientHeight);
    }
}

/**
 * Insert text at the current cursor position
 * @param {HTMLTextAreaElement} textarea - The textarea element
 * @param {string} prefix - Text to insert before selection
 * @param {string} suffix - Text to insert after selection
 * @param {string} placeholder - Text to use if no selection
 * @returns {string} The new content
 */
export function insertTextAtCursor(textarea, prefix, suffix, placeholder) {
    const start = textarea.selectionStart;
    const end = textarea.selectionEnd;
    const text = textarea.value;
    const selectedText = text.substring(start, end);
    const replacement = selectedText.length > 0 ? selectedText : placeholder;

    const newText = text.substring(0, start) +
        prefix +
        replacement +
        suffix +
        text.substring(end);

    // Update textarea value
    textarea.value = newText;

    // Set new cursor position
    const newCursorPos = start + prefix.length + replacement.length;
    textarea.setSelectionRange(newCursorPos, newCursorPos);

    return newText;
}

/**
 * Handle tab key in textarea (for indentation)
 * @param {HTMLTextAreaElement} textarea - The textarea element
 * @param {boolean} isShiftKey - Whether shift key is pressed
 * @returns {string} The new content
 */
export function handleTabKey(textarea, isShiftKey) {
    const start = textarea.selectionStart;
    const end = textarea.selectionEnd;
    const text = textarea.value;

    // If multiple lines are selected
    if (start !== end && text.substring(start, end).includes('\n')) {
        return handleMultiLineTab(textarea, isShiftKey);
    }

    // Single line or no selection
    if (isShiftKey) {
        // Unindent: remove a tab or spaces at the beginning of the line
        const lineStart = text.lastIndexOf('\n', start - 1) + 1;
        const lineEnd = text.indexOf('\n', start);
        const line = lineEnd !== -1 ? text.substring(lineStart, lineEnd) : text.substring(lineStart);

        if (line.startsWith('\t')) {
            // Remove one tab
            const newText = text.substring(0, lineStart) + line.substring(1) + text.substring(lineEnd !== -1 ? lineEnd : text.length);
            textarea.value = newText;
            textarea.setSelectionRange(start - 1 > lineStart ? start - 1 : lineStart, end - 1 > lineStart ? end - 1 : lineStart);
            return newText;
        } else if (line.startsWith('    ')) {
            // Remove one level of space indentation (4 spaces)
            const newText = text.substring(0, lineStart) + line.substring(4) + text.substring(lineEnd !== -1 ? lineEnd : text.length);
            textarea.value = newText;
            textarea.setSelectionRange(start - 4 > lineStart ? start - 4 : lineStart, end - 4 > lineStart ? end - 4 : lineStart);
            return newText;
        }

        return text; // No change
    } else {
        // Insert a tab
        const newText = text.substring(0, start) + '\t' + text.substring(end);
        textarea.value = newText;
        textarea.setSelectionRange(start + 1, start + 1);
        return newText;
    }
}

/**
 * Handle tab key for multi-line selection
 * @param {HTMLTextAreaElement} textarea - The textarea element
 * @param {boolean} isShiftKey - Whether shift key is pressed
 * @returns {string} The new content
 */
function handleMultiLineTab(textarea, isShiftKey) {
    const start = textarea.selectionStart;
    const end = textarea.selectionEnd;
    const text = textarea.value;

    // Find the start of the first line
    const lineStart = text.lastIndexOf('\n', start - 1) + 1;

    // Get the selected text plus the start of the first line
    const selectionWithStartLine = text.substring(lineStart, end);

    // Split into lines
    const lines = selectionWithStartLine.split('\n');

    // Process each line
    const processedLines = lines.map(line => {
        if (isShiftKey) {
            // Remove indentation
            if (line.startsWith('\t')) {
                return line.substring(1);
            } else if (line.startsWith('    ')) {
                return line.substring(4);
            }
            return line;
        } else {
            // Add indentation
            return '\t' + line;
        }
    });

    // Join lines back together
    const newSelection = processedLines.join('\n');

    // Calculate change in length
    const lengthDifference = newSelection.length - selectionWithStartLine.length;

    // Create new text
    const newText = text.substring(0, lineStart) + newSelection + text.substring(end);

    // Update textarea
    textarea.value = newText;

    // Set selection
    const newStart = start;
    const newEnd = end + lengthDifference;
    textarea.setSelectionRange(newStart, newEnd);

    return newText;
}