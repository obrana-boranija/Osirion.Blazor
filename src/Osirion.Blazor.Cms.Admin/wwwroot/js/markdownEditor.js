/**
 * Minimal JavaScript for the Markdown Editor component
 * This provides essential text manipulation functions while minimizing JS dependencies
 */

// Focus on a specific element
export function focusElement(element) {
    if (element) {
        element.focus();

        // If it's a textarea, place cursor at the end
        if (element.tagName.toLowerCase() === 'textarea') {
            const length = element.value.length;
            element.setSelectionRange(length, length);
        }
    }
}

/**
 * Inserts text at the current cursor position in a textarea
 * @param {HTMLTextAreaElement} textarea - The textarea element
 * @param {string} prefix - Text to insert before the selection/cursor
 * @param {string} suffix - Text to insert after the selection/cursor
 * @param {string} placeholder - Text to insert if no text is selected
 * @returns {string} - The updated textarea value
 */
export function insertTextAtCursor(textarea, prefix, suffix, placeholder) {
    if (!textarea) return "";

    // Save the current scroll position before modifying text
    const scrollTop = textarea.scrollTop;

    // Get the current selection start and end positions
    const startPos = textarea.selectionStart;
    const endPos = textarea.selectionEnd;

    // Get the current value of the textarea
    const text = textarea.value;

    // Get the selected text
    const selectedText = text.substring(startPos, endPos);

    // Determine what text to insert
    const insertText = selectedText.length > 0 ? selectedText : placeholder;

    // Create the new text value with the insertion
    const newText =
        text.substring(0, startPos) +
        prefix +
        insertText +
        suffix +
        text.substring(endPos);

    // Set the new value
    textarea.value = newText;

    // Calculate where the cursor should be after insertion
    const newCursorPos = startPos + prefix.length + insertText.length + suffix.length;

    // Set the selection range to position the cursor
    setTimeout(() => {
        textarea.focus();
        textarea.setSelectionRange(newCursorPos, newCursorPos);

        // Restore the scroll position
        textarea.scrollTop = scrollTop;

        // Trigger an input event to update bound values
        const event = new Event('input', { bubbles: true });
        textarea.dispatchEvent(event);
    }, 0);

    // Return the new text value
    return newText;
}

/**
 * Gets scroll information from an element
 * @param {HTMLElement} element - The element to get scroll info from
 * @returns {object} - Scroll information including percentage
 */
export function getScrollInfo(element) {
    if (!element) {
        return { scrollTop: 0, scrollHeight: 0, clientHeight: 0, percentage: 0 };
    }

    const scrollTop = element.scrollTop;
    const scrollHeight = element.scrollHeight;
    const clientHeight = element.clientHeight;

    // Calculate the scroll percentage (0 to 1)
    const maxScroll = scrollHeight - clientHeight;
    const percentage = maxScroll <= 0 ? 0 : scrollTop / maxScroll;

    return {
        scrollTop,
        scrollHeight,
        clientHeight,
        percentage
    };
}

/**
 * Sets the scroll position of an element based on a percentage
 * @param {HTMLElement} element - The element to scroll
 * @param {number} percentage - The scroll percentage (0 to 1)
 */
export function setScrollPercentage(element, percentage) {
    if (!element) return;

    const maxScroll = element.scrollHeight - element.clientHeight;
    if (maxScroll <= 0) return;

    // Ensure percentage is between 0 and 1
    const clampedPercentage = Math.max(0, Math.min(1, percentage));

    // Set the scroll position
    element.scrollTop = maxScroll * clampedPercentage;
}

/**
 * Handles tab key press in the textarea for indentation
 * @param {HTMLTextAreaElement} textarea - The textarea element
 * @param {boolean} isShiftKey - Whether shift key was pressed with tab
 * @returns {string} - The updated textarea value
 */
export function handleTabKey(textarea, isShiftKey) {
    if (!textarea) return "";

    // Save the current scroll position
    const scrollTop = textarea.scrollTop;

    const start = textarea.selectionStart;
    const end = textarea.selectionEnd;
    const text = textarea.value;

    // Check if multiple lines are selected
    const selectedText = text.substring(start, end);
    const hasNewline = selectedText.indexOf('\n') !== -1;

    let result;

    // Handle multi-line indentation/outdentation
    if (hasNewline) {
        // Split text into lines
        const lines = selectedText.split('\n');
        let newText;

        if (isShiftKey) {
            // Remove indentation (outdent)
            newText = lines.map(line => line.startsWith('    ')
                ? line.substring(4)
                : (line.startsWith('\t') ? line.substring(1) : line)
            ).join('\n');
        } else {
            // Add indentation (indent)
            newText = lines.map(line => '    ' + line).join('\n');
        }

        // Replace the selected text with the indented/outdented text
        result = text.substring(0, start) + newText + text.substring(end);
        textarea.value = result;

        // Update selection
        setTimeout(() => {
            textarea.focus();
            textarea.setSelectionRange(start, start + newText.length);
            textarea.scrollTop = scrollTop;

            // Trigger an input event to update bound values
            const event = new Event('input', { bubbles: true });
            textarea.dispatchEvent(event);
        }, 0);
    }
    // Handle single line or no selection
    else {
        if (isShiftKey) {
            // Handle outdent logic for single line if needed
            return text;
        } else {
            // Insert a tab (as 4 spaces) at cursor position
            result = text.substring(0, start) + '    ' + text.substring(end);
            textarea.value = result;

            setTimeout(() => {
                textarea.focus();
                textarea.setSelectionRange(start + 4, start + 4);
                textarea.scrollTop = scrollTop;

                // Trigger an input event to update bound values
                const event = new Event('input', { bubbles: true });
                textarea.dispatchEvent(event);
            }, 0);
        }
    }

    return result || text;
}