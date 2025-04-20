function escapeHtml(text) {
    return text
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;");
}

function highlightCSharp(codeElement) {
    let text = codeElement.textContent;

    const keywords = ["public", "private", "protected", "static", "void", "class", "return", "new", "if", "else", "switch", "case", "break", "int", "string", "float", "double", "bool", "char", "enum", "Awake", "Start", "Update", "FixedUpdate", "OnDestoy", "OnEnable", "OnDisable", "OnDrawGizmosSelected", "OnDrawGizmos", "OnCollisionEnter", "OnTriggerEnter", "OnTriggerStay", "OnTriggerExit"];
    const types = ["Console", "Header", "SerializeField", "MonoBehaviour", "List", "Transform", "HideInInspector", "Space", "Animator", "Rigidbody", "GameObject", "Health", "NavMeshAgent", "Gizmos"];
    const commentPattern = /(\/\/.*)/g;
    const stringPattern = /"([^"\\]*(\\.[^"\\]*)*)"/g;

    // Store replacements first
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

    // Highlight keywords
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

    // Highlight types
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

    // Escape the entire thing safely now
    // Insert space between adjacent tokens to preserve spacing
    text = text.replace(/(__\w+_\d+__)(__\w+_\d+__)/g, '$1 $2');
    text = escapeHtml(text);

    // Replace preserved placeholders with their HTML
    replacements.forEach(replacement => {
        text = text.replaceAll(replacement.token, replacement.html);
    });

    // Convert whitespace
    text = text.replace(/^(\s+)/gm, match =>
        match.replace(/ /g, '&nbsp;').replace(/\t/g, '&nbsp;&nbsp;&nbsp;&nbsp;')
    );

    // Add line numbers
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