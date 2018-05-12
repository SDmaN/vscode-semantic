import * as path from "path";
import { ExtensionContext } from "vscode";
import { isDebug } from "../enviroment";

export class ExtensionPaths {
    public readonly base: string;
    public readonly viewsMarkup: string;
    public readonly templates: string;

    constructor(context: ExtensionContext) {
        const scriptsFolder = isDebug ? "src" : "out";

        this.base = path.join(context.extensionPath);
        this.viewsMarkup = path.join(this.base, scriptsFolder, "views", "markup");
        this.templates = path.join(this.base, "templates");
    }

    public getViewPath(relativePath: string) {
        return path.join(this.viewsMarkup, relativePath);
    }

    public getTemplatePath(relativePath: string) {
        return path.join(this.templates, relativePath);
    }
}

export function initExtensionPaths(context: ExtensionContext) {
    extensionPaths = new ExtensionPaths(context);
}

export let extensionPaths: ExtensionPaths;
