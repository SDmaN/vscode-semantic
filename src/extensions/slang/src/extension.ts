"use strict";

import { spawn } from "child_process";
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

    process.env.Path += ";C:/Programming/Projects/vscode-semantic/src/language-tools/src/SlangCompiller/bin/Release/netcoreapp2.0/publish";

    const p = spawn("SlangCompiller", ["tr"]);

    p.stdout.on("data", (data: any) => {
        console.log(`stdout: ${data}`);
    });

    p.stderr.on("data", (data) => {
        console.log(`stderr: ${data}`);
    });

    p.on("close", (code) => {
        console.log(`child process exited with code ${code}`);
    });


    console.log(process.env.Path);
}

export function deactivate() {
}
