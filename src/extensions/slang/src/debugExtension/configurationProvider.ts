import * as vscode from "vscode";
import { projectManager } from "../projectManager";

export class SlangDebugConfigurationProvider implements vscode.DebugConfigurationProvider {
    public resolveDebugConfiguration(folder: vscode.WorkspaceFolder | undefined,
        config: vscode.DebugConfiguration, token?: vscode.CancellationToken): vscode.ProviderResult<vscode.DebugConfiguration> {

        if (!config.program) {
            config.program = "${workspaceFolder}/out/bin/" + projectManager.openedProjectName;
        } else {
            config.program = config.program.replace("{programName}", projectManager.openedProjectName);
        }

        return config;
    }
}
