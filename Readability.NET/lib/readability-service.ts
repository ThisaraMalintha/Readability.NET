import { Readability } from "@mozilla/readability";
import { DOMParser, parseHTML } from "linkedom/worker";

export default function parse(html: string) {
    const htmlDoc = parseHTML(html);

    (<any>htmlDoc.document.firstChild).__JSDOMParser__ = new DOMParser(); // Inject the DOM parser into the Readability.js

    const reader = new Readability(htmlDoc.document, { debug: false });

    return reader.parse();
}