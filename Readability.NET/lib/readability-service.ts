import { Readability } from "@mozilla/readability";
import { DOMParser } from "linkedom/worker";
import { parseHTML } from "linkedom/worker";

export default function parse(html: string) {
    const document = parseHTML(html);

    (<any>document.window.document.firstChild).__JSDOMParser__ = new DOMParser(); // Readability needs this

    const reader = new Readability(document.window.document);
    return reader.parse();
}