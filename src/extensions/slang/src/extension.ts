'use strict';

import * as vscode from 'vscode';
import { initExtensionPaths } from './utils/extensionPaths';
import { openView, registerViewContentProvider } from './views/viewContentProvider';

export function activate(context: vscode.ExtensionContext) {
    initExtensionPaths(context);
    registerViewContentProvider(context);

    openView('Добро пожаловать в Slang!', 'start.html');


    // let disposable = vscode.commands.registerCommand('extension.sayHello', () => {

    // });

    // context.subscriptions.push(disposable);
}

export function deactivate() {
}
