grammar Slang;

/*
 * Parser Rules
 */

 start: moduleImports module;

 moduleImports: ('import' Id)*;


 module: 'module' Id moduleBlock;
 moduleBlock: BeginBlock (func | proc)* EndBlock;

 func: AccessModifier 'fun' Type Id '(' argList ')' statementBlock;
 proc: AccessModifier 'proc' Id '(' argList ')' statementBlock;
 argList: Type Id (',' Type Id)* | /* нет аргументов */ ;

 statementBlock: BeginBlock statementSequence EndBlock;
 statementSequence: (statement)*;
 statement: input | output;

 declare: Type Id ('=' mathExp)?;
 input: 'input' Id;
 output: 'output' mathExp;

 mathExp: mathTerm #MathExpEmpty | mathTerm '+' mathExp #MathExpSum | mathTerm '-' mathExp #MathExpDiv;
 mathTerm: mathFactor | mathFactor '*' mathTerm | mathFactor '/' mathTerm | mathFactor '%' mathTerm;
 mathFactor : mathAtom | '(' mathExp ')' | '+' mathFactor | '-' mathFactor;
 mathAtom: IntValue | RealValue | Id;

/*
 * Lexer Rules
 */

 BeginBlock: 'begin';
 EndBlock: 'end';

 Type: Int | Real | Bool;
 Int: 'int';
 Real: 'float';
 Bool: 'bool';

 AccessModifier: PublicModifier | InternalModifier;

 PublicModifier: 'public';
 InternalModifier: 'internal';

 Id: [_a-zA-Z][_a-zA-Z0-9]*;

 IntValue: Digit+;
 RealValue: [0-9]*'.'?[0-9]+([eE][-+]?[0-9]+)?;
 fragment Digit: [0-9];

 Comment: ('//' ~[\r\n]* | '/*' .*? '*/') -> skip;
 Ws: [ \t\r\n] -> skip;