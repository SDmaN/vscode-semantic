import * as vscode from "vscode";
import { ProjectManager, projectManager } from "../projectManager";
import { openView } from "../views/viewContentProvider";

export function startPageCommand() {
    openView("Добро пожаловать в Slang!", "start.html");
}

export function createProjectCommand() {
    vscode.window.showInputBox({
        placeHolder: "Название проекта",
        prompt: "Введите название проекта",
        validateInput: validateProjectName
    }).then(projectName => {
        if (projectName && isProjectNameValid(projectName)) {
            vscode.window.showOpenDialog({
                canSelectFiles: false,
                canSelectFolders: true,
                openLabel: "Создать"
            }).then(async urls => {
                if (urls && urls.length > 0) {
                    try {
                        if (await projectManager.createProject(projectName, urls[0])) {
                            vscode.commands.executeCommand("vscode.openFolder", urls[0]);
                        }
                    } catch (e) {
                        vscode.window.showErrorMessage(getErrorMessage(e));
                    }
                }
            }, error => vscode.window.showErrorMessage(getErrorMessage(error)));
        }
    });
}

export function openProjectCommand() {
    vscode.window.showOpenDialog({
        canSelectFiles: true,
        canSelectFolders: false,
        filters: { "Slang project file": [ProjectManager.projectFileExtension] },
        openLabel: "Открыть"
    }).then(async urls => {
        try {
            if (urls && urls.length > 0) {
                if (await projectManager.openProject(urls[0])) {
                    vscode.commands.executeCommand("vscode.openFolder", projectManager.openedProjectDirPath);
                }
            }
        } catch (e) {
            vscode.window.showErrorMessage(getErrorMessage(e));
        }
    }, error => vscode.window.showErrorMessage(getErrorMessage(error)));
}

function isProjectNameValid(name: string) {
    const regexp = /^[a-zA-Z][a-zA-Z\d]+$/;
    return regexp.test(name);
}

function validateProjectName(name: string) {
    if (!isProjectNameValid(name)) {
        return "Название должно содержать только английские символы и цифры";
    }

    return null;
}

function getErrorMessage(e: any) {
    return e.message ? e.message : e;
}
