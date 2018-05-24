import * as path from "path";
import { ExtensionContext, Uri } from "vscode";
import { isDebug } from "../enviroment";

export class ExtensionPaths {
    public readonly base: Uri;
    public readonly viewsMarkup: Uri;
    public readonly templates: Uri;
    public readonly compiller: Uri;

    constructor(context: ExtensionContext) {
        const scriptsFolder = isDebug ? "src" : "out";

        this.base = Uri.file(path.join(context.extensionPath));
        this.viewsMarkup = Uri.file(path.join(this.base.fsPath, scriptsFolder, "views", "markup"));
        this.templates = Uri.file(path.join(this.base.fsPath, scriptsFolder, "templates"));
        this.compiller = Uri.file(path.join(this.base.fsPath, scriptsFolder, "compiller", "SlangCompiller"));
    }

    public getViewPath(relativePath: string) {
        return Uri.file(path.join(this.viewsMarkup.fsPath, relativePath));
    }

    public getTemplatePath(relativePath: string) {
        return Uri.file(path.join(this.templates.fsPath, relativePath));
    }
}

export function initExtensionPaths(context: ExtensionContext) {
    extensionPaths = new ExtensionPaths(context);
}

export let extensionPaths: ExtensionPaths;
