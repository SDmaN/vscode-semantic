import * as vscode from "vscode";
import { ProjectManager } from "../projectManager";
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
            }).then(urls => {
                if (urls && urls.length > 0) {
                    const projectManager = new ProjectManager();
                    projectManager.createProject(projectName, urls[0].fsPath)
                        .then(() => { vscode.commands.executeCommand("vscode.openFolder", urls[0]); })
                        .catch(error => vscode.window.showErrorMessage(error));
                }
            }, error => vscode.window.showErrorMessage(error));
        }
    });
}

export function openProjectCommand() {
    vscode.window.showOpenDialog({
        canSelectFiles: true,
        canSelectFolders: false,
        filters: { "Slang": ["slproj"] },
        openLabel: "Открыть"
    }).then(urls => {
        if (urls && urls.length > 0) {
            const projectManager = new ProjectManager();
            const folder = projectManager.getProjectFolder(urls[0].fsPath);
            const folderUrl = vscode.Uri.parse(folder);

            vscode.commands.executeCommand("vscode.openFolder", folderUrl);
        }
    });
}

function isProjectNameValid(name: string) {
    const regexp = /^[a-zA-Z\d]+$/;
    return regexp.test(name);
}

function validateProjectName(name: string) {
    if (!isProjectNameValid(name)) {
        return "Название должно содержать только английские символы и цифры";
    }

    return null;
}
