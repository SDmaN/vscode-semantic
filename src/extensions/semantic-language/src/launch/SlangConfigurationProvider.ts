import * as vscode from 'vscode';
import { DebugConfigurationProvider,
    WorkspaceFolder,
    DebugConfiguration, 
    CancellationToken,
    ProviderResult
} from 'vscode';

export class SlangConfigurationProvider implements DebugConfigurationProvider {
	/**
	 * Massage a debug configuration just before a debug session is being launched,
	 * e.g. add all missing attributes to the debug configuration.
	 */
    resolveDebugConfiguration(folder: WorkspaceFolder | undefined, 
        config: DebugConfiguration, token?: CancellationToken): ProviderResult<DebugConfiguration> {

		return config;
	}
}