import * as fs from "fs-extra";
import * as path from "path";
import { Uri } from "vscode";
import { extensionPaths } from "./utils/extensionPaths";

export class ProjectManager {
    public static readonly projectFileExtension = "slproj";
    private readonly moduleExtension = "slang";

    private readonly mainModuleName = "Main";
    private readonly mainModuleFile = `${this.mainModuleName}.${this.moduleExtension}`;

    private _openedProjectName?: string;
    public get openedProjectName(): string | undefined {
        return this._openedProjectName;
    }

    private _openedProjectFilePath?: Uri;
    public get openedProjectFilePath(): Uri | undefined {
        return this._openedProjectFilePath;
    }

    private _openedProjectDirPath?: Uri;
    public get openedProjectDirPath(): Uri | undefined {
        return this._openedProjectDirPath;
    }

    public async openProject(projectFile: Uri) {
        const fileExists = await fs.pathExists(projectFile.fsPath);

        if (fileExists) {
            this._openedProjectName = path.basename(projectFile.fsPath, "." + ProjectManager.projectFileExtension);
            this._openedProjectFilePath = projectFile;
            this._openedProjectDirPath = Uri.parse(path.dirname(projectFile.fsPath));
        } else {
            this.closeProject();
        }

        return fileExists;
    }

    public closeProject() {
        this._openedProjectName = undefined;
        this._openedProjectFilePath = undefined;
        this._openedProjectDirPath = undefined;
    }

    public async createProject(projectName: string, folderPath: Uri) {
        const exists = await fs.pathExists(folderPath.fsPath);

        if (exists) {
            const projectFileUri = await this.writeProjectFile(this.mainModuleName, projectName, folderPath);
            await this.writeModule(this.mainModuleFile, folderPath);
            await this.writeLaunch(folderPath);

            await this.openProject(projectFileUri);
        } else {
            this.closeProject();
        }

        return exists;
    }

    private async writeProjectFile(mainModuleName: string, projectName: string, folderPath: Uri) {
        const projectFileName = path.join(folderPath.fsPath, `${projectName}.${ProjectManager.projectFileExtension}`);
        const projectUri = Uri.parse(projectFileName);

        const content = JSON.stringify({
            "MainModule": mainModuleName
        });

        await fs.writeFile(projectFileName, content);
        return projectUri;
    }

    private async writeModule(mainModuleName: string, folderPath: Uri) {
        const source = extensionPaths.getTemplatePath(mainModuleName).fsPath;
        const destination = path.join(folderPath.fsPath, mainModuleName);

        return fs.copy(source, destination);
    }

    private async writeLaunch(folderPath: Uri) {
        const source = extensionPaths.getTemplatePath(".vscode/launch.json").fsPath;
        const destination = path.join(folderPath.fsPath, ".vscode", "launch.json");

        return fs.copy(source, destination);
    }
}

export const projectManager = new ProjectManager();
