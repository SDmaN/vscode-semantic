import * as vscode from "vscode";
import * as path from "path";

export default class ExtensionPaths {
    private _basePath: string = "";
    private _viewsMarkupPath = "";

    constructor(context: vscode.ExtensionContext) {
        this._basePath = context.extensionPath;
        this._viewsMarkupPath = path.join(this._basePath, "src", "views", "markup");
    }

    public get basePath(): string {
        return this._basePath;
    }

    public get viewsMarkupPath(): string {
        return this._viewsMarkupPath;
    }

    public getViewPath(viewRelativePath: string): string {
        return path.join(this._viewsMarkupPath, viewRelativePath);
    }
}