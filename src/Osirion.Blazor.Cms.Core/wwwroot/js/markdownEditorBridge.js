// markdownEditorBridge.js
import * as editor from './markdownEditorInterop.js';

// Bridge functions to global scope
window.initializeMarkdownEditor = editor.initializeMarkdownEditor;
window.initializeMarkdownPreview = editor.initializeMarkdownPreview;
window.getScrollInfo = editor.getScrollInfo;
window.setScrollPosition = editor.setScrollPosition;
window.getTextAreaSelection = editor.getTextAreaSelection;
window.insertTextAtCursor = editor.insertTextAtCursor;
window.wrapTextSelection = editor.wrapTextSelection;
window.handleTabKey = editor.handleTabKey;
window.getElementValue = editor.getElementValue;
window.focusElement = editor.focusElement;

console.log('Markdown Editor functions bridged to global scope');