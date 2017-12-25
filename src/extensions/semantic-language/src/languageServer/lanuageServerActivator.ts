import * as vscode from 'vscode';
import * as lsp from 'vscode-languageclient';
import * as path from 'path';

const isDebug = true;

export function activateLanguageServer(context: vscode.ExtensionContext) {
    let serverModule;

    if(isDebug) {
        serverModule = context.asAbsolutePath("../../language-server/src/PluginServer/bin/Debug/netcoreapp2.0/PluginServer.dll");
    }

    let serverWorkPath = path.dirname(serverModule);

    let serverOptions: lsp.ServerOptions = {
        run: { command: "dotnet", args: [ serverModule ], options: { cwd: serverWorkPath }},
        debug: { command: "dotnet", args: [ serverModule ], options: { cwd: serverWorkPath }},
    };

    let clientOptions: lsp.LanguageClientOptions = {
        documentSelector: [ {scheme: "file", language: "plaintext" } ],
        synchronize: {
            configurationSection: "semanticLanguage",
            fileEvents: [
                vscode.workspace.createFileSystemWatcher("**/.clientrc"),
                vscode.workspace.createFileSystemWatcher("**/.demo")
            ]
        }
    };

    let client = new lsp.LanguageClient("semanticLanguageServer", "Semantic Language Server",
        serverOptions, clientOptions);

    // let disposable = client.start();

    // context.subscriptions.push(disposable);
}