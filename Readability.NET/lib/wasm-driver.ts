import parse from "./readability-service";

const { readFileSync, writeFileSync, STDIO } = require('javy/fs');

const textEncoder = new TextEncoder();

const inputBuffer = readFileSync(STDIO.Stdin);

const inputHtml = new TextDecoder().decode(inputBuffer);

if (!inputHtml) {
    throw "Please provide a valid html string"
}

const readabilityResult = parse(inputHtml);

writeFileSync(STDIO.Stdout, textEncoder.encode(JSON.stringify(readabilityResult)));