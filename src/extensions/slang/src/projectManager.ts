import * as fs from "fs-extra";
import * as path from "path";
import { extensionPaths } from "./utils/extensionPaths";

export class ProjectManager {
    public static readonly projectFileExtension = ".slproj";
    private readonly moduleExtension = ".sl";
    private readonly mainModuleName = "Main";
    private readonly mainModuleFile = this.mainModuleName + this.moduleExtension;

    public async createProject(projectName: string, folderPath: string) {
        fs.exists(folderPath, (folderExists) => {
            if (folderExists) {
                this.writeProjectFile(this.mainModuleName, projectName, folderPath);
                this.writeModule(this.mainModuleFile, folderPath);
            }
        });
    }

    public getProjectFolder(projectFile: string) {
        return path.dirname(projectFile);
    }

    private async writeProjectFile(mainModuleName: string, projectName: string, folderPath: string) {
        const projectFileName = path.join(folderPath, projectName + ProjectManager.projectFileExtension);
        const content = JSON.stringify({
            "MainModule": mainModuleName
        });

        return fs.writeFile(projectFileName, content);
    }

    private async writeModule(mainModuleName: string, folderPath: string) {
        const source = extensionPaths.getTemplatePath(mainModuleName).fsPath;
        const destination = path.join(folderPath, mainModuleName);

        return fs.copy(source, destination);
    }
}
