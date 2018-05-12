import * as path from 'path';
import { ExtensionContext } from 'vscode';
import { isDebug } from '../enviroment';

export class ExtensionPaths {
    public readonly base: string;
    public readonly viewsMarkup: string;

    constructor(context: ExtensionContext) {
        this.base = path.join(context.extensionPath, isDebug ? 'src' : 'out');
        this.viewsMarkup = path.join(this.base, 'views', 'markup');
    }

    public getViewPath(relativePath: string): string {
        return path.join(this.viewsMarkup, relativePath);
    }
}

export function initExtensionPaths(context: ExtensionContext) {
    extensionPaths = new ExtensionPaths(context);
}

export let extensionPaths: ExtensionPaths;
