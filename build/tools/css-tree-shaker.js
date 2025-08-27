const fs = require('fs');
const path = require('path');

function getUsedClassesFromFiles(files) {
    const classSet = new Set();
    const patterns = [
        /class\s*=\s*["']([^"']+)["']/gi,
        /@class\s*=\s*["']([^"']+)["']/gi,
        /CssClass\s*=\s*["']([^"']+)["']/gi,
        /Class\s*=\s*["']([^"']+)["']/gi,
        /ClassName\s*=\s*["']([^"']+)["']/gi,
        /\$["']([^"']*\s+)?([a-zA-Z][a-zA-Z0-9_-]*(\s+[a-zA-Z][a-zA-Z0-9_-]*)*)[^"']*["']/gi,
        /const\s+\w+\s*=\s*["']([^"']+)["']/gi,
        /var\s+\w+\s*=\s*["']([^"']+)["']/gi,
        /readonly\s+string\s+\w+\s*=\s*["']([^"']+)["']/gi
    ];
    files.forEach(file => {
        if (!fs.existsSync(file)) return;
        const content = fs.readFileSync(file, 'utf8');
        patterns.forEach(pattern => {
            let match;
            while ((match = pattern.exec(content)) !== null) {
                const classString = match[1] || match[0];
                if (classString) {
                    classString.split(/\s+/).forEach(cls => {
                        if (cls && /^[a-zA-Z][a-zA-Z0-9_-]*$/.test(cls)) classSet.add(cls);
                    });
                }
            }
        });
    });
    return classSet;
}

function shakeCss(cssFile, usedClasses, outputDir) {
    if (!fs.existsSync(cssFile)) return;
    const css = fs.readFileSync(cssFile, 'utf8');
    const ruleRegex = /([^\{]+)\{([^}]*)\}/g;
    let result = '';
    let match;
    while ((match = ruleRegex.exec(css)) !== null) {
        const selector = match[1].trim();
        const body = match[2];
        // Keep rules with no class selectors (e.g. html, body, tags)
        if (!selector.includes('.')) {
            result += `${selector}{${body}}`;
            continue;
        }
        // Extract all classes from selector
        const classMatches = [...selector.matchAll(/\.([a-zA-Z][a-zA-Z0-9_-]*)/g)];
        if (classMatches.some(m => usedClasses.has(m[1]))) {
            result += `${selector}{${body}}`;
        }
    }
    // Write optimized CSS
    const outFile = path.join(outputDir, path.basename(cssFile).replace('.css', '.optimized.css'));
    fs.mkdirSync(path.dirname(outFile), { recursive: true });
    fs.writeFileSync(outFile, result, 'utf8');
    console.log(`Tree-shaken: ${cssFile} -> ${outFile}`);
}

// Entry point
const [cssFilesArg, sourceFilesArg, outputDir] = process.argv.slice(2);
const cssFiles = cssFilesArg.split(';').filter(Boolean);
const sourceFiles = sourceFilesArg.split(';').filter(Boolean);

const usedClasses = getUsedClassesFromFiles(sourceFiles);
cssFiles.forEach(cssFile => shakeCss(cssFile, usedClasses, outputDir));
