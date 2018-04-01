import {
    TextDocumentContentProvider,
    Event,
    EventEmitter,
    Uri,
    CancellationToken,
    ExtensionContext,
    Disposable
} from "vscode";

import * as vscode from "vscode";
import * as path from "path";
import * as fs from "fs";
import ExtensionPathManager from "../ExtensionPaths";

const stylesVariable = "$${STYLE_PATH}$$";

export class ViewContentProvider implements TextDocumentContentProvider {
    private _onDidChange = new EventEmitter<Uri>();
    private readonly _extensionPaths: ExtensionPathManager;

    constructor(extensionPaths: ExtensionPathManager) {
        this._extensionPaths = extensionPaths;
    }

    public get onDidChange(): Event<Uri> {
        return this._onDidChange.event;
    }

    public async provideTextDocumentContent(uri: Uri, token: CancellationToken): Promise<string> {
        const fileRelativePath = path.join(uri.authority, uri.path);
        const fileFullPath = this._extensionPaths.getViewPath(fileRelativePath);

        return await this.readFileContent(fileFullPath);
    }

    public update(viewPath: string) {
        const uri = getViewContentProviderUri(viewPath);
        this._onDidChange.fire(uri);
    }

    private async readFileContent(filePath: string): Promise<string> {
        return new Promise<string>((resolve, reject) => {
            fs.readFile(filePath, (err, data) => {
                if (!err) {
                    const stylesPath = this._extensionPaths.getViewPath("styles.css");
                    const htmlString = data.toString().replace(stylesVariable, stylesPath);

                    resolve(htmlString);
                } else {
                    reject(err);
                }
            });
        });
    }
}

export function registerViewContentProvider(context: ExtensionContext,
    pathManager: ExtensionPathManager): ViewContentProvider {

    const viewsProvider = new ViewContentProvider(pathManager);;
    const registration = vscode.workspace.registerTextDocumentContentProvider("views", viewsProvider);

    context.subscriptions.push(registration);
    return viewsProvider;
}

export function openView(title: string, filePath: string): void {
    const uri = getViewContentProviderUri(filePath);

    vscode.commands.executeCommand("vscode.previewHtml", uri, vscode.ViewColumn.One, title)
        .then(undefined, (error) => {
            vscode.window.showErrorMessage(error);
        });
}

export function openViewWithWatch(title: string, viewRelativePath: string,
    extensionPaths: ExtensionPathManager, onChange: () => void): Disposable {
    openView(title, viewRelativePath);
    const viewFilePath = extensionPaths.getViewPath(viewRelativePath);
    const watcher = fs.watch(viewFilePath).addListener("change", onChange);

    return {
        dispose: () => watcher.close()
    }
}

function getViewContentProviderUri(viewFilePath: string) {
    return Uri.parse(`views://${viewFilePath}`);
}