import * as vscode from "vscode";
import { ExtensionContext } from "vscode";
import { createProjectCommand, openProjectCommand, startPageCommand } from "./projectCommands";

export function initCommands(context: ExtensionContext) {
    const startPage = vscode.commands.registerCommand("slang.startPage", startPageCommand);
    const createProject = vscode.commands.registerCommand("slang.createProject", createProjectCommand);
    const openProject = vscode.commands.registerCommand("slang.openProject", openProjectCommand);

    context.subscriptions.push(startPage, createProject, openProject);
}
