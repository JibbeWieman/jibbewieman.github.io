const keywords = ["public", "private", "protected", "static", "void", "class", "return", "new", "if", "else", "switch", "case", "break"];
const types = ["int", "string", "float", "double", "bool", "char", "decimal", "long", "short", "object", "Console"];
const commentPattern = /(\/\/.*)/g;
const stringPattern = /"([^"\\]*(\\.[^"\\]*)*)"/g;

function escapeHtml(text) {
    return text
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;");
}

function highlightCSharp(codeElement) {
    let text = codeElement.textContent;

    // Escape HTML first
    text = escapeHtml(text);

    // Step 1: Replace comments and strings with placeholders
    const replacements = [];
    let i = 0;

    text = text.replace(/^(\s+)/gm, (match) => {
        return match.replace(/ /g, '&nbsp;').replace(/\t/g, '&nbsp;&nbsp;&nbsp;&nbsp;');
    });


    text = text.replace(commentPattern, (match) => {
        const token = `__COMMENT_${i}__`;
        replacements.push({
            token,
            original: match,
            html: `<span class="comment">${match}</span>`
        });
        i++;
        return token;
    });

    text = text.replace(stringPattern, (match) => {
        const token = `__STRING_${i}__`;
        replacements.push({
            token,
            original: match,
            html: `<span class="string">${match}</span>`
        });
        i++;
        return token;
    });

    // Step 2: Highlight keywords with a trailing space
    keywords.forEach(keyword => {
        const regex = new RegExp(`\\b(${keyword})(\\b)(?![^<]*>)`, 'g');
        text = text.replace(regex, (match, word, boundary, offset, fullText) => {
            const nextChar = fullText[offset + match.length];
            const needsSpace =
                nextChar && ![' ', '\t', '\n', ';', ')', '.', ',', ':', '<'].includes(nextChar);
            return `<span class="keyword">${word}</span>${needsSpace ? ' ' : ''}`;
        });
    });

    // Step 3: Highlight types (no trailing space)
    types.forEach(type => {
        const regex = new RegExp(`\\b${type}\\b`, 'g');
        text = text.replace(regex, `<span class="type">${type}</span>`);
    });

    // Step 3.5: Normalize spacing after spans (ensure one space or punctuation, no duplication)
    text = text.replace(/<\/span>(?!\s)/g, '</span> ');


    // Step 4: Restore highlighted comments and strings
    replacements.forEach(replacement => {
        text = text.replaceAll(replacement.token, replacement.html);
    });

    // Step 5: Add line numbers
    const lines = text.split('\n').map((line, index) => {
        return `<div class="code-line"><span class="line-number">${index + 1}</span> ${line}</div>`;
    });

    codeElement.innerHTML = lines.join('');

}

document.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('code.language-csharp').forEach(codeBlock => {
        if (!codeBlock.dataset.highlighted) {
            highlightCSharp(codeBlock);
            codeBlock.dataset.highlighted = "true";
        }
    });
});
