const fs = require('fs');
const path = require('path');
const CleanCSS = require('clean-css');

function minifyCssFile(filePath) {
    if (!fs.existsSync(filePath)) return;
    const css = fs.readFileSync(filePath, 'utf8');
    const minified = new CleanCSS({}).minify(css);
    if (minified.errors.length) {
        console.error(`Minification errors in ${filePath}:`, minified.errors);
        return;
    }
    const minPath = filePath.replace(/\.css$/, '.min.css');
    fs.writeFileSync(minPath, minified.styles, 'utf8');
    console.log(`Minified: ${filePath} -> ${minPath}`);
}

function minifyAllInDir(dir) {
    if (!fs.existsSync(dir)) return;
    fs.readdirSync(dir).forEach(file => {
        if (file.endsWith('.css') && !file.endsWith('.min.css')) {
            minifyCssFile(path.join(dir, file));
        }
    });
}

// Entry point: node minify-css.js <dir1> <dir2> ...
process.argv.slice(2).forEach(minifyAllInDir);
