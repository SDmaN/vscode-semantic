"use strict";

import * as vscode from "vscode";
import { initCommands } from "./commands";
import { startPageCommand } from "./commands/projectCommands";
import { ProjectManager } from "./projectManager";
import { initExtensionPaths } from "./utils/extensionPaths";
import { registerViewContentProvider } from "./views/viewContentProvider";

export function activate(context: vscode.ExtensionContext) {
    initExtensionPaths(context);
    registerViewContentProvider(context);
    initCommands(context);

    vscode.workspace.findFiles("*" + ProjectManager.projectFileExtension)
        .then(urls => {
            if (!urls || urls.length === 0) {
                startPageCommand();
            }
        });
}

export function deactivate() {
}
