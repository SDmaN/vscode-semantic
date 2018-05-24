import * as vscode from "vscode";
import { ExtensionContext } from "vscode";
import { extensionPaths } from "../utils/extensionPaths";

export function initBuildTaskProvider(context: ExtensionContext) {
    if (vscode.workspace.workspaceFolders && vscode.workspace.workspaceFolders.length > 0) {
        const args = "b . ./out/";
        const commandLine = extensionPaths.compiller + " " + args;
        const execution = new vscode.ShellExecution(commandLine);

        const taskDefinition: vscode.TaskDefinition = {
            type: "shell",
            label: "Slang: Build"
        };

        const folder = vscode.workspace.workspaceFolders[0];
        const buildTask = new vscode.Task(taskDefinition, folder, "Build", "Slang", execution, "$slang");
        buildTask.group = vscode.TaskGroup.Build;
        buildTask.presentationOptions = {
            echo: false,
            focus: false,
            panel: vscode.TaskPanelKind.Dedicated,
            reveal: vscode.TaskRevealKind.Always
        };

        const taskProvider: vscode.TaskProvider = {
            provideTasks: () => {
                return [
                    buildTask
                ];
            },
            resolveTask: () => {
                return undefined;
            }
        };

        const d = vscode.workspace.registerTaskProvider("slang", taskProvider);
        context.subscriptions.push(d);
    }
}
