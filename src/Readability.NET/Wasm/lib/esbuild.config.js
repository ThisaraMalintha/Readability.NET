const esbuild = require('esbuild');

esbuild.build({
    entryPoints: ["./readability-javy-wrapper.ts"],
    outfile: "./dist/mozilla-readability.js",
    bundle: true,
    minify: false,
    sourcemap: true,
    format: 'esm', // or 'cjs' for CommonJS
    splitting: false,
    target: 'es2020',
    logLevel: 'info',
}).catch(() => {
    process.exit(1);
});