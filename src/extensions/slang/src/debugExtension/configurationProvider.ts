import * as vscode from "vscode";

export class SlangDebugConfigurationProvider implements vscode.DebugConfigurationProvider {
    public resolveDebugConfiguration(folder: vscode.WorkspaceFolder | undefined,
        config: vscode.DebugConfiguration, token?: vscode.CancellationToken): vscode.ProviderResult<vscode.DebugConfiguration> {

        if (!config.program) {
            return vscode.window.showInformationMessage("Не указан файл для запуска.").then(_ => {
                return undefined;
            });
        }

        return config;
    }
}
