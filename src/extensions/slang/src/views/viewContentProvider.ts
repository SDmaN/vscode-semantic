import * as fs from 'fs';
import * as path from 'path';
import * as vscode from 'vscode';
import { CancellationToken, EventEmitter, ExtensionContext, TextDocumentContentProvider, Uri } from 'vscode';
import { extensionPaths } from '../utils/extensionPaths';
import { readFileAsString } from '../utils/io';

export class ViewContentProvider implements TextDocumentContentProvider {
    private readonly _didChangeEmmiter = new EventEmitter<Uri>();

    public async provideTextDocumentContent(uri: Uri, token: CancellationToken) {
        const relativePath = path.join(uri.authority, uri.path);
        const fullPath = extensionPaths.getViewPath(relativePath);

        let viewContent = await readFileAsString(fullPath);
        viewContent = this.addStyles(viewContent);

        return viewContent;
    }

    public get onDidChange() {
        return this._didChangeEmmiter.event;
    }

    public updateView(viewPath: string) {
        const viewUri = getViewContentProviderUri(viewPath);
        this._didChangeEmmiter.fire(viewUri);
    }

    private addStyles(viewContent: string) {
        const stylesVariable = '$${STYLE_PATH}$$';
        const cssPath = extensionPaths.getViewPath('css/styles.css');

        return viewContent.replace(stylesVariable, cssPath);
    }
}

const viewContentProvider = new ViewContentProvider();

export function updateView(viewPath: string) {
    viewContentProvider.updateView(viewPath);
}

export function registerViewContentProvider(context: ExtensionContext) {
    const registration = vscode.workspace.registerTextDocumentContentProvider('views', viewContentProvider);
    context.subscriptions.push(registration);
}

export function openView(title: string, filePath: string) {
    const uri = getViewContentProviderUri(filePath);

    vscode.commands.executeCommand('vscode.previewHtml', uri, vscode.ViewColumn.One, title)
        .then(undefined, (error: any) => {
            vscode.window.showErrorMessage(error);
        });
}

export function openWatchableView(title: string, filePath: string): vscode.Disposable {
    openView(title, filePath);

    const fullPath = extensionPaths.getViewPath(filePath);
    const watcher = fs.watch(fullPath).addListener('change', () => {
        updateView(filePath);
    });

    return {
        dispose: () => watcher.close()
    };
}

function getViewContentProviderUri(viewFilePath: string) {
    return Uri.parse(`views://${viewFilePath}`);
}
