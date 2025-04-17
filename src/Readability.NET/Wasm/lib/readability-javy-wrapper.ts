import { Readability } from "@mozilla/readability";
import { parseHTML } from "linkedom/worker";

const { readFileSync, writeFileSync, STDIO } = require('javy/fs');

const textEncoder = new TextEncoder();

const inputBuffer = readFileSync(STDIO.Stdin);

const input = JSON.parse(new TextDecoder().decode(inputBuffer));

const { html, options } = input;

if (!html) {
    throw "Please provide a valid html string"
}

const readabilityResult = parse(html, options);

if (options.debug) {
    // Write the debug log separator, so that we can reiliably identify the debug log and the readability result separately.
    writeFileSync(STDIO.Stdout, textEncoder.encode("###DEBUG_END###"));
}

writeFileSync(STDIO.Stdout, textEncoder.encode(JSON.stringify(readabilityResult)));

function parse(html: string, options: unknown) {
    const htmlDoc = parseHTML(html);

    const reader = new Readability(htmlDoc.document, options);

    return reader.parse();
}