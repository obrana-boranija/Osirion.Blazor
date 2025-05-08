/**
 * Markdown editor functionality with progressive enhancement for Blazor
 * Supports both client-side and server-side rendering
 */

// Helper function to safely get element even if it doesn't exist yet
function getElement(id) {
    return document.getElementById(id);
}

/**
 * Initializes a markdown editor on the specified element
 * @param {string} editorId - The ID of the textarea element
 * @param {string} previewId - The ID of the preview element
 * @returns {Object} - Editor controller with methods
 */
export function initializeEditor(editorId, previewId) {
    const editorElement = getElement(editorId);
    const previewElement = getElement(previewId);

    if (!editorElement) {
        console.warn(`Markdown editor element with ID '${editorId}' not found`);
        return {
            getValue: () => "",
            setValue: () => { },
            focus: () => { },
            getSelectionRange: () => ({ start: 0, end: 0 }),
            insertTextAtCursor: () => { }
        };
    }

    // Set up basic editor behavior - can be enhanced with a markdown library if needed
    const setupTabHandler = () => {
        editorElement.addEventListener('keydown', function (e) {
            if (e.key === 'Tab') {
                e.preventDefault();

                const start = this.selectionStart;
                const end = this.selectionEnd;

                // Set textarea value to: text before caret + tab + text after caret
                this.value = this.value.substring(0, start) +
                    "    " + this.value.substring(end); // 4 spaces for a tab

                // Move caret to proper position
                this.selectionStart = this.selectionEnd = start + 4;
            }
        });
    };

    // Initialize the editor
    setupTabHandler();

    // Return API object for Blazor interop
    return {
        getValue: () => editorElement.value,

        setValue: (text) => {
            editorElement.value = text;
        },

        focus: () => {
            editorElement.focus();
        },

        getSelectionRange: () => {
            return {
                start: editorElement.selectionStart,
                end: editorElement.selectionEnd
            };
        },

        insertTextAtCursor: (text) => {
            const start = editorElement.selectionStart;
            const end = editorElement.selectionEnd;

            // Insert text at cursor position
            editorElement.value = editorElement.value.substring(0, start) +
                text + editorElement.value.substring(end);

            // Move cursor after inserted text
            const newPosition = start + text.length;
            editorElement.selectionStart = editorElement.selectionEnd = newPosition;

            // Focus back to editor
            editorElement.focus();
        },

        // Apply formatting to selected text or insert at cursor
        applyFormatting: (formatType) => {
            const start = editorElement.selectionStart;
            const end = editorElement.selectionEnd;
            const selectedText = editorElement.value.substring(start, end);

            let formattedText = '';
            let cursorOffset = 0;

            switch (formatType) {
                case 'bold':
                    formattedText = `**${selectedText}**`;
                    cursorOffset = selectedText ? 0 : 2;
                    break;
                case 'italic':
                    formattedText = `*${selectedText}*`;
                    cursorOffset = selectedText ? 0 : 1;
                    break;
                case 'heading1':
                    formattedText = `# ${selectedText}`;
                    cursorOffset = selectedText ? 0 : 2;
                    break;
                case 'heading2':
                    formattedText = `## ${selectedText}`;
                    cursorOffset = selectedText ? 0 : 3;
                    break;
                case 'heading3':
                    formattedText = `### ${selectedText}`;
                    cursorOffset = selectedText ? 0 : 4;
                    break;
                case 'link':
                    formattedText = selectedText ? `[${selectedText}](url)` : '[](url)';
                    cursorOffset = selectedText ? 1 : 1;
                    break;
                case 'image':
                    formattedText = `![${selectedText}](url)`;
                    cursorOffset = selectedText ? 0 : 2;
                    break;
                case 'code':
                    formattedText = `\`${selectedText}\``;
                    cursorOffset = selectedText ? 0 : 1;
                    break;
                case 'codeblock':
                    formattedText = `\`\`\`\n${selectedText}\n\`\`\``;
                    cursorOffset = selectedText ? 0 : 4;
                    break;
                case 'quote':
                    formattedText = `> ${selectedText}`;
                    cursorOffset = selectedText ? 0 : 2;
                    break;
                case 'list':
                    formattedText = `- ${selectedText}`;
                    cursorOffset = selectedText ? 0 : 2;
                    break;
                default:
                    formattedText = selectedText;
            }

            // Replace selected text with formatted text
            editorElement.value = editorElement.value.substring(0, start) +
                formattedText + editorElement.value.substring(end);

            // Position cursor correctly
            if (selectedText) {
                // If there was selected text, place cursor at the end of the formatted text
                editorElement.selectionStart = editorElement.selectionEnd = start + formattedText.length;
            } else {
                // If no text was selected, place cursor within the formatting marks
                const newPosition = start + formattedText.length - cursorOffset;
                editorElement.selectionStart = editorElement.selectionEnd = newPosition;
            }

            // Focus back to editor
            editorElement.focus();
        }
    };
}

/**
 * Updates the preview with the rendered HTML
 * @param {string} previewId - The ID of the preview element
 * @param {string} html - The HTML content to display
 */
export function updatePreview(previewId, html) {
    const previewElement = getElement(previewId);
    if (previewElement) {
        previewElement.innerHTML = html;
    }
}

/**
 * Clean up the editor instance
 * @param {string} editorId - The ID of the editor element
 */
export function disposeEditor(editorId) {
    // Currently nothing to clean up with this simple implementation
    // This method exists for future enhancements
}