import * as vscode from 'vscode';
import { SlangConfigurationProvider } from './SlangConfigurationProvider';

export function activateLauncher(context: vscode.ExtensionContext) {
    let configProvider = vscode.debug.registerDebugConfigurationProvider("slang", 
        new SlangConfigurationProvider());

    context.subscriptions.push(configProvider);
}