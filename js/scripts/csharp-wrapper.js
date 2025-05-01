function escapeHtml(text) {
    return text
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;");
}

/* BROKEN
/// <summary>
/// Highlights C# syntax inside a code element.
/// </summary>
function highlightCSharp(codeElement) {
    let text = codeElement.textContent;

    const keywords = ["public", "private", "protected", "static", "void", "class", "return", "new", "if", "else", "switch", "case", "break", "int", "string", "float", "double", "bool", "char", "enum", "Awake", "Start", "Update", "FixedUpdate", "OnDestroy", "OnEnable", "OnDisable", "OnDrawGizmosSelected", "OnDrawGizmos", "OnCollisionEnter", "OnTriggerEnter", "OnTriggerStay", "OnTriggerExit"];
    const types = ["Console", "Header", "SerializeField", "MonoBehaviour", "List", "Transform", "HideInInspector", "Space", "Animator", "Rigidbody", "GameObject", "Health", "NavMeshAgent", "Gizmos"];
    const commentPattern = /(\/\/.*)/g;
    const stringPattern = /"([^"\\]*(\\.[^"\\]*)*)"/g;

    const replacements = [];
    let i = 0;

    text = text.replace(commentPattern, (match) => {
        const token = `__COMMENT_${i}__`;
        replacements.push({
            token,
            html: `<span class="comment">${escapeHtml(match)}</span>`
        });
        i++;
        return token;
    });

    text = text.replace(stringPattern, (match) => {
        const token = `__STRING_${i}__`;
        replacements.push({
            token,
            html: `<span class="string">${escapeHtml(match)}</span>`
        });
        i++;
        return token;
    });

    keywords.forEach(keyword => {
        const regex = new RegExp(`\\b(${keyword})\\b`, 'g');
        text = text.replace(regex, (match) => {
            const token = `__KEYWORD_${i}__`;
            replacements.push({
                token,
                html: `<span class="keyword">${match}</span>`
            });
            i++;
            return token;
        });
    });

    types.forEach(type => {
        const regex = new RegExp(`\\b(${type})\\b`, 'g');
        text = text.replace(regex, (match) => {
            const token = `__TYPE_${i}__`;
            replacements.push({
                token,
                html: `<span class="type">${match}</span>`
            });
            i++;
            return token;
        });
    });

    text = text.replace(/(__\w+_\d+__)(__\w+_\d+__)/g, '$1 $2');
    text = escapeHtml(text);

    replacements.forEach(replacement => {
        text = text.replaceAll(replacement.token, replacement.html);
    });

    text = text.replace(/^(\s+)/gm, match =>
        match.replace(/ /g, '&nbsp;').replace(/\t/g, '&nbsp;&nbsp;&nbsp;&nbsp;')
    );

    codeElement.innerHTML = text;
} */

/// <summary>
/// Adds line numbers to each line of the code inside a code element.
/// </summary>
function addLineNumbers(codeElement) {
    const text = codeElement.innerHTML;
    const lines = text.split('\n').map((line, index) => {
        return `<div class="code-line"><span class="line-number">${index + 1}</span> ${line}</div>`;
    });

    codeElement.innerHTML = lines.join('');
}

// --------- Initialise Highlighting and Line Numbering ---------
document.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('code.language-csharp').forEach(codeBlock => {
        if (!codeBlock.dataset.highlighted) {
            //highlightCSharp(codeBlock);
            addLineNumbers(codeBlock);
            codeBlock.dataset.highlighted = "true";
        }
    });
});
