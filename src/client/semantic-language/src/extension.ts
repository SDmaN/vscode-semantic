'use strict';
// The module 'vscode' contains the VS Code extensibility API
// Import the module and reference it with the alias vscode in your code below
import * as vscode from 'vscode';
import * as lsp from 'vscode-languageclient';
import * as path from 'path';

const isDebug = true;

// this method is called when your extension is activated
// your extension is activated the very first time the command is executed
export function activate(context: vscode.ExtensionContext) {
    let serverModule;

    if(isDebug) {
        serverModule = context.asAbsolutePath("../../server/src/PluginServer/bin/Debug/netcoreapp2.0/PluginServer.dll");
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

    console.log("Slang activated");
}

// this method is called when your extension is deactivated
export function deactivate() {
    console.log("Slang activated");
}