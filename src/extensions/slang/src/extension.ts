"use strict";

import * as vscode from "vscode";
import { registerViewContentProvider, openViewWithWatch } from "./views/ViewContentProvider";
import ExtensionPaths from "./ExtensionPaths";
import * as ep from "./ExtensionPaths";

export function activate(context: vscode.ExtensionContext) {
    const extensionPaths = new ExtensionPaths(context);
    const viewsProvider = registerViewContentProvider(context, extensionPaths);;

    const d = vscode.commands.registerCommand("slang.RefreshStart", () => {
        console.log("CLICKED!!!");
    });

    const viewWatcher = openViewWithWatch("Добро пожаловать в Slang!", "start.html", extensionPaths, () => {
        viewsProvider.update("start.html");
    });

    context.subscriptions.push(d, viewWatcher);
}

export function deactivate() {
}