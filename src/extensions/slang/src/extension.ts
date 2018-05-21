"use strict";

import * as vscode from "vscode";
import { initCommands } from "./commands";
import { startPageCommand } from "./commands/projectCommands";
import { ProjectManager } from "./projectManager";
import { initBuildTaskProvider } from "./tasks/buildTaskProvider";
import { initExtensionPaths } from "./utils/extensionPaths";
import { registerViewContentProvider } from "./views/viewContentProvider";

export function activate(context: vscode.ExtensionContext) {
    initExtensionPaths(context);
    registerViewContentProvider(context);
    initCommands(context);
    initBuildTaskProvider(context);

    vscode.workspace.findFiles("*" + ProjectManager.projectFileExtension)
        .then(urls => {
            if (!urls || urls.length === 0) {
                startPageCommand();
            }
        });
}

export function deactivate() {
}
