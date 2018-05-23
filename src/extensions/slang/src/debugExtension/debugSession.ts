import { ChildProcess, spawn } from "child_process";
import * as path from "path";
import { DebugSession, InitializedEvent, OutputEvent, TerminatedEvent } from "vscode-debugadapter";
import { DebugProtocol } from "vscode-debugprotocol";

interface LaunchRequestArguments extends DebugProtocol.LaunchRequestArguments {
    program: string;
}

export class SlangDebugSession extends DebugSession {
    private _childProcess?: ChildProcess = undefined;

    public constructor() {
        super();
    }

    protected initializeRequest(response: DebugProtocol.InitializeResponse,
        args: DebugProtocol.InitializeRequestArguments): void {
        this.sendResponse(response);
        this.sendEvent(new InitializedEvent());
    }

    protected configurationDoneRequest(response: DebugProtocol.ConfigurationDoneResponse,
        args: DebugProtocol.ConfigurationDoneArguments): void {
        super.configurationDoneRequest(response, args);
    }

    protected async launchRequest(response: DebugProtocol.LaunchResponse, args: LaunchRequestArguments) {
        const cwd = path.dirname(args.program);
        const termArgs: DebugProtocol.RunInTerminalRequestArguments = {
            kind: "external",
            title: "Slang Console",
            cwd: cwd,
            args: [args.program],
            env: {}
        };

        this.runInTerminalRequest(termArgs, 5000, r => {
            if (r.success) {
                this._childProcess = spawn(args.program);

                this._childProcess.on("close", (code: number) => {
                    this.terminate();
                });

                this._childProcess.stdin.end();
            } else {
                this.sendEvent(new OutputEvent("Не удалось запустить программу.", "stderr"));
                this.terminate();
            }
        });

        this.sendResponse(response);
    }

    protected setBreakPointsRequest(response: DebugProtocol.SetBreakpointsResponse, args: DebugProtocol.SetBreakpointsArguments): void {
        this.sendResponse(response);
    }

    protected threadsRequest(response: DebugProtocol.ThreadsResponse): void {
        this.sendResponse(response);
    }

    protected stackTraceRequest(response: DebugProtocol.StackTraceResponse, args: DebugProtocol.StackTraceArguments): void {
        this.sendResponse(response);
    }

    protected scopesRequest(response: DebugProtocol.ScopesResponse, args: DebugProtocol.ScopesArguments): void {
        this.sendResponse(response);
    }

    protected variablesRequest(response: DebugProtocol.VariablesResponse, args: DebugProtocol.VariablesArguments): void {
        this.sendResponse(response);
    }

    protected continueRequest(response: DebugProtocol.ContinueResponse, args: DebugProtocol.ContinueArguments): void {
        this.sendResponse(response);
    }

    protected reverseContinueRequest(response: DebugProtocol.ReverseContinueResponse, args: DebugProtocol.ReverseContinueArguments): void {
        this.sendResponse(response);
    }

    protected nextRequest(response: DebugProtocol.NextResponse, args: DebugProtocol.NextArguments): void {
        this.sendResponse(response);
    }

    protected stepBackRequest(response: DebugProtocol.StepBackResponse, args: DebugProtocol.StepBackArguments): void {
        this.sendResponse(response);
    }

    protected evaluateRequest(response: DebugProtocol.EvaluateResponse, args: DebugProtocol.EvaluateArguments): void {
        this.sendResponse(response);
    }

    private terminate() {
        this._childProcess = undefined;
        this.sendEvent(new TerminatedEvent());
    }
}
