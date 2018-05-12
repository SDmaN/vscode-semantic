'use strict';

import * as vscode from 'vscode';

export function activate(context: vscode.ExtensionContext) {
    vscode.window.showInformationMessage('Hello World!');


    // let disposable = vscode.commands.registerCommand('extension.sayHello', () => {

    // });

    // context.subscriptions.push(disposable);
}

export function deactivate() {
}
